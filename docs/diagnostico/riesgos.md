# Riesgos de Pedidos Legado

<!--
Etapa 2 · Encontrar.
Un riesgo es algo que puede salir mal en producción o al migrar.
- Evidencia: archivo:línea. Sin evidencia, es una sospecha, no un riesgo.
- Categoría: Seguridad · Datos e integridad · Fechas · Mantenibilidad.
- Prioridad: Alta · Media · Baja (¿qué tan grave es y qué tan probable?).
Punto de control E2: al menos 5 riesgos con evidencia y prioridad.
-->

> Rutas relativas a `legado/`. Lo marcado «por verificar» todavía no está confirmado por ejecución ni por el negocio.

| ID | Riesgo | Evidencia (archivo:línea) | Categoría | Prioridad |
|---|---|---|---|---|
| RS-01 | SQL armado concatenando texto que escribe el usuario: permite inyección SQL y cualquier comilla rompe la consulta. `EscapaComillas` existe pero no se llama en ningún lado | `src/UFrmPedidos.pas:102`, `:152`, `:262` (observación); `src/UFrmClientes.pas:76`; `src/UFrmInformes.pas:69`; `src/UFrmProductos.pas:61`; `src/UDMPrincipal.pas:85`; `src/UStock.pas:87`; función sin uso en `src/UUtilidades.pas:164-166` | Seguridad | Alta |
| RS-02 | Clave de la base de datos escrita en el código versionado | `src/UDMPrincipal.pas:41` | Seguridad | Alta |
| RS-03 | No hay autenticación: el nivel de permisos depende del nombre que se escribe en un `InputBox`, y quien escriba «ADMIN» obtiene el nivel 9 | `src/UFrmPrincipal.pas:46-50` | Seguridad | Alta |
| RS-04 | Hay dos validadores de RUT incompatibles. El del maestro de clientes asigna el dígito verificador al revés (resto 11 → 'K', resto 10 → '0'), así que los RUT con DV 0 o K no pasarían al grabar. Además hay datos con 'k' minúscula. Por verificar: si `ValidaRut` distingue mayúsculas de minúsculas | `src/UFrmClientes.pas:193-196` vs. `src/UUtilidades.pas:73-76`; dato `db/datos/CLIENTES.csv:19` | Datos e integridad | Alta |
| RS-05 | Los totales se calculan dos veces con reglas distintas (límite `>` vs. `>=`, navidad, flete sin líneas): el cliente ve un total y se graba otro. La alerta de diferencia solo aparece con `gModoPrueba`, que siempre es `False` | `src/UFrmPedidos.pas:327-331`; `src/UGlobales.pas:34`; ejemplo en el folio 1011, `db/datos/PEDIDOS.csv:12` | Datos e integridad | Alta |
| RS-06 | No hay transacción: la cabecera, las líneas, el UPDATE del total y el descuento de stock son sentencias separadas. Un corte a mitad del proceso deja pedidos con TOTAL 0 o con el stock sin descontar | `src/UFrmPedidos.pas:255` (`// TODO transaccion`), `:262`, `:320`, `:324` | Datos e integridad | Alta |
| RS-07 | Condiciones de carrera: el folio y el ID de cliente se obtienen con `MAX + 1`, y el stock se lee y luego se escribe como valor absoluto. Con varios puestos de trabajo puede haber folios duplicados y stock pisado | `src/UFrmPedidos.pas:249-251`; `src/UFrmClientes.pas:160`; `src/UStock.pas:38-52` | Datos e integridad | Alta |
| RS-08 | El informe deja el IVA fijo en 0,19 y saca el neto del TOTAL, que incluye el flete: el neto que ve contabilidad es incorrecto | `src/UFrmInformes.pas:92` | Datos e integridad | Media |
| RS-09 | El dinero se maneja en `Double` y se mete al SQL con `FloatToStr`, que depende de la configuración regional. Por verificar: si con coma decimal se rompe el SQL de un monto con decimales | `src/UCalculoTotales.pas:12`; `src/UFrmPedidos.pas:271`, `:320` | Datos e integridad | Media |
| RS-10 | Años de 2 dígitos con pivote, y `StrToInt` sin control de errores. El archivo que se exporta a contabilidad usa `dd/mm/yy` | `src/UUtilidades.pas:102-106`, `:112`; `src/UFrmInformes.pas:119` | Fechas | Media |
| RS-11 | La fecha del pedido depende del reloj del PC (`Now`), no hay calendario de feriados y antes de las 18:00 no se revisa si el día es hábil | `src/UFrmPedidos.pas:240`; `src/UUtilidades.pas:124`, `:133-137` | Fechas | Media |
| RS-12 | El registro de movimientos de stock falla sin aviso: `except end;` vacío, y la tabla `MOV_STOCK` no existe en el esquema | `src/UStock.pas:84-90`; ausente en `db/esquema-dbisam.sql` | Datos e integridad | Media |
| RS-13 | Se graban pedidos sin stock, solo con un mensaje, y ya hay stock negativo en los datos | `src/UStock.pas:43`, `:71`; `db/datos/PRODUCTOS.csv:10` (−12) | Datos e integridad | Media |
| RS-14 | Variables globales mutables y parámetros o variables muertos: `gFleteBase` y `gFechaSistema` no se usan, y nadie lee los parámetros `PEDIDO_MINIMO`, `DESC_NAVIDAD` ni `EMPRESA`. Cambiar un parámetro no cambia nada | `src/UGlobales.pas:12-21`, `:32-33`, `:38`; `db/datos/PARAMETROS.csv:3`, `:6`, `:7` | Mantenibilidad | Media |
| RS-15 | Comentarios y documentación que contradicen el código e inducen a migrar reglas equivocadas: «5% sobre 400.000» (el código usa 500.000/1.000.000 y 8%), «2 = supervisor» (nadie asigna ese nivel), «aviso a vendedor» (solo en modo prueba), y el README dice Paradox/BDE cuando el código usa DBISAM 4.24 | `src/UCalculoTotales.pas:4`; `src/UGlobales.pas:13`; `src/UFrmPedidos.pas:326`; `README.md:5-7` vs. `src/UDMPrincipal.dfm:10` | Mantenibilidad | Media |
| RS-16 | Rutas e impresora hardcodeadas | `src/UGlobales.pas:36-37`; `src/UDMPrincipal.dfm:12` | Mantenibilidad | Baja |
| RS-17 | La búsqueda de cliente usa el RUT tal como se escribe, sin normalizar puntos ni guion. Por verificar: si «76.600.853-4» deja de encontrar al cliente | `src/UFrmPedidos.pas:90`, `:102` | Datos e integridad | Baja |
| RS-18 | Al comparar PedidosWeb con el legado (marcha en paralelo, cuadratura de totales o rechazos) van a aparecer diferencias **intencionales**, por las decisiones D1, D5a, D5b y D6. Si no están documentadas, se van a reportar como errores o, peor, alguien las va a «corregir» para que vuelvan a coincidir con el legado | Ver la tabla «Diferencias intencionales con el legado» más abajo; decisiones en `docs/diagnostico/reglas-negocio.md` | Datos e integridad | Media |

