using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PedidosWeb.Core.Tests.Api;

/// <summary>
/// Contrato HTTP de GET /api/pedidos (HU-01): forma del JSON, 200 y 400 con ProblemDetails.
/// La lógica de filtros y validación se prueba en Core (ListadoPedidosTests y FiltroPedidosTests).
/// </summary>
public class ListarPedidosEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string TituloError = "Uno o más parámetros no son válidos.";

    private static readonly string[] CamposPedido =
        ["id", "folio", "clienteId", "clienteNombre", "fecha", "estado", "total", "sucursal"];

    private readonly HttpClient _cliente;

    public ListarPedidosEndpointTests(WebApplicationFactory<Program> fabrica)
    {
        _cliente = fabrica.CreateClient();
    }

    // CA10c
    [Fact]
    public async Task GetApiPedidos_ParametrosVacios_DevuelveLoMismoQueSinFiltros()
    {
        var conVacios = await GetJson("/api/pedidos?estado=&clienteId=", HttpStatusCode.OK);
        var sinFiltros = await GetJson("/api/pedidos", HttpStatusCode.OK);

        Assert.Equal(45, conVacios.GetProperty("totalRegistros").GetInt32());
        Assert.Equal(sinFiltros.GetRawText(), conVacios.GetRawText());
    }

    // CA12
    [Fact]
    public async Task GetApiPedidos_CadaPedido_TraeLos8CamposConLosValoresDeSuDetalle()
    {
        var listado = await GetJson("/api/pedidos?tamanoPagina=100", HttpStatusCode.OK);
        var pedidos = listado.GetProperty("pedidos").EnumerateArray().ToList();

        Assert.Equal(45, pedidos.Count);
        foreach (var pedido in pedidos)
        {
            Assert.Equal(CamposPedido, pedido.EnumerateObject().Select(c => c.Name));

            var id = pedido.GetProperty("id").GetInt32();
            var detalle = await GetJson($"/api/pedidos/{id}", HttpStatusCode.OK);
            Assert.Equal(detalle.GetRawText(), pedido.GetRawText());
        }

        // Ejemplo de la spec: pedido 8.
        var pedido8 = pedidos.Single(p => p.GetProperty("id").GetInt32() == 8);
        Assert.Equal("PED-STGO-000008", pedido8.GetProperty("folio").GetString());
        Assert.Equal("C-001", pedido8.GetProperty("clienteId").GetString());
        Assert.Equal("Ferretería El Clavo", pedido8.GetProperty("clienteNombre").GetString());
        Assert.Equal("2026-10-20", pedido8.GetProperty("fecha").GetString());
        Assert.Equal("Entregado", pedido8.GetProperty("estado").GetString());
        Assert.Equal(120100m, pedido8.GetProperty("total").GetDecimal());
        Assert.Equal("STGO", pedido8.GetProperty("sucursal").GetString());
    }

    // RN15: forma de la respuesta exitosa (apoya CA12).
    [Fact]
    public async Task GetApiPedidos_FiltroDelEjemplo_DevuelveMetadatosEnCamelCase()
    {
        var respuesta = await GetJson(
            "/api/pedidos?clienteId=C-001&estado=Entregado&desde=2026-10-01&hasta=2026-10-31",
            HttpStatusCode.OK);

        Assert.Equal(
            ["pagina", "tamanoPagina", "totalRegistros", "totalPaginas", "pedidos"],
            respuesta.EnumerateObject().Select(c => c.Name));
        Assert.Equal(1, respuesta.GetProperty("pagina").GetInt32());
        Assert.Equal(20, respuesta.GetProperty("tamanoPagina").GetInt32());
        Assert.Equal(1, respuesta.GetProperty("totalRegistros").GetInt32());
        Assert.Equal(1, respuesta.GetProperty("totalPaginas").GetInt32());
        Assert.Equal(8, respuesta.GetProperty("pedidos")[0].GetProperty("id").GetInt32());
    }

    // CA17
    [Fact]
    public async Task GetApiPedidos_EstadoEnviado_Devuelve400ConErrorEnEstado()
    {
        var errores = await GetProblemDetails("/api/pedidos?estado=Enviado");

        Assert.Equal(["estado"], errores.Keys);
        Assert.Equal(
            ["El estado debe ser Pendiente, Confirmado, Despachado, Entregado o Anulado."],
            errores["estado"]);
    }

    // CA18
    [Theory]
    [InlineData("desde=31-10-2026", "desde")]
    [InlineData("hasta=2026-02-30", "hasta")]
    public async Task GetApiPedidos_FechaInvalida_Devuelve400ConErrorEnLaFecha(string consulta, string parametro)
    {
        var errores = await GetProblemDetails($"/api/pedidos?{consulta}");

        Assert.Equal([parametro], errores.Keys);
    }

    // CA19
    [Fact]
    public async Task GetApiPedidos_FechasInvertidas_Devuelve400ConErrorEnDesde()
    {
        var errores = await GetProblemDetails("/api/pedidos?desde=2026-10-31&hasta=2026-10-01");

        Assert.Equal(["desde"], errores.Keys);
    }

    // CA20
    [Theory]
    [InlineData("pagina=0", "pagina")]
    [InlineData("tamanoPagina=0", "tamanoPagina")]
    [InlineData("tamanoPagina=101", "tamanoPagina")]
    public async Task GetApiPedidos_PaginacionFueraDeRango_Devuelve400(string consulta, string parametro)
    {
        var errores = await GetProblemDetails($"/api/pedidos?{consulta}");

        Assert.Equal([parametro], errores.Keys);
    }

    // CA21
    [Theory]
    [InlineData("pagina=abc", "pagina")]
    [InlineData("tamanoPagina=abc", "tamanoPagina")]
    public async Task GetApiPedidos_PaginacionNoNumerica_Devuelve400ConErrorEnElParametro(string consulta, string parametro)
    {
        var errores = await GetProblemDetails($"/api/pedidos?{consulta}");

        Assert.Equal([parametro], errores.Keys);
    }

    private async Task<JsonElement> GetJson(string url, HttpStatusCode estadoEsperado)
    {
        using var respuesta = await _cliente.GetAsync(url);
        Assert.Equal(estadoEsperado, respuesta.StatusCode);

        using var documento = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
        return documento.RootElement.Clone();
    }

    /// <summary>
    /// Verifica un 400 con ProblemDetails (variante ValidationProblem) y devuelve sus errores por parámetro.
    /// </summary>
    private async Task<Dictionary<string, string[]>> GetProblemDetails(string url)
    {
        using var respuesta = await _cliente.GetAsync(url);
        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        Assert.Equal("application/problem+json", respuesta.Content.Headers.ContentType?.MediaType);

        using var documento = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
        var raiz = documento.RootElement;
        Assert.Equal(400, raiz.GetProperty("status").GetInt32());
        Assert.Equal(TituloError, raiz.GetProperty("title").GetString());

        var errores = raiz.GetProperty("errors").EnumerateObject().ToDictionary(
            e => e.Name,
            e => e.Value.EnumerateArray().Select(m => m.GetString()!).ToArray());
        Assert.All(errores.Values, mensajes => Assert.NotEmpty(mensajes));
        return errores;
    }
}
