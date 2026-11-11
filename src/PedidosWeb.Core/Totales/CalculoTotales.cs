namespace PedidosWeb.Core.Totales;

/// <summary>
/// Migración a C# de <c>legado/src/UCalculoTotales.pas</c>, defectos incluidos (Unidad 5, etapa 2).
/// Su contrato es <c>migracion/golden-master.csv</c>: tiene que igualar los 60 casos.
/// Se porta tal cual, función por función, cambiando <c>Double</c> por <c>decimal</c>.
/// Los defectos se corrigen recién en la etapa 3 (y quedan en <c>docs/migracion/defectos-corregidos.md</c>);
/// el cálculo aprobado para el negocio (HU-03) es <see cref="CalculadoraTotales"/>.
/// </summary>
public static class CalculoTotales
{
    /// <summary>
    /// Calcula el desglose del total de un pedido tal como lo hace hoy el legado
    /// (<c>CalcularTotales</c>, <c>UCalculoTotales.pas:108-122</c>).
    /// </summary>
    /// <param name="lineas">Líneas del pedido.</param>
    /// <param name="esVip">Si el cliente es VIP (<c>CLIENTES.TIPO = 'V'</c>).</param>
    /// <param name="region">Región del cliente.</param>
    /// <param name="fleteBase">Flete base de la región del cliente, en pesos chilenos.</param>
    /// <param name="tasaIva">Tasa de IVA como fracción (0,19).</param>
    public static ResultadoTotales Calcular(
        IEnumerable<LineaCalculo> lineas,
        bool esVip,
        int region,
        decimal fleteBase,
        decimal tasaIva)
    {
        ArgumentNullException.ThrowIfNull(lineas);
        var lista = lineas.ToList();

        // Igual que el legado: sin validar cantidades ni precios (el legado no valida nada aquí).
        var subtotal = CalcularSubtotal(lista);
        var baseDescuento = CalcularBaseDescuento(lista);
        var porcentaje = PorcentajeDescuento(baseDescuento, esVip);
        var descuento = Calc3(baseDescuento * porcentaje);
        var neto = subtotal - descuento;
        var flete = lista.Count == 0 ? 0m : CalcularFlete(fleteBase, region); // :116-119
        var iva = CalcularIva(neto, flete, tasaIva);

        return new ResultadoTotales(subtotal, baseDescuento, porcentaje, descuento, neto, flete, iva,
            neto + flete + iva);
    }

    /// <summary>
    /// «Redondeo al peso» del legado: <c>Trunc(AValor + 0.49)</c> (<c>:48-51</c>).
    /// Un monto terminado en ,50 BAJA. <c>Trunc</c> corta hacia cero, igual que <see cref="Math.Truncate(decimal)"/>.
    /// </summary>
    internal static decimal Calc3(decimal valor) => Math.Truncate(valor + 0.49m);

    // :53-60
    internal static decimal CalcularSubtotal(IReadOnlyList<LineaCalculo> lineas)
    {
        var resultado = 0m;
        foreach (var linea in lineas)
            resultado += linea.Cantidad * linea.PrecioUnitario;
        return resultado;
    }

    // :63-73 · suma de las líneas que entran al descuento (todas menos OFE).
    internal static decimal CalcularBaseDescuento(IReadOnlyList<LineaCalculo> lineas)
    {
        var resultado = 0m;
        foreach (var linea in lineas)
        {
            if (UpperCaseDelphi(TrimDelphi(linea.Categoria)) != "OFE")
                resultado += linea.Cantidad * linea.PrecioUnitario;
        }
        return resultado;
    }

    // :75-83 · DEFECTO del legado: límites con '>' (500.000 y 1.000.000 exactos no tienen descuento).
    // El comentario de :4 («5 % sobre 400.000») no corresponde al código.
    internal static decimal PorcentajeVolumen(decimal baseDescuento)
    {
        if (baseDescuento > 1_000_000m)
            return 0.08m;
        if (baseDescuento > 500_000m)
            return 0.05m;
        return 0m;
    }

    // :85-92 · VIP suma 3 puntos; tope 10 %.
    internal static decimal PorcentajeDescuento(decimal baseDescuento, bool esVip)
    {
        var resultado = PorcentajeVolumen(baseDescuento);
        if (esVip)
            resultado += 0.03m;
        if (resultado > 0.10m)
            resultado = 0.10m;
        return resultado;
    }

    // :95-101 · flete por región, con 15 % de recargo en las regiones 11 y 12.
    internal static decimal CalcularFlete(decimal fleteBase, int region)
    {
        if (region == 11 || region == 12)
            return Calc3(fleteBase * 1.15m);
        return Calc3(fleteBase);
    }

    // :103-106
    internal static decimal CalcularIva(decimal neto, decimal flete, decimal tasa) => Calc3((neto + flete) * tasa);

    // Trim de Delphi (SysUtils): quita los caracteres <= ' ' de ambos extremos. Un string de Delphi nunca es nil.
    private static string TrimDelphi(string? texto) => (texto ?? string.Empty).Trim(CaracteresDeControlYEspacio);

    // UpperCase de Delphi: solo convierte 'a'..'z' (ASCII), no letras con tilde.
    private static string UpperCaseDelphi(string texto) =>
        string.Create(texto.Length, texto, (destino, origen) =>
        {
            for (var i = 0; i < origen.Length; i++)
                destino[i] = origen[i] is >= 'a' and <= 'z' ? (char)(origen[i] - 32) : origen[i];
        });

    private static readonly char[] CaracteresDeControlYEspacio =
        Enumerable.Range(0, 33).Select(c => (char)c).ToArray();
}
