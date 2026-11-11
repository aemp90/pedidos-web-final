# HU-03 · Cálculo de totales del pedido

Estado: aprobada (2026-11-10)

**Historia:** Como vendedor, quiero que PedidosWeb calcule el total del pedido con descuentos, flete e IVA
igual que lo que hoy se graba en Pedidos Legado, para cobrarle al cliente lo correcto y que el total que
veo sea el mismo que queda guardado.

Origen: módulo «Cálculo de totales», primero en la matriz de decisión (Modernizar, 4,40;
`docs/diagnostico/matriz-decision.md`). Reglas 1–11 y decisiones D1, D3 y D7 de
`docs/diagnostico/reglas-negocio.md`. Cierra RS-05 (dos cálculos distintos): hay **un solo** cálculo.

## Qué recibe

- **Líneas** del pedido: cantidad, precio unitario y categoría del producto.
- **Si el cliente es VIP** (`TIPO = 'V'` en el legado).
- **Región** del cliente (1 a 16) y **flete base** de esa región (columna `FLETE_BASE` de `REGIONES`).
- **Tasa de IVA**, como fracción: 0,19 por defecto (regla 11).

## Qué devuelve

Subtotal, base del descuento, porcentaje de descuento, descuento, neto, flete, IVA y total. Todos los montos
son `decimal` en pesos chilenos, sin decimales.

## Cómo se calcula

1. **Subtotal** = Σ (cantidad × precio unitario) de todas las líneas.
2. **Base del descuento** = lo mismo, pero sin las líneas de categoría `OFE` (regla 3).
3. **Porcentaje por volumen (D1):** base `>= 1.000.000` → 8 %; base `>= 500.000` → 5 %; si no, 0 %.
4. **VIP:** suma 3 puntos (regla 4). **Tope:** el porcentaje total no pasa de 10 % (regla 6).
5. **Descuento** = redondeo(base × porcentaje). **Neto** = subtotal − descuento.
6. **Flete** = redondeo(flete base × 1,15) en las regiones 11 y 12; redondeo(flete base) en las demás (regla 8).
   Sin líneas, el flete es 0 (regla 9).
7. **IVA** = redondeo((neto + flete) × tasa) (regla 10). **Total** = neto + flete + IVA.

**Redondeo (D7):** se replica el del legado, `Trunc(x + 0.49)`, que para montos positivos equivale a
`Math.Floor(x + 0.49m)`. Ojo: un monto terminado en ,50 **baja** (18.963,50 → 18.963). Es a propósito,
para que los totales cuadren con el histórico.

## Criterios de aceptación

Cada criterio se puede comprobar con un sí o un no. Los casos son pedidos reales de
`legado/db/datos/` (folio entre paréntesis) y el total esperado es el que el legado grabó. Tasa de IVA 0,19,
salvo en CA-10.

- **CA-01 · Sin descuento.** Dadas las líneas 5 × 8.210 (BEB) y 30 × 5.000 (CON), cliente normal de la
  región 13 con flete base 3.500 (folio 1002), cuando calculo, entonces subtotal 191.050, descuento 0,
  flete 3.500, IVA 36.964 y total 231.514.
- **CA-02 · Límite exacto de 500.000 → 5 %.** Dada la línea 40 × 12.500 (ABA), cliente normal de la región 1
  con flete base 8.900 (folio 1019), cuando calculo, entonces porcentaje 5 %, descuento 25.000 y total 575.841.
- **CA-03 · Límite exacto de 1.000.000 → 8 %.** Dada la línea 20 × 50.000 (ABA), cliente normal de la región 6
  con flete base 4.900 (folio 1035), cuando calculo, entonces porcentaje 8 %, descuento 80.000 y total 1.100.631.
- **CA-04 · VIP suma 3 puntos.** Dada la línea 50 × 10.000 (BEB), cliente VIP de la región 10 con flete base
  9.900 (folio 1011), cuando calculo, entonces porcentaje 8 % (5 + 3), descuento 40.000 y total 559.181.
  (La pantalla del legado mostraba 588.931: esta es la diferencia que cierra D1).
- **CA-05 · VIP sin descuento por volumen.** Dada la línea 4 × 8.210 (BEB), cliente VIP de la región 13 con
  flete base 3.500 (folio 1038), cuando calculo, entonces porcentaje 3 %, descuento 985 y total 42.072.
- **CA-06 · Tope de 10 % y recargo de la región 11.** Dadas las líneas 30 × 29.990 (LIM), 1 × 39.990 (OFE) y
  20 × 22.350 (CON), cliente VIP de la región 11 con flete base 14.990 (folio 1027), cuando calculo, entonces
  base 1.346.700, porcentaje 10 % (no 11 %), descuento 134.670, flete 17.238 y total 1.510.417.
