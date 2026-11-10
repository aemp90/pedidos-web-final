# Pedidos Web

Portal web de pedidos de la distribuidora. Es la modernización de **Pedidos Legado**, un sistema de escritorio hecho en Delphi con DBISAM que la empresa usa hace 18 años.

Este repositorio es el **caso hilo** del curso *Desarrollando con IA* (Vela LATAM). Crece unidad a unidad: la biblioteca de dominio (Unidad 1), una API en ASP.NET Core con 45 pedidos de ejemplo (Unidad 2), las líneas de cada pedido y un `ClientesService` heredado (Unidad 3) y, en la **Unidad 4**, el código de Pedidos Legado para diagnosticarlo **sin tocarlo**. Después se suman el cálculo de totales migrado del legado, PostgreSQL y el pipeline de CI.

> Todo el contenido es ficticio y tiene fines educativos.

## Stack

| Pieza | Versión | Para qué |
|---|---|---|
| .NET SDK | 10 | Compilar y ejecutar |
| C# | el que trae el SDK | Lenguaje del proyecto |
| xUnit | 2.9 | Tests |
| JetBrains Rider | la versión vigente del All Products Pack | IDE del curso |
| Claude Code | 2.1.277 o superior | Agente de código (lee `AGENTS.md`) |
| Delphi 7 + DBISAM 4 | solo lectura · no se instala | El sistema legado que diagnosticas en la U4 |

## Estructura

```
pedidos-web/
├── AGENTS.md                         ← el tuyo (lo escribiste en la U2)
├── PedidosWeb.sln
├── src/
│   ├── PedidosWeb.Core/              ← lógica de dominio (Folios, Pedidos, Lineas, Clientes)
│   └── PedidosWeb.Api/               ← U2: API REST (Minimal API)
├── tests/
│   └── PedidosWeb.Core.Tests/
├── legado/                           ← U4: código de Pedidos Legado, solo lectura   (nuevo)
│   ├── PedidosLegado.dpr
│   ├── README.md
│   ├── src/                          ← unidades .pas y formularios .dfm
│   └── db/                           ← esquema DBISAM y datos en CSV
├── docs/
│   ├── plantilla-spec-breve.md       ← U3
│   ├── plantilla-skill.md            ← U3
│   ├── diagnostico/                  ← U4: 5 plantillas del diagnóstico          (nuevo)
│   │   ├── inventario.md
│   │   ├── dependencias.md
│   │   ├── reglas-negocio.md
│   │   ├── riesgos.md
│   │   └── matriz-decision.md
│   └── plantilla-subagente.md        ← U4: para tu subagente analista-legado     (nuevo)
└── specs/                            ← U2: una carpeta por historia
```

## Cómo empezar la Unidad 4

### A. Ya tienes tu repo `pedidos-web` (lo normal)

**No copies la carpeta completa**: pisarías tu `README.md`, tu `AGENTS.md`, tu API y tu trabajo de las unidades anteriores. La U4 solo **suma** tres cosas: la carpeta `legado/`, la carpeta `docs/diagnostico/` y la plantilla `docs/plantilla-subagente.md`. No reemplaza nada.

La forma recomendada es pedírselo a Claude Code, abierto en tu `pedidos-web`:

> Trae a pedidos-web solo lo nuevo de la Unidad 4 desde el repo de material (legado/, docs/diagnostico/ y docs/plantilla-subagente.md). No toques nada más y explícame cada paso.

Si prefieres hacerlo tú, trabaja en la carpeta que contiene tu `pedidos-web` y el material del curso, uno al lado del otro:

```bash
# macOS, Linux o WSL
cd material-curso-ia-desarrolladores && git pull && cd ..
U4=material-curso-ia-desarrolladores/unidad-4/taller/pedidos-web
cp -r $U4/legado pedidos-web/
cp -r $U4/docs/diagnostico pedidos-web/docs/
cp $U4/docs/plantilla-subagente.md pedidos-web/docs/
```

```powershell
# Windows PowerShell
cd material-curso-ia-desarrolladores; git pull; cd ..
$U4 = "material-curso-ia-desarrolladores/unidad-4/taller/pedidos-web"
Copy-Item -Recurse "$U4/legado" pedidos-web/
Copy-Item -Recurse "$U4/docs/diagnostico" pedidos-web/docs/
Copy-Item "$U4/docs/plantilla-subagente.md" pedidos-web/docs/
```

