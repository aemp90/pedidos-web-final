# Pedidos Web

Portal web de pedidos de la distribuidora. Es la modernización de **Pedidos Legado**, un sistema de escritorio hecho en Delphi con DBISAM que la empresa usa hace 18 años.

Este repositorio es el **caso hilo** del curso *Desarrollando con IA* (Vela LATAM). Crece unidad a unidad: la biblioteca de dominio (Unidad 1), una API en ASP.NET Core con 45 pedidos de ejemplo (Unidad 2), las líneas de cada pedido y un `ClientesService` heredado (Unidad 3), el código de Pedidos Legado para diagnosticarlo sin tocarlo (Unidad 4), el golden master para migrar el cálculo de totales (Unidad 5), el pipeline de CI con Claude diagnosticando las fallas (Unidad 6), el proceso de code review (Unidad 7), la gobernanza del uso de IA (Unidad 8) y, en la **Unidad 9**, la **adopción en toda la BU**: tu forma de trabajar empaquetada como un plugin que otro equipo instala con un comando, y un plan de evolución en PDF.

> Todo el contenido es ficticio y tiene fines educativos.

## Stack

| Pieza | Versión | Para qué |
|---|---|---|
| .NET SDK | 10 | Compilar y ejecutar |
| C# | el que trae el SDK | Lenguaje del proyecto |
| xUnit | 2.9 | Tests |
| JetBrains Rider | la versión vigente | IDE del curso |
| Claude Code | 2.1.281 o superior, con tu cuenta Claude Team | Agente de código, plugins y marketplaces |
| Python | 3 con Playwright y Chromium | El script de tu Skill `pdf-con-estilo` (U9) |
| Git Bash (solo Windows) | el que trae Git for Windows | Claude Code corre los hooks con bash |
| GitHub | repo **público** | Tu repo es también tu marketplace |
| Delphi 7 + DBISAM 4 | solo lectura · no se instala | El sistema legado que migras por partes |

## Estructura

```
pedidos-web/
├── AGENTS.md                         ← el tuyo (lo escribiste en la U2)
├── REVIEW.md                         ← U7
├── PedidosWeb.sln
├── .claude-plugin/marketplace.json   ← U9: lo creas tú en la etapa 2
├── plugins/pedidos-playbook/         ← U9: lo creas tú en la etapa 2
├── .claude/
│   ├── settings.json                 ← U4, U5 y U8 (no se toca en la U9)
│   ├── hooks/guardian.sh             ← U8
│   ├── agents/                       ← analista-legado (U4) y revisor (U7)
│   └── skills/                       ← documentar-modulo (U3) · pdf-con-estilo (U9: etapa 3)
├── .github/                          ← U6, U7 y U8
├── src/  ·  tests/
├── legado/                           ← U4: solo lectura
├── migracion/                        ← U5
├── docs/
│   ├── diagnostico/  ·  migracion/  ·  calidad/  ·  gobernanza/  ·  checklist-revision.md
│   └── adopcion/                     ← U9: el CSV y una plantilla                           (nuevo)
│       ├── analytics-ficticio.csv
│       └── plan-evolucion.md
└── specs/
```

## Cómo empezar la Unidad 9

### Antes de la clase: Playwright

Tu Skill de PDF usa Playwright. Instálalo antes de llegar (tarda unos minutos):

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

**No copies la carpeta completa**: pisarías tu `README.md`, tu `AGENTS.md`, tu `.claude/` y tu trabajo de las unidades anteriores. La U9 solo **suma** una carpeta: `docs/adopcion/`, con el CSV y una plantilla. No reemplaza nada.

La forma recomendada es pedírselo a Claude Code, abierto en tu `pedidos-web`:

> Trae a mi repo pedidos-web lo nuevo de la Unidad 9: docs/adopcion/analytics-ficticio.csv y docs/adopcion/plan-evolucion.md. No toques ningún otro archivo y muéstrame el diff antes del commit.

Si prefieres hacerlo tú, trabaja en la carpeta que contiene tu `pedidos-web` y el material del curso, uno al lado del otro:

```bash
# macOS, Linux o WSL
cd material-curso-ia-desarrolladores && git pull && cd ..
U9=material-curso-ia-desarrolladores/unidad-9/taller/pedidos-web
cp -r $U9/docs/adopcion pedidos-web/docs/
```

```powershell
# Windows PowerShell
cd material-curso-ia-desarrolladores; git pull; cd ..
$U9 = "material-curso-ia-desarrolladores/unidad-9/taller/pedidos-web"
Copy-Item -Recurse "$U9/docs/adopcion" pedidos-web/docs/
```

**No se copian:** `README.md`, `AGENTS.md`, `REVIEW.md`, `PedidosWeb.sln`, `src/`, `tests/`, `legado/`, `migracion/`, `.claude/`, `.github/`, el resto de `docs/` ni `specs/`.

### Preparación (7 min)

Comprueba tu versión de Claude Code y sube los archivos nuevos:

