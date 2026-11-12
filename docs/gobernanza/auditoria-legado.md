# Auditoría del legado · pedidos-web

> Alcance: `legado/src/UFrmPedidos.pas` y `legado/src/UDMPrincipal.pas` (las reglas que siguen a otros archivos se citan donde viven). Los hallazgos los propuso el subagente `analista-legado` y se revisaron uno por uno contra el código el 2026-11-12. Las rutas son relativas a `legado/`. No se modificó ningún archivo del legado.

## Hallazgos

| ID | Hallazgo | Evidencia (archivo:línea) | Tipo | Severidad | Estado | Argumento |
|---|---|---|---|---|---|---|
| AU-01 | La clave de la base de datos está escrita en el código versionado | `src/UDMPrincipal.pas:41` (`CLAVE_BD = 'Distrib#2008'`), se usa en `:59` | Seguridad | Alta | Validado | La línea 41 declara la constante con la clave en texto plano, y la 59 la pasa a `Session.AddPassword`. |
| AU-02 | El SQL se arma concatenando lo que escribe el usuario: permite inyección SQL y una comilla rompe la consulta | `src/UFrmPedidos.pas:102` (RUT), `:152` (código de producto), `:262` (observación); `src/UDMPrincipal.pas:85` (`LeerParametro`), `:92-98` (`EjecutarSQL` ejecuta cualquier texto) | Seguridad | Alta | Validado | Las cuatro líneas concatenan el texto sin escapar. El subagente dejó pendiente si `ValidaRut` (`:91`) protege el RUT, así que lo revisé: no lo protege. `LimpiaRut` solo quita `.`, `-` y espacios (`src/UUtilidades.pas:46`), y `StrToIntDef(..., 0)` cuenta como 0 cualquier carácter que no sea dígito (`src/UUtilidades.pas:67`). Por eso, un texto con comillas pasa el módulo 11 si su último carácter coincide con el DV calculado. Además, a la consulta va `vRut` original, sin limpiar. |
| AU-03 | No hay autenticación: el usuario se escribe en un `InputBox` sin contraseña, y escribir «ADMIN» o «JEFEVENTAS» da el nivel 9 | `src/UFrmPrincipal.pas:46`, `:49-50` | Seguridad | Alta | Validado | Las líneas 46 y 49-50 hacen lo que dice el hallazgo. `UFrmPedidos.pas` no consulta `gNivelUsuario` en ninguna parte: según el grep, solo lo usan `UFrmClientes`, `UFrmPrincipal` y `UFrmProductos`. |
| AU-04 | El `USUARIO` de cada pedido se puede falsificar y además entra al SQL sin escapar | `src/UFrmPedidos.pas:261`; también `src/UStock.pas:87` | Seguridad | Media | Validado | La línea 261 concatena `gUsuario`, que es el texto libre de AU-03. La auditoría del pedido no sirve, porque cualquiera graba a nombre de otro. |
| AU-05 | El bloqueo por deuda vencida solo se revisa al buscar el cliente, no al grabar, y no se revisa si el cliente está `ACTIVO` | `src/UFrmPedidos.pas:111-112`; `btnGrabarClick` en `:204-235` no lo repite; la consulta de `:99-102` no trae `ACTIVO` (`db/esquema-dbisam.sql:23`) | Regla de negocio | Media | Validado | La regla vive solo en `btnBuscarClienteClick`. `btnGrabarClick` valida el cliente, las líneas, el precio y el mínimo, pero no la deuda. `DaysBetween` no tiene signo, así que también bloquea un vencimiento futuro a más de 60 días (decisión D5b). |
| AU-06 | No hay control de límite de crédito | Ninguna; el subagente dice «no encontré evidencia» | Regla de negocio | Media | Descartado | El hallazgo no trae `archivo:línea` y el legado no tiene esa regla. La tabla `CLIENTES` (`db/esquema-dbisam.sql:15-23`) no tiene una columna de límite, solo `SALDO_VENCIDO` y `FECHA_VENCIMIENTO`, que ya cubre AU-05. No es una regla que se pueda saltar, sino una regla que no existe; si el negocio la quiere, va como historia nueva. |
| AU-07 | El stock no se valida antes de grabar: se descuenta después del INSERT, y si no alcanza solo aparece un mensaje | `src/UFrmPedidos.pas:148-168` (al agregar una línea no se mira el stock), `:324`; `src/UStock.pas:43`, `:48`, `:71` | Regla de negocio | Alta | Validado | `DescontarStockPedido` se llama con el pedido ya insertado. Cuando `DescontarStock` devuelve `False` (`UStock.pas:43-49`), solo se hace `ShowMessage` (`:71`) y el pedido queda grabado. |
| AU-08 | El descuento se calcula con reglas distintas en pantalla y al grabar, así que el vendedor ve un total y se graba otro | `src/UFrmPedidos.pas:296`, `:298` (`>=`) vs. `src/UCalculoTotales.pas:77`, `:79` (`>`); `src/UFrmPedidos.pas:304` (navidad solo al grabar); aviso solo en `:330` | Regla de negocio | Alta | Validado | Las líneas confirman `>=` contra `>`. Un matiz: lo de navidad hoy no produce diferencias, porque `gDescNavidad := False` (`src/UGlobales.pas:33`), y eso cuadra con la decisión D3. El comentario de `UCalculoTotales.pas:4` («5% sobre 400.000») contradice el código. |
| AU-09 | El mínimo de pedido, el máximo de 40 líneas y el precio mayor que 0 se pueden saltar por otra vía, porque `tblPedidos` y `tblLineas` quedan abiertas | `src/UFrmPedidos.pas:214`, `:231`; `src/UDMPrincipal.pas:64-65` | Regla de negocio | Media | Descartado | Es cierto que esas reglas viven solo en `btnGrabarClick`, pero la evidencia no muestra ninguna vía para saltarlas. `tblPedidos` y `tblLineas` solo se abren y se cierran (`src/UDMPrincipal.pas:64-65`, `:73-74`), y ningún formulario las edita (grep sin otros usos). El único INSERT en `PEDIDOS` es `src/UFrmPedidos.pas:259`, que pasa antes por las validaciones de `:204-235`. Queda como deuda de diseño para PedidosWeb, con las reglas en Core, pero no es una vía de bypass en el legado. |
| AU-10 | El estado del pedido siempre se graba como `'C'` y no hay transiciones como anular o despachar | `src/UFrmPedidos.pas:262`; el único otro uso es el filtro de `src/UFrmInformes.pas:71` | Regla de negocio | Baja | Validado | La línea 262 deja `'C'` fijo, y el grep de `ESTADO` no encuentra otro valor. Un pedido mal ingresado no se puede anular desde el sistema. |
| AU-11 | No hay transacción entre la cabecera, las líneas, el total y el stock | `src/UFrmPedidos.pas:255` (`// TODO transaccion`), `:262`, `:272`, `:320`, `:324` | Integridad | Alta | Validado | Son cinco escrituras independientes con `EjecutarSQL`. Un fallo a mitad del proceso deja una cabecera con `TOTAL 0` o con las líneas grabadas y el stock sin descontar. |
| AU-12 | El folio se obtiene con `MAX(FOLIO) + 1`, sin bloqueo | `src/UFrmPedidos.pas:249-251`; `db/esquema-dbisam.sql:52` | Integridad | Media | Validado | La línea 249 hace lo que dice el hallazgo, pero la clave primaria de `FOLIO` (esquema, `:52`) **impide** los folios duplicados. Si dos puestos graban a la vez, el segundo INSERT falla con una excepción sin capturar. Como es la primera escritura, no deja datos a medias. Por eso bajé la severidad de Alta a Media. |
| AU-13 | El stock se lee y después se escribe como valor absoluto, sin bloqueo: dos pedidos simultáneos se pisan | `src/UStock.pas:38-39`, `:51` | Integridad | Alta | Validado | `vNuevo := vActual - ACantidad` y después `SET STOCK = <vNuevo>`. El último que escribe borra el descuento del otro. |
| AU-14 | El dinero se maneja en `Double`, con redondeo propio, y entra al SQL con `FloatToStr` | `src/UFrmPedidos.pas:199`, `:271`, `:320`; `src/UUtilidades.pas:30` | Integridad | Media | Validado | Las líneas lo confirman. Falta verificar si `FloatToStr` sale con coma decimal en un PC configurado en español; depende de la máquina. |
| AU-15 | El registro del movimiento de stock traga los errores (`except end;` vacío) | `src/UStock.pas:89-90`; `MOV_STOCK` no está en `db/esquema-dbisam.sql` | Integridad | Media | Validado | El bloque está vacío. Si la tabla no existe, el INSERT falla siempre sin aviso y no queda trazabilidad. |
| AU-16 | Los mensajes de error exponen datos internos | `src/UFrmPedidos.pas:331` | Seguridad | Baja | Descartado | Lo refuta `src/UFrmPedidos.pas:330` (`if gModoPrueba then`): el único mensaje con datos internos sale solo en modo prueba, y `gModoPrueba` es `False` en producción (ver RS-05). El propio subagente lo marcó «no confirmado», y lo de las excepciones sin manejar que mostrarían SQL no tiene ninguna línea que lo respalde. |

