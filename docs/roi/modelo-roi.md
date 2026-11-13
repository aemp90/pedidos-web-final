# Modelo de ROI · piloto de Pedidos Web

> Plantilla de la Unidad 10. Complétala con Claude Code en las etapas 1 y 2 y revísala tú.
> Fuente de datos: `docs/roi/datos-piloto.csv` (equipo Pedidos Web de 6 devs; semanas 1-6 antes, 7-12 con IA; datos ficticios) y tus entregables de las unidades 5 a 9.
> Regla: ningún número sin su fuente o su supuesto. Si un dato no está, escribe FALTA.

**Autor:** <tu nombre> · **Fecha:** <fecha> · **Horizonte:** <meses>

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
- `docs/calidad/registro-u6.md` (U6): una falla provocada a mano dejó 16 de 189 tests en rojo, `@claude` la diagnosticó y se aceptó su corrección (CI en verde, merge `165d3e6`). FALTA la etapa 1 (tests propuestos, guardados y borrados) y no hay tiempos: no aporta horas.
- `docs/checklist-revision.md` (U7): FALTA. La plantilla está sin completar; no hay PR, hallazgos ni tiempos de revisión.
- `docs/gobernanza/politica-ia.md` (U8): 1 de 9 controles probado (`guardian.sh` bloqueó la lectura de `.env` por la shell el 2026-11-12); los otros 8 están PENDIENTE. Aporta riesgo, no cifras de ahorro.
- `docs/adopcion/plan-evolucion.md` (U9): Pedidos Web está en nivel 3 (Equipo con reglas): Claude Code en 86 de 107 PR (80 %) y `tasa_aceptacion` de 0,74 a 0,78 en 8 semanas (fuente: `docs/adopcion/analytics-ficticio.csv`). Ese CSV es otro período y no se cruza con `datos-piloto.csv`.

**Lo que veo en las semanas 7 y 8:** la curva de adopción: en esas dos semanas todo empeora frente al «antes» (3 fallas en 4 despliegues, 75 %; retrabajo de 15 y 14 h contra 12; 3 y 2 defectos; lead time de 7,0 y 6,5 días) y el ahorro reportado es bajo (3 y 4 h). La mejora aparece desde la semana 9, así que el promedio 7-12 incluye ese costo inicial.

## 2. Supuestos (etapa 2)

Tipos: **oficial** (publicado, con fecha) · **medido** (dato del piloto) · **estimado** (juicio del equipo, el más débil).

| Supuesto | Valor | Tipo | Fuente |
|---|---|---|---|
| Asiento | | | |
| Capacitación | | | |
| Curva de adopción | | | |
| Valor hora | | | |
| Horas ahorradas | | | |
| Defectos evitados | | | |

## 3. Costos

| Costo | Cálculo | USD |
|---|---|---|
| Asientos | | |
| Capacitación | | |
| Curva de adopción | | |
| **Total** | | |

## 4. Beneficios y escenarios

ROI = (beneficio − costo) / costo

| | Conservador | Optimista |
|---|---|---|
| Horas ahorradas × valor hora | | |
| Defectos evitados × costo en producción | | |
| **Beneficio total** | | |
| **ROI** | | |

## 5. Sensibilidad

**Supuesto que más pesa:** <cuál y por qué>

| Si… | ROI |
|---|---|
| El ahorro es la mitad del conservador | |

## 6. Punto de equilibrio (Tu reto)

<Escríbelo tú: qué despejas, el resultado y una línea para la gerencia.>

## 7. Limitaciones

- Los datos son ficticios y la muestra es pequeña: este ROI es un ejercicio de método.
- <otra limitación>
