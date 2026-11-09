# HU-01 · Listar pedidos con filtros

Estado: aprobada por el PO (2026-11-09)

## Historia de usuario
Como vendedor, quiero listar pedidos filtrando por cliente, estado y rango de fechas.

## Objetivo
El vendedor necesita encontrar rápido los pedidos de un cliente, de un estado o de un período sin revisar
pedido por pedido. El portal ofrece un listado paginado que acepta esos filtros, opcionales y combinables, y
devuelve los pedidos que cumplen todos, del más reciente al más antiguo. Así se reemplaza la búsqueda
equivalente de Pedidos Legado.

## Reglas de negocio
### Filtros
- RN1. Todos los filtros son opcionales: cliente, estado, fecha desde y fecha hasta.
- RN2. Sin filtros, se listan todos los pedidos (paginados según RN10).
- RN3. Si vienen varios filtros, se aplican todos a la vez (Y lógico).
- RN4. El cliente se filtra por `ClienteId` completo (ej. `C-001`), sin distinguir mayúsculas de minúsculas
  (`c-001` equivale a `C-001`). No se busca por nombre ni por texto parcial.
- RN5. Estados válidos para filtrar: `Pendiente`, `Confirmado`, `Despachado`, `Entregado` y `Anulado`.
  Cualquier otro texto es un error de validación.
- RN6. El estado no distingue mayúsculas de minúsculas: `entregado`, `ENTREGADO` y `Entregado` son equivalentes.
- RN7. Las fechas se reciben solo en formato `AAAA-MM-DD`. Otro formato, o una fecha inexistente
  (ej. `2026-02-30`), es un error de validación.
- RN8. El rango de fechas es inclusivo. "desde" y "hasta" son opcionales por separado: solo "desde" significa
  desde ese día en adelante; solo "hasta", hasta ese día inclusive.
- RN9. Si vienen las dos fechas y "desde" es posterior a "hasta", es un error de validación.
- RN9b. Un parámetro enviado vacío (ej. `estado=`) cuenta como no enviado: no filtra y no es error.

### Orden y paginación
- RN10. El listado se pagina. Por defecto se devuelve la página 1 con 20 pedidos. El tamaño de página va de
  1 a 100.
- RN11. Una página menor que 1, un tamaño de página fuera de 1..100, o un valor que no es número entero
  (ej. `pagina=abc`), es un error de validación.
- RN12. Una página mayor que la última no es un error: se responde 200 con la lista vacía y los metadatos.
- RN13. Orden: `Fecha` descendente; si dos pedidos tienen la misma fecha, `Id` ascendente. El orden se aplica
  antes de paginar.

### Respuesta
- RN14. Si los filtros son válidos pero ningún pedido coincide, la respuesta es 200 con la lista vacía.
  Esto incluye un `ClienteId` que no existe.
- RN15. La respuesta es un objeto con metadatos de paginación (`pagina`, `tamanoPagina`, `totalRegistros`,
  `totalPaginas`) y la lista `pedidos`. Si no hay resultados, `totalRegistros` y `totalPaginas` valen 0.
- RN16. Cada pedido trae los mismos 8 campos que `GET /api/pedidos/{id}`: id, folio, clienteId, clienteNombre,
  fecha, estado, total y sucursal.

## Criterios de aceptación
Con los 45 pedidos de ejemplo de `PedidoRepositorioEnMemoria`:

### Filtros y orden
- [ ] CA1. Sin filtros devuelve la página 1 con 20 pedidos (primero el Id 21, del 2026-10-31; último el Id 18,
      del 2026-10-05), con `totalRegistros` 45, `totalPaginas` 3, `pagina` 1 y `tamanoPagina` 20.
- [ ] CA2. Cliente `C-001` devuelve 5 pedidos en este orden: 8, 16, 24, 32, 40.
- [ ] CA3. Estado `Pendiente` devuelve 5 pedidos en este orden: 27, 18, 45, 9, 36.
- [ ] CA4. Estado `entregado` (en minúsculas) devuelve los mismos 15 pedidos que `Entregado`.
- [ ] CA5. Rango 2026-10-01 a 2026-10-31 da `totalRegistros` 23; rango 2026-09-01 a 2026-09-30 da 22.
- [ ] CA6. Rango inclusivo: desde = hasta = 2026-09-02 devuelve solo el Id 40; desde = hasta = 2026-10-31
      devuelve solo el Id 21.
- [ ] CA7. Rango abierto: solo desde = 2026-10-25 devuelve 7 pedidos (21, 42, 2, 23, 44, 4, 25); solo
      hasta = 2026-09-05 devuelve 3 pedidos (38, 19, 40).
- [ ] CA8. Combinación: cliente `C-001` + estado `Entregado` devuelve 8, 24, 40; si además el rango es
      2026-10-01 a 2026-10-31, devuelve solo el 8.
- [ ] CA9. Cliente `C-999` devuelve 200 con `pedidos` vacío, `totalRegistros` 0 y `totalPaginas` 0.
- [ ] CA10. Cliente `C-002` + estado `Despachado` + rango 2026-09-01 a 2026-09-30 devuelve 200 con `pedidos` vacío
      y `totalPaginas` 0.
