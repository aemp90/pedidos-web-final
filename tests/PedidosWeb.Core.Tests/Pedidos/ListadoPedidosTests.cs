using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

/// <summary>Criterios de HU-01 sobre filtros, orden y paginación, con los 45 pedidos de ejemplo.</summary>
public class ListadoPedidosTests
{
    private readonly ListadoPedidos _listado = new(new PedidoRepositorioEnMemoria());

    private static FiltroPedidos Filtro(
        string? clienteId = null,
        string? estado = null,
        string? desde = null,
        string? hasta = null,
        string? pagina = null,
        string? tamanoPagina = null)
    {
        var resultado = FiltroPedidos.Crear(clienteId, estado, desde, hasta, pagina, tamanoPagina);
        Assert.True(resultado.EsValido, "El filtro de prueba debería ser válido.");
        return resultado.Filtro!;
    }

    private static int[] Ids(PaginaPedidos pagina) => pagina.Pedidos.Select(p => p.Id).ToArray();

    // CA1
    [Fact]
    public void Listar_SinFiltros_DevuelvePagina1Con20PedidosYMetadatos()
    {
        var resultado = _listado.Listar(Filtro());

        Assert.Equal(1, resultado.Pagina);
        Assert.Equal(20, resultado.TamanoPagina);
        Assert.Equal(45, resultado.TotalRegistros);
        Assert.Equal(3, resultado.TotalPaginas);
        Assert.Equal(20, resultado.Pedidos.Count);
        Assert.Equal(21, resultado.Pedidos[0].Id);
        Assert.Equal(new DateOnly(2026, 10, 31), resultado.Pedidos[0].Fecha);
        Assert.Equal(18, resultado.Pedidos[^1].Id);
        Assert.Equal(new DateOnly(2026, 10, 5), resultado.Pedidos[^1].Fecha);
    }

    // CA2
    [Fact]
    public void Listar_ClienteC001_Devuelve5PedidosOrdenados()
    {
        var resultado = _listado.Listar(Filtro(clienteId: "C-001"));

        Assert.Equal([8, 16, 24, 32, 40], Ids(resultado));
        Assert.Equal(5, resultado.TotalRegistros);
    }

    // CA3
    [Fact]
    public void Listar_EstadoPendiente_Devuelve5PedidosOrdenados()
    {
        var resultado = _listado.Listar(Filtro(estado: "Pendiente"));

        Assert.Equal([27, 18, 45, 9, 36], Ids(resultado));
        Assert.Equal(5, resultado.TotalRegistros);
    }

    // CA4
    [Fact]
    public void Listar_EstadoEnMinusculas_DevuelveLosMismos15PedidosQueCapitalizado()
    {
        var minusculas = _listado.Listar(Filtro(estado: "entregado"));
        var capitalizado = _listado.Listar(Filtro(estado: "Entregado"));

        Assert.Equal(15, minusculas.TotalRegistros);
        Assert.Equal(15, capitalizado.TotalRegistros);
        Assert.Equal(Ids(capitalizado), Ids(minusculas));
    }

    // CA5
    [Theory]
    [InlineData("2026-10-01", "2026-10-31", 23)]
    [InlineData("2026-09-01", "2026-09-30", 22)]
    public void Listar_RangoDeUnMes_DevuelveTotalDelMes(string desde, string hasta, int totalEsperado)
    {
        var resultado = _listado.Listar(Filtro(desde: desde, hasta: hasta));

        Assert.Equal(totalEsperado, resultado.TotalRegistros);
    }

    // CA6
    [Theory]
    [InlineData("2026-09-02", 40)]
    [InlineData("2026-10-31", 21)]
    public void Listar_DesdeIgualAHasta_DevuelveSoloElPedidoDeEseDia(string fecha, int idEsperado)
    {
        var resultado = _listado.Listar(Filtro(desde: fecha, hasta: fecha));

        Assert.Equal([idEsperado], Ids(resultado));
    }

    // CA7
    [Fact]
    public void Listar_SoloDesde_DevuelvePedidosDesdeEseDiaEnAdelante()
    {
        var resultado = _listado.Listar(Filtro(desde: "2026-10-25"));

        Assert.Equal([21, 42, 2, 23, 44, 4, 25], Ids(resultado));
    }

    // CA7
    [Fact]
    public void Listar_SoloHasta_DevuelvePedidosHastaEseDiaInclusive()
    {
        var resultado = _listado.Listar(Filtro(hasta: "2026-09-05"));

        Assert.Equal([38, 19, 40], Ids(resultado));
    }

    // CA8
    [Fact]
    public void Listar_ClienteYEstado_Devuelve8_24_40()
    {
        var resultado = _listado.Listar(Filtro(clienteId: "C-001", estado: "Entregado"));

        Assert.Equal([8, 24, 40], Ids(resultado));
    }

