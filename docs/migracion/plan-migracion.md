# Plan de migración · Pedidos Legado → Pedidos Web

> Base: `docs/diagnostico/matriz-decision.md` (decisiones M1–M3), `riesgos.md`, `reglas-negocio.md` (D1–D7) y `defectos-corregidos.md`.
> Estrategia: strangler fig, un módulo a la vez. El legado sigue siendo el sistema oficial de cada módulo hasta que se cumpla su criterio de corte.

## 1. Etapas
1. **Cálculo de totales en C#** (`src/PedidosWeb.Core/Totales/CalculoTotales.cs`): hecho en la Unidad 5. Un solo cálculo (HU-03), con golden master de 60 casos en verde y R05 (redondeo) y D2 (umbrales `>=`) corregidos.
2. **Base de datos y acceso** (módulo 7, M3): PostgreSQL y ASP.NET Core Identity con los roles Vendedor, Jefatura y Administrador. Se cargan los datos de DBISAM y la clave sale del código (cierra RS-02 y RS-03). Es requisito de las etapas siguientes.
3. **Utilidades y Clientes**: un solo validador de RUT (RS-04), RUT normalizado (RS-17), `DateOnly` en vez de años de 2 dígitos (RS-10), ID generado por la base (RS-07) y bloqueo de clientes inactivos (D5a).
4. **Toma de pedidos y Stock**: el pedido usa `CalculoTotales` (cierra RS-05), con folio de secuencia, transacción única para cabecera, líneas, total y stock (RS-06, RS-07), stock validado antes de grabar (D6, RS-13), deuda según D5b y SQL parametrizado (RS-01).
5. **Informes**: neto e IVA correctos (RS-08) y fechas completas en el CSV para contabilidad (RS-10).
6. **Productos**: el módulo más sano y con menos margen en la matriz; va al final.

## 2. Convivencia
- **Una sola fuente de verdad por módulo.** Mientras una etapa no pasa su corte, el legado sigue grabando y PedidosWeb solo lee o calcula en paralelo. Después se invierte: PedidosWeb graba y el legado queda en solo lectura.
- **Marcha en paralelo de totales:** durante la etapa 4, cada pedido se calcula en los dos sistemas y se compara el total. Una diferencia cuenta como **intencional** solo si está en `defectos-corregidos.md` o en las decisiones D1–D7 (RS-18). Hoy son ±1–2 pesos en los montos que terminan en ,50 (R05) y, solo frente a la **pantalla** del legado, el umbral exacto de 500.000 / 1.000.000 (D2).
- **Golden master como contrato:** `migracion/golden-master.csv` solo cambia con una corrección registrada, y en un commit separado del «igual al legado».
- **Datos:** la migración DBISAM → PostgreSQL se repite en cada ensayo y se verifica con conteos y la suma de `TOTAL` por mes contra el legado.

## 3. Riesgos
| Riesgo | Mitigación |
|---|---|
| Contabilidad reporta como error las diferencias de redondeo (R05) frente al histórico | Lista de folios afectados en `defectos-corregidos.md` (D1) y CA-11, que fija exactamente cuáles descuadran (D7 confirmada el 2026-11-11); entregar esa lista a contabilidad antes de la marcha en paralelo |
| Vuelve a aparecer un segundo cálculo de totales (RS-05 en el sistema nuevo), por ejemplo en la API o en un informe | Un solo cálculo, `CalculoTotales` (unificado el 2026-11-11); todo endpoint o informe lo llama a él, y el golden master (CA-13) protege cualquier cambio |
| Folios o stock duplicados al convivir dos sistemas que graban (RS-07) | Nunca graban los dos a la vez sobre el mismo módulo; el corte fija el folio inicial de PedidosWeb por encima del `MAX` del legado |
| Pérdida o corrupción de datos al pasar de DBISAM a PostgreSQL, incluido el `Double` mal convertido (RS-09) | Ensayos repetibles con conciliación de conteos y sumas; montos en `decimal` sin decimales |
| Se migran reglas equivocadas por comentarios que contradicen el código (RS-15, por ejemplo «5 % sobre 400.000») | Toda regla sale de `reglas-negocio.md` con archivo:línea verificado y de una spec aprobada |

## 4. Criterio de corte
- **Cuándo se apaga el legado en una etapa:** tests en verde; dos semanas de marcha en paralelo sin diferencias fuera de las documentadas; conciliación de fin de mes aceptada por contabilidad; y visto bueno del dueño del módulo (comercial en totales y pedidos, contabilidad en informes).
- **Cómo se vuelve atrás:** el legado queda en solo lectura, no se borra. Si falla algo grave (un total mal cobrado, un pedido perdido o un folio duplicado), se reactiva la grabación en el legado, se cargan a mano los pedidos tomados en PedidosWeb desde el corte y se corrige antes de reintentar. La vuelta atrás se ensaya una vez antes de cada corte.
- **Apagado final:** cuando las etapas 2 a 6 pasan su corte y se completa un cierre contable entero solo con PedidosWeb.
