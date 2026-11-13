#!/usr/bin/env python3
"""Valida la documentación generada por la skill planificador-proyectos-web y,
opcionalmente, la empaqueta en un .zip listo para descomprimir en la raíz del repositorio.

Uso:
    python validar_documentacion.py <directorio> --tipo frontend|backend|fullstack
        [--nivel pequeno|mediano|complejo] [--api-rest] [--zip salida.zip]

El nivel (por defecto, mediano) decide qué archivos son obligatorios: auditoría técnica y
carpeta de especificaciones desde nivel mediano; contrato OpenAPI desde nivel mediano si
el proyecto expone una API REST (--api-rest).

Errores (código de salida 1): archivos obligatorios faltantes, nombres no ASCII,
settings.json inválido, marcadores de plantilla sin rellenar, DESIGN.md en un backend,
registro sin backlog, iteraciones o especificaciones sin criterios de aceptación.
Advertencias: posibles rastros de tiempo en los documentos inmutables, criterios vagos,
iteraciones con más de 8 tareas, una épica 00 de línea base mal formada, falta de la
definición de preparada o del comando /siguiente-epica, y marcadores PENDIENTE_ que
conviene revisar. El zip solo se genera si no hay errores.
"""

import argparse
import fnmatch
import json
import re
import sys
import zipfile
from pathlib import Path

INMUTABLES = ["AGENTS.md", "DESIGN.md", "README.md", "_planificacion/README.md"]

OBLIGATORIOS = [
    "AGENTS.md",
    "README.md",
    ".claude/settings.json",
    ".claude/README.md",
    "_planificacion/README.md",
    "_planificacion/00_producto/producto.md",
    "_planificacion/00_producto/registro-pendientes.md",
    "_planificacion/00_producto/decisiones.md",
]

# Obligatorios que dependen del nivel de proceso.
DESDE_MEDIANO = ["_planificacion/00_producto/auditoria-tecnica.md"]
NIVELES = ["pequeno", "mediano", "complejo"]
OPENAPI = ["docs/api/openapi.yaml", "docs/api/openapi.yml", "docs/api/openapi.json"]

# Criterios que no se pueden comprobar con una medición o un comando.
VAGOS = re.compile(
    r"se ve bien|funciona bien|funciona correctamente|de forma adecuada|adecuadamente|intuitiv|amigable|como corresponde",
    re.I,
)

# Rastros de tiempo que no deben aparecer en los documentos inmutables.
RASTROS = [
    (r"\b20\d{2}-\d{2}-\d{2}\b", "fecha"),
    (r"\biteraci[oó]n(es)?\s+\d{2}-\d{2}\b", "número de iteración"),
    (r"\b[ée]pica\s+\d{1,2}\b", "número de épica"),
    (r"\bRDA-\d{3}\b", "referencia a una RDA"),
    (r"\bAUD-\d{2}-\d{3}\b", "referencia a un hallazgo"),
    (r"\ba[uú]n no existe\b", "estado temporal"),
    (r"\bse (crea|creará|publica|publicará|agrega|agregará) en\b", "estado temporal"),
    (r"\b(por ahora|hasta ahora|a partir de ahora|antes era|ya no)\b", "estado temporal"),
    (r"\b(registro|historial) de cambios\b", "registro de cambios"),
    (r"\bchangelog\b", "registro de cambios"),
]

MARCADOR_PLANTILLA = re.compile(r"\{\{[A-Z0-9_]+\}\}|PLANTILLA:")
EXTENSIONES_CON_MARCADORES = ("*.md", "*.yaml", "*.yml", "*.json")

# Archivos que solo explican cómo usar los marcadores: sus menciones no son datos pendientes.
SIN_PENDIENTES_REALES = {
    "AGENTS.md",
    "_planificacion/README.md",
    ".claude/README.md",
    "_planificacion/00_producto/registro-pendientes.md",
}

# Líneas que nombran el marcador como regla o criterio, no como dato faltante.
EXPLICATIVA = re.compile(
    r"marcador|mientras falten|se usa `PENDIENTE|usa `PENDIENTE|sin `PENDIENTE|ning[uú]n[^|]*`PENDIENTE|no (tiene|tenga|quede)[^|]*`PENDIENTE|queda alguno",
    re.I,
)


def seccion(texto: str, nombre: str):
    """Devuelve el texto de la sección «## N. Nombre» (por nombre, no por número)."""
    m = re.search(rf"^## (?:\d+\.\s*)?{re.escape(nombre)}.*?(?=^## |\Z)", texto, flags=re.S | re.M)
    return m.group(0) if m else None


