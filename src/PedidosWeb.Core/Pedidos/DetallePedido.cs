namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Detalle de un pedido con su cliente, sus líneas y su total (HU-02).
/// Montos en pesos chilenos (CLP), <c>decimal</c> sin decimales y sin IVA.
/// </summary>
/// <param name="Id">Número correlativo del pedido.</param>
/// <param name="Folio">Folio del pedido, por ejemplo "PED-STGO-000008".</param>
/// <param name="Fecha">Fecha de emisión (sin hora).</param>
/// <param name="Estado">Estado del pedido tal como lo guarda el legado.</param>
/// <param name="Sucursal">Código de la sucursal que emitió el pedido.</param>
/// <param name="Cliente">Id y nombre del cliente.</param>
/// <param name="Lineas">Líneas ordenadas por número, cada una con su subtotal.</param>
/// <param name="Total">Suma de los subtotales; 0 si el pedido no tiene líneas.</param>
public sealed record DetallePedido(
    int Id,
    string Folio,
    DateOnly Fecha,
    string Estado,
    string Sucursal,
    ClienteDetalle Cliente,
    IReadOnlyList<LineaDetalle> Lineas,
    decimal Total);

/// <summary>Datos del cliente que muestra el detalle de un pedido.</summary>
public sealed record ClienteDetalle(string Id, string Nombre);

/// <summary>Una línea del detalle: la línea del pedido más su subtotal (cantidad × precio unitario).</summary>
public sealed record LineaDetalle(
    int Numero,
    string Producto,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);
