using System.Globalization;
using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

/// <summary>
/// Golden master de la migración del cálculo de totales (Unidad 5, etapa 1).
/// Cada fila de migracion/golden-master.csv es un caso: lo que calcula hoy UCalculoTotales.pas,
/// defectos incluidos. El CSV no se edita para que un test pase (ver migracion/README.md).
/// </summary>
public class CalculoTotalesTests
{
    private static readonly string RutaGoldenMaster = Path.Combine(AppContext.BaseDirectory, "golden-master.csv");

    private static readonly Lazy<IReadOnlyDictionary<int, CasoGoldenMaster>> Casos = new(LeerCasos);

    /// <summary>Un dato por caso: el número basta para que el explorador de tests muestre 60 tests separados.</summary>
    public static TheoryData<int> NumerosDeCaso()
    {
        var datos = new TheoryData<int>();
        foreach (var caso in Casos.Value.Keys.Order())
            datos.Add(caso);
        return datos;
    }

    [Theory]
    [MemberData(nameof(NumerosDeCaso))]
    public void Calcular_CasoDelGoldenMaster_IgualaDescuentoFleteIvaYTotalDelLegado(int caso)
    {
        var c = Casos.Value[caso];

        var r = CalculoTotales.Calcular(c.Lineas, c.EsVip, c.Region, c.FleteBase, c.TasaIva);

        // Se comparan juntos para que, si falla, el mensaje muestre los cuatro valores del caso (folio en c.Folio).
        Assert.Equal(
            (c.Folio, c.Descuento, c.Flete, c.Iva, c.Total),
            (c.Folio, r.Descuento, r.Flete, r.Iva, r.Total));
    }

    private sealed record CasoGoldenMaster(
        int Caso,
        int Folio,
        bool EsVip,
        int Region,
        decimal FleteBase,
        decimal TasaIva,
        IReadOnlyList<LineaCalculo> Lineas,
        decimal Descuento,
        decimal Flete,
        decimal Iva,
        decimal Total);

    // Columnas: caso,folio,tipo_cliente,region,flete_base,tasa_iva,lineas,subtotal,base_descuento,
    //           porc_descuento,descuento,flete,neto,iva,total
    private static IReadOnlyDictionary<int, CasoGoldenMaster> LeerCasos()
    {
        var filas = File.ReadAllLines(RutaGoldenMaster)
            .Skip(1)
            .Where(f => !string.IsNullOrWhiteSpace(f));

        var casos = new Dictionary<int, CasoGoldenMaster>();
        foreach (var fila in filas)
        {
            var c = fila.Split(',');
            var caso = new CasoGoldenMaster(
                Caso: int.Parse(c[0], CultureInfo.InvariantCulture),
                Folio: int.Parse(c[1], CultureInfo.InvariantCulture),
                EsVip: c[2] == "V",
                Region: int.Parse(c[3], CultureInfo.InvariantCulture),
                FleteBase: Decimal(c[4]),
                TasaIva: Decimal(c[5]),
                Lineas: LeerLineas(c[6]),
                Descuento: Decimal(c[10]),
                Flete: Decimal(c[11]),
                Iva: Decimal(c[13]),
                Total: Decimal(c[14]));
            casos.Add(caso.Caso, caso);
        }
        return casos;
    }

    // Formato: cantidad x precio x categoría, separadas por '|'. Ej.: 5x8210xBEB|30x5000xCON
    private static IReadOnlyList<LineaCalculo> LeerLineas(string texto) =>
        texto.Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Split('x'))
            .Select(p => new LineaCalculo(int.Parse(p[0], CultureInfo.InvariantCulture), Decimal(p[1]), p[2]))
            .ToList();

    private static decimal Decimal(string valor) => decimal.Parse(valor, NumberStyles.Number, CultureInfo.InvariantCulture);
}