```bash
claude --version     # 2.1.281 o superior
git status           # solo docs/adopcion/ debe aparecer como nuevo
git add docs/adopcion
git commit -m "chore: base de la Unidad 9 (datos de adopción)"
git push
```

Si `claude --version` es menor: `claude update`.

**Resultado esperado:** el diff solo muestra los 2 archivos de `docs/adopcion/`, tu repo sigue **público** en GitHub y sabes qué compañero de tu grupo de 3 instalará tu plugin.

### B. No tienes tu repo de las unidades anteriores

Sigue el paso 1 del README de `unidad-1/taller/pedidos-web` (crear tu repo **público** en GitHub), pero copia esta carpeta (`unidad-9/taller/pedidos-web`) en vez de la de la Unidad 1. En este camino el `AGENTS.md` viene vacío, no hay workflows, `REVIEW.md`, subagente `revisor`, Skill `documentar-modulo` ni `guardian.sh`, y `.claude/settings.json` trae solo el deny de `legado/`. Para la etapa 2, pídele a Claude Code que cree las piezas que te faltan siguiendo los README de las unidades 3, 7 y 8: el plugin se arma igual.

---

## Taller de la Unidad 9: lo que encuentras en el repo

La misión: **que otro equipo instale con un comando la forma de trabajar de Pedidos Web, y entregar el plan para que la BU la adopte.** Trabajo individual en tu repo; tu grupo de 3 sirve para que un compañero instale tu plugin. 90 minutos.

| Etapa | Min | Dónde queda tu trabajo |
|---|---|---|
| Preparación | 7 | `docs/adopcion/` en tu repo |
| 1 · Diagnóstico | 18 | El diagnóstico en `docs/adopcion/plan-evolucion.md` |
| 2 · Playbook instalable | 35 | `.claude-plugin/marketplace.json` y `plugins/pedidos-playbook/` en `main`, instalado por tu compañero |
| 3 · El plan en PDF | 20 | `.claude/skills/pdf-con-estilo/` y `docs/adopcion/plan-evolucion.pdf` en `main` |
| Tu reto (obligatorio) | 10 | `pedidos-playbook` en versión 1.1.0, recibida por tu compañero |

