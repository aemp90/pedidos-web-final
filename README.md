# Pedidos Web

Portal web de pedidos de la distribuidora. Es la modernización de **Pedidos Legado**, un sistema de escritorio hecho en Delphi con DBISAM que la empresa usa hace 18 años.

Este repositorio es el **caso hilo** del curso *Desarrollando con IA* (Vela LATAM). Crece unidad a unidad: la biblioteca de dominio (Unidad 1), una API en ASP.NET Core con 45 pedidos de ejemplo (Unidad 2) y, en la Unidad 3, las líneas de cada pedido y un `ClientesService` heredado. Después se suman el cálculo de totales migrado del legado, PostgreSQL y el pipeline de CI.

> Todo el contenido es ficticio y tiene fines educativos.

## Stack

| Pieza | Versión | Para qué |
|---|---|---|
| .NET SDK | 10 | Compilar y ejecutar |
| C# | el que trae el SDK | Lenguaje del proyecto |
| xUnit | 2.9 | Tests |
| JetBrains Rider | la versión vigente del All Products Pack | IDE del curso |
| Claude Code | 2.1.277 o superior | Agente de código (lee `AGENTS.md`) |

## Estructura

```
pedidos-web/
├── AGENTS.md                         ← el tuyo (lo escribiste en la U2)
├── PedidosWeb.sln
├── src/
│   ├── PedidosWeb.Core/              ← lógica de dominio
│   │   ├── Folios/                   ← U1: FormatearFolio()
│   │   ├── Pedidos/                  ← U2: modelo Pedido y repositorio en memoria
│   │   ├── Lineas/                   ← U3: líneas de los 45 pedidos          (nuevo)
│   │   └── Clientes/                 ← U3: ClientesService heredado          (nuevo)
│   └── PedidosWeb.Api/               ← U2: API REST (Minimal API)
├── tests/
│   └── PedidosWeb.Core.Tests/
│       ├── Folios/ · Pedidos/
│       ├── Lineas/                   ← U3: 3 tests                           (nuevo)
│       └── Clientes/                 ← U3: 4 tests                           (nuevo)
├── docs/
│   ├── plantilla-spec-breve.md       ← U3: para tu spec de la HU-02          (nuevo)
│   └── plantilla-skill.md            ← U3: para tu Skill documentar-modulo   (nuevo)
└── specs/                            ← U2: una carpeta por historia
```

## Cómo empezar la Unidad 3

### A. Ya tienes tu repo `pedidos-web` (lo normal)

**No copies la carpeta completa**: pisarías tu `README.md`, tu `AGENTS.md`, tu API y tu trabajo de la U2. La U3 solo **suma** dos carpetas de código (con sus tests) y dos plantillas. No reemplaza nada.

La forma recomendada es pedírselo a Claude Code, abierto en tu `pedidos-web`:

> Trae a pedidos-web solo lo nuevo de la Unidad 3 desde el repo de material (Lineas, Clientes, sus tests y las 2 plantillas). No toques nada más. Explícame cada paso antes de ejecutarlo.

Si prefieres hacerlo tú, trabaja en la carpeta que contiene tu `pedidos-web` y el material del curso, uno al lado del otro:

```bash
# macOS, Linux o WSL
cd material-curso-ia-desarrolladores && git pull && cd ..
U3=material-curso-ia-desarrolladores/unidad-3/taller/pedidos-web
cp -r $U3/src/PedidosWeb.Core/Lineas $U3/src/PedidosWeb.Core/Clientes pedidos-web/src/PedidosWeb.Core/
cp -r $U3/tests/PedidosWeb.Core.Tests/Lineas $U3/tests/PedidosWeb.Core.Tests/Clientes pedidos-web/tests/PedidosWeb.Core.Tests/
cp $U3/docs/plantilla-spec-breve.md $U3/docs/plantilla-skill.md pedidos-web/docs/
```

