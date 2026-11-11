namespace PedidosWeb.Core.Totales;

/// <summary>
/// Una línea del pedido con lo justo para calcular sus totales (HU-03).
/// </summary>
/// <param name="Cantidad">Unidades pedidas; debe ser mayor que 0.</param>
/// <param name="PrecioUnitario">Precio de una unidad en pesos chilenos (CLP), sin IVA; debe ser mayor que 0.</param>
/// <param name="Categoria">Categoría del producto (ej. <c>ABA</c>, <c>OFE</c>). Las líneas <c>OFE</c> no entran en la base del descuento.</param>
public sealed record LineaCalculo(int Cantidad, decimal PrecioUnitario, string Categoria);