**Resultado:** 13 validados y 3 descartados.

## Cruce con el diagnóstico de la U4

- **Coinciden:**
  - AU-01 con RS-02.
  - AU-02 con RS-01.
  - AU-03 con RS-03.
  - AU-05 con las decisiones D5a y D5b.
  - AU-07 con RS-13 y D6.
  - AU-08 con RS-05, D1 y RS-15 (comentario de los 400.000).
  - AU-11 con RS-06.
  - AU-12 y AU-13 con RS-07.
  - AU-14 con RS-09.
  - AU-15 con RS-12.
- **Nuevos en esta auditoría:**
  - **AU-04:** `gUsuario` concatenado al SQL de `UFrmPedidos.pas:261`. No está entre las líneas de RS-01.
  - **AU-10:** el estado fijo en `'C'`, sin transiciones.
  - **Detalle nuevo de AU-02:** `ValidaRut` no impide la inyección por el RUT.
  - **Corrección a RS-07 (AU-12):** la clave primaria de `FOLIO` evita los folios duplicados. El riesgo real es una excepción sin manejar, no datos duplicados.
- **De la U4 que el subagente no vio, aun estando en los archivos auditados:**
  - **RS-11:** la fecha del pedido depende de `Now` (`UFrmPedidos.pas:240`).
  - **RS-14:** el pedido mínimo está escrito en código como 30.000 y no se lee el parámetro `PEDIDO_MINIMO` (`UFrmPedidos.pas:231`).
  - **RS-15:** el comentario «aviso a vendedor» de `UFrmPedidos.pas:326`.
  - **RS-17:** el RUT no se normaliza antes de la consulta (`UFrmPedidos.pas:90`, `:102`).
  - RS-04, RS-08, RS-10 y RS-16 caen fuera del alcance porque están en otros archivos o en el `.dfm`.
