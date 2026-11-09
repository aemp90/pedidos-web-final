using System.Text.RegularExpressions;

namespace PedidosWeb.Core.Folios;

/// <summary>
/// Genera el folio de un pedido con el mismo formato del sistema legado.
/// Las reglas completas y los ejemplos están en el README, sección «La función del taller».
/// </summary>
public static class FolioFormatter
{
    private const int NumeroMinimo = 1;
    private const int NumeroMaximo = 999999;

    // R3: solo letras A–Z ASCII (sin Ñ ni tildes), entre 2 y 4. CultureInvariant evita sorpresas por idioma del equipo.
    private static readonly Regex PatronSucursal =
        new(@"^[A-Z]{2,4}$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    /// <summary>
    /// Devuelve el folio con el formato PED-{SUCURSAL}-{NUMERO}, por ejemplo "PED-STGO-000123".
    /// </summary>
    /// <param name="numero">Número correlativo del pedido, entre 1 y 999999.</param>
    /// <param name="sucursal">Código de la sucursal: de 2 a 4 letras de la A a la Z.</param>
    /// <exception cref="ArgumentOutOfRangeException">Si <paramref name="numero"/> está fuera de rango (R1).</exception>
    /// <exception cref="ArgumentException">Si <paramref name="sucursal"/> es nula, vacía o no cumple R3.</exception>
    public static string FormatearFolio(int numero, string sucursal)
    {
        // R1 y R5: el número se valida antes que la sucursal.
        if (numero < NumeroMinimo || numero > NumeroMaximo)
            throw new ArgumentOutOfRangeException(nameof(numero), numero,
                $"El número debe estar entre {NumeroMinimo} y {NumeroMaximo}.");

        string codigoSucursal = NormalizarSucursal(sucursal);

        // R4: número con ceros a la izquierda hasta 6 dígitos.
        return $"PED-{codigoSucursal}-{numero:D6}";
    }

    /// <summary>
    /// Aplica R2 (trim + mayúsculas) y valida R3.
    /// </summary>
    private static string NormalizarSucursal(string sucursal)
    {
        if (string.IsNullOrWhiteSpace(sucursal))
            throw new ArgumentException("La sucursal no puede ser nula ni vacía.", nameof(sucursal));

        // R2: ToUpperInvariant para no depender de la cultura (p. ej. la 'i' turca).
        string codigo = sucursal.Trim().ToUpperInvariant();

        if (!PatronSucursal.IsMatch(codigo))
            throw new ArgumentException(
                "La sucursal debe tener entre 2 y 4 letras de la A a la Z.", nameof(sucursal));

        return codigo;
    }
}
