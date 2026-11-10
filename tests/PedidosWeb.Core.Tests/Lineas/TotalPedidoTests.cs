using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Lineas;

/// <summary>HU-02 · Incremento 1 de 3: calcular el total (specs/HU-02-detalle-pedido/plan.md).</summary>
public class TotalPedidoTests
{
    private readonly LineaPedidoRepositorioEnMemoria _lineas = new();

    // CA-02
    [Fact]
    public void CalcularSubtotal_LineasDelPedido8_DevuelveCantidadPorPrecio()
    {
        var subtotales = _lineas.ObtenerPorPedido(8).Select(TotalPedido.CalcularSubtotal);

        Assert.Equal(new[] { 64500m, 35400m, 20200m }, subtotales);
    }

    // CA-03
    [Fact]
    public void CalcularTotal_Pedido8_Devuelve120100()
    {
        var total = TotalPedido.CalcularTotal(_lineas.ObtenerPorPedido(8));

        Assert.Equal(120100m, total);
    }

    // CA-04
    [Fact]
    public void CalcularTotal_Los45PedidosDeEjemplo_CoincideConPedidoTotal()
    {
        var pedidos = new PedidoRepositorioEnMemoria().ObtenerTodos();

        var descuadrados = pedidos
            .Where(p => TotalPedido.CalcularTotal(_lineas.ObtenerPorPedido(p.Id)) != p.Total)
            .Select(p => p.Id)
            .ToList();

        Assert.Equal(45, pedidos.Count);
        Assert.Empty(descuadrados);
    }

    // Apoyo (CA-06 se cubre en los incrementos 2 y 3)
    [Fact]
    public void CalcularTotal_SinLineas_DevuelveCero()
    {
        var total = TotalPedido.CalcularTotal(Array.Empty<LineaPedido>());

        Assert.Equal(0m, total);
    }
}
