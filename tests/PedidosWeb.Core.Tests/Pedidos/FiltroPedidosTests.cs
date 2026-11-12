using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

/// <summary>
/// Tests de apoyo para FiltroPedidos.Crear (validación y normalización). Los criterios CA17–CA21
/// se verifican sobre HTTP en ListarPedidosEndpointTests; estos tests prueban la misma lógica en Core.
/// </summary>
public class FiltroPedidosTests
{
    private static ResultadoFiltro Crear(
        string? clienteId = null,
        string? estado = null,
        string? desde = null,
        string? hasta = null,
        string? pagina = null,
        string? tamanoPagina = null) =>
        FiltroPedidos.Crear(clienteId, estado, desde, hasta, pagina, tamanoPagina);

    // RN1, RN10
    [Fact]
    public void Crear_SinParametros_UsaPagina1YTamano20()
    {
        var resultado = Crear();

        Assert.True(resultado.EsValido);
        Assert.Empty(resultado.Errores);
        var filtro = resultado.Filtro!;
        Assert.Null(filtro.ClienteId);
        Assert.Null(filtro.Estado);
        Assert.Null(filtro.Desde);
        Assert.Null(filtro.Hasta);
        Assert.Equal(1, filtro.Pagina);
        Assert.Equal(20, filtro.TamanoPagina);
    }

    // RN9b (apoya CA10c)
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_ParametrosVaciosOEspacios_CuentanComoNoEnviados(string vacio)
    {
        var resultado = Crear(vacio, vacio, vacio, vacio, vacio, vacio);

        Assert.True(resultado.EsValido);
        var filtro = resultado.Filtro!;
        Assert.Null(filtro.ClienteId);
        Assert.Null(filtro.Estado);
        Assert.Null(filtro.Desde);
        Assert.Null(filtro.Hasta);
        Assert.Equal(1, filtro.Pagina);
        Assert.Equal(20, filtro.TamanoPagina);
    }

    [Fact]
    public void Crear_ValoresConEspaciosAlrededor_LosRecorta()
    {
        var resultado = Crear(clienteId: " C-001 ", estado: " Entregado ", desde: " 2026-10-01 ");

        Assert.True(resultado.EsValido);
        Assert.Equal("C-001", resultado.Filtro!.ClienteId);
        Assert.Equal("Entregado", resultado.Filtro.Estado);
        Assert.Equal(new DateOnly(2026, 10, 1), resultado.Filtro.Desde);
    }

    // RN5, RN6
    [Theory]
    [InlineData("Pendiente")]
    [InlineData("Confirmado")]
    [InlineData("Despachado")]
    [InlineData("Entregado")]
    [InlineData("Anulado")]
    [InlineData("entregado")]
    [InlineData("ENTREGADO")]
    public void Crear_EstadoValido_EsAceptado(string estado)
    {
        var resultado = Crear(estado: estado);

        Assert.True(resultado.EsValido);
        Assert.Equal(estado, resultado.Filtro!.Estado);
    }

    // RN5 (apoya CA17)
    [Fact]
    public void Crear_EstadoEnviado_DevuelveErrorEnEstado()
    {
        var resultado = Crear(estado: "Enviado");

        Assert.False(resultado.EsValido);
        Assert.Null(resultado.Filtro);
        Assert.Equal(
            ["El estado debe ser Pendiente, Confirmado, Despachado, Entregado o Anulado."],
            resultado.Errores["estado"]);
    }

    // RN7
    [Fact]
    public void Crear_FechasValidas_LasConvierteADateOnly()
    {
        var resultado = Crear(desde: "2026-10-01", hasta: "2026-10-31");

        Assert.True(resultado.EsValido);
        Assert.Equal(new DateOnly(2026, 10, 1), resultado.Filtro!.Desde);
        Assert.Equal(new DateOnly(2026, 10, 31), resultado.Filtro.Hasta);
    }

    // RN7 (apoya CA18)
    [Theory]
    [InlineData("31-10-2026")]
    [InlineData("2026/10/31")]
    [InlineData("2026-1-5")]
    [InlineData("2026-02-30")]
    [InlineData("hoy")]
    public void Crear_DesdeInvalido_DevuelveErrorEnDesde(string desde)
    {
        var resultado = Crear(desde: desde);

        Assert.False(resultado.EsValido);
        Assert.Equal(["desde"], resultado.Errores.Keys);
    }

    // RN7 (apoya CA18)
    [Theory]
    [InlineData("2026-02-30")]
    [InlineData("31-10-2026")]
    public void Crear_HastaInvalido_DevuelveErrorEnHasta(string hasta)
    {
        var resultado = Crear(hasta: hasta);

        Assert.False(resultado.EsValido);
        Assert.Equal(["hasta"], resultado.Errores.Keys);
    }

