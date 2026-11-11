# Plan HU-03 · Cálculo de totales del pedido

Estado: aprobado (2026-11-10) · Spec: [spec.md](spec.md)

> **Actualización 2026-11-11:** `CalculadoraTotales` se unificó en `CalculoTotales` (mismos métodos y
> constantes) y `RedondearAlPeso` pasó a redondeo comercial (D7 modificada, R05). Donde este plan dice
> `CalculadoraTotales` o `Math.Floor(valor + 0.49m)`, léase lo nuevo; detalle en la spec.

## Contexto
El módulo «Cálculo de totales» es el primero en la matriz de decisión (Modernizar, 4,40). Hoy el legado
calcula el total en dos lugares con reglas distintas (RS-05). Esta historia lleva a `PedidosWeb.Core` **un
solo** cálculo (descuento por volumen y VIP con tope, OFE fuera de la base, flete con recargo en las regiones
11 y 12, IVA, redondeo `Trunc(x + 0.49)`), y lo prueba contra los 60 pedidos históricos, que ya comprobé que
cuadran con las reglas aprobadas (D1, D3, D7).

## Enfoque
- Cálculo puro en Core, sin infraestructura (igual que `UCalculoTotales.pas`, que es una unidad pura).
- Namespace nuevo `PedidosWeb.Core.Totales`. **No** se toca `Lineas/TotalPedido.cs` ni `LineaPedido`
  (HU-02 sigue igual; `LineaPedido` no tiene categoría y agregarla cambiaría los datos de ejemplo).
- Todo en `decimal`; la tasa de IVA y los porcentajes también son `decimal` (0.19m, 0.05m…).
- Sin paquetes NuGet nuevos y sin cambios en la Api.

## Diseño · `src/PedidosWeb.Core/Totales/`

| Archivo | Qué contiene |
|---|---|
| `LineaCalculo.cs` | `public sealed record LineaCalculo(int Cantidad, decimal PrecioUnitario, string Categoria)`. |
| `ResultadoTotales.cs` | `public sealed record ResultadoTotales(decimal Subtotal, decimal BaseDescuento, decimal PorcentajeDescuento, decimal Descuento, decimal Neto, decimal Flete, decimal Iva, decimal Total)`. |
| `CalculadoraTotales.cs` | Clase estática con constantes con nombre (`TasaIvaPorDefecto = 0.19m`, `LimiteVolumenAlto = 1_000_000m`, `LimiteVolumenBajo = 500_000m`, `DescuentoVolumenAlto = 0.08m`, `DescuentoVolumenBajo = 0.05m`, `DescuentoVip = 0.03m`, `TopeDescuento = 0.10m`, `RecargoFleteAustral = 1.15m`, `CategoriaOferta = "OFE"`) y los métodos de abajo. |

Métodos públicos de `CalculadoraTotales` (comentarios XML en español citando regla y archivo:línea del legado):
- `RedondearAlPeso(decimal valor)` → `Math.Floor(valor + 0.49m)` (D7, `UCalculoTotales.pas:50`).
- `CalcularPorcentajeDescuento(decimal baseDescuento, bool esVip)` → 8 % si `>= 1.000.000`, 5 % si `>= 500.000`,
  si no 0; `+ 3 %` si VIP; tope 10 % (D1, reglas 1, 4 y 6).
- `CalcularFlete(decimal fleteBase, int region)` → `RedondearAlPeso(fleteBase * 1.15m)` en regiones 11 y 12;
  si no `RedondearAlPeso(fleteBase)` (regla 8).
- `Calcular(IEnumerable<LineaCalculo> lineas, bool esVip, int region, decimal fleteBase, decimal tasaIva = TasaIvaPorDefecto)`
  → `ResultadoTotales`, siguiendo los 7 pasos de la spec. Sin líneas: flete 0 y todo 0 (regla 9).
  - Validación (CA-12): `ArgumentNullException.ThrowIfNull(lineas)`; si alguna línea tiene `Cantidad <= 0` o
    `PrecioUnitario <= 0` → `ArgumentException` con mensaje en español (ej. «La línea 2 tiene cantidad 0; debe ser mayor que 0.»), antes de calcular nada.
  - OFE: comparación `string.Equals(Categoria?.Trim(), "OFE", StringComparison.OrdinalIgnoreCase)`
    (igual que `UpperCase(Trim(...))` del legado, `UCalculoTotales.pas:70`).
  - Sin parámetro de fecha: no existe descuento de navidad (D3).

## Incrementos (cada uno termina con `dotnet build` y `dotnet test` en verde)

