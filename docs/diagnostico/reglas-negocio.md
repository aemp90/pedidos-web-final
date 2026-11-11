# Reglas de negocio de Pedidos Legado

<!--
Etapa 2 · Encontrar (modo por defecto + subagente analista-legado).
Una regla es una decisión del negocio escondida en el código: un monto, un porcentaje, una condición, un tope.
- Archivo:línea: dónde está. Sin esta evidencia, la regla no cuenta.
- Condición: cuándo se aplica. Efecto: qué cambia.
- Verificada: «Sí» solo si TÚ abriste el archivo en esa línea y leíste el código.
  Si la regla aparece en más de un lugar, anota todos.
Punto de control E2: al menos 8 reglas, y al menos 5 verificadas leyendo el código.
-->

> Rutas relativas a `legado/`. Las reglas describen lo que **ejecuta el código**, no lo que dicen los comentarios ni el README (ver RS-15 en `riesgos.md`).
> «Sí» = línea abierta y leída en esta revisión. «Por verificar» = la línea existe, pero falta confirmar una parte del comportamiento o la intención del negocio.

| # | Regla | Archivo:línea | Condición | Efecto | Verificada |
|---|---|---|---|---|---|
| 1 | Descuento por volumen **al grabar** | `src/UFrmPedidos.pas:296-301` | Base (sin OFE) `>= 1.000.000` / `>= 500.000` | 8% / 5% | Sí |
| 2 | Descuento por volumen **en pantalla**, con otro límite (duplicada) | `src/UCalculoTotales.pas:77-80` | Base `> 1.000.000` / `> 500.000` | Con 500.000 o 1.000.000 exactos, la pantalla aplica menos descuento que lo grabado. Evidencia: folio 1011 grabado en 559.181 (5% + 3%), cuando la pantalla habría mostrado 588.931 (`db/datos/PEDIDOS.csv:12`, `db/datos/PEDIDO_LINEAS.csv:41`). El comentario `src/UCalculoTotales.pas:4` («5% sobre 400.000») no coincide con el código | Sí |
| 3 | La categoría OFE no entra en la base del descuento | `src/UCalculoTotales.pas:70`; `src/UFrmPedidos.pas:287` | Línea con categoría `OFE` | Se excluye de la base del descuento, pero suma al subtotal | Sí |
| 4 | Cliente VIP: +3% de descuento | `src/UCalculoTotales.pas:88-89`; `src/UFrmPedidos.pas:302-303`; marca VIP en `src/UFrmPedidos.pas:119` | `CLIENTES.TIPO = 'V'` | Suma 3 puntos al porcentaje | Sí |
| 5 | Descuento de navidad: +2% (solo al grabar) | `src/UFrmPedidos.pas:304-305`; bandera en `src/UGlobales.pas:33` | `gDescNavidad` y mes 12 | +2%. En la práctica no se aplica nunca: la bandera es `False` y el parámetro `DESC_NAVIDAD` (`db/datos/PARAMETROS.csv:6`) no se lee en ningún lado | Sí |
| 6 | Tope de descuento: 10% | `src/UCalculoTotales.pas:90-91`; `src/UFrmPedidos.pas:306-307` | Porcentaje acumulado > 10% | Se limita a 10% (por ejemplo, 8% + 3% → 10%) | Sí |
| 7 | Redondeo «al peso» con `Trunc(x + 0.49)` (duplicado) | `src/UCalculoTotales.pas:50`; `src/UUtilidades.pas:30` | Descuento, flete e IVA | Un valor terminado en ,50 baja en vez de subir: no es redondeo comercial | Sí |
| 8 | Recargo de flete del 15% en las regiones 11 y 12 | `src/UCalculoTotales.pas:97-98`; `src/UFrmPedidos.pas:312-313` | `Region = 11` o `Region = 12` | Flete base × 1,15 | Sí |
| 9 | Sin líneas no se cobra flete (solo en pantalla) | `src/UCalculoTotales.pas:116-117` | 0 líneas | Flete 0. Al grabar no aplica, porque la regla 14 impide grabar pedidos sin líneas | Sí |
| 10 | IVA sobre neto + flete; total = neto + flete + IVA | `src/UCalculoTotales.pas:105`; `src/UFrmPedidos.pas:318-319` | Siempre | El flete también paga IVA | Sí |
| 11 | La tasa de IVA sale de un parámetro (0,19 por defecto) | `src/UFrmPrincipal.pas:62-64`; `db/datos/PARAMETROS.csv:2`; valor por defecto en `src/UGlobales.pas:31` | Al iniciar la aplicación | `TASA_IVA / 100`. El informe la deja fija en 0,19 (ver RS-08) | Sí |
| 12 | Pedido mínimo de $30.000 fuera de la región 13 | `src/UFrmPedidos.pas:231` | Subtotal < 30.000 y región ≠ 13 | Rechaza el pedido. El parámetro `PEDIDO_MINIMO` vale 25.000 (`db/datos/PARAMETROS.csv:3`), pero no se lee: manda el valor hardcodeado | Sí |
| 13 | Bloqueo por deuda vencida | `src/UFrmPedidos.pas:111-112` | `SALDO_VENCIDO > 0` y `DaysBetween(Date, FECHA_VENCIMIENTO) > 60` | No deja vender al cliente. `DaysBetween` (DateUtils) entrega una diferencia absoluta, así que un vencimiento a más de 60 días **en el futuro** también bloquea. Decisión D5b: en PedidosWeb solo bloquea una deuda vencida hace más de 60 días | Sí |
| 14 | Validaciones de las líneas del pedido | `src/UFrmPedidos.pas:209`, `:214`, `:221`, `:143` | Sin líneas; más de 40 líneas; precio ≤ 0; cantidad ≤ 0 | Rechaza | Sí |
| 15 | Observación con tope de 200 caracteres | `src/UFrmPedidos.pas:257-258`; `db/esquema-dbisam.sql:50` | Más de 200 caracteres | La corta sin avisar | Sí |
| 16 | Un pedido desde las 18:00 pasa al siguiente día hábil (lunes a viernes) | `src/UUtilidades.pas:133-137`, `:124`; se usa en `src/UFrmPedidos.pas:240` | Hora ≥ 18 | La fecha avanza al siguiente día hábil. Antes de las 18:00 no se revisa si el día es hábil (un pedido del sábado queda con fecha de sábado) y no se consideran feriados | Sí |
| 17 | Folio = `MAX(FOLIO) + 1`, con estado inicial `'C'` | `src/UFrmPedidos.pas:249-251`, `:262` | Al grabar | Folio correlativo. `'C'` = confirmado: el informe lo filtra con «Solo confirmados» (`src/UFrmInformes.pas:70-71`). El estado `'A'` aparece en los datos (`db/datos/PEDIDOS.csv:19`, folio 1018), pero ningún archivo de `legado/` lo escribe ni lo lee: se asignó fuera de la aplicación (a mano o con otra herramienta). Su significado lo tiene que confirmar el negocio | Por verificar |
| 18 | Stock: un VIP puede dejarlo en −20; un cliente normal, en 0 | `src/UStock.pas:43`, `:48` | Cliente VIP o normal | Si el stock no alcanza, solo se muestra un mensaje (`src/UStock.pas:71`): el pedido ya quedó grabado | Sí |
| 19 | Aviso del primer pedido del cliente | `src/UFrmPedidos.pas:341-342` | El cliente tiene 1 pedido | Mensaje «Avisar a cobranza» | Sí |
| 20 | Validaciones del cliente: TIPO N/V; para marcarlo VIP hace falta nivel ≥ 2; región entre 1 y 16 | `src/UFrmClientes.pas:137`, `:143`, `:148-149` | Al grabar el cliente | Rechaza. La región no se compara contra la tabla REGIONES | Sí |
| 21 | Valores por defecto de un cliente nuevo | `src/UFrmClientes.pas:90-93` | Alta de cliente | TIPO N, región 13, saldo 0, activo | Sí |
| 22 | Un cliente con pedidos no se puede eliminar | `src/UFrmClientes.pas:107-110` | Tiene pedidos | Bloquea el borrado | Sí |
| 23 | Validaciones del producto | `src/UFrmProductos.pas:82`, `:87`, `:93`, `:99-100`, `:101-102` | Al grabar el producto | Código obligatorio, precio > 0, categoría de 3 letras, descripción en mayúsculas, STOCK_MINIMO 10 por defecto | Sí |
| 24 | Permisos por nombre de usuario | `src/UFrmPrincipal.pas:49-50`, `:95`; `src/UFrmProductos.pas:44`; valor por defecto en `src/UGlobales.pas:30` | Usuario ADMIN o JEFEVENTAS → nivel 9; el resto → 1 | Informes requieren nivel ≥ 2 y borrar productos, nivel 9. Ningún código asigna el nivel 2, aunque el comentario `src/UGlobales.pas:13` lo menciona | Sí |
| 25 | RUT validado con módulo 11 (resto 11 → '0', resto 10 → 'K') | `src/UUtilidades.pas:72-79` | Al buscar el cliente en el pedido | Rechaza un RUT inválido. Hay un segundo validador con el resultado invertido (ver RS-04) | Sí |
| 26 | No se valida `CLIENTES.ACTIVO` al tomar un pedido | Ausencia: `ACTIVO` solo aparece en `src/UFrmClientes.pas:93`; dato de ejemplo: el cliente 30 está inactivo (`db/datos/CLIENTES.csv:31`) y tiene el pedido `db/datos/PEDIDOS.csv:21` | Cliente inactivo | Se le puede vender igual. Decisión D5a: en PedidosWeb se bloquea | Sí |

