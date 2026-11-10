# HU-02 · Detalle de un pedido

Estado: aprobada por el PO (2026-11-10)

**Historia:** Como vendedor, quiero ver el detalle de un pedido con sus líneas y su total, para revisar
qué se pidió sin abrir Pedidos Legado.

## Qué devuelve

La ruta `GET /api/pedidos/{id}/detalle` devuelve:

- Del pedido: id, folio, fecha, estado y sucursal.
- Del cliente: id y nombre (los que ya trae `Pedido`).
- De cada línea, ordenadas por número: número, producto, cantidad, precio unitario y subtotal
  (cantidad × precio unitario).
- Total: la suma de los subtotales de las líneas, en pesos chilenos y sin IVA. Coincide con `Pedido.Total`.

Todos los montos son `decimal` sin decimales y sin IVA.

## Criterios de aceptación

Cada criterio se puede comprobar con un sí o un no. Se usan los datos de ejemplo en memoria.

- **CA-01** · Dado el pedido 8, cuando pido su detalle, entonces responde 200 con folio `PED-STGO-000008`,
  fecha `2026-10-20`, estado `Entregado`, sucursal `STGO` y cliente `C-001` · `Ferretería El Clavo`.
- **CA-02** · Dado el pedido 8, cuando pido su detalle, entonces trae 3 líneas en orden 1, 2, 3 con subtotales
  64500 (3 × 21500), 35400 (12 × 2950) y 20200 (10 × 2020).
- **CA-03** · Dado el pedido 8, cuando pido su detalle, entonces el total es 120100.
- **CA-04** · Dado cualquiera de los 45 pedidos de ejemplo, cuando pido su detalle, entonces el total es igual
  a la suma de los subtotales y a su `Pedido.Total`.
- **CA-05** · Dado el id 999, que no existe, cuando pido su detalle, entonces responde 404 con ProblemDetails,
  título `"El pedido no existe."` y detalle `"No existe un pedido con id 999."`.
- **CA-06** · Dado un pedido que existe pero no tiene líneas (con un repositorio de prueba; los datos de ejemplo
  no tienen este caso), cuando pido su detalle, entonces responde 200 con la lista de líneas vacía y total 0.

## Si el pedido no tiene líneas

Responde **HTTP 200** con los datos del pedido y del cliente, la lista de líneas vacía y total 0. El total
siempre se calcula desde las líneas, no se copia de `Pedido.Total`.

## Si el pedido no existe

Responde **HTTP 404** con ProblemDetails (RFC 9457), con el título en español
`"El pedido no existe."` y en el detalle el id que se buscó (ej. `"No existe un pedido con id 999."`).
Un id que no es número entero (ej. `/api/pedidos/abc/detalle`) no calza con la ruta y también responde 404.

## Fuera de alcance

- Cálculo de IVA, descuentos o recargos.
- Otros datos del cliente (RUT, región, si está activo).
- Editar el pedido o sus líneas.
- Cambiar la respuesta actual de `GET /api/pedidos/{id}`.
- Pantalla web (frontend) y autenticación del vendedor.
- Base de datos (sigue en memoria hasta la Unidad 5).

## Preguntas abiertas

Ninguna por ahora.
