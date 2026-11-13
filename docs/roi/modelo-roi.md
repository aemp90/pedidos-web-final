# Modelo de ROI · piloto de Pedidos Web

> Plantilla de la Unidad 10. Complétala con Claude Code en las etapas 1 y 2 y revísala tú.
> Fuente de datos: `docs/roi/datos-piloto.csv` (equipo Pedidos Web de 6 devs; semanas 1-6 antes, 7-12 con IA; datos ficticios) y tus entregables de las unidades 5 a 9.
> Regla: ningún número sin su fuente o su supuesto. Si un dato no está, escribe FALTA.

**Autor:** Andres · **Fecha:** 2026-11-13 · **Horizonte:** 12 meses

## 1. Datos (etapa 1)

Antes = promedio de las semanas 1-6; Con IA = promedio de las semanas 7-12.

| Dato | Antes | Con IA | Fuente (archivo y semanas) |
|---|---|---|---|
| Despliegues por semana | 2,0 (12 en 6 semanas) | 3,0 (18 en 6 semanas) | `datos-piloto.csv`, `despliegues`, semanas 1-6 y 7-12 |
| Lead time (días) | 6,6 (39,5 / 6) | 5,2 (31,0 / 6) | `datos-piloto.csv`, `lead_time_dias`, semanas 1-6 y 7-12 |
| Change fail rate | 41,7 % (5 de 12 despliegues) | 33,3 % (6 de 18 despliegues) | `datos-piloto.csv`, `cambios_con_falla` / `despliegues`, semanas 1-6 y 7-12 |
| Tiempo de recuperación (horas) | 6,0 por falla (30 h / 5 fallas) | 4,0 por falla (24 h / 6 fallas) | `datos-piloto.csv`, `horas_recuperacion` / `cambios_con_falla`, semanas 1-6 y 7-12 |
| Defectos escapados por semana | 2,0 (12 en 6 semanas) | 1,5 (9 en 6 semanas) | `datos-piloto.csv`, `defectos_escapados`, semanas 1-6 y 7-12 |
| Horas de retrabajo por semana | 12,0 (72 en 6 semanas) | 10,7 (64 en 6 semanas) | `datos-piloto.csv`, `horas_retrabajo`, semanas 1-6 y 7-12 |
| Horas ahorradas por semana (equipo) | 0 | 8,5 (51 en 6 semanas; 6 devs ≈ 1,4 h por dev) | `datos-piloto.csv`, `horas_ahorradas_equipo`, semanas 7-12 (las semanas 1-6 vienen en 0). Es un dato **reportado** por el equipo, no medido por una herramienta. |

**Evidencia propia** (una línea por archivo: qué dato aporta):

- `docs/migracion/defectos-corregidos.md` (U5): 2 defectos del legado corregidos en la migración (D1, redondeo de los ,50; D2, umbrales del descuento con `>=`). Cambian 23 de los 60 casos del golden master (13 por D1 y 13 por D2; 3 se repiten: 8, 23 y 42), y la suma de la columna total pasa de 58.202.135 a 57.773.744 (−428.391 pesos). Sin fecha por semana: no se puede ubicar en el piloto.
- `docs/calidad/registro-u6.md` (U6): una falla provocada a mano dejó 16 de 189 tests en rojo, `@claude` la diagnosticó y se aceptó su corrección (CI en verde, merge `165d3e6`). En la etapa 1, Claude propuso 5 tests de borde para `CalculoTotales` y los 5 se quedaron, porque fallaron con un cambio a mano. No hay tiempos: no aporta horas.
- `docs/checklist-revision.md` (U7): checklist de revisión con 12 comprobaciones (antes del PR, al revisar, con los hallazgos de la IA y para el merge). PR de HU-03: #5, donde la revisión de la IA no marcó hallazgos y no hubo revisión de personas. FALTAN los tiempos de revisión: no aporta horas.
- `docs/gobernanza/politica-ia.md` (U8): 1 de 9 controles probado (`guardian.sh` bloqueó la lectura de `.env` por la shell el 2026-11-12); los otros 8 están PENDIENTE. Aporta riesgo, no cifras de ahorro.
- `docs/adopcion/plan-evolucion.md` (U9): Pedidos Web está en nivel 3 (Equipo con reglas): Claude Code en 86 de 107 PR (80 %) y `tasa_aceptacion` de 0,74 a 0,78 en 8 semanas (fuente: `docs/adopcion/analytics-ficticio.csv`). Ese CSV es otro período y no se cruza con `datos-piloto.csv`.