## Decisiones para PedidosWeb (2026-11-10)
| # | Duda | Decisión | Reglas |
|---|---|---|---|
| D1 | ¿Límites del descuento: `>` o `>=`? ¿400.000 o 500.000/1.000.000? | `>= 500.000` → 5% y `>= 1.000.000` → 8%, igual que lo que se graba (coincide con el histórico, folio 1011). La pantalla usa el mismo cálculo; se descarta el 400.000 del comentario | 1, 2 |
| D2 | ¿Pedido mínimo 30.000 (código) o 25.000 (parámetro)? | Configurable, con 30.000 por defecto (el valor que se aplica hoy) | 12 |
| D3 | ¿Sigue vigente el descuento de navidad? | No se migra: hoy no tiene efecto. Queda documentado como regla muerta | 5 |
| D4 | ¿Qué significa el estado `'A'` y quién lo asigna? | Se investigó: ningún código lo escribe. **Sigue abierta para el negocio**; mientras tanto, PedidosWeb solo crea pedidos en estado `'C'` | 17 |
| D5a | ¿Se bloquea a los clientes inactivos? | Sí: se rechaza el pedido de un cliente con `ACTIVO` = falso (cambio respecto del legado) | 26 |
| D5b | ¿Un vencimiento en el futuro bloquea? | No: bloquea solo si `SALDO_VENCIDO > 0` y el vencimiento quedó **más de 60 días en el pasado** (se corrige el bug de `DaysBetween`) | 13 |
| D6 | ¿Stock negativo para VIP? ¿Se graban pedidos sin stock? | Se mantiene el margen VIP de −20 (normal: 0), pero el stock se valida **antes de grabar** y se rechaza el pedido si se pasa del límite | 18 |
| D7 | ¿Qué redondeo exige contabilidad? | **Modificada y confirmada el 2026-11-11:** redondeo comercial al peso (un ,50 sube). Corrige R05; 13 folios del histórico difieren en ±1 o 2 pesos (`docs/migracion/defectos-corregidos.md`, D1). Antes: se replicaba `Trunc(x + 0.49)` | 7 |

## Dudas abiertas para el negocio
1. ¿Qué significa el estado `'A'` (folio 1018) y quién lo asigna? (D4)
