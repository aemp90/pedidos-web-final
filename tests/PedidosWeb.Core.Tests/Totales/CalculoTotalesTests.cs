using System.Globalization;
using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

/// <summary>
/// HU-03 · Cálculo de totales (specs/HU-03-calculo-totales/spec.md).
/// Los casos son pedidos reales de legado/db/datos/ (folio en el comentario). Los totales esperados son los
/// grabados por el legado, salvo donde cambia por R05 (redondeo) o D2 (umbrales): ver
/// docs/migracion/defectos-corregidos.md.
/// </summary>
public class CalculoTotalesTests
{
    private static LineaCalculo L(int cantidad, decimal precio, string categoria) => new(cantidad, precio, categoria);

    // CA-01 · folio 1002 (el legado grabó IVA 36.964 y total 231.514: R05)
    [Fact]
    public void Calcular_Folio1002SinDescuento_DevuelveTotal231515()
    {
        var r = CalculoTotales.Calcular([L(5, 8210m, "BEB"), L(30, 5000m, "CON")],
            esVip: false, region: 13, fleteBase: 3500m);

        Assert.Equal(191050m, r.Subtotal);
        Assert.Equal(0m, r.PorcentajeDescuento);
        Assert.Equal(0m, r.Descuento);
        Assert.Equal(3500m, r.Flete);
        Assert.Equal(36965m, r.Iva); // 194.550 × 0,19 = 36.964,50 → sube
        Assert.Equal(231515m, r.Total);
    }

    // CA-02 · folio 1019
    [Fact]
    public void Calcular_BaseExacta500000_Aplica5Porciento()
    {
        var r = CalculoTotales.Calcular([L(40, 12500m, "ABA")],
            esVip: false, region: 1, fleteBase: 8900m);

        Assert.Equal(0.05m, r.PorcentajeDescuento);
        Assert.Equal(25000m, r.Descuento);
        Assert.Equal(575841m, r.Total);
    }

    // CA-03 · folio 1035
    [Fact]
    public void Calcular_BaseExacta1000000_Aplica8Porciento()
    {
        var r = CalculoTotales.Calcular([L(20, 50000m, "ABA")],
            esVip: false, region: 6, fleteBase: 4900m);

        Assert.Equal(0.08m, r.PorcentajeDescuento);
        Assert.Equal(80000m, r.Descuento);
        Assert.Equal(1100631m, r.Total);
    }

    // CA-04 · folio 1011 (la pantalla del legado mostraba 588.931)
    [Fact]
    public void Calcular_ClienteVipConBase500000_Aplica8Porciento()
    {
        var r = CalculoTotales.Calcular([L(50, 10000m, "BEB")],
            esVip: true, region: 10, fleteBase: 9900m);

        Assert.Equal(0.08m, r.PorcentajeDescuento);
        Assert.Equal(40000m, r.Descuento);
        Assert.Equal(559181m, r.Total);
    }

    // CA-05 · folio 1038
    [Fact]
    public void Calcular_ClienteVipSinVolumen_Aplica3Porciento()
    {
        var r = CalculoTotales.Calcular([L(4, 8210m, "BEB")],
            esVip: true, region: 13, fleteBase: 3500m);

        Assert.Equal(0.03m, r.PorcentajeDescuento);
        Assert.Equal(985m, r.Descuento);
        Assert.Equal(42072m, r.Total);
    }

    // CA-06 · folio 1027 (el legado grabó flete 17.238 y total 1.510.417: R05)
    [Fact]
    public void Calcular_VipConBaseSobreUnMillon_TopaEn10PorcientoYRecargaRegion11()
    {
        var r = CalculoTotales.Calcular(
            [L(30, 29990m, "LIM"), L(1, 39990m, "OFE"), L(20, 22350m, "CON")],
            esVip: true, region: 11, fleteBase: 14990m);

        Assert.Equal(1346700m, r.BaseDescuento);
        Assert.Equal(0.10m, r.PorcentajeDescuento);
        Assert.Equal(134670m, r.Descuento);
        Assert.Equal(17239m, r.Flete); // 14.990 × 1,15 = 17.238,50 → sube
        Assert.Equal(1510418m, r.Total);
    }

