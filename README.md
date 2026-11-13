# Pedidos Web

Portal web de pedidos de la distribuidora. Es la modernización de **Pedidos Legado**, un sistema de escritorio hecho en Delphi con DBISAM que la empresa usa hace 18 años.

Este repositorio es el **caso hilo** del curso *Desarrollando con IA* (Vela LATAM). Crece unidad a unidad: la biblioteca de dominio (Unidad 1), una API en ASP.NET Core con 45 pedidos de ejemplo (Unidad 2), las líneas de cada pedido y un `ClientesService` heredado (Unidad 3), el código de Pedidos Legado para diagnosticarlo sin tocarlo (Unidad 4), el golden master para migrar el cálculo de totales (Unidad 5), el pipeline de CI con Claude diagnosticando las fallas (Unidad 6), el proceso de code review (Unidad 7), la gobernanza del uso de IA (Unidad 8), la adopción en toda la BU con un plugin instalable (Unidad 9) y, en la **Unidad 10**, el **plan integrado**: cuánto cuesta, cuánto aporta y qué sigue, en un PDF para la gerencia.

> Todo el contenido es ficticio y tiene fines educativos.

## Stack

| Pieza | Versión | Para qué |
|---|---|---|
| .NET SDK | 10 | Compilar y ejecutar |
| C# | el que trae el SDK | Lenguaje del proyecto |
| xUnit | 2.9 | Tests |
| JetBrains Rider | la versión vigente | IDE del curso |
| Claude Code | 2.1.281 o superior, con tu cuenta Claude Team | Agente de código |
| Python | 3 con Playwright y Chromium | El script de tu Skill `pdf-con-estilo` (U9) |
| Git Bash (solo Windows) | el que trae Git for Windows | Claude Code corre los hooks con bash |
| GitHub | repo **público** | Tu repo es también tu marketplace (U9) |
| Delphi 7 + DBISAM 4 | solo lectura · no se instala | El sistema legado que migras por partes |

## Estructura

```
pedidos-web/
├── AGENTS.md                         ← el tuyo (lo escribiste en la U2)
├── REVIEW.md                         ← U7
├── PedidosWeb.sln
├── .claude-plugin/marketplace.json   ← U9
├── plugins/pedidos-playbook/         ← U9
├── .claude/
│   ├── settings.json                 ← U4, U5 y U8 (no se toca en la U10)
│   ├── hooks/guardian.sh             ← U8
│   ├── agents/                       ← analista-legado (U4) y revisor (U7)
│   └── skills/                       ← documentar-modulo (U3) · pdf-con-estilo (U9)
├── .github/                          ← U6, U7 y U8
├── src/  ·  tests/
├── legado/                           ← U4: solo lectura
├── migracion/                        ← U5
├── docs/
│   ├── diagnostico/  ·  migracion/  ·  calidad/  ·  gobernanza/  ·  adopcion/
│   ├── checklist-revision.md
│   └── roi/                          ← U10: el CSV y dos plantillas                         (nuevo)
│       ├── datos-piloto.csv
│       ├── modelo-roi.md
│       └── plan-integrado.md
└── specs/
```

## Cómo empezar la Unidad 10

### Antes de la clase: tu Skill de PDF

El taller termina con tu Skill `pdf-con-estilo` de la U9, que usa Playwright. Comprueba que sigue funcionando: pídele a Claude Code que genere de nuevo `docs/adopcion/plan-evolucion.pdf` con tu Skill. Si falla, reinstala Playwright:

```bash
# macOS, Linux, WSL o Git Bash
python3 -m pip install playwright markdown
python3 -m playwright install chromium
```

```powershell
# Windows PowerShell
py -m pip install playwright markdown
py -m playwright install chromium
```

### A. Ya tienes tu repo `pedidos-web` (lo normal)

**No copies la carpeta completa**: pisarías tu `README.md`, tu `AGENTS.md`, tu `.claude/`, tu plugin y tu trabajo de las unidades anteriores. La U10 solo **suma** una carpeta: `docs/roi/`, con el CSV y dos plantillas. No reemplaza nada.

La forma recomendada es pedírselo a Claude Code, abierto en tu `pedidos-web`:

> Trae a mi repo pedidos-web lo nuevo de la Unidad 10: la carpeta docs/roi/ con datos-piloto.csv, modelo-roi.md y plan-integrado.md. No toques ningún otro archivo y muéstrame el diff antes del commit.

Si prefieres hacerlo tú, trabaja en la carpeta que contiene tu `pedidos-web` y el material del curso, uno al lado del otro:

```bash
# macOS, Linux o WSL
cd material-curso-ia-desarrolladores && git pull && cd ..
U10=material-curso-ia-desarrolladores/unidad-10/taller/pedidos-web
cp -r $U10/docs/roi pedidos-web/docs/
```

```powershell
# Windows PowerShell
cd material-curso-ia-desarrolladores; git pull; cd ..
$U10 = "material-curso-ia-desarrolladores/unidad-10/taller/pedidos-web"
Copy-Item -Recurse "$U10/docs/roi" pedidos-web/docs/
```

**No se copian:** `README.md`, `AGENTS.md`, `REVIEW.md`, `PedidosWeb.sln`, `src/`, `tests/`, `legado/`, `migracion/`, `.claude/`, `.claude-plugin/`, `plugins/`, `.github/`, el resto de `docs/` ni `specs/`.

### Preparación (5 min)

Sube los archivos nuevos:

```bash
git status           # solo docs/roi/ debe aparecer como nuevo
git add docs/roi
git commit -m "chore: base de la Unidad 10 (datos del piloto y plantillas)"
git push
```