```powershell
# Windows PowerShell
cd material-curso-ia-desarrolladores; git pull; cd ..
$U3 = "material-curso-ia-desarrolladores/unidad-3/taller/pedidos-web"
Copy-Item -Recurse "$U3/src/PedidosWeb.Core/Lineas", "$U3/src/PedidosWeb.Core/Clientes" pedidos-web/src/PedidosWeb.Core/
Copy-Item -Recurse "$U3/tests/PedidosWeb.Core.Tests/Lineas", "$U3/tests/PedidosWeb.Core.Tests/Clientes" pedidos-web/tests/PedidosWeb.Core.Tests/
Copy-Item "$U3/docs/plantilla-spec-breve.md", "$U3/docs/plantilla-skill.md" pedidos-web/docs/
```

**No se copian:** `README.md`, `AGENTS.md`, `PedidosWeb.sln`, `src/PedidosWeb.Api/` ni la carpeta `Pedidos/`. Como las carpetas nuevas quedan dentro de proyectos que ya existen, no hay que tocar la solución.

Comprueba y guarda el punto de partida:

```bash
cd pedidos-web
dotnet test
git add .
git commit -m "chore: base de la Unidad 3 (líneas y ClientesService)"
git push
```

**Resultado esperado:** todos los tests en verde, incluidos los 3 nuevos de `Lineas` y los 4 de `ClientesService`.

### B. No tienes tu repo de las unidades anteriores

Sigue el paso 1 del README de `unidad-1/taller/pedidos-web` (crear tu repo en GitHub), pero copia esta carpeta (`unidad-3/taller/pedidos-web`) en vez de la de la Unidad 1. En este camino el `AGENTS.md` viene vacío y los 3 tests de `FolioFormatterTests` fallan con `NotImplementedException`: es normal, esa función es del taller de la Unidad 1.

---

## Taller de la Unidad 3: lo que encuentras en el repo

### Las líneas de los pedidos (`src/PedidosWeb.Core/Lineas/`)

- **Modelo** `LineaPedido`: `PedidoId`, `Numero`, `Producto`, `Cantidad` y `PrecioUnitario` (CLP, `decimal` sin decimales, **sin IVA**).
- **Repositorio en memoria** con las líneas de los 45 pedidos (entre 2 y 4 por pedido). Método: `ObtenerPorPedido(pedidoId)`.
- En cada pedido, la suma de `Cantidad × PrecioUnitario` de sus líneas es igual a su `Pedido.Total`.
- Todavía **no existe** el detalle del pedido: lo construyes tú en la etapa 1, con una ruta nueva `GET /api/pedidos/{id}/detalle`. La ruta `GET /api/pedidos/{id}` no se toca.

### El módulo heredado (`src/PedidosWeb.Core/Clientes/`)

- `ClientesService` lo escribió un ex integrante del equipo, con apuro. Funciona y tiene **4 tests**, pero sus nombres no se entienden y tiene código repetido.
- Usa los mismos 8 clientes de los pedidos (`C-001` a `C-008`).
- En la etapa 2 lo ordenas **sin cambiar lo que hace**: los 4 tests son tu red.

### Las plantillas (`docs/`)

- `plantilla-spec-breve.md`: la copias como `specs/HU-02-detalle-pedido/spec.md` en la etapa 1.
- `plantilla-skill.md`: el punto de partida de tu Skill `.claude/skills/documentar-modulo/SKILL.md` en la etapa 3.

### Si te trabas con la spec

Usa la spec de respaldo de la **carpeta** `unidad-3/taller/u3-respaldo` del repo de material (no es una rama).

La guía del taller, con todo el recorrido, está en el [sitio del curso](https://ffelipecuevasc.github.io/curso-ia-desarrolladores/) → Unidad 3.

## Licencias

- Claude Team Standard (Claude Code se autentica con la cuenta Team).
- JetBrains All Products Pack (Rider).

## Talleres anteriores

- **Unidad 1** (`FormatearFolio()` y `docs/mapa-sdlc.md`): ver el README de `unidad-1/taller/pedidos-web`.
- **Unidad 2** (API, 45 pedidos y HU-01 listar pedidos): ver el README de `unidad-2/taller/pedidos-web`.
