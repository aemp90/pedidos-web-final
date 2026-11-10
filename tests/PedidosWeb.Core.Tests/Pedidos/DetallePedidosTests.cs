using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

/// <summary>HU-02 · Incremento 2 de 3: obtener el detalle (specs/HU-02-detalle-pedido/plan.md).</summary>
public class DetallePedidosTests
{
    private readonly DetallePedidos _detalles =
        new(new PedidoRepositorioEnMemoria(), new LineaPedidoRepositorioEnMemoria());

    // CA-01
    [Fact]
    public void Obtener_Pedido8_DevuelveDatosDelPedidoYDelCliente()
    {
        var detalle = _detalles.Obtener(8);

        Assert.NotNull(detalle);
        Assert.Equal(8, detalle.Id);
        Assert.Equal("PED-STGO-000008", detalle.Folio);
        Assert.Equal(new DateOnly(2026, 10, 20), detalle.Fecha);
        Assert.Equal("Entregado", detalle.Estado);
        Assert.Equal("STGO", detalle.Sucursal);
        Assert.Equal(new ClienteDetalle("C-001", "Ferretería El Clavo"), detalle.Cliente);
    }

    // CA-06
    [Fact]
    public void Obtener_PedidoSinLineas_DevuelveLineasVaciasYTotalCero()
    {
        var detalles = new DetallePedidos(new PedidoRepositorioEnMemoria(), new LineasFalsas());

        var detalle = detalles.Obtener(8);

        Assert.NotNull(detalle);
        Assert.Empty(detalle.Lineas);
        Assert.Equal(0m, detalle.Total);
    }

    // Apoyo
    [Fact]
    public void Obtener_Pedido8_DevuelveLineasEnOrdenConSubtotalYTotal()
    {
        var detalle = _detalles.Obtener(8);

        Assert.NotNull(detalle);
        Assert.Equal(
            new[]
            {
                new LineaDetalle(1, "Harina para pan 25 kg", 3, 21500m, 64500m),
                new LineaDetalle(2, "Mantequilla 250 g", 12, 2950m, 35400m),
                new LineaDetalle(3, "Bebida cola 1,5 L", 10, 2020m, 20200m),
            },
            detalle.Lineas);
        Assert.Equal(120100m, detalle.Total);
    }

    // Apoyo
    [Fact]
    public void Obtener_IdInexistente_DevuelveNull()
    {
        Assert.Null(_detalles.Obtener(999));
    }

    /// <summary>Repositorio de prueba para CA-06: los datos de ejemplo no tienen pedidos sin líneas.</summary>
    private sealed class LineasFalsas : ILineaPedidoRepositorio
    {
        public IReadOnlyList<LineaPedido> ObtenerPorPedido(int pedidoId) => [];
    }
}
