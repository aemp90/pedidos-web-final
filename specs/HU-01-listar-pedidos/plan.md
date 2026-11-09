# Plan HU-01 · Listar pedidos con filtros

Estado: aprobado por el PO (2026-11-09) · Spec: [spec.md](spec.md)

## Contexto
La spec `specs/HU-01-listar-pedidos/spec.md` ya está aprobada (2026-11-09): tiene las reglas RN1–RN16 y los
criterios CA1–CA21. Este plan define **cómo** construirla.

**Al aprobarlo, solo se guarda este plan en `specs/HU-01-listar-pedidos/plan.md`.** La implementación viene
después, tarea por tarea y con tu visto bueno.

## Enfoque
Las reglas viven en Core para poder probarlas con tests unitarios. La Api solo traduce HTTP ↔ Core.

La Api recibe **todos los parámetros como `string?`**. Si declarara `int?` o `DateOnly?`, ASP.NET rechazaría
`pagina=abc` o `desde=31-10-2026` con su propio 400, sin nuestro ProblemDetails ni mensajes en español
(RN7, RN11, CA18, CA21). Por eso la conversión y la validación las hace Core.

## Diseño

### Core · `src/PedidosWeb.Core/Pedidos/`
| Archivo | Qué contiene |
|---|---|
| `EstadosPedido.cs` | Lista estática de los 5 estados válidos (RN5). |
| `FiltroPedidos.cs` | Record ya validado: `ClienteId?`, `Estado?`, `Desde?` (`DateOnly`), `Hasta?`, `Pagina`, `TamanoPagina`. Fábrica estática `Crear(string? clienteId, string? estado, string? desde, string? hasta, string? pagina, string? tamanoPagina)` que devuelve `ResultadoFiltro`. |
| `ResultadoFiltro.cs` | El `FiltroPedidos` válido, o los errores como `IReadOnlyDictionary<string, string[]>` indexados por nombre de parámetro, con mensajes en español. |
| `PaginaPedidos.cs` | Record de respuesta: `Pagina`, `TamanoPagina`, `TotalRegistros`, `TotalPaginas`, `Pedidos`. Serializa a camelCase, que es la forma de RN15. |
| `ListadoPedidos.cs` | Servicio que recibe un `IPedidoRepositorio` y expone `Listar(FiltroPedidos)`: filtra, ordena, cuenta y pagina. |

Reglas dentro de `FiltroPedidos.Crear`:
- Un valor nulo, vacío o solo con espacios cuenta como no enviado (RN9b).
- Estado: se compara con `StringComparison.OrdinalIgnoreCase` contra `EstadosPedido` (RN5, RN6).
- Fechas: `DateOnly.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, ...)`. Esto también
  rechaza una fecha inexistente como `2026-02-30` (RN7).
- `Desde > Hasta` es un error, que se asocia a `desde` (RN9).
- Página: `int.TryParse` con `NumberStyles.None` e `InvariantCulture`, y debe ser ≥ 1. Tamaño de página:
  igual, entre 1 y 100. Los valores por defecto son 1 y 20 (RN10, RN11).
- Se juntan **todos** los errores antes de responder, no solo el primero.

Reglas dentro de `ListadoPedidos.Listar`:
- `ClienteId`: igualdad con `OrdinalIgnoreCase` (RN4).
- Estado: `OrdinalIgnoreCase`.
- Rango de fechas inclusivo; cada extremo es opcional (RN8).
- Orden: `OrderByDescending(Fecha).ThenBy(Id)`, antes de paginar (RN13).
- `TotalPaginas` = división redondeada hacia arriba de `total / tamaño`; da 0 cuando no hay resultados (RN15).
  Se calcula en enteros, sin `double`.
- `Skip((pagina - 1) * tamaño).Take(tamaño)`. Una página después de la última da lista vacía (RN12).

Se reutilizan `IPedidoRepositorio.ObtenerTodos()` y el record `Pedido` tal como están.

