namespace PedidosWeb.Core.Totales;

/// <summary>
/// Desglose del total de un pedido (HU-03). Montos en pesos chilenos (CLP), <c>decimal</c> sin decimales.
/// </summary>
/// <param name="Subtotal">Suma de cantidad × precio unitario de todas las líneas.</param>
/// <param name="BaseDescuento">Lo mismo que el subtotal, pero sin las líneas de categoría <c>OFE</c>.</param>
/// <param name="PorcentajeDescuento">Porcentaje aplicado, como fracción (0,05 = 5 %), con tope de 0,10.</param>
/// <param name="Descuento">Base del descuento × porcentaje, redondeado al peso.</param>
/// <param name="Neto">Subtotal − descuento.</param>
/// <param name="Flete">Flete de la región, con recargo en las regiones 11 y 12; 0 si no hay líneas.</param>
/// <param name="Iva">(Neto + flete) × tasa de IVA, redondeado al peso.</param>
/// <param name="Total">Neto + flete + IVA.</param>
public sealed record ResultadoTotales(
    decimal Subtotal,
    decimal BaseDescuento,
    decimal PorcentajeDescuento,
    decimal Descuento,
    decimal Neto,
    decimal Flete,
    decimal Iva,
    decimal Total);
