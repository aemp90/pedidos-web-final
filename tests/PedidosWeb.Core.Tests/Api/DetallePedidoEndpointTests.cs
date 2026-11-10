using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PedidosWeb.Core.Lineas;

namespace PedidosWeb.Core.Tests.Api;

/// <summary>
/// HU-02 · Incremento 3 de 3: contrato HTTP de GET /api/pedidos/{id}/detalle (200 y 404 con ProblemDetails).
/// El armado del detalle y el cálculo del total se prueban en Core (DetallePedidosTests y TotalPedidoTests).
/// </summary>
public class DetallePedidoEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fabrica;

    public DetallePedidoEndpointTests(WebApplicationFactory<Program> fabrica)
    {
        _fabrica = fabrica;
    }

    // CA-05
    [Fact]
    public async Task GetDetalle_IdInexistente_Devuelve404ConProblemDetails()
    {
        var respuesta = await _fabrica.CreateClient().GetAsync("/api/pedidos/999/detalle");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
        Assert.Equal("application/problem+json", respuesta.Content.Headers.ContentType?.MediaType);

        var problema = await LeerJson(respuesta);
        Assert.Equal(404, problema.GetProperty("status").GetInt32());
        Assert.Equal("El pedido no existe.", problema.GetProperty("title").GetString());
        Assert.Equal("No existe un pedido con id 999.", problema.GetProperty("detail").GetString());
    }

    // CA-01 (vía HTTP)
    [Fact]
    public async Task GetDetalle_Pedido8_DevuelveJsonConPedidoClienteLineasYTotal()
    {
        var respuesta = await _fabrica.CreateClient().GetAsync("/api/pedidos/8/detalle");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var detalle = await LeerJson(respuesta);

        Assert.Equal(8, detalle.GetProperty("id").GetInt32());
        Assert.Equal("PED-STGO-000008", detalle.GetProperty("folio").GetString());
        Assert.Equal("2026-10-20", detalle.GetProperty("fecha").GetString());
        Assert.Equal("Entregado", detalle.GetProperty("estado").GetString());
        Assert.Equal("STGO", detalle.GetProperty("sucursal").GetString());

        var cliente = detalle.GetProperty("cliente");
        Assert.Equal("C-001", cliente.GetProperty("id").GetString());
        Assert.Equal("Ferretería El Clavo", cliente.GetProperty("nombre").GetString());

        var lineas = detalle.GetProperty("lineas").EnumerateArray().ToList();
        Assert.Equal(new[] { 1, 2, 3 }, lineas.Select(l => l.GetProperty("numero").GetInt32()));
        Assert.Equal(new[] { 64500m, 35400m, 20200m }, lineas.Select(l => l.GetProperty("subtotal").GetDecimal()));

        var primera = lineas[0];
        Assert.Equal("Harina para pan 25 kg", primera.GetProperty("producto").GetString());
        Assert.Equal(3, primera.GetProperty("cantidad").GetInt32());
        Assert.Equal(21500m, primera.GetProperty("precioUnitario").GetDecimal());

        Assert.Equal(120100m, detalle.GetProperty("total").GetDecimal());
    }

    // CA-06 (vía HTTP)
    [Fact]
    public async Task GetDetalle_PedidoSinLineas_Devuelve200ConLineasVaciasYTotalCero()
    {
        var cliente = _fabrica
            .WithWebHostBuilder(b => b.ConfigureTestServices(servicios =>
                servicios.AddSingleton<ILineaPedidoRepositorio, LineasFalsas>()))
            .CreateClient();

        var respuesta = await cliente.GetAsync("/api/pedidos/8/detalle");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var detalle = await LeerJson(respuesta);
        Assert.Equal(8, detalle.GetProperty("id").GetInt32());
        Assert.Empty(detalle.GetProperty("lineas").EnumerateArray());
        Assert.Equal(0m, detalle.GetProperty("total").GetDecimal());
    }

    private static async Task<JsonElement> LeerJson(HttpResponseMessage respuesta)
    {
        using var documento = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
        return documento.RootElement.Clone();
    }

    /// <summary>Repositorio de prueba para CA-06: los datos de ejemplo no tienen pedidos sin líneas.</summary>
    private sealed class LineasFalsas : ILineaPedidoRepositorio
    {
        public IReadOnlyList<LineaPedido> ObtenerPorPedido(int pedidoId) => [];
    }
}