## Diferencias intencionales con el legado

> PedidosWeb se aparta del legado **a propósito** en estos casos (decisiones del 2026-11-10 en `reglas-negocio.md`). Si hay una diferencia de este tipo al comparar los dos sistemas, es lo esperado y no un error.

| Decisión | Legado | PedidosWeb | Qué se va a ver al comparar | Riesgo relacionado |
|---|---|---|---|---|
| D1 · Límite del descuento | La pantalla usa `>`; al grabar se usa `>=` (`src/UCalculoTotales.pas:77-80` vs. `src/UFrmPedidos.pas:296-301`) | Pantalla y grabación usan `>= 500.000` / `>= 1.000.000` | Los totales **grabados** coinciden. Con una base de 500.000 o 1.000.000 exactos, el total que se **muestra** en PedidosWeb es menor que el que mostraba la pantalla del legado (ej. folio 1011) | Cierra RS-05 en cuanto al límite |
| D5a · Cliente inactivo | Se le vende igual (no se lee `ACTIVO`) | Se rechaza el pedido | Pedidos que el legado aceptaba y PedidosWeb rechaza (ej. cliente 30, `db/datos/CLIENTES.csv:31`) | — |
| D5b · Bloqueo por deuda | `DaysBetween` sin signo: también bloquea si el vencimiento está a más de 60 días en el futuro (`src/UFrmPedidos.pas:110-111`) | Solo bloquea si `SALDO_VENCIDO > 0` y el vencimiento quedó más de 60 días en el pasado | Clientes con vencimiento lejano en el futuro: el legado los bloqueaba y PedidosWeb les vende | — |
| D6 · Stock | Graba el pedido y después solo avisa si falta stock (`src/UStock.pas:43`, `:48`, `:71`) | Revisa el stock **antes de grabar** (normal ≥ 0; VIP ≥ −20) y rechaza si se pasa del límite | Pedidos que el legado grababa sin stock y PedidosWeb rechaza; no se producen nuevos stocks bajo el límite | Cierra RS-13 para los pedidos nuevos (el stock negativo que ya está en los datos se mantiene) |

> Sin diferencias de comportamiento: D2 (pedido mínimo configurable, pero 30.000 por defecto, igual que hoy), D3 (navidad: hoy tampoco se aplica) y D7 (se replica `Trunc(x + 0.49)`). Ojo con D2: si alguien cambia el valor configurado, empiezan a aparecer diferencias.
