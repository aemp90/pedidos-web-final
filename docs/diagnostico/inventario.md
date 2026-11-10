# Inventario de Pedidos Legado

<!--
Etapa 1 · Entender (plan mode).
Una fila por cada archivo de legado/src/ (.pas y .dfm). Puedes agregar legado/PedidosLegado.dpr al final.
- Tipo: Formulario, Data module, Unidad de lógica, Utilidades, Globales…
- Responsabilidad: qué hace, en una frase y con tus palabras.
- Líneas: pídele a Claude que las cuente con una herramienta; no las estimes a ojo.
Punto de control E1: están todos los archivos de legado/src/.
-->

Líneas contadas con `wc -l` el 2026-11-10.

| Archivo | Tipo | Responsabilidad | Líneas |
|---|---|---|---|
| `src/UCalculoTotales.pas` | Unidad de lógica | Calcula subtotal, descuento por volumen y VIP (con tope de 10 %), flete por región, IVA y total de un pedido en memoria, con redondeo al peso. No depende de la base ni de la UI. | 147 |
| `src/UDMPrincipal.pas` | Data module | Abre y cierra la conexión DBISAM y sus tablas, lee claves de `PARAMETROS` y ejecuta SQL de escritura genérico (`EjecutarSQL`) para el resto del sistema. | 100 |
| `src/UDMPrincipal.dfm` | Diseño de data module | Declara la base `PEDIDOS` (`C:\PEDIDOS\DATOS`), una `TDBISAMTable` por cada tabla y dos queries compartidas (`qryGeneral`, `qryAux`). | 65 |
| `src/UFrmClientes.pas` | Formulario | Mantenedor de clientes: busca por razón social, crea, graba y elimina. Valida el RUT (con su propio algoritmo), el tipo N/V, la región y que solo jefatura cree clientes VIP. Asigna el ID con `MAX+1`. | 215 |
| `src/UFrmClientes.dfm` | Diseño de formulario | Pantalla de clientes con buscador, grilla, campos enlazados a datos y una tabla propia `tblCli` sobre `CLIENTES`. | 213 |
| `src/UFrmInformes.pas` | Formulario | Informe de ventas por rango de fechas, cliente y estado. Totaliza neto e IVA con una tasa fija de 19 % y exporta a CSV para contabilidad. | 137 |
| `src/UFrmInformes.dfm` | Diseño de formulario | Pantalla del informe con filtros, grilla, panel de resumen y una query propia `qryInforme`. | 160 |
| `src/UFrmPedidos.pas` | Formulario | Toma de pedidos: busca el cliente (con bloqueo por deuda), agrega líneas, valida el mínimo y el máximo de líneas, asigna el folio, graba el pedido y sus líneas, recalcula el total por su cuenta y descuenta stock. | 420 |
| `src/UFrmPedidos.dfm` | Diseño de formulario | Pantalla de pedido con panel de cliente, ingreso de líneas, grilla de texto (`TStringGrid`), totales y observación. | 226 |
| `src/UFrmPrincipal.pas` | Formulario | Ventana principal: pide el usuario, define el nivel de acceso, conecta la base, carga parámetros (IVA, impresora) y abre los demás formularios desde el menú. | 104 |
| `src/UFrmPrincipal.dfm` | Diseño de formulario | Menú principal (Archivo, Maestros, Ventas) y barra de estado. | 62 |
| `src/UFrmProductos.pas` | Formulario | Mantenedor de productos: filtra por descripción o bajo stock mínimo, valida código, precio y categoría, y solo el admin puede borrar. | 105 |
| `src/UFrmProductos.dfm` | Diseño de formulario | Pantalla de productos con filtro, grilla, navegador y una tabla propia `tblProd` sobre `PRODUCTOS`. | 98 |
| `src/UGlobales.pas` | Globales | Variables globales mutables (usuario, nivel, tasa de IVA, flete base, ruta de datos, último folio…) con sus valores iniciales. | 41 |
| `src/UStock.pas` | Unidad de lógica | Consulta y descuenta stock por producto o por pedido. Permite que los VIP queden hasta 20 unidades en negativo y registra el movimiento en `MOV_STOCK`. | 93 |
| `src/UUtilidades.pas` | Utilidades | Funciones puras de apoyo: redondeo y formato de pesos, RUT (limpiar, validar módulo 11, formatear), fechas (dd/mm/yy, SQL, día hábil, corte de las 18:00) y textos. | 189 |
| `PedidosLegado.dpr` | Programa | Punto de entrada: registra las unidades, crea `DMPrincipal` y `FrmPrincipal` y arranca la aplicación. | 24 |