- **CA-07 · Las líneas OFE no entran en la base.** Dadas las líneas 3 × 18.900 (ABA), 2 × 18.350 (LIM) y
  36 × 35.990 (OFE), cliente normal de la región 8 con flete base 6.900 (folio 1031), cuando calculo, entonces
  subtotal 1.389.040, base 93.400, porcentaje 0 % y total 1.661.169.
- **CA-08 · Recargo de la región 12 con redondeo hacia abajo.** Dadas las líneas 3 × 19.200 (PER) y
  15 × 5.000 (CON), cliente normal de la región 12 con flete base 16.490 (folio 1018), cuando calculo,
  entonces flete 18.963 (16.490 × 1,15 = 18.963,50) y total 180.360.
- **CA-09 · Pedido sin líneas.** Dado un pedido sin líneas, cliente normal de la región 13 con flete base
  3.500, cuando calculo, entonces subtotal, descuento, flete, IVA y total son 0.
- **CA-10 · La tasa de IVA es un dato de entrada.** Dadas las líneas del CA-01 con tasa 0,10, cuando calculo,
  entonces IVA 19.455 y total 214.005.
- **CA-11 · Cuadratura con el histórico.** Dado cualquiera de los 60 pedidos de `legado/db/datos/PEDIDOS.csv`
  (líneas de `PEDIDO_LINEAS.csv`, categoría de `PRODUCTOS.csv`, VIP y región de `CLIENTES.csv`, flete base de
  `REGIONES.csv`), cuando calculo, entonces el total es igual a su `TOTAL` grabado. Comprobado antes de
  escribir esta spec: los 60 coinciden.

- **CA-12 · Líneas imposibles.** Dada una línea con cantidad ≤ 0 o con precio unitario ≤ 0, cuando calculo,
  entonces se lanza `ArgumentException` y no se devuelve ningún total.

## Si llegan datos inválidos

- Una línea con cantidad ≤ 0 o precio unitario ≤ 0 lanza `ArgumentException` (CA-12): nunca se calcula un
  total con datos imposibles. Las demás validaciones del pedido (regla 14) siguen fuera de alcance.

## Reglas del legado que no se migran

- **Descuento de navidad (D3):** no existe. El cálculo no recibe la fecha y ningún pedido recibe el +2 %.
- **Límites con `>` de la pantalla del legado (D1):** se descartan; se usa `>=` en todos los casos.
- **Comentario «5 % sobre 400.000»** (`UCalculoTotales.pas:4`): no corresponde al código y se ignora (RS-15).

## Fuera de alcance

- Endpoint en la API y pantalla web (se agregan con la historia de toma de pedidos).
- El resto de las validaciones del pedido (al menos una línea, más de 40 líneas: regla 14), el pedido mínimo (D2), el bloqueo
  por deuda o por cliente inactivo (D5), el stock (D6) y el folio.
- Leer la tasa de IVA o el flete base desde PostgreSQL: por ahora son datos de entrada.
- Cambiar `TotalPedido` de HU-02 (suma de líneas sin IVA), que sigue igual.

## Notas de implementación (2026-11-10)

Todos los criterios se cumplen (`dotnet test` en verde: 121 tests, 0 advertencias). Implementado según
`plan.md`, sin cambios respecto a lo escrito. Código en `src/PedidosWeb.Core/Totales/`.

Prueba de mutación: con `>` en vez de `>=` en los límites (el bug de la pantalla del legado) fallan CA-02,
CA-03, CA-04 y CA-11; se restauró el código.

| Criterio | Test (`Totales/CalculadoraTotalesTests`) |
|---|---|
| CA-01 | `Calcular_Folio1002SinDescuento_DevuelveTotal231514` |
| CA-02 | `Calcular_BaseExacta500000_Aplica5Porciento` |
| CA-03 | `Calcular_BaseExacta1000000_Aplica8Porciento` |
| CA-04 | `Calcular_ClienteVipConBase500000_Aplica8Porciento` |
| CA-05 | `Calcular_ClienteVipSinVolumen_Aplica3Porciento` |
| CA-06 | `Calcular_VipConBaseSobreUnMillon_TopaEn10PorcientoYRecargaRegion11` |
| CA-07 | `Calcular_LineasOfe_NoEntranEnLaBase` |
| CA-08 | `Calcular_Region12_RecargaFleteYRedondeaHaciaAbajo` |
| CA-09 | `Calcular_SinLineas_DevuelveTodoCero` |
| CA-10 | `Calcular_TasaIva10Porciento_UsaLaTasaRecibida` |
| CA-11 | `Calcular_Los60PedidosDelLegado_CoincidenConTotalGrabado` (lee los CSV de `legado/db/datos/`, enlazados en el `.csproj` de tests) |
| CA-12 | `Calcular_LineaConCantidadOPrecioNoPositivo_LanzaArgumentException` (4 casos) |

## Preguntas abiertas

Ninguna. Resuelta el 2026-11-10: una línea con cantidad o precio ≤ 0 lanza `ArgumentException` (CA-12).
