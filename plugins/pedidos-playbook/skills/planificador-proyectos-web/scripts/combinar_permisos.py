#!/usr/bin/env python3
"""Arma el .claude/settings.json de un proyecto combinando los fragmentos de permisos
de assets/permisos/: siempre base.json, más uno por cada pieza del stack.

Uso:
    python combinar_permisos.py --stack astro-pnpm --salida <proyecto>/.claude/settings.json
    python combinar_permisos.py --stack astro-pnpm:frontend --stack django:backend --api-rest \
        --salida <proyecto>/.claude/settings.json

- `--stack nombre` usa el fragmento assets/permisos/<nombre>.json.
- `--stack nombre:carpeta` sirve para un monorrepo: las reglas de edición del fragmento
  (`Edit(/src/**)`) se aplican dentro de esa carpeta (`Edit(/frontend/src/**)`) y se permite
  `cd carpeta`. Las reglas de base.json no se mueven: valen para la raíz.
- `--api-rest` permite editar el contrato docs/api/**.
- `--listar` muestra los fragmentos disponibles.

El resultado conserva el orden de grupos de la skill (comandos del proyecto, Git, sistema de
archivos, edición, lectura, web), sin reglas repetidas, y avisa si una regla de allow o ask
queda anulada por una de deny. Después hay que revisar y ajustar el archivo al proyecto real:
los fragmentos son un punto de partida, no un formulario.
"""

import argparse
import fnmatch
import json
import re
import sys
from pathlib import Path

PERMISOS = Path(__file__).resolve().parent.parent / "assets" / "permisos"
GRUPOS = ["comandos", "git", "sistema", "edicion", "lectura", "web"]
LISTAS = ["allow", "ask", "deny"]


def cargar(nombre: str) -> dict:
    ruta = PERMISOS / f"{nombre}.json"
    if not ruta.is_file():
        disponibles = ", ".join(sorted(p.stem for p in PERMISOS.glob("*.json") if p.stem != "base"))
        sys.exit(f"No existe el fragmento «{nombre}». Disponibles: {disponibles}")
    return json.loads(ruta.read_text(encoding="utf-8"))


def con_carpeta(regla: str, carpeta: str) -> str:
    """Edit(/src/**) -> Edit(/frontend/src/**); Read(./x) -> Read(./frontend/x)."""
    m = re.match(r"^(Edit|Read)\((\.?/)(.*)\)$", regla)
    if not m:
        return regla
    herramienta, raiz, resto = m.groups()
    return f"{herramienta}({raiz}{carpeta.strip('/')}/{resto})"


def conflictos(permisos: dict) -> list:
    def partes(regla):
        m = re.match(r"^(\w+)\((.*)\)$", regla)
        return (m.group(1), m.group(2)) if m else (None, None)

    salida = []
    for lista in ("allow", "ask"):
        for regla in permisos[lista]:
            h1, valor = partes(regla)
            for negada in permisos["deny"]:
                h2, patron = partes(negada)
                if h1 and h1 == h2 and regla != negada and fnmatch.fnmatchcase(valor, patron):
                    salida.append(f"«{regla}» en {lista} queda anulada por «{negada}» en deny.")
    return salida


def combinar(stacks: list, api_rest: bool) -> dict:
    grupos = {lista: {g: [] for g in GRUPOS} for lista in LISTAS}

    def agregar(fragmento: dict, carpeta: str = ""):
        for lista in LISTAS:
            for grupo, reglas in fragmento.get(lista, {}).items():
                destino = grupos[lista][grupo]
                for regla in reglas:
                    regla = con_carpeta(regla, carpeta) if carpeta else regla
                    if regla not in destino:
                        destino.append(regla)

    agregar(cargar("base"))
    for item in stacks:
        nombre, _, carpeta = item.partition(":")
        agregar(cargar(nombre), carpeta)
        if carpeta:
            regla = f"Bash(cd {carpeta.strip('/')})"
            if regla not in grupos["allow"]["sistema"]:
                grupos["allow"]["sistema"].append(regla)
    if api_rest:
        grupos["allow"]["edicion"].append("Edit(/docs/api/**)")
    return grupos


def escribir(grupos: dict) -> str:
    """JSON válido con una línea en blanco entre grupos, como la plantilla de la skill."""
    lineas = ["{", '  "$schema": "https://json.schemastore.org/claude-code-settings.json",',
              '  "permissions": {', '    "defaultMode": "default",']
    for i, lista in enumerate(LISTAS):
        bloques = [g for g in GRUPOS if grupos[lista][g]]
        lineas.append(f'    "{lista}": [')
        reglas_totales = [(g, r) for g in bloques for r in grupos[lista][g]]
        for j, (grupo, regla) in enumerate(reglas_totales):
            if j > 0 and reglas_totales[j - 1][0] != grupo:
                lineas.append("")
            coma = "," if j < len(reglas_totales) - 1 else ""
            lineas.append(f"      {json.dumps(regla, ensure_ascii=False)}{coma}")
        lineas.append("    ]" + ("," if i < len(LISTAS) - 1 else ""))
    lineas += ["  }", "}", ""]
    return "\n".join(lineas)


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--stack", action="append", default=[], help="Fragmento, opcionalmente con carpeta: nombre[:carpeta]")
    ap.add_argument("--api-rest", action="store_true", help="Permite editar docs/api/**")
    ap.add_argument("--salida", type=Path, help="Ruta del settings.json a escribir")
    ap.add_argument("--listar", action="store_true", help="Muestra los fragmentos disponibles")
    args = ap.parse_args()

    if args.listar:
        for p in sorted(PERMISOS.glob("*.json")):
            print(f"{p.stem:14} {json.loads(p.read_text(encoding='utf-8')).get('descripcion', '')}")
        return
    if not args.stack or not args.salida:
        ap.error("indica al menos un --stack y la --salida")

    grupos = combinar(args.stack, args.api_rest)
    texto = escribir(grupos)
    datos = json.loads(texto)  # comprueba que el resultado sea JSON válido
    problemas = conflictos(datos["permissions"])
    args.salida.parent.mkdir(parents=True, exist_ok=True)
    args.salida.write_text(texto, encoding="utf-8")
    total = sum(len(datos["permissions"][l]) for l in LISTAS)
    print(f"settings.json escrito en {args.salida} ({total} reglas).")
    for p in problemas:
        print(f"AVISO: {p}")


if __name__ == "__main__":
    main()