def filas_de_seccion(texto: str, nombre: str) -> int:
    """Cuenta las filas con datos de la tabla de una sección del registro."""
    m = seccion(texto, nombre)
    if not m:
        return 0
    filas = [l for l in m.splitlines() if l.startswith("|")]
    datos = [l for l in filas[2:] if not re.match(r"^\|\s*—\s*\|", l)]
    return len(datos)


def revisar_registro(raiz: Path, pendientes: dict, errores: list, avisos: list):
    """Todo marcador real debe tener dónde estar registrado en registro-pendientes.md."""
    registro = raiz / "_planificacion/00_producto/registro-pendientes.md"
    if not pendientes or not registro.is_file():
        return
    texto = registro.read_text(encoding="utf-8")
    secciones = {"CLIENTE": "Datos pendientes del cliente", "DESARROLLADOR": "Decisiones por tomar"}
    for tipo, archivos in sorted(pendientes.items()):
        nombre = secciones[tipo]
        lista = ", ".join(sorted(archivos))
        if filas_de_seccion(texto, nombre) == 0:
            errores.append(f"Hay PENDIENTE_{tipo} en {lista}, pero la sección «{nombre}» de registro-pendientes.md está vacía.")
        else:
            avisos.append(f"PENDIENTE_{tipo} en {lista}: comprueba que cada uno tenga su fila en «{nombre}».")


def conflictos_de_permisos(permisos: dict) -> list:
    """Reglas de allow o ask que nunca se aplican porque una de deny las cubre."""
    def partes(regla):
        m = re.match(r"^(\w+)\((.*)\)$", regla)
        return (m.group(1), m.group(2)) if m else (None, None)

    conflictos = []
    for lista in ("allow", "ask"):
        for regla in permisos.get(lista, []):
            herramienta, valor = partes(regla)
            for negada in permisos.get("deny", []):
                h2, patron = partes(negada)
                if herramienta and herramienta == h2 and regla != negada and fnmatch.fnmatchcase(valor, patron):
                    conflictos.append(f"settings.json: «{regla}» en {lista} queda anulada por «{negada}» en deny.")
    return conflictos


def sin_codigo(texto: str) -> str:
    """Quita bloques de código y código en línea: ahí van ejemplos de formato, no rastros."""
    texto = re.sub(r"```.*?```", "", texto, flags=re.S)
    return re.sub(r"`[^`\n]*`", "", texto)


def revisar_criterios(rel: str, texto: str, errores: list, avisos: list, minimo: str):
    """Una iteración o especificación debe tener criterios de aceptación, y conviene que no sean vagos."""
    criterios = seccion(texto, "Criterios de aceptación")
    if criterios is None:
        errores.append(f"{rel} no tiene la sección «Criterios de aceptación».")
        return
    lineas = [l for l in criterios.splitlines()[1:] if re.match(minimo, l.strip())]
    if not lineas:
        errores.append(f"{rel}: la sección «Criterios de aceptación» no tiene ningún criterio.")
    for l in lineas:
        m = VAGOS.search(l)
        if m:
            avisos.append(f"{rel}: criterio posiblemente vago («{m.group(0)}»); un criterio se comprueba con una medición o un comando.")


MAX_TAREAS = 8


def revisar_tamano(rel: str, texto: str, avisos: list):
    """Tamaño máximo de una iteración: 8 tareas. Si se pasa, conviene partirla."""
    tareas = seccion(texto, "Tareas")
    if tareas is None:
        return
    n = len([l for l in tareas.splitlines()[1:] if re.match(r"^\d+\.\s", l.strip())])
    if n > MAX_TAREAS:
        avisos.append(f"{rel}: tiene {n} tareas (máximo {MAX_TAREAS}); conviene partir la iteración.")


def revisar_linea_base(ep: Path, avisos: list):
    """La épica 00 registra lo que ya existía: Terminada, sin iteraciones ni bitácoras."""
    definicion = ep / "definicion-epica-00.md"
    if definicion.is_file() and not re.search(r"^- \*\*Estado:\*\* Terminada", definicion.read_text(encoding="utf-8"), flags=re.M):
        avisos.append(f"{ep.name}: la épica de línea base debe estar «Terminada».")
    if list(ep.glob("iteracion-*.md")):
        avisos.append(f"{ep.name}: la épica de línea base no lleva iteraciones.")
    if (ep.parent.parent / "99_bitacora" / ep.name).exists():
        avisos.append(f"{ep.name}: la épica de línea base no lleva carpeta de bitácoras.")


