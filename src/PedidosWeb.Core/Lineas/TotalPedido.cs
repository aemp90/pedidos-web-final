namespace PedidosWeb.Core.Lineas;

/// <summary>
/// Cálculo de subtotales y total de un pedido a partir de sus líneas (HU-02).
/// Montos en pesos chilenos (CLP), <c>decimal</c> sin decimales y sin IVA.
/// </summary>
public static class TotalPedido
{
    /// <summary>Subtotal de una línea: cantidad × precio unitario.</summary>
    public static decimal CalcularSubtotal(LineaPedido linea)
    {
        ArgumentNullException.ThrowIfNull(linea);
        return linea.Cantidad * linea.PrecioUnitario;
    }

    /// <summary>
    /// Total del pedido: suma de los subtotales de sus líneas. Sin líneas, el total es 0.
    /// Siempre se calcula desde las líneas; no se copia de <c>Pedido.Total</c>.
    /// </summary>
    public static decimal CalcularTotal(IEnumerable<LineaPedido> lineas)
    {
        ArgumentNullException.ThrowIfNull(lineas);
        return lineas.Sum(CalcularSubtotal);
    }
}
