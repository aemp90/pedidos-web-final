# Plan HU-02 · Detalle de un pedido

Estado: aprobado por el PO (2026-11-10) · Spec: [spec.md](spec.md)

## Contexto
La spec `specs/HU-02-detalle-pedido/spec.md` ya está aprobada (2026-11-10): tiene los criterios CA-01 a CA-06.
Este plan define **cómo** construirla, en 3 incrementos. Cada incremento se implementa por separado, con tu visto
bueno, y termina con `dotnet test` en verde.

## Enfoque
Las reglas viven en Core para probarlas con tests unitarios; la Api solo traduce HTTP ↔ Core.
El total **siempre se calcula desde las líneas**, no se copia de `Pedido.Total`. Así, un pedido sin líneas
tiene total 0 (CA-06), y el CA-04 comprueba que el cálculo cuadra con el legado.

Se reutilizan sin cambios:
- `IPedidoRepositorio.ObtenerPorId` y `PedidoRepositorioEnMemoria` (`src/PedidosWeb.Core/Pedidos/`).
- `ILineaPedidoRepositorio.ObtenerPorPedido` y `LineaPedidoRepositorioEnMemoria` (`src/PedidosWeb.Core/Lineas/`),
  que ya devuelven las líneas ordenadas por número.
- Los records `Pedido` y `LineaPedido`.

## Diseño

### Core
| Archivo | Qué contiene |
|---|---|
| `Lineas/TotalPedido.cs` | Clase estática. `CalcularSubtotal(LineaPedido)` = `Cantidad * PrecioUnitario`; `CalcularTotal(IEnumerable<LineaPedido>)` = suma de subtotales (0 si no hay líneas). Todo en `decimal`. |
| `Pedidos/DetallePedido.cs` | Records de respuesta: `DetallePedido(Id, Folio, Fecha, Estado, Sucursal, Cliente, Lineas, Total)`, `ClienteDetalle(Id, Nombre)` y `LineaDetalle(Numero, Producto, Cantidad, PrecioUnitario, Subtotal)`. `Fecha` es `DateOnly`; montos `decimal`. |
| `Pedidos/DetallePedidos.cs` | Servicio que recibe `IPedidoRepositorio` e `ILineaPedidoRepositorio` y expone `Obtener(int id)`: devuelve `DetallePedido?` (null si el pedido no existe). Usa `TotalPedido` para subtotales y total. |

### Api · `src/PedidosWeb.Api/Program.cs`
- Registrar `ILineaPedidoRepositorio` → `LineaPedidoRepositorioEnMemoria` y `DetallePedidos` como singleton,
  igual que lo existente.
- `app.MapGet("/api/pedidos/{id:int}/detalle", (int id, DetallePedidos detalles) => ...)`:
  - Con detalle: `Results.Ok(detalle)`.
  - Sin detalle: `Results.Problem(statusCode: 404, title: "El pedido no existe.", detail: $"No existe un pedido con id {id}.")`.
- No se modifican `/salud`, `/api/pedidos` ni `/api/pedidos/{id}`.
- JSON en camelCase por defecto; `DateOnly` sale como `"2026-10-20"` y `decimal` como número.

Forma esperada (pedido 8):

    {
      "id": 8, "folio": "PED-STGO-000008", "fecha": "2026-10-20", "estado": "Entregado", "sucursal": "STGO",
      "cliente": { "id": "C-001", "nombre": "Ferretería El Clavo" },
      "lineas": [
        { "numero": 1, "producto": "Harina para pan 25 kg", "cantidad": 3, "precioUnitario": 21500, "subtotal": 64500 },
        ...
      ],
      "total": 120100
    }

## Incrementos

### 1 · Calcular el total
- Toca: `src/PedidosWeb.Core/Lineas/TotalPedido.cs` (nuevo), `tests/PedidosWeb.Core.Tests/Lineas/TotalPedidoTests.cs` (nuevo).
- No toca: `Pedidos/`, la Api, `LineaPedido`, `Pedido` ni los repositorios.
- Criterios: CA-02, CA-03, CA-04.
- Tests:
  - CA-02 · `CalcularSubtotal_LineasDelPedido8_DevuelveCantidadPorPrecio` (64500, 35400, 20200).
  - CA-03 · `CalcularTotal_Pedido8_Devuelve120100`.
  - CA-04 · `CalcularTotal_Los45PedidosDeEjemplo_CoincideConPedidoTotal`.
  - Apoyo · `CalcularTotal_SinLineas_DevuelveCero`.

