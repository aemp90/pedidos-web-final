namespace PedidosWeb.Core.Totales;

/// <summary>
/// Cálculo del total de un pedido: descuento por volumen y VIP, flete por región e IVA (HU-03).
/// Es el único cálculo de totales: reemplaza a los dos del legado (<c>UCalculoTotales.pas</c> y
/// <c>UFrmPedidos.pas:296-319</c>), que no coincidían (RS-05).
/// Reglas en <c>docs/diagnostico/reglas-negocio.md</c> (reglas 1–11, decisiones D1, D3 y D7).
/// </summary>
public static class CalculadoraTotales
{
    /// <summary>Tasa de IVA por defecto (regla 11, parámetro <c>TASA_IVA</c> del legado).</summary>
    public const decimal TasaIvaPorDefecto = 0.19m;

    /// <summary>Desde esta base (inclusive) se aplica el 8 % (D1).</summary>
    public const decimal LimiteVolumenAlto = 1_000_000m;

    /// <summary>Desde esta base (inclusive) se aplica el 5 % (D1).</summary>
    public const decimal LimiteVolumenBajo = 500_000m;

    public const decimal DescuentoVolumenAlto = 0.08m;
    public const decimal DescuentoVolumenBajo = 0.05m;

    /// <summary>Puntos que suma un cliente VIP (regla 4).</summary>
    public const decimal DescuentoVip = 0.03m;

    /// <summary>Porcentaje máximo de descuento (regla 6).</summary>
    public const decimal TopeDescuento = 0.10m;

    /// <summary>Recargo del flete en las regiones 11 y 12 (regla 8).</summary>
    public const decimal RecargoFleteAustral = 1.15m;

    /// <summary>Categoría de las ofertas, que no entran en la base del descuento (regla 3).</summary>
    public const string CategoriaOferta = "OFE";

    /// <summary>
    /// Redondeo «al peso» del legado: <c>Trunc(x + 0.49)</c> (<c>UCalculoTotales.pas:50</c>).
    /// OJO: un monto terminado en ,50 BAJA (18.963,50 → 18.963). No es redondeo comercial, y es a propósito
    /// (D7): así los totales cuadran con el histórico. No lo "corrijas" sin hablar con contabilidad.
    /// </summary>
    public static decimal RedondearAlPeso(decimal valor) => Math.Floor(valor + 0.49m);

    /// <summary>
    /// Porcentaje de descuento como fracción: 8 % si la base es ≥ 1.000.000, 5 % si es ≥ 500.000, si no 0;
    /// un VIP suma 3 puntos, y el total no pasa de 10 % (D1, reglas 1, 4 y 6).
    /// No hay descuento de navidad (D3).
    /// </summary>
    public static decimal CalcularPorcentajeDescuento(decimal baseDescuento, bool esVip)
    {
        var porcentaje = baseDescuento >= LimiteVolumenAlto ? DescuentoVolumenAlto
            : baseDescuento >= LimiteVolumenBajo ? DescuentoVolumenBajo
            : 0m;

        if (esVip)
            porcentaje += DescuentoVip;

        return Math.Min(porcentaje, TopeDescuento);
    }

    /// <summary>Flete de la región: con recargo de 15 % en las regiones 11 y 12, redondeado al peso (regla 8).</summary>
    public static decimal CalcularFlete(decimal fleteBase, int region)
    {
        var esAustral = region is 11 or 12;
        return RedondearAlPeso(esAustral ? fleteBase * RecargoFleteAustral : fleteBase);
    }

    /// <summary>
    /// Calcula el desglose del total del pedido. Sin líneas, todo es 0 (regla 9).
    /// </summary>
    /// <param name="lineas">Líneas del pedido.</param>
    /// <param name="esVip">Si el cliente es VIP.</param>
    /// <param name="region">Región del cliente (1 a 16).</param>
    /// <param name="fleteBase">Flete base de la región del cliente.</param>
    /// <param name="tasaIva">Tasa de IVA como fracción (0,19 por defecto).</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="lineas"/> es null.</exception>
    /// <exception cref="ArgumentException">Si alguna línea tiene cantidad o precio unitario ≤ 0.</exception>
    public static ResultadoTotales Calcular(
        IEnumerable<LineaCalculo> lineas,
        bool esVip,
        int region,
        decimal fleteBase,
        decimal tasaIva = TasaIvaPorDefecto)
    {
        ArgumentNullException.ThrowIfNull(lineas);
        var lista = lineas.ToList();
        Validar(lista);

        var subtotal = lista.Sum(SubtotalLinea);
        var baseDescuento = lista.Where(l => !EsOferta(l)).Sum(SubtotalLinea);
        var porcentaje = CalcularPorcentajeDescuento(baseDescuento, esVip);
        var descuento = RedondearAlPeso(baseDescuento * porcentaje);
        var neto = subtotal - descuento;
        var flete = lista.Count == 0 ? 0m : CalcularFlete(fleteBase, region);
        var iva = RedondearAlPeso((neto + flete) * tasaIva);

        return new ResultadoTotales(
            subtotal, baseDescuento, porcentaje, descuento, neto, flete, iva, neto + flete + iva);
    }

    private static decimal SubtotalLinea(LineaCalculo linea) => linea.Cantidad * linea.PrecioUnitario;

    // Igual que UpperCase(Trim(...)) <> 'OFE' del legado (UCalculoTotales.pas:70).
    private static bool EsOferta(LineaCalculo linea) =>
        string.Equals(linea.Categoria?.Trim(), CategoriaOferta, StringComparison.OrdinalIgnoreCase);

    // CA-12: nunca se calcula un total con datos imposibles.
    private static void Validar(IReadOnlyList<LineaCalculo> lineas)
    {
        for (var i = 0; i < lineas.Count; i++)
        {
            var linea = lineas[i] ?? throw new ArgumentException($"La línea {i + 1} es nula.", nameof(lineas));
            if (linea.Cantidad <= 0)
                throw new ArgumentException(
                    $"La línea {i + 1} tiene cantidad {linea.Cantidad}; debe ser mayor que 0.", nameof(lineas));
            if (linea.PrecioUnitario <= 0)
                throw new ArgumentException(
                    $"La línea {i + 1} tiene precio unitario {linea.PrecioUnitario}; debe ser mayor que 0.", nameof(lineas));
        }
    }
}
