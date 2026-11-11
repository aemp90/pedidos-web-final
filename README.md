# Pedidos Web

Portal web de pedidos de la distribuidora. Es la modernización de **Pedidos Legado**, un sistema de escritorio hecho en Delphi con DBISAM que la empresa usa hace 18 años.

Este repositorio es el **caso hilo** del curso *Desarrollando con IA* (Vela LATAM). Crece unidad a unidad: la biblioteca de dominio (Unidad 1), una API en ASP.NET Core con 45 pedidos de ejemplo (Unidad 2), las líneas de cada pedido y un `ClientesService` heredado (Unidad 3), el código de Pedidos Legado para diagnosticarlo sin tocarlo (Unidad 4) y, en la **Unidad 5**, el **golden master** para migrar a C# el cálculo de totales. Después viene el pipeline de CI.

> Todo el contenido es ficticio y tiene fines educativos.

## Stack

| Pieza | Versión | Para qué |
|---|---|---|
| .NET SDK | 10 | Compilar y ejecutar |
| C# | el que trae el SDK | Lenguaje del proyecto |
| xUnit | 2.9 | Tests |
| JetBrains Rider | la versión vigente del All Products Pack | IDE del curso |
| Claude Code | la versión vigente | Agente de código (lee `AGENTS.md`) |
| Delphi 7 + DBISAM 4 | solo lectura · no se instala | El sistema legado que migras por partes |

## Estructura

```
pedidos-web/
├── AGENTS.md                         ← el tuyo (lo escribiste en la U2)
├── PedidosWeb.sln
├── .claude/
│   ├── settings.json                 ← U4: regla deny sobre legado/ (U5: le sumas el hook)
│   └── agents/analista-legado.md     ← U4: subagente de solo lectura
├── src/
│   ├── PedidosWeb.Core/              ← lógica de dominio (U5: aquí creas Totales/)
│   └── PedidosWeb.Api/               ← U2: API REST (Minimal API)
├── tests/
│   └── PedidosWeb.Core.Tests/        ← (U5: aquí creas Totales/)
├── legado/                           ← U4: código de Pedidos Legado, solo lectura
├── migracion/                        ← U5: el contrato de la migración                 (nuevo)
│   ├── README.md                     ← qué es cada columna
│   └── golden-master.csv             ← 60 casos, uno por pedido
├── docs/
│   ├── diagnostico/                  ← U4: tu diagnóstico
│   └── migracion/                    ← U5: 2 plantillas                                (nuevo)
│       ├── defectos-corregidos.md
│       └── plan-migracion.md
└── specs/                            ← U2: una carpeta por historia
```

## Cómo empezar la Unidad 5

### A. Ya tienes tu repo `pedidos-web` (lo normal)

**No copies la carpeta completa**: pisarías tu `README.md`, tu `AGENTS.md`, tu API y tu trabajo de las unidades anteriores. La U5 solo **suma** dos carpetas: `migracion/` y `docs/migracion/`. No reemplaza nada.

La forma recomendada es pedírselo a Claude Code, abierto en tu `pedidos-web`:

> Trae a pedidos-web solo lo nuevo de la Unidad 5 desde el repo de material (migracion/ y docs/migracion/). No toques nada más y explícame cada paso.

Si prefieres hacerlo tú, trabaja en la carpeta que contiene tu `pedidos-web` y el material del curso, uno al lado del otro:

```bash
# macOS, Linux o WSL
cd material-curso-ia-desarrolladores && git pull && cd ..
U5=material-curso-ia-desarrolladores/unidad-5/taller/pedidos-web
cp -r $U5/migracion pedidos-web/
cp -r $U5/docs/migracion pedidos-web/docs/
```

```powershell
# Windows PowerShell
cd material-curso-ia-desarrolladores; git pull; cd ..
$U5 = "material-curso-ia-desarrolladores/unidad-5/taller/pedidos-web"
Copy-Item -Recurse "$U5/migracion" pedidos-web/
Copy-Item -Recurse "$U5/docs/migracion" pedidos-web/docs/
```

**No se copian:** `README.md`, `AGENTS.md`, `PedidosWeb.sln`, `src/`, `tests/`, `legado/`, `specs/`, `.claude/` ni `docs/README.md`. Las carpetas `Totales/` de `src/` y `tests/` las crea Claude Code en la etapa 1, y el hook se **suma** a tu `.claude/settings.json` en la etapa 2.

Comprueba y guarda el punto de partida:

```bash
cd pedidos-web
dotnet test
git add .
git commit -m "chore: base de la Unidad 5 (golden master y plantillas de migración)"
git push
```

**Resultado esperado:** `dotnet test` da el mismo resultado que al terminar la U4. La preparación no suma código C#.

### B. No tienes tu repo de las unidades anteriores