### 1 · Calculadora y casos de la spec
- Toca (nuevos): `src/PedidosWeb.Core/Totales/{LineaCalculo,ResultadoTotales,CalculadoraTotales}.cs`,
  `tests/PedidosWeb.Core.Tests/Totales/CalculadoraTotalesTests.cs`.
- No toca: `Lineas/`, `Pedidos/`, `Clientes/`, la Api.
- Tests (los datos de cada folio van escritos en el test, tomados de la spec):
  | CA | Test |
  |---|---|
  | 01 | `Calcular_Folio1002SinDescuento_DevuelveTotal231514` (verifica subtotal, descuento, flete, IVA y total) |
  | 02 | `Calcular_BaseExacta500000_Aplica5Porciento` (folio 1019) |
  | 03 | `Calcular_BaseExacta1000000_Aplica8Porciento` (folio 1035) |
  | 04 | `Calcular_ClienteVipConBase500000_Aplica8Porciento` (folio 1011) |
  | 05 | `Calcular_ClienteVipSinVolumen_Aplica3Porciento` (folio 1038) |
  | 06 | `Calcular_VipConBaseSobreUnMillon_TopaEn10PorcientoYRecargaRegion11` (folio 1027) |
  | 07 | `Calcular_LineasOfe_NoEntranEnLaBase` (folio 1031) |
  | 08 | `Calcular_Region12_RecargaFleteYRedondeaHaciaAbajo` (folio 1018, flete 18.963) |
  | 09 | `Calcular_SinLineas_DevuelveTodoCero` |
  | 10 | `Calcular_TasaIva10Porciento_UsaLaTasaRecibida` (IVA 19.455, total 214.005) |
  | 12 | `Calcular_LineaConCantidadOPrecioNoPositivo_LanzaArgumentException` (`[Theory]`: cantidad 0 y −1, precio 0 y −1) |

### 2 · Cuadratura con el histórico (CA-11)
- Toca: `tests/PedidosWeb.Core.Tests/PedidosWeb.Core.Tests.csproj` (agrega los CSV del legado como archivos
  enlazados que se copian a la salida: `<None Include="..\..\legado\db\datos\*.csv" Link="DatosLegado\%(Filename)%(Extension)" CopyToOutputDirectory="PreserveNewest" />`)
  y `Totales/CalculadoraTotalesTests.cs` (nuevo test + lector CSV privado en el mismo archivo).
- No toca: nada en `legado/` (solo se lee) ni en `src/`.
- Lector CSV: mínimo, sin paquetes; respeta campos entre comillas porque `PRODUCTOS.csv:13` tiene
  `"AGUA MINERAL PACK 6 X 1,6 L"`. Lee por nombre de columna del encabezado, en UTF-8.
- Test CA-11 · `Calcular_Los60PedidosDelLegado_CoincidenConTotalGrabado`: arma cada pedido con
  `PEDIDO_LINEAS` + categoría de `PRODUCTOS` + tipo y región de `CLIENTES` + `FLETE_BASE` de `REGIONES`,
  calcula y compara con `PEDIDOS.TOTAL`. Afirma `60` pedidos y lista vacía de folios descuadrados
  (mismo patrón que `TotalPedidoTests.CalcularTotal_Los45PedidosDeEjemplo_CoincideConPedidoTotal`).

### Cierre
- En la spec: tabla «Notas de implementación» con criterio → test (como en HU-02).
- Agregar `docs/modulos/Totales.md` no es parte de este plan (se puede pedir después con la Skill
  `documentar-modulo`).

## Riesgos
- **Enlazar CSV de `legado/`** desde el proyecto de tests: si alguien edita esos datos, el CA-11 falla. Es lo
  buscado (el histórico es la referencia), pero hay que saberlo.
- **Redondeo a propósito «incorrecto»**: alguien podría «arreglar» `RedondearAlPeso` a redondeo comercial; el
  CA-08 (18.963,50 → 18.963) y el CA-11 lo impiden. El comentario XML lo dirá explícitamente (D7, RS-18).

## Fuera de este plan
- Endpoint, pantalla, PostgreSQL, validaciones del pedido (regla 14), pedido mínimo, stock, folio.
- No hay commit: lo haces tú después de leer el diff.

## Verificación
- `dotnet build` sin warnings nuevos y `dotnet test` en verde al cerrar cada incremento: los 12 criterios más
  todos los tests previos de HU-01 y HU-02.
- Revisar que el CA-11 realmente leyó 60 pedidos (el test lo afirma) y que los CSV aparecen en
  `bin/.../DatosLegado/`.
