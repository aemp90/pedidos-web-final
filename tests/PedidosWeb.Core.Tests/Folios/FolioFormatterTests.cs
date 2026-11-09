using PedidosWeb.Core.Folios;

namespace PedidosWeb.Core.Tests.Folios;

public class FolioFormatterTests
{
    // Casos base del README (tres primeras filas de la tabla de ejemplos).
    [Theory]
    [InlineData(123, "STGO", "PED-STGO-000123")]
    [InlineData(1, "VAL", "PED-VAL-000001")]
    [InlineData(999999, "CO", "PED-CO-999999")]
    public void FormatearFolio_CasosBase_DevuelveElFolioEsperado(int numero, string sucursal, string esperado)
    {
        var folio = FolioFormatter.FormatearFolio(numero, sucursal);

        Assert.Equal(esperado, folio);
    }

    // R2: se quitan espacios del inicio y del final, y se pasa a mayúsculas.
    [Fact]
    public void FormatearFolio_SucursalConEspaciosYMinusculas_LaNormaliza()
    {
        var folio = FolioFormatter.FormatearFolio(45, " stgo ");

        Assert.Equal("PED-STGO-000045", folio);
    }

    // R1: número fuera de 1..999999.
    [Theory]
    [InlineData(0, "STGO")]
    [InlineData(1000000, "STGO")]
    [InlineData(-5, "VAL")]
    public void FormatearFolio_NumeroFueraDeRango_LanzaArgumentOutOfRange(int numero, string sucursal)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FolioFormatter.FormatearFolio(numero, sucursal));
    }

    // R3: sucursal nula, vacía o que no son 2 a 4 letras A–Z.
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("S")]
    [InlineData("SANTI")]
    [InlineData("ÑUÑO")]
    [InlineData("ST1")]
    public void FormatearFolio_SucursalInvalida_LanzaArgumentException(string? sucursal)
    {
        Assert.ThrowsAny<ArgumentException>(() => FolioFormatter.FormatearFolio(7, sucursal!));
    }

    // R5: si número y sucursal son inválidos, gana la validación del número.
    [Fact]
    public void FormatearFolio_NumeroYSucursalInvalidos_ValidaPrimeroElNumero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FolioFormatter.FormatearFolio(0, "S"));
    }
}
