using PedidosWeb.Core.Pedidos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPedidoRepositorio, PedidoRepositorioEnMemoria>();
builder.Services.AddSingleton<ListadoPedidos>();

var app = builder.Build();

// Para comprobar que la API está arriba.
app.MapGet("/salud", () => Results.Ok(new { estado = "ok" }));

// Detalle de un pedido por id.
app.MapGet("/api/pedidos/{id:int}", (int id, IPedidoRepositorio repositorio) =>
    repositorio.ObtenerPorId(id) is { } pedido
        ? Results.Ok(pedido)
        : Results.NotFound());

// HU-01 · Listar pedidos con filtros (specs/HU-01-listar-pedidos/spec.md).
// Los parámetros llegan como string? para que la validación y los mensajes en español los ponga Core,
// y no el 400 automático de ASP.NET.
app.MapGet("/api/pedidos", (
    string? clienteId,
    string? estado,
    string? desde,
    string? hasta,
    string? pagina,
    string? tamanoPagina,
    ListadoPedidos listado) =>
{
    var resultado = FiltroPedidos.Crear(clienteId, estado, desde, hasta, pagina, tamanoPagina);

    return resultado.Filtro is { } filtro
        ? Results.Ok(listado.Listar(filtro))
        : Results.ValidationProblem(
            resultado.Errores.ToDictionary(),
            title: "Uno o más parámetros no son válidos.");
});

app.Run();

// Permite crear tests de integración con WebApplicationFactory<Program> más adelante.
public partial class Program { }