    // RN9 (apoya CA19)
    [Fact]
    public void Crear_DesdePosteriorAHasta_DevuelveErrorEnDesde()
    {
        var resultado = Crear(desde: "2026-10-31", hasta: "2026-10-01");

        Assert.False(resultado.EsValido);
        Assert.Equal(
            ["La fecha desde no puede ser posterior a la fecha hasta."],
            resultado.Errores["desde"]);
    }

    // RN8: desde = hasta es válido (rango inclusivo).
    [Fact]
    public void Crear_DesdeIgualAHasta_EsValido()
    {
        var resultado = Crear(desde: "2026-10-31", hasta: "2026-10-31");

        Assert.True(resultado.EsValido);
    }

    // Si una fecha tiene formato inválido no se compara el rango.
    [Fact]
    public void Crear_HastaInvalidoYDesdeValido_NoComparaElRango()
    {
        var resultado = Crear(desde: "2026-10-31", hasta: "2026-02-30");

        Assert.False(resultado.EsValido);
        Assert.Equal(["hasta"], resultado.Errores.Keys);
    }

    // RN10
    [Fact]
    public void Crear_PaginaYTamanoValidos_LosConvierte()
    {
        var resultado = Crear(pagina: "3", tamanoPagina: "100");

        Assert.True(resultado.EsValido);
        Assert.Equal(3, resultado.Filtro!.Pagina);
        Assert.Equal(100, resultado.Filtro.TamanoPagina);
    }

    // RN11 (apoya CA20 y CA21)
    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("+1")]
    [InlineData("1.5")]
    [InlineData("1,000")]
    [InlineData("abc")]
    [InlineData("99999999999")]
    public void Crear_PaginaInvalida_DevuelveErrorEnPagina(string pagina)
    {
        var resultado = Crear(pagina: pagina);

        Assert.False(resultado.EsValido);
        Assert.Equal(["pagina"], resultado.Errores.Keys);
    }

    // RN11 (apoya CA20 y CA21)
    [Theory]
    [InlineData("0")]
    [InlineData("101")]
    [InlineData("-5")]
    [InlineData("+20")]
    [InlineData("abc")]
    public void Crear_TamanoPaginaInvalido_DevuelveErrorEnTamanoPagina(string tamanoPagina)
    {
        var resultado = Crear(tamanoPagina: tamanoPagina);

        Assert.False(resultado.EsValido);
        Assert.Equal(["tamanoPagina"], resultado.Errores.Keys);
    }

    [Fact]
    public void Crear_VariosParametrosInvalidos_JuntaTodosLosErrores()
    {
        var resultado = Crear(
            estado: "Enviado",
            desde: "31-10-2026",
            hasta: "2026-02-30",
            pagina: "abc",
            tamanoPagina: "0");

        Assert.False(resultado.EsValido);
        Assert.Equal(
            ["desde", "estado", "hasta", "pagina", "tamanoPagina"],
            resultado.Errores.Keys.Order());
    }

    // ---------- HU-03 · EntreFechas ----------

    private static readonly DateOnly Desde = new(2026, 10, 1);
    private static readonly DateOnly Hasta = new(2026, 10, 31);

    private static Pedido PedidoDel(DateOnly fecha, int id = 1) =>
        new(id, $"PED-STGO-{id:D6}", "C-001", "Cliente de prueba", fecha, "Pendiente", 10_000m, "STGO");

    // CA-01
    [Fact]
    public void EntreFechas_PedidoEnFechaDesde_Aparece()
    {
        var pedido = PedidoDel(Desde);

        var resultado = FiltroPedidos.EntreFechas([pedido], Desde, Hasta);

        Assert.Equal([pedido], resultado);
    }

    // CA-02
    [Fact]
    public void EntreFechas_PedidoEnFechaHasta_Aparece()
    {
        var pedido = PedidoDel(Hasta);

        var resultado = FiltroPedidos.EntreFechas([pedido], Desde, Hasta);

        Assert.Equal([pedido], resultado);
    }

    // CA-03
    [Fact]
    public void EntreFechas_PedidosFueraDelRango_NoAparecen()
    {
        var antes = PedidoDel(new DateOnly(2026, 9, 30), id: 1);
        var dentro = PedidoDel(new DateOnly(2026, 10, 15), id: 2);
        var despues = PedidoDel(new DateOnly(2026, 11, 1), id: 3);

        var resultado = FiltroPedidos.EntreFechas([antes, dentro, despues], Desde, Hasta);

        Assert.Equal([dentro], resultado);
    }

    // CA-04
    [Fact]
    public void EntreFechas_DesdePosteriorAHasta_LanzaArgumentException()
    {
        var excepcion = Assert.Throws<ArgumentException>(
            () => FiltroPedidos.EntreFechas([PedidoDel(Desde)], Hasta, Desde));

        Assert.Equal("desde", excepcion.ParamName);
        Assert.Contains("no puede ser posterior a la fecha hasta", excepcion.Message);
    }
}