### Api · `src/PedidosWeb.Api/Program.cs`
- Registrar `ListadoPedidos` como singleton, igual que el repositorio.
- Reemplazar el comentario "HU-01 · PENDIENTE" por
  `app.MapGet("/api/pedidos", (string? clienteId, string? estado, string? desde, string? hasta, string? pagina, string? tamanoPagina, ListadoPedidos listado) => ...)`.
- Si hay errores: `Results.ValidationProblem(errores, title: "Uno o más parámetros no son válidos.")`,
  que responde 400 con ProblemDetails.
- Si no hay errores: `Results.Ok(listado.Listar(filtro))`.
- `DateOnly` se serializa como `"2026-10-20"` y `decimal` como número, así que no hace falta configurar JSON.
- `GET /api/pedidos/{id:int}` no choca con `/api/pedidos`.

### Tests · `tests/PedidosWeb.Core.Tests/` (aprobado en la spec)
- En `PedidosWeb.Core.Tests.csproj` se agrega la referencia a `src/PedidosWeb.Api` y el paquete
  `Microsoft.AspNetCore.Mvc.Testing`, en la última versión **10.0.x** (`--version 10.0.*`) para que calce con
  .NET 10. Ojo: no está en la caché local, así que se descarga de nuget.org.
- `Pedidos/FiltroPedidosTests.cs`: validación y normalización (RN5–RN11, RN9b).
- `Pedidos/ListadoPedidosTests.cs`: filtros, orden y paginación sobre los 45 pedidos de ejemplo. El desempate
  (CA11) usa un repositorio falso privado del archivo de test.
- `Api/ListarPedidosEndpointTests.cs`: `WebApplicationFactory<Program>` (`Program` ya es `partial`). Verifica el
  contrato HTTP: forma JSON, 200 y 400 con ProblemDetails.

Mapa de criterios a archivos de test (un test por criterio; `[Theory]` cuando el criterio trae varios casos):
| Criterio | Archivo |
|---|---|
| CA1–CA10, CA10b, CA11, CA13–CA16 | `ListadoPedidosTests` |
| CA17–CA21 (errores y parámetro asociado) | `FiltroPedidosTests` y `ListarPedidosEndpointTests` (400 + ProblemDetails) |
| CA10c (parámetros vacíos) | `ListarPedidosEndpointTests` |
| CA12 (8 campos iguales al detalle) | `ListarPedidosEndpointTests` |

Los nombres siguen la convención `Metodo_Escenario_Resultado`, por ejemplo
`Listar_ClienteC001_Devuelve5PedidosOrdenados`.

## Tareas de implementación (cada una termina con `dotnet test` en verde)
1. `EstadosPedido`, `FiltroPedidos`, `ResultadoFiltro` y sus tests (`FiltroPedidosTests`).
2. `PaginaPedidos`, `ListadoPedidos` y sus tests (`ListadoPedidosTests`).
3. Referencias y paquete en el proyecto de tests, más el endpoint en `Program.cs`.
4. `ListarPedidosEndpointTests`.
5. Si algo cambió respecto a lo escrito, actualizar la spec. Marcar los CA cumplidos.

## Fuera de este plan
- No se crea `tareas.md` (las tareas están arriba), salvo que lo pidas.
- No hay commit: lo haces tú después de leer el diff.

## Verificación
- `dotnet build` sin warnings nuevos.
- `dotnet test` en verde: 19 tests actuales más los nuevos de HU-01.
- Prueba manual con `dotnet run --project src/PedidosWeb.Api`:
  - `/api/pedidos?clienteId=C-001&estado=Entregado` debe dar los Ids 8, 24 y 40;
  - `/api/pedidos?pagina=4` debe dar 200 con lista vacía y `totalPaginas` 3;
  - `/api/pedidos?estado=Enviado` debe dar 400 con ProblemDetails.