**Lo que veo en las semanas 7 y 8:** la curva de adopción: en esas dos semanas todo empeora frente al «antes» (3 fallas en 4 despliegues, 75 %; retrabajo de 15 y 14 h contra 12; 3 y 2 defectos; lead time de 7,0 y 6,5 días) y el ahorro reportado es bajo (3 y 4 h). La mejora aparece desde la semana 9, así que el promedio 7-12 incluye ese costo inicial.

## 2. Supuestos (etapa 2)

Tipos: **oficial** (publicado, con fecha) · **medido** (dato del piloto) · **estimado** (juicio del equipo, el más débil).

| Supuesto | Valor | Tipo | Fuente |
|---|---|---|---|
| Equipo | 6 devs | medido | `docs/roi/datos-piloto.csv` (encabezado del modelo: equipo Pedidos Web de 6 devs) |
| Asiento | USD 25 por dev al mes (Team Standard, pago mensual; USD 20 con pago anual) | oficial | [claude.com/pricing](https://claude.com/pricing) y [support.claude.com · What is the Team plan?](https://support.claude.com/en/articles/9266767-what-is-the-team-plan), consultados el 2026-11-13. Precio para EE. UU., sin impuestos. |
| Capacitación | 16 h por dev | estimado | Juicio del equipo: talleres y práctica del curso. FALTA el registro real de horas. |
| Curva de adopción | 9 h del equipo, una sola vez | medido | `datos-piloto.csv`, semanas 7-8 contra el promedio de las semanas 1-6: retrabajo (15 − 12) + (14 − 12) = 5 h; recuperación (8 + 6) − 2 × 5,0 = 4 h. |
| Valor hora | USD 20 por hora de dev | estimado | Juicio del equipo. FALTA el costo hora real de RR. HH. |
| Semanas trabajadas al año | 48 | estimado | 52 semanas menos 4 de vacaciones y feriados. |
| Horas ahorradas (conservador) | 8,5 h por semana (equipo) | medido (reportado) | `datos-piloto.csv`, `horas_ahorradas_equipo`, semanas 7-12: 51 / 6. Incluye la curva. |
| Horas ahorradas (optimista) | 11,0 h por semana (equipo) | medido (reportado) | `datos-piloto.csv`, `horas_ahorradas_equipo`, semanas 9-12: (9 + 11 + 12 + 12) / 4 = 44 / 4. Sin la curva. |
| Defectos evitados (conservador) | 0,5 por semana | medido | `datos-piloto.csv`, `defectos_escapados`: 2,0 (semanas 1-6) − 1,5 (semanas 7-12). |
| Defectos evitados (optimista) | 1,0 por semana | medido | `datos-piloto.csv`, `defectos_escapados`: 2,0 (semanas 1-6) − 1,0 (semanas 9-12: 4 / 4). |
| Costo de un defecto en producción | 6 h × USD 20 = USD 120 | estimado | Usa como referencia un dato medido: 6,0 h de recuperación por falla, semanas 1-6 (30 h / 5 fallas). FALTA el costo real de un defecto escapado. |

## 3. Costos

Los costos son los mismos en ambos escenarios; solo cambian los beneficios.

| Costo | Cálculo | USD |
|---|---|---|
| Asientos | 25 × 6 devs × 12 meses | 1.800 |
| Capacitación | 16 h × 6 devs × 20 | 1.920 |
| Curva de adopción | 9 h × 20 | 180 |
| **Total** | 1.800 + 1.920 + 180 | **3.900** |

Con pago anual los asientos bajan a 20 × 6 × 12 = 1.440 y el total queda en 3.540.

## 4. Beneficios y escenarios

ROI = (beneficio − costo) / costo

| | Conservador | Optimista |
|---|---|---|
| Horas ahorradas × valor hora | 8,5 × 48 = 408 h × 20 = 8.160 | 11,0 × 48 = 528 h × 20 = 10.560 |
| Defectos evitados × costo en producción | 0,5 × 48 = 24 × 120 = 2.880 | 1,0 × 48 = 48 × 120 = 5.760 |
| **Beneficio total** | 8.160 + 2.880 = **11.040** | 10.560 + 5.760 = **16.320** |
| **ROI** | (11.040 − 3.900) / 3.900 = 7.140 / 3.900 = **183 %** | (16.320 − 3.900) / 3.900 = 12.420 / 3.900 = **318 %** |

**ROI a 12 meses: entre 183 % y 318 %.**

## 5. Sensibilidad

**Supuesto que más pesa:** las horas ahorradas. Son el 74 % del beneficio conservador (8.160 / 11.040), las reporta el propio equipo (no las mide una herramienta) y vienen de solo 6 semanas proyectadas a 48. El valor hora también pesa, pero mueve beneficios y costos a la vez.

| Si… | Cálculo | ROI |
|---|---|---|
| El ahorro es la mitad del conservador | Horas: 204 h × 20 = 4.080. Beneficio: 4.080 + 2.880 = 6.960. ROI: (6.960 − 3.900) / 3.900 = 3.060 / 3.900 | **78 %** |
| El ahorro y los defectos evitados son la mitad del conservador | Beneficio: 4.080 + 1.440 = 5.520. ROI: (5.520 − 3.900) / 3.900 = 1.620 / 3.900 | **42 %** |
| El valor hora es USD 15 (conservador) | Beneficio: 408 × 15 + 24 × 90 = 6.120 + 2.160 = 8.280. Costo: 1.800 + 96 × 15 + 9 × 15 = 1.800 + 1.440 + 135 = 3.375. ROI: (8.280 − 3.375) / 3.375 = 4.905 / 3.375 | **145 %** |

Aun con el ahorro a la mitad, el ROI sigue siendo positivo.

## 6. Punto de equilibrio (Tu reto)

**Qué despejo:** las horas ahorradas por dev al mes que dejan el ROI en 0, es decir, donde el beneficio iguala al costo.

**Qué dejo fijo:** el costo total (USD 3.900), el valor hora (USD 20) y el equipo (6 devs, 12 meses). Los defectos evitados **no se cuentan** (valen 0): el equilibrio depende solo de las horas, sin apoyarse en el costo por defecto, que es estimado.

**Cálculo:**

- ROI = 0 ⇒ beneficio = costo ⇒ H × 20 = 3.900
- H = 3.900 / 20 = 195 h al año (equipo)
- Por dev al mes: 195 / 6 devs / 12 meses = **2,7 h**
- Medido en el piloto (conservador): 8,5 h por semana × 48 / 12 / 6 = 5,67 h por dev al mes, unas **2 veces** el equilibrio (5,67 / 2,71 = 2,1).

**Para la gerencia:** basta que cada dev ahorre 2,7 h al mes para cubrir el costo; el piloto muestra 5,7.

## 7. Limitaciones

- Los datos son ficticios y la muestra es pequeña: este ROI es un ejercicio de método.
- Las horas ahorradas son autorreportadas y salen de 6 semanas; el modelo supone que se mantienen las 48 semanas del año.
- La baja del retrabajo (12,0 → 10,7 h por semana) no se suma como beneficio aparte, para no contarla dos veces con las horas ahorradas que reporta el equipo.
- El valor hora, la capacitación y el costo de un defecto son estimados: FALTAN los datos reales de RR. HH. y del registro de incidentes.
- La curva de adopción solo cuenta las horas extra medidas (retrabajo y recuperación); no incluye la productividad perdida mientras el equipo aprendía.
