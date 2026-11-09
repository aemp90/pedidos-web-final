namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Una página del listado de pedidos con sus metadatos (RN15). Se serializa en camelCase:
/// pagina, tamanoPagina, totalRegistros, totalPaginas y pedidos.
/// </summary>
/// <param name="Pagina">Número de página pedido (desde 1).</param>
/// <param name="TamanoPagina">Cantidad máxima de pedidos por página.</param>
/// <param name="TotalRegistros">Pedidos que cumplen los filtros, sumando todas las páginas.</param>
/// <param name="TotalPaginas">Cantidad de páginas; 0 si no hay resultados.</param>
/// <param name="Pedidos">Los pedidos de esta página; vacío si la página está después de la última (RN12).</param>
public sealed record PaginaPedidos(
    int Pagina,
    int TamanoPagina,
    int TotalRegistros,
    int TotalPaginas,
    IReadOnlyList<Pedido> Pedidos);