Sigue el paso 1 del README de `unidad-1/taller/pedidos-web` (crear tu repo en GitHub), pero copia esta carpeta (`unidad-5/taller/pedidos-web`) en vez de la de la Unidad 1. Trae también `.claude/` con la regla deny y el subagente `analista-legado`. En este camino el `AGENTS.md` viene vacío y los 3 tests de `FolioFormatterTests` fallan con `NotImplementedException`: es normal, esa función es del taller de la Unidad 1. Las plantillas de `docs/diagnostico/` vienen sin completar.

---

## Taller de la Unidad 5: lo que encuentras en el repo

La misión: **migrar el cálculo de totales sin perder un peso.** Llevas `legado/src/UCalculoTotales.pas` a C# sin que ningún cliente pague un peso distinto, salvo el defecto que corriges y registras.

| Etapa | Min | Dónde queda tu trabajo |
|---|---|---|
| Preparación | 10 | `migracion/` y `docs/migracion/` en tu repo |
| 1 · Congelar | 20 | `tests/PedidosWeb.Core.Tests/Totales/` (60 tests en rojo) y `src/PedidosWeb.Core/Totales/` (solo la firma) |
| 2 · Migrar | 25 | `src/PedidosWeb.Core/Totales/` (60 de 60 en verde) y el hook en `.claude/settings.json` |
| 3 · Corregir y planificar | 20 | Un commit aparte con la corrección, `docs/migracion/defectos-corregidos.md` y `docs/migracion/plan-migracion.md` |
| Cierre | 5 | — |

La guía del taller, con todo el recorrido, está en el [sitio del curso](https://ffelipecuevasc.github.io/curso-ia-desarrolladores/) → Unidad 5.

### El golden master (`migracion/golden-master.csv`)

60 casos, uno por pedido, con lo que calcula hoy `UCalculoTotales.pas`, **defectos incluidos**. Es el contrato: el C# tiene que igualarlo y **el CSV no se edita para que un test pase**. Solo cambia en la etapa 3, y cada caso que cambia queda registrado. Las columnas están en `migracion/README.md`.

Para que los tests lo encuentren, el proyecto de tests debe copiarlo a su carpeta de salida. Claude Code lo resuelve; si te lo pregunta, esto va en `tests/PedidosWeb.Core.Tests/PedidosWeb.Core.Tests.csproj`:

```xml
<ItemGroup>
  <None Include="..\..\migracion\golden-master.csv" Link="golden-master.csv" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

### El hook de la etapa 2

Un hook `PostToolUse` corre `dotnet test` después de cada edición de Claude. Si un test falla, sale con código 2 y Claude recibe el error para corregirlo. Se **suma** a tu `.claude/settings.json`, junto a lo que ya tienes (por ejemplo, la regla deny de la U4):

```json
"hooks": {
  "PostToolUse": [
    {
      "matcher": "Edit|Write",
      "hooks": [
        { "type": "command", "command": "cd \"$CLAUDE_PROJECT_DIR\" && dotnet test --nologo 1>&2 || exit 2" }
      ]
    }
  ]
}
```

Después de agregarlo, **cierra y vuelve a abrir Claude Code**, y comprueba con `/hooks` que aparece. Actívalo en la etapa 2, cuando ya tienes tests: mientras haya tests en rojo, cada edición de Claude recibe el error. *(Verificar en tu equipo: el comando usa sintaxis de bash; en Windows no está probado.)*

### Las plantillas (`docs/migracion/`)

| Plantilla | Etapa | Qué completas |
|---|---|---|
| `defectos-corregidos.md` | 3 | El defecto que corriges (qué hacía, qué casos cambian, el commit) y el hallazgo que no corriges |
| `plan-migracion.md` | 3 | Etapas, convivencia, riesgos y criterio de corte, en una página |

### Si te trabas en la etapa 1

Usa la **carpeta** `unidad-5/taller/u5-respaldo` del repo de material (no es una rama): trae el lector del golden master y un `CalculoTotales` vacío para saltar a la etapa 2. Su README explica cómo traerlo.

### Extensión opcional: PostgreSQL en Docker

Si terminas antes y tu equipo permite Docker, lleva `PEDIDOS.csv` y `PEDIDO_LINEAS.csv` a PostgreSQL 16 con un esquema propio (`db/001_pedidos.sql`, montos en `numeric`) y valida la carga con conteos y sumas. No es parte del entregable.

## Licencias

- Claude Team Standard (Claude Code se autentica con la cuenta Team).
- JetBrains All Products Pack (Rider).
- Docker no se exige. No se necesita licencia de Delphi ni de DBISAM: el legado no se compila.

## Talleres anteriores

- **Unidad 1** (`FormatearFolio()` y `docs/mapa-sdlc.md`): ver el README de `unidad-1/taller/pedidos-web`.
- **Unidad 2** (API, 45 pedidos y HU-01 listar pedidos): ver el README de `unidad-2/taller/pedidos-web`.
- **Unidad 3** (HU-02 detalle del pedido, refactor de `ClientesService` y Skill `documentar-modulo`): ver el README de `unidad-3/taller/pedidos-web`.
- **Unidad 4** (diagnóstico de Pedidos Legado): ver el README de `unidad-4/taller/pedidos-web`.