    // CA-07 · folio 1031
    [Fact]
    public void Calcular_LineasOfe_NoEntranEnLaBase()
    {
        var r = CalculoTotales.Calcular(
            [L(3, 18900m, "ABA"), L(2, 18350m, "LIM"), L(36, 35990m, "OFE")],
            esVip: false, region: 8, fleteBase: 6900m);

        Assert.Equal(1389040m, r.Subtotal);
        Assert.Equal(93400m, r.BaseDescuento);
        Assert.Equal(0m, r.PorcentajeDescuento);
        Assert.Equal(1661169m, r.Total);
    }

    // CA-08 · folio 1018 (R05: el legado grabó flete 18.963 y total 180.360)
    [Fact]
    public void Calcular_Region12_RecargaFleteYRedondeaHaciaArriba()
    {
        var r = CalculoTotales.Calcular([L(3, 19200m, "PER"), L(15, 5000m, "CON")],
            esVip: false, region: 12, fleteBase: 16490m);

        Assert.Equal(18964m, r.Flete); // 16.490 × 1,15 = 18.963,50 → sube
        Assert.Equal(180361m, r.Total);
    }

    // CA-09
    [Fact]
    public void Calcular_SinLineas_DevuelveTodoCero()
    {
        var r = CalculoTotales.Calcular([], esVip: false, region: 13, fleteBase: 3500m);

        Assert.Equal(new ResultadoTotales(0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m), r);
    }

    // CA-10 · líneas del folio 1002 con otra tasa
    [Fact]
    public void Calcular_TasaIva10Porciento_UsaLaTasaRecibida()
    {
        var r = CalculoTotales.Calcular([L(5, 8210m, "BEB"), L(30, 5000m, "CON")],
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
            CalculoTotales.Calcular(lineas, esVip: false, region: 13, fleteBase: 3500m));

        Assert.Contains("línea 2", ex.Message);
    }

    // ---------------------------------------------------------------------------------------------
    // U6 · Tests de borde que el golden master no cubre.
    // ---------------------------------------------------------------------------------------------

    // Un peso bajo cada umbral (D1): el CSV solo trae bases de 500.000 y 1.000.000 exactos.
    [Theory]
    [InlineData(499_999, 0.00, 0)]          // queda en 0 %
    [InlineData(999_999, 0.05, 50_000)]     // 49.999,95 → 50.000, queda en 5 %
    public void Calcular_BaseUnPesoBajoElUmbral_NoSubeDeTramo(int precio, decimal porcentaje, int descuento)
    {
        var r = CalculoTotales.Calcular([L(1, precio, "ABA")], esVip: false, region: 13, fleteBase: 3500m);

        Assert.Equal(porcentaje, r.PorcentajeDescuento);
        Assert.Equal(descuento, r.Descuento);
    }

    // Regla 3: igual que UpperCase(Trim(...)) del legado; el CSV siempre trae "OFE" limpio.
    [Theory]
    [InlineData("ofe")]
    [InlineData(" OFE ")]
    [InlineData("Ofe")]
    public void Calcular_CategoriaOfeConOtraEscritura_NoEntraEnLaBase(string categoria)
    {
        var r = CalculoTotales.Calcular([L(1, 100_000m, "ABA"), L(1, 600_000m, categoria)],
            esVip: false, region: 13, fleteBase: 3500m);

        Assert.Equal(100_000m, r.BaseDescuento);
        Assert.Equal(0m, r.PorcentajeDescuento);
    }

    // Sin categoría no es oferta: entra en la base y no se cae.
    [Fact]
    public void Calcular_LineaSinCategoria_EntraEnLaBaseSinFallar()
    {
        var r = CalculoTotales.Calcular([L(1, 500_000m, null!)], esVip: false, region: 13, fleteBase: 3500m);

        Assert.Equal(500_000m, r.BaseDescuento);
        Assert.Equal(0.05m, r.PorcentajeDescuento);
    }

    // CA-12: una línea nula se informa con su número, no como NullReferenceException.
    [Fact]
    public void Calcular_LineaNula_LanzaArgumentExceptionConElNumeroDeLinea()
    {
        LineaCalculo[] lineas = [L(1, 1000m, "ABA"), null!];

        var ex = Assert.Throws<ArgumentException>(() =>
            CalculoTotales.Calcular(lineas, esVip: false, region: 13, fleteBase: 3500m));

        Assert.Contains("línea 2", ex.Message);
    }