    // CA8
    [Fact]
    public void Listar_ClienteEstadoYRangoDeOctubre_DevuelveSoloEl8()
    {
        var resultado = _listado.Listar(
            Filtro(clienteId: "C-001", estado: "Entregado", desde: "2026-10-01", hasta: "2026-10-31"));

        Assert.Equal([8], Ids(resultado));
    }

    // CA9
    [Fact]
    public void Listar_ClienteInexistente_DevuelveListaVaciaConTotalesEnCero()
    {
        var resultado = _listado.Listar(Filtro(clienteId: "C-999"));

        Assert.Empty(resultado.Pedidos);
        Assert.Equal(0, resultado.TotalRegistros);
        Assert.Equal(0, resultado.TotalPaginas);
    }

    // CA10
    [Fact]
    public void Listar_FiltrosSinCoincidencias_DevuelveListaVaciaYTotalPaginas0()
    {
        var resultado = _listado.Listar(
            Filtro(clienteId: "C-002", estado: "Despachado", desde: "2026-09-01", hasta: "2026-09-30"));

        Assert.Empty(resultado.Pedidos);
        Assert.Equal(0, resultado.TotalPaginas);
    }

    // CA10b
    [Fact]
    public void Listar_ClienteEnMinusculas_DevuelveLosMismos5PedidosQueEnMayusculas()
    {
        var minusculas = _listado.Listar(Filtro(clienteId: "c-001"));
        var mayusculas = _listado.Listar(Filtro(clienteId: "C-001"));

        Assert.Equal(5, minusculas.TotalRegistros);
        Assert.Equal(Ids(mayusculas), Ids(minusculas));
    }

    // CA11
    [Fact]
    public void Listar_DosPedidosConLaMismaFecha_SalePrimeroElDeMenorId()
    {
        var mismaFecha = new DateOnly(2026, 10, 15);
        var repositorio = new RepositorioFalso(
        [
            new Pedido(7, "PED-STGO-000007", "C-001", "Cliente A", mismaFecha, "Pendiente", 1000, "STGO"),
            new Pedido(3, "PED-STGO-000003", "C-001", "Cliente A", mismaFecha, "Pendiente", 2000, "STGO"),
            new Pedido(5, "PED-STGO-000005", "C-001", "Cliente A", new DateOnly(2026, 10, 1), "Pendiente", 3000, "STGO")
        ]);
        var listado = new ListadoPedidos(repositorio);

        var resultado = listado.Listar(Filtro());

        Assert.Equal([3, 7, 5], Ids(resultado));
    }

    // CA13
    [Fact]
    public void Listar_Pagina3_DevuelveLosUltimos5Pedidos()
    {
        var resultado = _listado.Listar(Filtro(pagina: "3"));

        Assert.Equal([36, 17, 38, 19, 40], Ids(resultado));
        Assert.Equal(3, resultado.Pagina);
    }

    // CA14
    [Fact]
    public void Listar_Pagina2_Devuelve20PedidosDel39Al15()
    {
        var resultado = _listado.Listar(Filtro(pagina: "2"));

        Assert.Equal(20, resultado.Pedidos.Count);
        Assert.Equal(39, resultado.Pedidos[0].Id);
        Assert.Equal(15, resultado.Pedidos[^1].Id);
    }

    // CA15
    [Fact]
    public void Listar_PaginaDespuesDeLaUltima_DevuelveListaVaciaConMetadatos()
    {
        var resultado = _listado.Listar(Filtro(pagina: "4"));

        Assert.Empty(resultado.Pedidos);
        Assert.Equal(4, resultado.Pagina);
        Assert.Equal(45, resultado.TotalRegistros);
        Assert.Equal(3, resultado.TotalPaginas);
    }

    // CA16
    [Fact]
    public void Listar_TamanoPagina100_DevuelveLos45PedidosEnUnaPagina()
    {
        var resultado = _listado.Listar(Filtro(tamanoPagina: "100"));

        Assert.Equal(45, resultado.Pedidos.Count);
        Assert.Equal(1, resultado.TotalPaginas);
        Assert.Equal(100, resultado.TamanoPagina);
    }

    /// <summary>Repositorio de prueba para CA11: los datos de ejemplo no tienen fechas repetidas.</summary>
    private sealed class RepositorioFalso(IReadOnlyList<Pedido> pedidos) : IPedidoRepositorio
    {
        public IReadOnlyList<Pedido> ObtenerTodos() => pedidos;

        public Pedido? ObtenerPorId(int id) => pedidos.FirstOrDefault(p => p.Id == id);
    }
}