## Tablas de la base (DBISAM)

Definidas en `legado/db/esquema-dbisam.sql` (74 líneas). Las filas de los CSV de `legado/db/datos/` se contaron con `wc -l` menos el encabezado. El esquema no declara claves foráneas: las relaciones son implícitas.

| Tabla | Responsabilidad | PK / columnas clave | Línea en esquema | Filas CSV | Usada por |
|---|---|---|---|---|---|
| `REGIONES` | Catálogo de regiones con su flete base. | PK `CODIGO` | 4 | 16 | Solo UFrmPedidos (join con CLIENTES). El DM no tiene tabla para ella. |
| `CLIENTES` | Maestro de clientes: RUT, tipo N/V, región, saldo vencido y su vencimiento. | PK `ID_CLIENTE`; índice `RUT`; `REGION` → REGIONES | 12 | 30 | UDMPrincipal, UFrmClientes, UFrmPedidos, UFrmInformes |
| `PRODUCTOS` | Maestro de productos: código, categoría, precio y stock. | PK `ID_PRODUCTO`; índice `CODIGO` | 29 | 40 | UDMPrincipal, UFrmProductos, UFrmPedidos, UStock |
| `PEDIDOS` | Cabecera del pedido: folio, fecha, cliente, usuario, estado y total. | PK `FOLIO`; índices `ID_CLIENTE`, `FECHA`; `ID_CLIENTE` → CLIENTES | 43 | 60 | UDMPrincipal, UFrmPedidos, UFrmClientes, UFrmInformes |
| `PEDIDO_LINEAS` | Detalle del pedido: producto, cantidad y precio unitario de cada línea. | PK (`FOLIO`, `LINEA`); `FOLIO` → PEDIDOS; `ID_PRODUCTO` → PRODUCTOS | 58 | 169 | UDMPrincipal, UFrmPedidos, UStock |
| `PARAMETROS` | Configuración clave/valor (por ejemplo `TASA_IVA`, `IMPRESORA`). | PK `CLAVE` | 68 | 6 | UDMPrincipal (la usa UFrmPrincipal a través de `LeerParametro`) |
| `MOV_STOCK` ⚠ | Registro de movimientos de stock. **No existe en el esquema.** | — | — | — | UStock (`UStock.pas:85`) |

## Lo que me llamó la atención

- **El README no calza con el código.** `legado/README.md` dice «Paradox via BDE» y «Crystal Reports 8», pero el código usa DBISAM 4 (`dbisamtb`, `esquema-dbisam.sql:1`; incluso hay un comentario «antes era alias BDE» en `UDMPrincipal.pas:58`). El informe es una grilla con exportación a CSV (`UFrmInformes.pas:101`), no Crystal.
- **La tabla `MOV_STOCK` no existe en el esquema** y el `INSERT` que la usa está dentro de un `try … except end` vacío (`UStock.pas:84-90`), así que el registro de movimientos falla en silencio.
- **Hay lógica duplicada que no coincide.** El total que se graba se recalcula en `UFrmPedidos.pas:296-319` en vez de usar `UCalculoTotales`. Los umbrales de descuento son distintos (`>=` en `UFrmPedidos.pas:296` contra `>` en `UCalculoTotales.pas:77`) y solo el formulario suma el descuento de navidad. El RUT también se valida de dos formas distintas (`UUtilidades.pas:52` y `UFrmClientes.pas:170`, que invierte K y 0).
- **El acceso a la base está disperso.** UFrmProductos, UFrmInformes y UFrmClientes tienen sus propios componentes DBISAM, y UFrmPedidos y UStock crean queries al vuelo. La ruta `C:\PEDIDOS\DATOS` está fija en el código (`UGlobales.pas:36`, `UDMPrincipal.dfm:12`) y la clave de la base también (`UDMPrincipal.pas:41`; no se copia aquí).
