namespace PedidosWeb.Core.Pedidos;

/// <summary>Listado de pedidos con filtros, orden y paginación (HU-01).</summary>
public sealed class ListadoPedidos
{
    private readonly IPedidoRepositorio _repositorio;

    public ListadoPedidos(IPedidoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    /// <summary>Filtra, ordena, cuenta y pagina los pedidos según un filtro ya validado.</summary>
    public PaginaPedidos Listar(FiltroPedidos filtro)
    {
        IEnumerable<Pedido> consulta = _repositorio.ObtenerTodos();

        // RN3: todos los filtros se aplican a la vez.
        if (filtro.ClienteId is not null)
        {
            // RN4: ClienteId completo, sin distinguir mayúsculas.
            consulta = consulta.Where(p => string.Equals(p.ClienteId, filtro.ClienteId, StringComparison.OrdinalIgnoreCase));
        }

        if (filtro.Estado is not null)
        {
            // RN6
            consulta = consulta.Where(p => string.Equals(p.Estado, filtro.Estado, StringComparison.OrdinalIgnoreCase));
        }

        // RN8: rango inclusivo; cada extremo es opcional.
        if (filtro.Desde is { } desde)
        {
            consulta = consulta.Where(p => p.Fecha >= desde);
        }

        if (filtro.Hasta is { } hasta)
        {
            consulta = consulta.Where(p => p.Fecha <= hasta);
        }

        // RN13: el orden se aplica antes de paginar.
        var ordenados = consulta
            .OrderByDescending(p => p.Fecha)
            .ThenBy(p => p.Id)
            .ToList();

        var totalRegistros = ordenados.Count;

        // RN15: división entera redondeada hacia arriba; 0 si no hay resultados.
        var totalPaginas = (totalRegistros + filtro.TamanoPagina - 1) / filtro.TamanoPagina;

        // RN12: una página después de la última queda vacía. Se usa long para no desbordar con páginas enormes.
        var saltar = (long)(filtro.Pagina - 1) * filtro.TamanoPagina;
        List<Pedido> pedidos = saltar >= totalRegistros
            ? []
            : ordenados.Skip((int)saltar).Take(filtro.TamanoPagina).ToList();

        return new PaginaPedidos(filtro.Pagina, filtro.TamanoPagina, totalRegistros, totalPaginas, pedidos);
    }
}
