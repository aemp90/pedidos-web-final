# Defectos corregidos · migración del cálculo de totales

> Regla: si un valor esperado de `migracion/golden-master.csv` cambia, ese caso aparece aquí.
> El único cálculo de totales es `src/PedidosWeb.Core/Totales/CalculoTotales.cs` (HU-03). El 2026-11-11 se le
> unificó `CalculadoraTotales`. Su contrato es el golden master (60 casos, CA-13 de la spec).
> Suma de la columna total del golden master: 58.202.135 (legado) → 58.202.144 (D1) → 57.773.744 (D1 + D2).

## D1 · R05 · Redondeo al peso que baja los ,50 (CORREGIDO)

- **Qué hace el legado:** `Calc3` redondea con `Trunc(AValor + 0.49)`, así que un monto terminado en ,50 **baja** en vez de subir (18.963,50 → 18.963). Lo usan el descuento, el flete y el IVA (`legado/src/UCalculoTotales.pas:48-51`; se usa en `:98`, `:100`, `:105` y `:114`). Mismo redondeo, duplicado, en `RedondeaPesos` (`legado/src/UUtilidades.pas:28-31`), que es el que usa la grabación (`legado/src/UFrmPedidos.pas:308-318`).
- **Corrección:** `CalculoTotales.RedondearAlPeso` usa redondeo comercial: `Math.Round(valor, 0, MidpointRounding.AwayFromZero)`. Un ,50 sube. Lo prueban CA-01, CA-06 y CA-08 de HU-03 (folios 1002, 1027 y 1018).
- **Por qué:** el comentario del legado dice «redondeo al peso», pero el código no hace redondeo comercial: el cliente y contabilidad no pueden reproducir el total con una calculadora. En los datos solo cambian montos terminados exactamente en ,50: el descuento y el IVA tienen a lo más 2 decimales, y el flete con recargo de 15 % termina en ,00 o ,50.
- **Decisión:** D7 modificada y confirmada el 2026-11-11 (`docs/diagnostico/reglas-negocio.md`); antes decía «se replica el legado».
- **Casos del golden master que cambian:** 13 de 60.

  | Caso | Folio | Qué cambia | Total antes → después |
  |---|---|---|---|
  | 2 | 1002 | IVA 36.964 → 36.965 | 231.514 → 231.515 |
  | 6 | 1006 | descuento 25.767 → 25.768 | 1.041.908 → 1.041.907 |
  | 8 | 1008 | IVA 100.120 → 100.121 | 627.070 → 627.071 |
  | 18 | 1018 | flete 18.963 → 18.964 | 180.360 → 180.361 |
  | 21 | 1021 | flete 17.238 → 17.239; IVA 248.778 → 248.779 | 1.558.138 → 1.558.140 |
  | 23 | 1023 | flete 18.963 → 18.964 | 599.716 → 599.717 |
  | 25 | 1025 | descuento 46.425 → 46.426; IVA 410.542 → 410.541 | 2.571.287 → 2.571.285 |
  | 27 | 1027 | flete 17.238 → 17.239 | 1.510.417 → 1.510.418 |
  | 36 | 1036 | flete 18.963 → 18.964 | 1.833.007 → 1.833.008 |
  | 37 | 1037 | flete 18.963 → 18.964 | 724.114 → 724.115 |
  | 42 | 1042 | flete 18.963 → 18.964 | 1.153.066 → 1.153.067 |
  | 46 | 1046 | flete 17.238 → 17.239 | 1.708.835 → 1.708.836 |
  | 54 | 1054 | flete 17.238 → 17.239 | 704.391 → 704.392 |

  Ojo: un descuento que sube 1 peso **baja** el neto, y por eso en los casos 6 y 25 el total baja. Los casos 8, 23 y 42 cambian otra vez con D2; los totales de esta tabla son los que quedan solo con D1.
- **Suma de la columna total:** 58.202.135 → 58.202.144 (+9).
- **Commit:** `Corrige R05: redondeo comercial al peso en CalculoTotales` (separado del commit «igual al legado»).
- **Convivencia:** el histórico de `legado/db/datos/PEDIDOS.csv` se grabó con el redondeo del legado: los mismos 13 folios difieren en ±1 o 2 pesos (1002, 1006, 1008, 1018, 1021, 1023, 1025, 1027, 1036, 1037, 1042, 1046, 1054). Lo comprueba CA-11. Contabilidad tiene que tomar esa diferencia como **intencional** al conciliar (RS-18), no como error.

## D2 · Umbrales del descuento por volumen: `>` en pantalla y `>=` al grabar (CORREGIDO)

- **Qué hace el legado:** `UCalculoTotales` (el total que se **muestra**) usa `>` 1.000.000 / `>` 500.000 (`legado/src/UCalculoTotales.pas:77-80`); la grabación usa `>=` (`legado/src/UFrmPedidos.pas:296-299`). Con una base exacta, el cliente ve un total y se graba otro (RS-05; folio 1011: pantalla 588.931, grabado 559.181). El comentario `UCalculoTotales.pas:4` («5 % sobre 400.000») no coincide con ninguno de los dos (RS-15).
- **Corrección:** `CalculoTotales.CalcularPorcentajeDescuento` usa `>=` (D1 de `reglas-negocio.md`): pantalla y grabación dan lo mismo. Lo prueban CA-02, CA-03 y CA-04 de HU-03.
- **Por qué:** lo que se cobra es el total grabado; la pantalla mostraba un total más alto que el cobrado.
- **Casos del golden master que cambian:** 13 de 60, todos con base de descuento exacta.
  - Base 500.000 (5 puntos más), −29.750 cada uno: casos 8, 11, 19, 23, 50 y 55 (folios 1008, 1011, 1019, 1023, 1050, 1055).
  - Base 1.000.000 (8 % en vez de 5 %), −35.700 cada uno: casos 26, 32, 35, 39, 42, 48 y 60 (folios 1026, 1032, 1035, 1039, 1042, 1048, 1060).
  - En cada uno cambian `porc_descuento`, `descuento`, `neto`, `iva` y `total`.
- **Suma de la columna total:** 58.202.144 → 57.773.744 (−428.400).
- **Commit:** `Corrige D2: umbrales del descuento con >= y unifica CalculoTotales` (separado del de R05).
- **Convivencia:** sin diferencias con el histórico: los 13 folios se grabaron con `>=`. La única diferencia es con la **pantalla** del legado, que mostraba un total mayor. El área comercial debe saberlo si compara pantallas durante la marcha en paralelo.
