# Pedidos Web

Portal web de pedidos de la distribuidora. Es la modernización de **Pedidos Legado**, un sistema de escritorio hecho en Delphi con DBISAM que la empresa usa hace 18 años.

Este repositorio es el **caso hilo** del curso *Desarrollando con IA* (Vela LATAM). Crece unidad a unidad: la biblioteca de dominio (Unidad 1), una API en ASP.NET Core con 45 pedidos de ejemplo (Unidad 2), las líneas de cada pedido y un `ClientesService` heredado (Unidad 3), el código de Pedidos Legado para diagnosticarlo sin tocarlo (Unidad 4), el golden master para migrar el cálculo de totales (Unidad 5), el pipeline de CI con Claude diagnosticando las fallas (Unidad 6), el proceso de code review (Unidad 7) y, en la **Unidad 8**, la **gobernanza del uso de IA**: una política que se cumple por configuración, no de memoria.

> Todo el contenido es ficticio y tiene fines educativos.

## Stack

| Pieza | Versión | Para qué |
|---|---|---|
| .NET SDK | 10 | Compilar y ejecutar |
| C# | el que trae el SDK | Lenguaje del proyecto |
| xUnit | 2.9 | Tests |
| JetBrains Rider | la versión vigente | IDE del curso |
| Claude Code | 2.1.277 o superior, con tu Claude Pro | Agente de código (lee `AGENTS.md`) |
| Git Bash (solo Windows) | el que trae Git for Windows | Claude Code corre los hooks con bash |
| GitHub Actions | runners estándar (gratis en repos públicos) | CI y revisión en cada PR |
| Delphi 7 + DBISAM 4 | solo lectura · no se instala | El sistema legado que migras por partes |

## Estructura

```
pedidos-web/
├── AGENTS.md                         ← el tuyo (lo escribiste en la U2)
├── REVIEW.md                         ← U7
├── PedidosWeb.sln
├── .env                              ← U8: falso y solo local (lo ignora .gitignore)
├── .claude/
│   ├── settings.json                 ← U4: deny de legado/ · U5: hook PostToolUse · U8: le sumas reglas (etapa 2)
│   ├── hooks/guardian.sh             ← U8: lo creas tú en la etapa 2
│   └── agents/                       ← analista-legado (U4) y revisor (U7)
├── .github/
│   ├── workflows/                    ← U6 y U7
│   ├── pull_request_template.md      ← U7
│   └── dependabot.yml                ← U8: tu reto
├── src/  ·  tests/
├── legado/                           ← U4: código de Pedidos Legado, solo lectura
├── migracion/                        ← U5: el golden master
├── docs/
│   ├── diagnostico/  ·  migracion/  ·  calidad/  ·  checklist-revision.md
│   └── gobernanza/                   ← U8: dos plantillas                                    (nuevo)
│       ├── auditoria-legado.md
│       └── politica-ia.md
└── specs/
```

## Cómo empezar la Unidad 8

### A. Ya tienes tu repo `pedidos-web` (lo normal)

**No copies la carpeta completa**: pisarías tu `README.md`, tu `AGENTS.md`, tu `.claude/settings.json` y tu trabajo de las unidades anteriores. La U8 solo **suma** una carpeta: `docs/gobernanza/`, con dos plantillas. No reemplaza nada.

La forma recomendada es pedírselo a Claude Code, abierto en tu `pedidos-web`:

> Trae a pedidos-web solo lo nuevo de la Unidad 8 desde el repo de material. No toques nada más.

Si prefieres hacerlo tú, trabaja en la carpeta que contiene tu `pedidos-web` y el material del curso, uno al lado del otro:

```bash
# macOS, Linux o WSL
cd material-curso-ia-desarrolladores && git pull && cd ..
U8=material-curso-ia-desarrolladores/unidad-8/taller/pedidos-web
cp -r $U8/docs/gobernanza pedidos-web/docs/
```

```powershell
# Windows PowerShell
cd material-curso-ia-desarrolladores; git pull; cd ..
$U8 = "material-curso-ia-desarrolladores/unidad-8/taller/pedidos-web"
Copy-Item -Recurse "$U8/docs/gobernanza" pedidos-web/docs/
```

**No se copian:** `README.md`, `AGENTS.md`, `REVIEW.md`, `PedidosWeb.sln`, `src/`, `tests/`, `legado/`, `migracion/`, `.claude/`, `.github/`, el resto de `docs/` ni `specs/`.

### Preparación (9 min)

