using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

/// <summary>
/// HU-03 · Cálculo de totales (specs/HU-03-calculo-totales/spec.md).
/// Los casos son pedidos reales de legado/db/datos/ (folio en el comentario) y el total esperado es el grabado.
/// </summary>
public class CalculadoraTotalesTests
{
    private static LineaCalculo L(int cantidad, decimal precio, string categoria) => new(cantidad, precio, categoria);

    // CA-01 · folio 1002
    [Fact]
    public void Calcular_Folio1002SinDescuento_DevuelveTotal231514()
    {
        var r = CalculadoraTotales.Calcular([L(5, 8210m, "BEB"), L(30, 5000m, "CON")],
            esVip: false, region: 13, fleteBase: 3500m);

        Assert.Equal(191050m, r.Subtotal);
        Assert.Equal(0m, r.PorcentajeDescuento);
        Assert.Equal(0m, r.Descuento);
        Assert.Equal(3500m, r.Flete);
        Assert.Equal(36964m, r.Iva);
        Assert.Equal(231514m, r.Total);
    }

    // CA-02 · folio 1019
    [Fact]
    public void Calcular_BaseExacta500000_Aplica5Porciento()
    {
        var r = CalculadoraTotales.Calcular([L(40, 12500m, "ABA")],
            esVip: false, region: 1, fleteBase: 8900m);

        Assert.Equal(0.05m, r.PorcentajeDescuento);
        Assert.Equal(25000m, r.Descuento);
        Assert.Equal(575841m, r.Total);
    }

    // CA-03 · folio 1035
    [Fact]
    public void Calcular_BaseExacta1000000_Aplica8Porciento()
    {
        var r = CalculadoraTotales.Calcular([L(20, 50000m, "ABA")],
            esVip: false, region: 6, fleteBase: 4900m);

        Assert.Equal(0.08m, r.PorcentajeDescuento);
        Assert.Equal(80000m, r.Descuento);
        Assert.Equal(1100631m, r.Total);
    }

    // CA-04 · folio 1011 (la pantalla del legado mostraba 588.931)
    [Fact]
    public void Calcular_ClienteVipConBase500000_Aplica8Porciento()
    {
        var r = CalculadoraTotales.Calcular([L(50, 10000m, "BEB")],
            esVip: true, region: 10, fleteBase: 9900m);

        Assert.Equal(0.08m, r.PorcentajeDescuento);
        Assert.Equal(40000m, r.Descuento);
        Assert.Equal(559181m, r.Total);
    }

    // CA-05 · folio 1038
    [Fact]
    public void Calcular_ClienteVipSinVolumen_Aplica3Porciento()
    {
        var r = CalculadoraTotales.Calcular([L(4, 8210m, "BEB")],
            esVip: true, region: 13, fleteBase: 3500m);

        Assert.Equal(0.03m, r.PorcentajeDescuento);
        Assert.Equal(985m, r.Descuento);
        Assert.Equal(42072m, r.Total);
    }

    // CA-06 · folio 1027
    [Fact]
    public void Calcular_VipConBaseSobreUnMillon_TopaEn10PorcientoYRecargaRegion11()
    {
        var r = CalculadoraTotales.Calcular(
            [L(30, 29990m, "LIM"), L(1, 39990m, "OFE"), L(20, 22350m, "CON")],
            esVip: true, region: 11, fleteBase: 14990m);

        Assert.Equal(1346700m, r.BaseDescuento);
        Assert.Equal(0.10m, r.PorcentajeDescuento);
        Assert.Equal(134670m, r.Descuento);
        Assert.Equal(17238m, r.Flete);
        Assert.Equal(1510417m, r.Total);
    }

    // CA-07 · folio 1031
    [Fact]
    public void Calcular_LineasOfe_NoEntranEnLaBase()
    {
        var r = CalculadoraTotales.Calcular(
            [L(3, 18900m, "ABA"), L(2, 18350m, "LIM"), L(36, 35990m, "OFE")],
            esVip: false, region: 8, fleteBase: 6900m);

        Assert.Equal(1389040m, r.Subtotal);
        Assert.Equal(93400m, r.BaseDescuento);
        Assert.Equal(0m, r.PorcentajeDescuento);
        Assert.Equal(1661169m, r.Total);
    }

    // CA-08 · folio 1018
    [Fact]
    public void Calcular_Region12_RecargaFleteYRedondeaHaciaAbajo()
    {
        var r = CalculadoraTotales.Calcular([L(3, 19200m, "PER"), L(15, 5000m, "CON")],
            esVip: false, region: 12, fleteBase: 16490m);

        Assert.Equal(18963m, r.Flete); // 16.490 × 1,15 = 18.963,50 → baja
        Assert.Equal(180360m, r.Total);
    }