- [ ] CA10b. Cliente `c-001` (en minúsculas) devuelve los mismos 5 pedidos que `C-001`.
- [ ] CA10c. `estado=&clienteId=` (parámetros vacíos) devuelve lo mismo que sin filtros (`totalRegistros` 45).
- [ ] CA11. Desempate: con un repositorio de prueba que tenga dos pedidos de la misma fecha, sale primero el de
      menor `Id`. Los datos de ejemplo no tienen fechas repetidas, así que no sirven para este criterio.
- [ ] CA12. Cada pedido del listado trae los 8 campos de RN16, con los mismos valores que su detalle
      (ej. pedido 8, ver "Ejemplos").

### Paginación
- [ ] CA13. `pagina=3` (tamaño por defecto) devuelve 5 pedidos: 36, 17, 38, 19, 40.
- [ ] CA14. `pagina=2` devuelve 20 pedidos: el primero es el Id 39 y el último el Id 15.
- [ ] CA15. `pagina=4` devuelve 200 con `pedidos` vacío, `totalRegistros` 45 y `totalPaginas` 3.
- [ ] CA16. `tamanoPagina=100` devuelve los 45 pedidos en una sola página (`totalPaginas` 1).

### Errores (400 + ProblemDetails)
- [ ] CA17. `estado=Enviado` devuelve 400 con un error asociado a `estado`.
- [ ] CA18. `desde=31-10-2026` devuelve 400 con un error asociado a `desde`; `hasta=2026-02-30` devuelve 400
      con un error asociado a `hasta`.
- [ ] CA19. `desde=2026-10-31&hasta=2026-10-01` devuelve 400 (fechas invertidas).
- [ ] CA20. `pagina=0` devuelve 400; `tamanoPagina=0` y `tamanoPagina=101` devuelven 400.
- [ ] CA21. `pagina=abc` y `tamanoPagina=abc` devuelven 400 con ProblemDetails y el error asociado al parámetro.

## Ejemplos de entrada y salida
Ruta: `GET /api/pedidos?clienteId=&estado=&desde=&hasta=&pagina=&tamanoPagina=`

| Entrada | Salida |
|---|---|
| `GET /api/pedidos?clienteId=C-001&estado=Entregado` | 200 · Ids 8, 24, 40 · `totalRegistros` 3 |
| `GET /api/pedidos?desde=2026-10-31&hasta=2026-10-31` | 200 · Id 21 |
| `GET /api/pedidos?pagina=4` | 200 · `pedidos` vacío · `totalPaginas` 3 |
| `GET /api/pedidos?clienteId=C-999` | 200 · `pedidos` vacío · `totalRegistros` 0 |
| `GET /api/pedidos?estado=Enviado` | 400 · ProblemDetails |

Respuesta exitosa (filtro `clienteId=C-001&estado=Entregado&desde=2026-10-01&hasta=2026-10-31`):

    {
      "pagina": 1,
      "tamanoPagina": 20,
      "totalRegistros": 1,
      "totalPaginas": 1,
      "pedidos": [
        { "id": 8, "folio": "PED-STGO-000008", "clienteId": "C-001", "clienteNombre": "Ferretería El Clavo",
          "fecha": "2026-10-20", "estado": "Entregado", "total": 120100, "sucursal": "STGO" }
      ]
    }

Respuesta de error (`estado=Enviado`), forma de referencia:

    {
      "title": "Uno o más parámetros no son válidos.",
      "status": 400,
      "errors": { "estado": [ "El estado debe ser Pendiente, Confirmado, Despachado, Entregado o Anulado." ] }
    }

## Errores esperados
Todo error de validación responde **HTTP 400** con **ProblemDetails** (RFC 9457, variante ValidationProblem).
El diccionario `errors` va indexado por nombre de parámetro y los mensajes están en español.
- Estado fuera de la lista de RN5.
- `desde` o `hasta` con formato distinto de `AAAA-MM-DD`, o con una fecha inexistente.
- `desde` posterior a `hasta`.
- `pagina` menor que 1; `tamanoPagina` menor que 1 o mayor que 100; `pagina` o `tamanoPagina` que no son enteros.

No son error: no enviar filtros (RN2), parámetros vacíos (RN9b), un cliente sin pedidos (RN14) ni una página
después de la última (RN12).

## Fuera de alcance
- Buscar por nombre de cliente o por texto parcial.
- Filtrar por sucursal, folio o monto.
- Otro orden que no sea el de RN13.
- Crear, editar o anular pedidos.
- Pantalla web (frontend) y autenticación del vendedor.
- Base de datos (sigue en memoria hasta la Unidad 5).

## Decisiones técnicas aprobadas por el PO
- Los tests del endpoint van en `tests/PedidosWeb.Core.Tests`. Se aprueba agregarle la referencia a
  `PedidosWeb.Api` y el paquete NuGet `Microsoft.AspNetCore.Mvc.Testing` para los tests de integración.

## Preguntas abiertas
Ninguna por ahora.