Crea el `.env` **falso** en la raíz (valores inventados; nunca uses una clave real). Ya está en `.gitignore`:

```bash
# macOS, Linux, WSL o Git Bash
cd pedidos-web
printf 'DB_PASSWORD=falsa-no-usar-123\nAPI_KEY=sk-falsa-000000\n' > .env
```

```powershell
# Windows PowerShell
cd pedidos-web
Set-Content .env "DB_PASSWORD=falsa-no-usar-123"
Add-Content .env "API_KEY=sk-falsa-000000"
```

Luego comprueba que todo está en verde y sube las plantillas:

```bash
claude --version     # 2.1.277 o superior
dotnet test
git status           # .env NO debe aparecer
git add docs/gobernanza
git commit -m "chore: base de la Unidad 8 (plantillas de gobernanza)"
git push
```

**Resultado esperado:** `dotnet test` da el mismo resultado que al terminar la U7 (la preparación no suma código C#) y `.claude/settings.json` sigue con el deny de `legado/` (U4) y el hook `PostToolUse` (U5).

Si `claude --version` es menor: `claude update`.

### B. No tienes tu repo de las unidades anteriores

Sigue el paso 1 del README de `unidad-1/taller/pedidos-web` (crear tu repo **público** en GitHub), pero copia esta carpeta (`unidad-8/taller/pedidos-web`) en vez de la de la Unidad 1. En este camino el `AGENTS.md` viene vacío, no hay workflows ni `REVIEW.md`, y `.claude/settings.json` trae solo el deny de `legado/`: la etapa 2 funciona igual. Si algún test falla porque esa parte no está terminada, márcalo con `Skip` y un motivo; no lo borres.

---

## Taller de la Unidad 8: lo que encuentras en el repo

La misión: **que tu `pedidos-web` cumpla su política de IA por configuración, no de memoria.** Trabajo individual en tu repo. 94 minutos.

| Etapa | Min | Dónde queda tu trabajo |
|---|---|---|
| Preparación | 9 | `docs/gobernanza/` en tu repo y el `.env` falso solo en tu equipo |
| 1 · Auditar sin creer | 25 | `docs/gobernanza/auditoria-legado.md` |
| 2 · Reglas que bloquean de verdad | 30 | `.claude/settings.json` ampliado y `.claude/hooks/guardian.sh` en `main` |
| 3 · Tu política por escrito | 15 | `docs/gobernanza/politica-ia.md` en `main` |
| Tu reto (obligatorio) | 15 | `.github/dependabot.yml` en `main` |

La guía del taller, con todo el recorrido, está en el [sitio del curso](https://ffelipecuevasc.github.io/curso-ia-desarrolladores/) → Unidad 8.

### Comandos de cada etapa

**Etapa 1 · Auditar sin creer.** El subagente `analista-legado` audita `legado/src/UFrmPedidos.pas` y `legado/src/UDMPrincipal.pas` (el pedido está en la guía). Tú abres cada archivo:línea, completas la plantilla y la subes:

```bash
git add docs/gobernanza/auditoria-legado.md
git commit -m "docs: auditoría del legado con hallazgos validados y descartados"
git push
```

**Etapa 2 · Reglas que bloquean de verdad.** Claude Code **suma** a `.claude/settings.json` (sin borrar nada) un deny para leer `.env` y `*.pem`, un ask para `curl`, `wget`, `Invoke-WebRequest`, `WebFetch` y para editar `migracion/golden-master.csv`, y crea `.claude/hooks/guardian.sh` como hook `PreToolUse`. Revisa el diff: **no debe haber líneas borradas** en `settings.json`.

Después de cada cambio en `settings.json`, sal de Claude Code (`/exit`), vuelve a abrirlo **desde la raíz** y confírmalo:

```text
/permissions     # deben aparecer las reglas nuevas y el deny de legado/
/hooks           # deben aparecer PreToolUse y PostToolUse
```

Para probar la regla de `*.pem`, crea un certificado falso (también lo ignora `.gitignore`):

```bash
mkdir -p certs && echo "falso" > certs/prueba.pem          # bash o Git Bash
```

```powershell
New-Item -ItemType Directory -Force certs | Out-Null; Set-Content certs/prueba.pem "falso"   # PowerShell
```

Intenta violar cada regla (la tabla está en la guía), también el deny de `legado/`, y anota qué pasó. Luego súbelo:

```bash
git add .claude/settings.json .claude/hooks/guardian.sh
git commit -m "chore: reglas de gobernanza y hook guardián en .claude/"
git push
```

**Etapa 3 · Tu política por escrito.** Claude Code completa `docs/gobernanza/politica-ia.md` con tus notas de la etapa 2. Tú la revisas y la subes:

```bash
git add docs/gobernanza/politica-ia.md
git commit -m "docs: política de uso de IA con controles probados"
git push
```

### Etapa extra obligatoria · Tu reto (15 min)

**Tema nuevo:** cadena de suministro. Las dependencias también son input: hay que vigilarlas.

**Tu reto:** crea `.github/dependabot.yml` para que GitHub vigile las dependencias de tu `pedidos-web`: paquetes NuGet y las acciones de tus workflows. Esta vez el pedido a Claude Code lo escribes tú.

**Cómo pensarlo:**
1. ¿Qué tipos de dependencias tiene tu repo?
2. ¿En qué carpeta busca GitHub cada una?
3. ¿Cómo compruebas en GitHub que Dependabot leyó tu archivo?

**Listo cuando:** el archivo está en `main` · en *Insights › Dependency graph › Dependabot* aparecen NuGet y GitHub Actions · si llega un PR de Dependabot, no lo mergeas hoy: lo revisas como cualquier PR.

Si no alcanzas en clase, termínalo en tu casa: es parte del entregable.

### Reglas del taller

- **`settings.json` se amplía, nunca se reemplaza.** Conserva el deny de `legado/` (U4) y el hook `PostToolUse` (U5).
- **Solo valores falsos.** El `.env` y el `.pem` de este taller son inventados y nunca se suben.
- **Una regla que no intentaste violar no sabes si funciona.** Anota también lo que **no** se bloqueó.
- **La IA propone; tú validas.** Ningún hallazgo de la auditoría se marca Validado sin abrir su línea.

### Si algo falla

- **El hook no corre:** en Windows, revisa que tengas Git Bash y reinicia Claude Code desde la raíz. Si sigue, pídele a Claude Code que revise cómo quedó registrado el hook.
- **Perdiste el deny de la U4 o el hook de la U5:** reemplazaste `settings.json`. Vuelve al último commit y suma las reglas de nuevo, sin borrar.
- **La regla «no hace nada»:** revisa `/permissions`. Si no aparece, pídele a Claude Code que valide el JSON de `settings.json` y reinicia la sesión.

### Las plantillas (`docs/gobernanza/`)

| Plantilla | Etapa | Qué completas |
|---|---|---|
| `auditoria-legado.md` | 1 | Hallazgos con archivo:línea, estado (Validado o Descartado) y argumento, más el cruce con `docs/diagnostico/riesgos.md` de la U4 |
| `politica-ia.md` | 3 | Datos que nunca entran, controles y si se probaron, lo no cubierto, 3 escenarios y la línea «Uso de IA» de los PR |

## Licencias

- Claude Pro (incluye Claude Code). Permisos, hooks y subagentes funcionan en cualquier plan.
- JetBrains Rider y .NET 10 SDK.
- GitHub gratis con tu repo **público**. Dependabot no tiene costo en repos públicos *(no verificado)*.
- No se necesita Claude Team: los planes comerciales solo se mencionan en la clase.
- No se necesita licencia de Delphi ni de DBISAM: el legado no se compila.

*(Verificar en tu equipo: `guardian.sh` es bash y solo se probó en Linux; en Windows corre con Git Bash y no está probado.)*

## Talleres anteriores

- **Unidad 1** (`FormatearFolio()` y `docs/mapa-sdlc.md`): ver el README de `unidad-1/taller/pedidos-web`.
- **Unidad 2** (API, 45 pedidos y HU-01 listar pedidos): ver el README de `unidad-2/taller/pedidos-web`.
- **Unidad 3** (HU-02 detalle del pedido, refactor de `ClientesService` y Skill `documentar-modulo`): ver el README de `unidad-3/taller/pedidos-web`.
- **Unidad 4** (diagnóstico de Pedidos Legado): ver el README de `unidad-4/taller/pedidos-web`.
- **Unidad 5** (migración del cálculo de totales con golden master): ver el README de `unidad-5/taller/pedidos-web`.
- **Unidad 6** (tests que sí prueban, `ci.yml`, `claude.yml` y la falla diagnosticada por `@claude`): ver el README de `unidad-6/taller/pedidos-web`.
- **Unidad 7** (`REVIEW.md`, subagente `revisor`, `claude-review.yml` y la plantilla de PR): ver el README de `unidad-7/taller/pedidos-web`.