def validar(raiz: Path, tipo: str, nivel: str = "mediano", api_rest: bool = False):
    errores, avisos = [], []

    obligatorios = list(OBLIGATORIOS)
    if nivel != "pequeno":
        obligatorios += DESDE_MEDIANO
    if tipo in ("frontend", "fullstack"):
        obligatorios.append("DESIGN.md")
    for rel in obligatorios:
        if not (raiz / rel).is_file():
            errores.append(f"Falta el archivo obligatorio: {rel}")

    if tipo == "backend" and (raiz / "DESIGN.md").exists():
        errores.append("Un proyecto backend no lleva DESIGN.md: sus convenciones van en AGENTS.md.")

    for d in ("_planificacion/10_epicas", "_planificacion/99_bitacora"):
        if not (raiz / d).is_dir():
            errores.append(f"Falta la carpeta: {d}/")

    epicas = sorted(p for p in (raiz / "_planificacion/10_epicas").glob("epica-*") if p.is_dir())
    if not epicas:
        errores.append("No hay ninguna carpeta de épica en _planificacion/10_epicas/.")
    for ep in epicas:
        m = re.match(r"epica-(\d{2})-[a-z0-9-]+$", ep.name)
        if not m:
            errores.append(f"Nombre de épica fuera de convención: {ep.name} (se espera epica-NN-nombre)")
            continue
        nn = m.group(1)
        if not (ep / f"definicion-epica-{nn}.md").is_file():
            errores.append(f"Falta {ep.name}/definicion-epica-{nn}.md")
        for f in ep.glob("*.md"):
            if f.name.startswith("definicion-"):
                continue
            if not re.match(rf"(iteracion-{nn}-\d{{2}}-[a-z0-9-]+|evidencia-{nn}-\d{{2}})\.md$", f.name):
                errores.append(f"Nombre fuera de convención: {ep.name}/{f.name}")
            elif f.name.startswith("iteracion-"):
                texto_it = f.read_text(encoding="utf-8")
                revisar_criterios(f"{ep.name}/{f.name}", texto_it, errores, avisos, r"^- \[[ xX]\]")
                revisar_tamano(f"{ep.name}/{f.name}", texto_it, avisos)
        if nn == "00":
            revisar_linea_base(ep, avisos)
            continue
        if not (raiz / "_planificacion/99_bitacora" / ep.name).is_dir():
            avisos.append(f"Falta la carpeta de bitácoras de {ep.name} en 99_bitacora/ (créala con un .gitkeep).")

    specs_dir = raiz / "_planificacion/20_especificaciones"
    if nivel != "pequeno" and not specs_dir.is_dir():
        errores.append("Falta la carpeta: _planificacion/20_especificaciones/ (desde nivel mediano; puede nacer con un .gitkeep).")
    if specs_dir.is_dir():
        specs = sorted(specs_dir.glob("*.md"))
        if nivel == "pequeno" and specs:
            avisos.append("Hay especificaciones en un proyecto de nivel pequeño: si son varias, conviene revisar el nivel de proceso.")
        for f in specs:
            rel = f.relative_to(raiz).as_posix()
            if not re.match(r"spec-[a-z0-9-]+\.md$", f.name):
                errores.append(f"Nombre de especificación fuera de convención: {rel} (se espera spec-nombre.md)")
                continue
            texto = f.read_text(encoding="utf-8")
            if not re.search(r"^- \*\*Estado:\*\* (Borrador|Aprobada|Vigente|Reemplazada)", texto, flags=re.M):
                avisos.append(f"{rel}: no declara un estado válido (Borrador, Aprobada, Vigente o Reemplazada).")
            revisar_criterios(rel, texto, errores, avisos, r"^- (\*\*)?CA-\d{2}|^- \[[ xX]\]")

    if api_rest:
        contrato = next((raiz / c for c in OPENAPI if (raiz / c).is_file()), None)
        if contrato is None:
            if nivel == "pequeno":
                avisos.append("API REST sin contrato docs/api/openapi.yaml: en nivel pequeño es recomendado, no obligatorio.")
            else:
                errores.append("Falta el contrato de la API: docs/api/openapi.yaml (obligatorio desde nivel mediano con --api-rest).")
        else:
            texto = contrato.read_text(encoding="utf-8")
            if not re.search(r'^\s*"?openapi"?\s*:\s*"?3\.', texto, flags=re.M):
                errores.append(f"{contrato.relative_to(raiz).as_posix()} no declara una versión OpenAPI 3.x.")
            if not re.search(r'^\s*"?paths"?\s*:', texto, flags=re.M):
                errores.append(f"{contrato.relative_to(raiz).as_posix()} no tiene la sección «paths».")

    for f in (raiz / "_planificacion/99_bitacora").rglob("*.md") if (raiz / "_planificacion/99_bitacora").is_dir() else []:
        if not re.match(r"bitacora-\d{2}-\d{2}(-[a-z0-9-]+)?\.md$", f.name):
            errores.append(f"Nombre de bitácora fuera de convención: {f.relative_to(raiz)}")

    for p in raiz.rglob("*"):
        rel = p.relative_to(raiz).as_posix()
        if not rel.isascii():
            errores.append(f"Nombre con caracteres no ASCII (tildes o ñ): {rel}")

    ajustes = raiz / ".claude/settings.json"
    if ajustes.is_file():
        try:
            datos = json.loads(ajustes.read_text(encoding="utf-8"))
            permisos = datos.get("permissions", {})
            for lista in ("allow", "ask", "deny"):
                if lista not in permisos:
                    errores.append(f"settings.json no tiene la lista permissions.{lista}")
            deny = permisos.get("deny", [])
            for doc in INMUTABLES:
                if f"Edit(/{doc})" not in deny:
                    errores.append(f"settings.json no deniega la edición de {doc}")
            errores.extend(conflictos_de_permisos(permisos))
        except json.JSONDecodeError as e:
            errores.append(f"settings.json no es JSON válido: {e}")

    for patron in EXTENSIONES_CON_MARCADORES:
        for f in raiz.rglob(patron):
            texto = f.read_text(encoding="utf-8")
            rel = f.relative_to(raiz).as_posix()
            for m in MARCADOR_PLANTILLA.finditer(texto):
                errores.append(f"Marcador de plantilla sin rellenar en {rel}: {m.group(0)}")

    agentes = raiz / "AGENTS.md"
    if agentes.is_file() and "definición de preparada" not in agentes.read_text(encoding="utf-8").lower():
        avisos.append("AGENTS.md no tiene la «Definición de preparada»: sin ella, el agente empieza iteraciones que no están listas.")
    if not (raiz / ".claude/commands/siguiente-epica.md").is_file():
        avisos.append("Falta .claude/commands/siguiente-epica.md (el comando /siguiente-epica de Claude Code).")

    registro = raiz / "_planificacion/00_producto/registro-pendientes.md"
    if registro.is_file() and seccion(registro.read_text(encoding="utf-8"), "Backlog") is None:
        errores.append("registro-pendientes.md no tiene la sección «Backlog».")

    pendientes = {}
    for f in list(raiz.rglob("*.md")) + list(raiz.rglob("*.yaml")) + list(raiz.rglob("*.yml")):
        texto = f.read_text(encoding="utf-8")
        rel = f.relative_to(raiz).as_posix()
        if rel in SIN_PENDIENTES_REALES:
            continue
        for linea in texto.splitlines():
            if EXPLICATIVA.search(linea):
                continue
            for m in re.finditer(r"PENDIENTE_(CLIENTE|DESARROLLADOR)", linea):
                pendientes.setdefault(m.group(1), set()).add(rel)

    revisar_registro(raiz, pendientes, errores, avisos)

    for doc in INMUTABLES:
        f = raiz / doc
        if not f.is_file():
            continue
        lineas = sin_codigo(f.read_text(encoding="utf-8")).splitlines()
        for i, linea in enumerate(lineas, 1):
            for patron, tipo_rastro in RASTROS:
                for m in re.finditer(patron, linea, flags=re.I):
                    avisos.append(f"{doc}, línea ~{i}: posible {tipo_rastro} en un documento inmutable: «{m.group(0)}»")

    if (raiz / "README.md").is_file():
        texto = (raiz / "README.md").read_text(encoding="utf-8")
        for bloque in re.findall(r"```(\w*)", texto)[::2]:
            if bloque not in ("text", ""):
                errores.append(f"README.md tiene un bloque de código «{bloque}»: el README raíz no lleva código ni comandos (solo se admite «text» para el árbol de carpetas).")

    return errores, avisos