**No se copian:** `README.md`, `AGENTS.md`, `PedidosWeb.sln`, `src/`, `tests/`, `specs/` ni `docs/README.md`. La carpeta `legado/` queda **fuera de la solución .NET**: no hay que tocar `PedidosWeb.sln` ni ningún `.csproj`.

Comprueba y guarda el punto de partida:

```bash
cd pedidos-web
dotnet test
git add .
git commit -m "chore: base de la Unidad 4 (legado y plantillas del diagnóstico)"
git push
```

**Resultado esperado:** `dotnet test` da el mismo resultado que al terminar la U3. La U4 no suma código C#.

### B. No tienes tu repo de las unidades anteriores

Sigue el paso 1 del README de `unidad-1/taller/pedidos-web` (crear tu repo en GitHub), pero copia esta carpeta (`unidad-4/taller/pedidos-web`) en vez de la de la Unidad 1. En este camino el `AGENTS.md` viene vacío y los 3 tests de `FolioFormatterTests` fallan con `NotImplementedException`: es normal, esa función es del taller de la Unidad 1.

---

## Taller de la Unidad 4: lo que encuentras en el repo

### El sistema legado (`legado/`)

- **Pedidos Legado** es la aplicación de escritorio que la distribuidora usa hoy para clientes, productos, pedidos, stock e informes: Delphi 7 con base de datos DBISAM 4.
- Trae el proyecto (`PedidosLegado.dpr`), las unidades `.pas` con sus formularios `.dfm` (en formato texto), el esquema de la base y los datos de ejemplo en CSV.
- **No compila ni hay que instalar Delphi.** El código se lee, no se ejecuta. Object Pascal se lee parecido a C#: `begin … end` en vez de llaves y `:=` para asignar.
- Las reglas del negocio están **repartidas** por el código, y no todo lo que dicen los comentarios (ni `legado/README.md`) es cierto. Encontrarlas es tu trabajo.
- **No se modifica.** En la preparación del taller proteges la carpeta con una regla `deny` en `.claude/settings.json`; la guía explica cómo pedírselo a Claude Code.

### Las plantillas del diagnóstico (`docs/diagnostico/`)

| Plantilla | Etapa | Qué completas |
|---|---|---|
| `inventario.md` | 1 · Entender | Una fila por archivo de `legado/src/` |
| `dependencias.md` | 1 · Entender | Diagrama Mermaid de quién usa a quién, con la base de datos |
| `reglas-negocio.md` | 2 · Encontrar | Reglas con evidencia `archivo:línea` |
| `riesgos.md` | 2 · Encontrar | Riesgos con evidencia, categoría y prioridad |
| `matriz-decision.md` | 3 · Decidir | Pesos, puntajes por módulo, recomendación y resumen ejecutivo de 5 líneas |

Cada plantilla trae instrucciones breves en comentarios. Al terminar haces tú el commit: `docs: diagnóstico de Pedidos Legado (U4)`.

### La plantilla del subagente (`docs/plantilla-subagente.md`)

Es el punto de partida de tu subagente `analista-legado`, que usas en la etapa 2. El archivo final va en `.claude/agents/analista-legado.md` y solo tiene herramientas de lectura (`Read`, `Grep`, `Glob`).

### Cómo verlo en Rider

`legado/` no es parte de la solución, así que no aparece en la vista de la solución. Cámbiate a la vista de **archivos** del explorador para recorrerlo. *(Verificar en tu versión de Rider: el nombre de la vista puede cambiar.)*

### Si te trabas en la etapa 1

Usa el inventario y el diagrama de respaldo de la **carpeta** `unidad-4/taller/u4-respaldo` del repo de material (no es una rama) y salta a la etapa 2.

La guía del taller, con todo el recorrido, está en el [sitio del curso](https://ffelipecuevasc.github.io/curso-ia-desarrolladores/) → Unidad 4.

## Licencias

- Claude Team Standard (Claude Code se autentica con la cuenta Team).
- JetBrains All Products Pack (Rider).
- No se necesita licencia de Delphi ni de DBISAM: el legado no se compila.

## Talleres anteriores

- **Unidad 1** (`FormatearFolio()` y `docs/mapa-sdlc.md`): ver el README de `unidad-1/taller/pedidos-web`.
- **Unidad 2** (API, 45 pedidos y HU-01 listar pedidos): ver el README de `unidad-2/taller/pedidos-web`.
- **Unidad 3** (HU-02 detalle del pedido, refactor de `ClientesService` y Skill `documentar-modulo`): ver el README de `unidad-3/taller/pedidos-web`.