    // CA-09
    [Fact]
    public void Calcular_SinLineas_DevuelveTodoCero()
    {
        var r = CalculadoraTotales.Calcular([], esVip: false, region: 13, fleteBase: 3500m);

        Assert.Equal(new ResultadoTotales(0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m), r);
    }

    // CA-10 · líneas del folio 1002 con otra tasa
    [Fact]
    public void Calcular_TasaIva10Porciento_UsaLaTasaRecibida()
    {
        var r = CalculadoraTotales.Calcular([L(5, 8210m, "BEB"), L(30, 5000m, "CON")],
            esVip: false, region: 13, fleteBase: 3500m, tasaIva: 0.10m);

        Assert.Equal(19455m, r.Iva);
        Assert.Equal(214005m, r.Total);
    }

    // CA-12
    [Theory]
    [InlineData(0, 1000)]
    [InlineData(-1, 1000)]
    [InlineData(5, 0)]
    [InlineData(5, -1)]
    public void Calcular_LineaConCantidadOPrecioNoPositivo_LanzaArgumentException(int cantidad, int precio)
    {
        LineaCalculo[] lineas = [L(1, 1000m, "ABA"), L(cantidad, precio, "ABA")];

        var ex = Assert.Throws<ArgumentException>(() =>
            CalculadoraTotales.Calcular(lineas, esVip: false, region: 13, fleteBase: 3500m));

        Assert.Contains("línea 2", ex.Message);
    }

    // CA-11 · los 60 pedidos de legado/db/datos/ (copiados a bin/.../DatosLegado/ por el .csproj)
    [Fact]
    public void Calcular_Los60PedidosDelLegado_CoincidenConTotalGrabado()
    {
        var productos = LeerCsv("PRODUCTOS.csv").ToDictionary(p => p["ID_PRODUCTO"], p => p["CATEGORIA"]);
        var clientes = LeerCsv("CLIENTES.csv").ToDictionary(c => c["ID_CLIENTE"]);
        var fletes = LeerCsv("REGIONES.csv").ToDictionary(r => r["CODIGO"], r => decimal.Parse(r["FLETE_BASE"]));
        var lineasPorFolio = LeerCsv("PEDIDO_LINEAS.csv").ToLookup(l => l["FOLIO"]);
        var pedidos = LeerCsv("PEDIDOS.csv");

        var descuadrados = pedidos
            .Where(p =>
            {
                var cliente = clientes[p["ID_CLIENTE"]];
                var lineas = lineasPorFolio[p["FOLIO"]].Select(l => new LineaCalculo(
                    int.Parse(l["CANTIDAD"]), decimal.Parse(l["PRECIO_UNITARIO"]), productos[l["ID_PRODUCTO"]]));
                var r = CalculadoraTotales.Calcular(lineas,
                    esVip: cliente["TIPO"] == "V",
                    region: int.Parse(cliente["REGION"]),
                    fleteBase: fletes[cliente["REGION"]]);
                return r.Total != decimal.Parse(p["TOTAL"]);
            })
            .Select(p => p["FOLIO"])
            .ToList();

        Assert.Equal(60, pedidos.Count);
        Assert.Empty(descuadrados);
    }

    /// <summary>
    /// Lector CSV mínimo para los datos del legado: encabezado en la primera línea y campos entre comillas
    /// (PRODUCTOS.csv tiene "AGUA MINERAL PACK 6 X 1,6 L"). Los números vienen sin separador de miles.
    /// </summary>
    private static List<Dictionary<string, string>> LeerCsv(string archivo)
    {
        var ruta = Path.Combine(AppContext.BaseDirectory, "DatosLegado", archivo);
        var filas = File.ReadAllLines(ruta, System.Text.Encoding.UTF8)
            .Where(linea => linea.Length > 0)
            .Select(SepararCampos)
            .ToList();
        var encabezado = filas[0];

        return filas.Skip(1)
            .Select(campos => encabezado.Zip(campos).ToDictionary(par => par.First, par => par.Second))
            .ToList();
    }

    private static List<string> SepararCampos(string linea)
    {
        var campos = new List<string>();
        var actual = new System.Text.StringBuilder();
        var entreComillas = false;

        foreach (var c in linea)
        {
            if (c == '"')
                entreComillas = !entreComillas;
            else if (c == ',' && !entreComillas)
            {
                campos.Add(actual.ToString());
                actual.Clear();
            }
            else
                actual.Append(c);
        }

        campos.Add(actual.ToString());
        return campos;
    }
}