    // Lista nula ≠ sin líneas (CA-09): es un error de quien llama, no un pedido en cero.
    [Fact]
    public void Calcular_ListaNula_LanzaArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            CalculoTotales.Calcular(null!, esVip: false, region: 13, fleteBase: 3500m));

        Assert.Equal("lineas", ex.ParamName);
    }

    // ---------------------------------------------------------------------------------------------
    // CA-11 · Cuadratura con el histórico: los 60 pedidos de legado/db/datos/ (copiados a
    // bin/.../DatosLegado/ por el .csproj). Solo descuadran los 13 folios de R05, y por a lo más 2 pesos.
    // ---------------------------------------------------------------------------------------------

    private static readonly string[] FoliosQueCambianPorR05 =
        ["1002", "1006", "1008", "1018", "1021", "1023", "1025", "1027", "1036", "1037", "1042", "1046", "1054"];

    [Fact]
    public void Calcular_Los60PedidosDelLegado_SoloDescuadranLosFoliosDeR05PorHasta2Pesos()
    {
        var productos = LeerCsv("PRODUCTOS.csv").ToDictionary(p => p["ID_PRODUCTO"], p => p["CATEGORIA"]);
        var clientes = LeerCsv("CLIENTES.csv").ToDictionary(c => c["ID_CLIENTE"]);
        var fletes = LeerCsv("REGIONES.csv").ToDictionary(r => r["CODIGO"], r => Decimal(r["FLETE_BASE"]));
        var lineasPorFolio = LeerCsv("PEDIDO_LINEAS.csv").ToLookup(l => l["FOLIO"]);
        var pedidos = LeerCsv("PEDIDOS.csv");

        var diferencias = pedidos
            .Select(p =>
            {
                var cliente = clientes[p["ID_CLIENTE"]];
                var lineas = lineasPorFolio[p["FOLIO"]].Select(l => new LineaCalculo(
                    int.Parse(l["CANTIDAD"], CultureInfo.InvariantCulture),
                    Decimal(l["PRECIO_UNITARIO"]),
                    productos[l["ID_PRODUCTO"]]));
                var r = CalculoTotales.Calcular(lineas,
                    esVip: cliente["TIPO"] == "V",
                    region: int.Parse(cliente["REGION"], CultureInfo.InvariantCulture),
                    fleteBase: fletes[cliente["REGION"]]);
                return (Folio: p["FOLIO"], Diferencia: r.Total - Decimal(p["TOTAL"]));
            })
            .Where(d => d.Diferencia != 0)
            .ToList();

        Assert.Equal(60, pedidos.Count);
        Assert.Equal(FoliosQueCambianPorR05, diferencias.Select(d => d.Folio).Order());
        Assert.All(diferencias, d => Assert.InRange(Math.Abs(d.Diferencia), 1m, 2m));
    }

    // ---------------------------------------------------------------------------------------------
    // CA-13 · Golden master: cada fila de migracion/golden-master.csv es un caso. Es lo que calculaba
    // UCalculoTotales.pas, con R05 (D1) y los umbrales (D2) corregidos. No se edita para que un test pase.
    // ---------------------------------------------------------------------------------------------

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
    public void Calcular_CasoDelGoldenMaster_IgualaPorcentajeDescuentoFleteIvaYTotal(int caso)
    {
        var c = Casos.Value[caso];

        var r = CalculoTotales.Calcular(c.Lineas, c.EsVip, c.Region, c.FleteBase, c.TasaIva);

        // Se comparan juntos para que, si falla, el mensaje muestre todos los valores del caso (folio en c.Folio).
        Assert.Equal(
            (c.Folio, c.PorcentajeDescuento, c.Descuento, c.Flete, c.Iva, c.Total),
            (c.Folio, r.PorcentajeDescuento, r.Descuento, r.Flete, r.Iva, r.Total));
    }

    private sealed record CasoGoldenMaster(
        int Caso,
        int Folio,
        bool EsVip,
        int Region,
        decimal FleteBase,
        decimal TasaIva,
        IReadOnlyList<LineaCalculo> Lineas,
        decimal PorcentajeDescuento,
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
                PorcentajeDescuento: Decimal(c[9]),
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
