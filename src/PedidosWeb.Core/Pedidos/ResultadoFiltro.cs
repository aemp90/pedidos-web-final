namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Resultado de validar los parámetros del listado: un <see cref="FiltroPedidos"/> válido,
/// o los errores indexados por nombre de parámetro, con mensajes en español.
/// </summary>
public sealed class ResultadoFiltro
{
    private static readonly IReadOnlyDictionary<string, string[]> SinErrores =
        new Dictionary<string, string[]>();

    private ResultadoFiltro(FiltroPedidos? filtro, IReadOnlyDictionary<string, string[]> errores)
    {
        Filtro = filtro;
        Errores = errores;
    }

    /// <summary>El filtro validado; null si hubo errores.</summary>
    public FiltroPedidos? Filtro { get; }

    /// <summary>Errores por parámetro (ej. "estado"); vacío si el filtro es válido.</summary>
    public IReadOnlyDictionary<string, string[]> Errores { get; }

    public bool EsValido => Filtro is not null;

    internal static ResultadoFiltro Exito(FiltroPedidos filtro) => new(filtro, SinErrores);

    internal static ResultadoFiltro Fallo(IReadOnlyDictionary<string, string[]> errores) => new(null, errores);
}
