namespace PedidosWeb.Core.Pedidos;

/// <summary>Estados de pedido válidos para filtrar (RN5).</summary>
public static class EstadosPedido
{
    public static IReadOnlyList<string> Validos { get; } =
        ["Pendiente", "Confirmado", "Despachado", "Entregado", "Anulado"];

    /// <summary>True si el texto es un estado válido, sin distinguir mayúsculas de minúsculas (RN6).</summary>
    public static bool EsValido(string estado) =>
        Validos.Any(e => string.Equals(e, estado, StringComparison.OrdinalIgnoreCase));
}