La guía del taller, con todo el recorrido y los pedidos para Claude Code, está en el [sitio del curso](https://ffelipecuevasc.github.io/curso-ia-desarrolladores/) → Unidad 9.

### Comandos de cada etapa

**Etapa 1 · Diagnóstico.** Claude Code lee el CSV y completa el diagnóstico de la plantilla (el pedido está en la guía). Tú revisas que cada nivel tenga dos indicadores del CSV y lo subes:

```bash
git add docs/adopcion/plan-evolucion.md
git commit -m "docs: diagnóstico de adopción de la BU"
git push
```

**Etapa 2 · Playbook instalable.** Claude Code arma `plugins/pedidos-playbook/` con tu revisor (U7), tu Skill `documentar-modulo` (U3), la Skill `planificador-proyectos-web`, tu guardián (U8), una plantilla de `AGENTS.md` y el MCP `claude-academy`, y crea `.claude-plugin/marketplace.json` con el nombre `pedidos-<tu-usuario-github>`. Si Claude Code necesita la Skill del planificador, está en [github.com/ffelipecuevasc/planificador-proyectos-web](https://github.com/ffelipecuevasc/planificador-proyectos-web) (carpeta `skills/planificador-proyectos-web/`).

Valida desde la terminal, en la raíz de tu repo y en la carpeta del plugin:

```bash
claude plugin validate .
claude plugin validate plugins/pedidos-playbook
```

Súbelo (tu compañero instala desde GitHub, así que sin push no hay plugin):

```bash
git add .claude-plugin plugins
git commit -m "feat: plugin pedidos-playbook 1.0.0 y marketplace"
git push
```

**Punto de control: tu compañero instala tu plugin.** Él abre Claude Code en **su** `pedidos-web` y escribe, cambiando `<usuario>` por tu usuario de GitHub:

```text
/plugin marketplace add <usuario>/pedidos-web
/plugin install pedidos-playbook@pedidos-<usuario>
/mcp
```

**Resultado esperado:** `/plugin` muestra `pedidos-playbook` instalado y `/mcp` muestra `claude-academy`. Luego cambian roles: tú instalas el suyo.

**Etapa 3 · El plan en PDF.** Claude Code crea la Skill `.claude/skills/pdf-con-estilo/` (`SKILL.md`, `assets/plantilla.html` y `scripts/html_a_pdf.py` con Playwright) y genera `docs/adopcion/plan-evolucion.pdf`. Para la prueba de reutilización, pídele que use la misma Skill con `docs/gobernanza/politica-ia.md`: debe salir con el mismo diseño, sin tocar el CSS. Ese segundo PDF no se sube. Luego:

```bash
git add .claude/skills/pdf-con-estilo docs/adopcion
git commit -m "docs: plan de evolución en PDF con la Skill pdf-con-estilo"
git push
```

### Etapa extra obligatoria · Tu reto (10 min)

**Tema nuevo:** versionado de un plugin. Un plugin instalado es una copia: tu compañero no ve tus cambios hasta que los publicas como versión nueva.

**Tu reto:** haz un cambio tuyo en `pedidos-playbook` (por ejemplo, mejora la `description` de una Skill o suma una regla al revisor), publícalo como **1.1.0** y logra que tu compañero lo reciba. Esta vez el pedido a Claude Code lo escribes tú.

**Cómo pensarlo:**
1. ¿Qué campo de tu plugin le avisa a Claude Code que hay algo nuevo?
2. ¿Qué recibe tu compañero si haces push sin cambiar ese campo?
3. ¿Qué tiene que hacer tu compañero para traer la versión nueva?

**Listo cuando:** tu compañero ve la versión 1.1.0 en `/plugin` y tu cambio funciona en su equipo.

Si no alcanzas en clase, termínalo en tu casa: es parte del entregable.

### Reglas del taller

- **La U9 solo suma.** No reemplaces `settings.json`, `AGENTS.md` ni nada de las unidades anteriores: el plugin **copia** tus piezas, no las mueve.
- **Nada de rutas de tu máquina en el plugin.** Todo lo que el plugin ejecuta se ubica con `${CLAUDE_PLUGIN_ROOT}`.
- **Repo público y sin secretos.** El `.env` y el `.pem` falsos de la U8 siguen fuera del repo.
- **Un plugin que solo funciona en tu equipo no es un playbook.** La prueba es que tu compañero lo instale.

### Si algo falla

- **Tu compañero no logra agregar tu marketplace:** revisa que tu repo sea público, que hiciste push y que `marketplace.json` esté en `.claude-plugin/` en la raíz.
- **La validación marca una ruta del plugin:** la ruta de `source` parte de la raíz del marketplace con `./` y la carpeta existe.
- **El guardián no corre en el equipo de tu compañero:** la ruta del script usa `${CLAUDE_PLUGIN_ROOT}` y no una ruta de tu máquina. En Windows, revisa Git Bash.
- **`claude-academy` no aparece en `/mcp`:** `.mcp.json` va en la raíz del plugin, no dentro de `.claude-plugin/`. Tu compañero puede reiniciar Claude Code o usar `/reload-plugins`.
- **El PDF sale sin colores de fondo:** el script debe imprimir el fondo y el CSS no debe desactivarlo.

### Lo que encuentras en `docs/adopcion/`

| Archivo | Etapa | Qué es |
|---|---|---|
| `analytics-ficticio.csv` | 1 | 4 equipos (Pedidos Web, Facturacion, Bodega y Reportes) × 8 semanas. Columnas: `semana`, `equipo`, `usuarios_activos`, `sesiones`, `lineas_aceptadas`, `tasa_aceptacion`, `pr_con_cc`, `pr_totales`. Una `tasa_aceptacion` vacía significa que esa semana no hubo sesiones. |
| `plan-evolucion.md` | 1 y 3 | La plantilla del plan: resumen, diagnóstico, objetivos, playbook, hoja de ruta, indicadores y riesgos. |

## Licencias

- Claude Team Standard (tu cuenta del curso), con Claude Code. Plugins, marketplaces, hooks y MCP también funcionan con Claude Pro.
- JetBrains Rider y .NET 10 SDK.
- Python 3 con Playwright (gratis).
- GitHub gratis con tu repo **público**.
- El MCP de Claude Academy es público y no pide cuenta.
- No se necesita licencia de Delphi ni de DBISAM: el legado no se compila.

*(Verificar en tu equipo: los comandos de instalación del compañero y el hook del plugin en Windows no se probaron al preparar este material.)*

## Talleres anteriores

- **Unidad 1** (`FormatearFolio()` y `docs/mapa-sdlc.md`): ver el README de `unidad-1/taller/pedidos-web`.
- **Unidad 2** (API, 45 pedidos y HU-01 listar pedidos): ver el README de `unidad-2/taller/pedidos-web`.
- **Unidad 3** (HU-02 detalle del pedido, refactor de `ClientesService` y Skill `documentar-modulo`): ver el README de `unidad-3/taller/pedidos-web`.
- **Unidad 4** (diagnóstico de Pedidos Legado): ver el README de `unidad-4/taller/pedidos-web`.
- **Unidad 5** (migración del cálculo de totales con golden master): ver el README de `unidad-5/taller/pedidos-web`.
- **Unidad 6** (tests que sí prueban, `ci.yml`, `claude.yml` y la falla diagnosticada por `@claude`): ver el README de `unidad-6/taller/pedidos-web`.
- **Unidad 7** (`REVIEW.md`, subagente `revisor`, `claude-review.yml` y la plantilla de PR): ver el README de `unidad-7/taller/pedidos-web`.
- **Unidad 8** (auditoría del legado, reglas de `.claude/settings.json`, `guardian.sh`, política de IA y `dependabot.yml`): ver el README de `unidad-8/taller/pedidos-web`.
