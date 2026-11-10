using PedidosWeb.Core.Lineas;

namespace PedidosWeb.Core.Pedidos;

/// <summary>Arma el detalle de un pedido a partir del pedido y sus líneas (HU-02).</summary>
public sealed class DetallePedidos
{
    private readonly IPedidoRepositorio _pedidos;
    private readonly ILineaPedidoRepositorio _lineas;

    public DetallePedidos(IPedidoRepositorio pedidos, ILineaPedidoRepositorio lineas)
    {
        _pedidos = pedidos ?? throw new ArgumentNullException(nameof(pedidos));
        _lineas = lineas ?? throw new ArgumentNullException(nameof(lineas));
    }

    /// <summary>
    /// El detalle del pedido con ese id, o null si el pedido no existe.
    /// El total siempre se calcula desde las líneas, no se copia de <c>Pedido.Total</c>.
    /// </summary>
    public DetallePedido? Obtener(int id)
    {
        if (_pedidos.ObtenerPorId(id) is not { } pedido)
            return null;

        var lineas = _lineas.ObtenerPorPedido(id);

        var lineasDetalle = lineas
            .Select(l => new LineaDetalle(
                l.Numero, l.Producto, l.Cantidad, l.PrecioUnitario, TotalPedido.CalcularSubtotal(l)))
            .ToList();

        return new DetallePedido(
            pedido.Id,
            pedido.Folio,
            pedido.Fecha,
            pedido.Estado,
            pedido.Sucursal,
            new ClienteDetalle(pedido.ClienteId, pedido.ClienteNombre),
            lineasDetalle,
            TotalPedido.CalcularTotal(lineas));
    }
}
