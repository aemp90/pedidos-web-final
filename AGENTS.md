# AGENTS.md · Pedidos Web

## Proyecto
Portal web de pedidos de una distribuidora; moderniza Pedidos Legado (Delphi + DBISAM).
API REST en ASP.NET Core (.NET 10) con datos de ejemplo en memoria (sin base de datos).

## Comandos
- Compilar: `dotnet build`
- Tests: `dotnet test`
- Ejecutar la API: `dotnet run --project src/PedidosWeb.Api` (http://localhost:5080, prueba con `/salud`)

## Estilo de código
- Clases, métodos y propiedades del dominio en español (`Pedido`, `ObtenerPorId`, `FormatearFolio`).
- Dinero en pesos chilenos: `decimal` sin decimales; nunca `double` ni `float`.
- Fechas de pedido: `DateOnly` (sin hora).
- `src/PedidosWeb.Core`: modelo y reglas de dominio, sin dependencias de infraestructura.
- `src/PedidosWeb.Api`: Minimal API en `Program.cs`; expone endpoints y delega en Core (`IPedidoRepositorio`).

## Tests
- xUnit en `tests/PedidosWeb.Core.Tests/`: una carpeta por namespace de Core, un archivo `<Clase>Tests.cs` por clase.
- Nombre: `Metodo_Escenario_Resultado` (ej. `ObtenerPorId_IdInexistente_DevuelveNull`).
- Los tests de la Api también viven en `tests/PedidosWeb.Core.Tests/PedidosWeb.Core.Tests.csproj` (no se crea otro proyecto).
- Ejecuta `dotnet test` al terminar cada tarea de implementación, antes de darla por terminada.

## Seguridad
- Nunca escribas secretos, claves ni cadenas de conexión en el código ni en archivos versionados.

## Reglas de trabajo
- Responde, comenta el código y escribe en español de Chile.
- Toda funcionalidad parte de una spec aprobada en `specs/<historia>/spec.md`.
- Un test xUnit por criterio de aceptación.
- No hacer commit sin tests en verde.
- Nunca hagas commit, push ni merge: lo hace una persona después de leer el diff.
- Sin pedir permiso puedes leer el repo y ejecutar `dotnet build`, `dotnet test` y `dotnet run`.
- Con el plan de la historia aprobado, puedes editar `src/`, `tests/` y `specs/` sin pedir permiso archivo por archivo.
- Solo trabajas en `src/`, `tests/`, `docs/` y `specs/`; no toques `.idea/` ni este `AGENTS.md`.
- No agregues paquetes NuGet sin preguntar primero.