**Resultado esperado:** el diff solo muestra los 3 archivos de `docs/roi/` y tu Skill `pdf-con-estilo` genera un PDF de prueba.

### B. No tienes tu repo de las unidades anteriores

Sigue el paso 1 del README de `unidad-1/taller/pedidos-web` (crear tu repo en GitHub), pero copia esta carpeta (`unidad-10/taller/pedidos-web`) en vez de la de la Unidad 1. En este camino el `AGENTS.md` viene vacío, no hay plugin ni Skill `pdf-con-estilo`, y las plantillas de las unidades anteriores (`docs/migracion/`, `docs/calidad/`, `docs/gobernanza/`, `docs/adopcion/`) vienen sin completar: en la etapa 1, anota FALTA en la evidencia propia que no tengas. Para la etapa 3, pídele a Claude Code que cree la Skill `pdf-con-estilo` siguiendo el README de la Unidad 9.

---

## Taller de la Unidad 10: lo que encuentras en el repo

La misión: **decirle a la gerencia, en un PDF, cuánto cuesta, cuánto aporta y qué sigue con la forma de trabajar de Pedidos Web.** Trabajo individual en tu repo. 90 minutos.

| Etapa | Min | Dónde queda tu trabajo |
|---|---|---|
| Preparación | 5 | `docs/roi/` en tu repo |
| 1 · Medir | 20 | La sección «Datos» de `docs/roi/modelo-roi.md` |
| 2 · ROI con supuestos | 30 | `docs/roi/modelo-roi.md` completo: supuestos, costos, escenarios y sensibilidad |
| 3 · Plan integrado en PDF | 35 | `docs/roi/plan-integrado.md` y `docs/roi/plan-integrado.pdf` en `main` |
| Tu reto (obligatorio) | — | La sección «Punto de equilibrio» en tu modelo y en tu PDF |

La guía del taller, con todo el recorrido y los pedidos para Claude Code, está en el [sitio del curso](https://ffelipecuevasc.github.io/curso-ia-desarrolladores/) → Unidad 10.

### El CSV del piloto

`docs/roi/datos-piloto.csv` trae 12 semanas del equipo Pedidos Web (6 devs): las semanas 1 a 6 son **antes** y las 7 a 12 son **con IA**. Los datos son **ficticios**.

| Columna | Qué es |
|---|---|
| `semana` · `periodo` | Número de semana y `antes` o `con-ia` |
| `despliegues` | Despliegues a producción en la semana |
| `lead_time_dias` | Días promedio desde el commit hasta producción |
| `cambios_con_falla` | Despliegues que necesitaron un rollback o un hotfix |
| `horas_recuperacion` | Horas para recuperarse de los despliegues fallidos |
| `defectos_escapados` | Defectos que llegaron a producción |
| `horas_retrabajo` | Horas del equipo rehaciendo trabajo |
| `horas_ahorradas_equipo` | Horas que el equipo reporta haber ahorrado con IA |

### Comandos de cada etapa

**Etapa 1 · Medir.** Claude Code lee el CSV y tus entregables de las unidades 5 a 9, y completa la sección «Datos» de la plantilla (el pedido está en la guía). Tú revisas que cada número tenga archivo y semanas. No hay commit todavía.

**Etapa 2 · ROI con supuestos.** Claude Code completa el resto de `modelo-roi.md`. Tú revisas a mano al menos una cuenta y lo subes:

```bash
git add docs/roi/modelo-roi.md
git commit -m "docs: modelo de ROI del piloto de Pedidos Web"
git push
```

**Etapa 3 · Plan integrado en PDF.** Claude Code completa `plan-integrado.md` y genera `docs/roi/plan-integrado.pdf` con tu Skill `pdf-con-estilo`, sin tocar su CSS. Ábrelo: la recomendación debe estar en la página 1. Luego:

```bash
git add docs/roi
git commit -m "docs: plan integrado de adopción de IA en PDF"
git push
```

### Etapa extra obligatoria · Tu reto

**Tema nuevo:** el punto de equilibrio, donde el beneficio apenas cubre el costo.

**Tu reto:** calcula cuántas horas ahorradas por dev al mes dejan el ROI en 0, súmalo como sección a tu modelo y a tu PDF, y vuelve a subir ambos. Esta vez el pedido a Claude Code lo escribes tú.

**Cómo pensarlo:**
1. ¿Qué vale el ROI justo en el punto de equilibrio?
2. ¿Qué supuesto despejas y cuáles dejas fijos?
3. ¿Cómo se lo dices a la gerencia en una sola línea?

**Listo cuando:** `modelo-roi.md` y `plan-integrado.pdf` tienen la sección «Punto de equilibrio» y están en `main`.

Si no alcanzas en clase, termínalo en tu casa con Claude Pro: es parte del entregable.

### Reglas del taller

- **La U10 solo suma.** Nada fuera de `docs/roi/` cambia: ni el código, ni `.claude/`, ni tu plugin.
- **Ningún número sin supuesto.** Cada cifra lleva su tipo (oficial, medido o estimado) y su fuente.
- **El ROI es un rango.** Siempre dos escenarios y la sensibilidad.
- **Los datos son ficticios y la muestra es pequeña:** este ROI es un ejercicio de método, no una prueba de productividad.

### Si algo falla

- **El ROI da 900 %:** revisa si multiplicaste una sola tarea por todo el horizonte.
- **Los costos son solo los asientos:** faltan la capacitación y la curva de adopción.
- **Claude agregó un dato que no está en el CSV:** cada número necesita archivo y semanas; si no está, dice FALTA.
- **La Skill no genera el PDF:** revisa que Playwright esté instalado (arriba) y que la Skill esté en `.claude/skills/pdf-con-estilo/`.
