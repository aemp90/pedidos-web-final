namespace PedidosWeb.Core.Totales;

/// <summary>
/// Migración a C# de <c>legado/src/UCalculoTotales.pas</c>, defectos incluidos (Unidad 5).
/// Su contrato es <c>migracion/golden-master.csv</c>: tiene que igualar los 60 casos.
/// Etapa 1 (congelar): solo la firma; la implementación llega en la etapa 2.
/// </summary>
public static class CalculoTotales
{
    /// <summary>
    /// Calcula el desglose del total de un pedido tal como lo hace hoy el legado.
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
        throw new NotImplementedException("Etapa 2 de la migración: portar UCalculoTotales.pas.");
    }
}
