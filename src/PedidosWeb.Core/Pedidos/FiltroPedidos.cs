using System.Globalization;

namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Filtros ya validados del listado de pedidos (HU-01). Se construye solo con <see cref="Crear"/>.
/// </summary>
public sealed record FiltroPedidos
{
    public const int PaginaPorDefecto = 1;
    public const int TamanoPaginaPorDefecto = 20;
    public const int TamanoPaginaMaximo = 100;

    private const string FormatoFecha = "yyyy-MM-dd";

    private FiltroPedidos() { }

    public string? ClienteId { get; private init; }
    public string? Estado { get; private init; }
    public DateOnly? Desde { get; private init; }
    public DateOnly? Hasta { get; private init; }
    public int Pagina { get; private init; } = PaginaPorDefecto;
    public int TamanoPagina { get; private init; } = TamanoPaginaPorDefecto;

    /// <summary>
    /// Valida y convierte los parámetros tal como llegan por HTTP. Junta todos los errores antes de responder.
    /// </summary>
    public static ResultadoFiltro Crear(
        string? clienteId,
        string? estado,
        string? desde,
        string? hasta,
        string? pagina,
        string? tamanoPagina)
    {
        var errores = new Dictionary<string, string[]>();

        // RN9b: nulo, vacío o solo espacios cuenta como no enviado.
        clienteId = Normalizar(clienteId);
        estado = Normalizar(estado);
        desde = Normalizar(desde);
        hasta = Normalizar(hasta);
        pagina = Normalizar(pagina);
        tamanoPagina = Normalizar(tamanoPagina);

        // RN5, RN6
        if (estado is not null && !EstadosPedido.EsValido(estado))
        {
            errores["estado"] =
                ["El estado debe ser Pendiente, Confirmado, Despachado, Entregado o Anulado."];
        }

        // RN7
        var fechaDesde = LeerFecha(desde, "desde", errores);
        var fechaHasta = LeerFecha(hasta, "hasta", errores);

        // RN9: solo se compara si ambas fechas son válidas.
        if (fechaDesde is not null && fechaHasta is not null && fechaDesde > fechaHasta)
        {
            errores["desde"] = ["La fecha desde no puede ser posterior a la fecha hasta."];
        }

        // RN10, RN11
        var numeroPagina = LeerEntero(pagina, PaginaPorDefecto);
        if (numeroPagina is null or < 1)
        {
            errores["pagina"] = ["La página debe ser un número entero mayor o igual a 1."];
        }

        var numeroTamano = LeerEntero(tamanoPagina, TamanoPaginaPorDefecto);
        if (numeroTamano is null or < 1 or > TamanoPaginaMaximo)
        {
            errores["tamanoPagina"] =
                [$"El tamaño de página debe ser un número entero entre 1 y {TamanoPaginaMaximo}."];
        }

        if (errores.Count > 0)
        {
            return ResultadoFiltro.Fallo(errores);
        }

        return ResultadoFiltro.Exito(new FiltroPedidos
        {
            ClienteId = clienteId,
            Estado = estado,
            Desde = fechaDesde,
            Hasta = fechaHasta,
            Pagina = numeroPagina!.Value,
            TamanoPagina = numeroTamano!.Value
        });
    }

    /// <summary>
    /// Devuelve los pedidos cuya <see cref="Pedido.Fecha"/> está entre <paramref name="desde"/> y
    /// <paramref name="hasta"/>, ambos inclusive (HU-03). Conserva el orden de entrada.
    /// </summary>
    /// <exception cref="ArgumentNullException">Si <paramref name="pedidos"/> es null.</exception>
    /// <exception cref="ArgumentException">Si <paramref name="desde"/> es posterior a <paramref name="hasta"/> (CA-04).</exception>
    public static IReadOnlyList<Pedido> EntreFechas(IEnumerable<Pedido> pedidos, DateOnly desde, DateOnly hasta)
    {
        ArgumentNullException.ThrowIfNull(pedidos);

        if (desde > hasta)
        {
            throw new ArgumentException(
                $"La fecha desde ({desde:yyyy-MM-dd}) no puede ser posterior a la fecha hasta ({hasta:yyyy-MM-dd}).",
                nameof(desde));
        }

        // CA-01, CA-02: ambos límites son inclusive.
        return pedidos.Where(p => p.Fecha >= desde && p.Fecha <= hasta).ToList();
    }

    private static string? Normalizar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private static DateOnly? LeerFecha(string? valor, string parametro, Dictionary<string, string[]> errores)
    {
        if (valor is null)
        {
            return null;
        }

        // TryParseExact también rechaza fechas inexistentes como 2026-02-30.
        if (DateOnly.TryParseExact(valor, FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
        {
            return fecha;
        }

        errores[parametro] = [$"La fecha {parametro} debe tener el formato AAAA-MM-DD y ser una fecha válida."];
        return null;
    }

    /// <summary>Entero sin signo ni separadores; el valor por defecto si no se envió; null si no es entero.</summary>
    private static int? LeerEntero(string? valor, int porDefecto)
    {
        if (valor is null)
        {
            return porDefecto;
        }

        return int.TryParse(valor, NumberStyles.None, CultureInfo.InvariantCulture, out var numero)
            ? numero
            : null;
    }
}