### 2 · Obtener el detalle
- Toca: `src/PedidosWeb.Core/Pedidos/DetallePedido.cs` y `DetallePedidos.cs` (nuevos),
  `tests/PedidosWeb.Core.Tests/Pedidos/DetallePedidosTests.cs` (nuevo).
- No toca: la Api, `TotalPedido`, `Pedido`, `LineaPedido` ni los repositorios.
- Criterios: CA-01 y CA-06 (en Core).
- Tests:
  - CA-01 · `Obtener_Pedido8_DevuelveDatosDelPedidoYDelCliente`.
  - CA-06 · `Obtener_PedidoSinLineas_DevuelveLineasVaciasYTotalCero` (repositorio de prueba, ver Tests).
  - Apoyo · `Obtener_Pedido8_DevuelveLineasEnOrdenConSubtotalYTotal`.
  - Apoyo · `Obtener_IdInexistente_DevuelveNull`.

### 3 · Publicarlo en la API (`GET /api/pedidos/{id}/detalle`)
- Toca: `src/PedidosWeb.Api/Program.cs`, `tests/PedidosWeb.Core.Tests/Api/DetallePedidoEndpointTests.cs` (nuevo).
- No toca: `src/PedidosWeb.Core/`, los endpoints existentes ni sus tests.
- Criterios: CA-05; CA-01 y CA-06 vía HTTP.
- Tests (`WebApplicationFactory<Program>`, como `ListarPedidosEndpointTests`):
  - CA-05 · `GetDetalle_IdInexistente_Devuelve404ConProblemDetails` (status, title y detail con 999).
  - CA-01 · `GetDetalle_Pedido8_DevuelveJsonConPedidoClienteLineasYTotal`.
  - CA-06 · `GetDetalle_PedidoSinLineas_Devuelve200ConLineasVaciasYTotalCero`.
- Al cerrar: marcar los CA cumplidos en la spec y agregar notas de implementación si algo cambió.

## Tests
- Un test por criterio; los de "apoyo" no reemplazan a ningún criterio. Nombres `Metodo_Escenario_Resultado`.
- Mapa:
  | Criterio | Archivo |
  |---|---|
  | CA-02, CA-03, CA-04 | `Lineas/TotalPedidoTests` |
  | CA-01, CA-06 (Core) | `Pedidos/DetallePedidosTests` |
  | CA-05, CA-01 y CA-06 (HTTP) | `Api/DetallePedidoEndpointTests` |
- **CA-06**: ningún pedido de ejemplo está sin líneas.
  - En Core: un `LineasFalsas : ILineaPedidoRepositorio` privado del archivo de test que devuelve lista vacía,
    junto al `PedidoRepositorioEnMemoria` real (mismo patrón que `RepositorioFalso` del CA11 en `ListadoPedidosTests`).
  - En la Api: `fabrica.WithWebHostBuilder(b => b.ConfigureTestServices(s => ...))` reemplaza
    `ILineaPedidoRepositorio` por el falso. `ConfigureTestServices` ya viene en `Microsoft.AspNetCore.Mvc.Testing`.

## Decisiones técnicas aprobadas por el PO (2026-11-10)
- **Cliente anidado** en el JSON (`"cliente": { "id", "nombre" }`) en vez de `clienteId`/`clienteNombre` planos
  como en la HU-01. La spec separa "del pedido" y "del cliente"; anidado lo refleja.
- **Cambio de DI**: registrar `ILineaPedidoRepositorio` y `DetallePedidos` como singleton en `Program.cs`.
- **Sin paquetes nuevos**: todo lo necesario ya está en el proyecto de tests.

## Riesgos
- `Results.Problem` responde `application/problem+json`: el test del CA-05 debe leer ese content-type.
- Un id no numérico (`/api/pedidos/abc/detalle`) da 404 sin cuerpo por la restricción `{id:int}`; así lo dice
  la spec y no se prueba con ProblemDetails.

## Preguntas abiertas
Ninguna por ahora.

## Fuera de este plan
- No hay commit: lo haces tú después de leer el diff.

## Verificación
- `dotnet build` sin warnings nuevos y `dotnet test` en verde al cierre de cada incremento.
- Prueba manual con `dotnet run --project src/PedidosWeb.Api`:
  - `/api/pedidos/8/detalle` → 200, 3 líneas, total 120100.
  - `/api/pedidos/999/detalle` → 404 con ProblemDetails "El pedido no existe.".