def empaquetar(raiz: Path, salida: Path):
    salida.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(salida, "w", zipfile.ZIP_DEFLATED) as z:
        for p in sorted(raiz.rglob("*")):
            if p.is_file():
                z.write(p, p.relative_to(raiz).as_posix())
    return salida


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("directorio", type=Path)
    ap.add_argument("--tipo", required=True, choices=["frontend", "backend", "fullstack"])
    ap.add_argument("--nivel", choices=NIVELES, default="mediano", help="Nivel de proceso acordado (por defecto, mediano)")
    ap.add_argument("--api-rest", action="store_true", help="El proyecto expone una API REST")
    ap.add_argument("--zip", type=Path, help="Ruta del .zip a generar si no hay errores")
    args = ap.parse_args()

    raiz = args.directorio.resolve()
    if not raiz.is_dir():
        print(f"No existe el directorio: {raiz}")
        sys.exit(2)

    errores, avisos = validar(raiz, args.tipo, args.nivel, args.api_rest)
    for e in errores:
        print(f"ERROR: {e}")
    for a in avisos:
        print(f"AVISO: {a}")
    print(f"\n{len(errores)} error(es), {len(avisos)} aviso(s).")

    if errores:
        sys.exit(1)
    if args.zip:
        print(f"Zip generado: {empaquetar(raiz, args.zip.resolve())}")


if __name__ == "__main__":
    main()
