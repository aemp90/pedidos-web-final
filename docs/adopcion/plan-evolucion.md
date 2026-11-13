# Plan de evolución de la IA para una Business Unit

> Plantilla de la Unidad 9. Complétala con Claude Code en la etapa 1 y revísala tú. En la etapa 3 la conviertes en PDF con tu Skill `pdf-con-estilo`.
> Fuente de datos: `docs/adopcion/analytics-ficticio.csv` (4 equipos × 8 semanas, datos ficticios). No uses cifras que no estén en el CSV.

**BU:** Desarrollo de la distribuidora · **Autor:** <tu nombre> · **Fecha:** <fecha>

## 1. Resumen para la gerencia

<3 a 5 líneas: dónde está hoy la BU, qué propones y qué se instala.>

## 2. Diagnóstico por equipo

Modelo de madurez del curso: 1 Uso a escondidas · 2 Uso individual · 3 Equipo con reglas · 4 Proceso AI-native · 5 Organización que mide y mejora.

| Equipo | Nivel | Evidencia 1 (indicador del CSV) | Evidencia 2 (indicador del CSV) |
|---|---|---|---|
| Pedidos Web | 3 · Equipo con reglas | Uso parejo y de todo el equipo: 5 a 6 `usuarios_activos` y 78 a 90 `sesiones` cada una de las 8 semanas (674 en total), sin semanas en cero. | `tasa_aceptacion` alta y estable (0,74 a 0,78) y Claude Code presente en 86 de 107 PR (`pr_con_cc`/`pr_totales` = 80 %). |
| Facturacion | 1 · Uso a escondidas | Uso marginal de una sola persona: `usuarios_activos` entre 0 y 1, 15 `sesiones` en 8 semanas y 2 semanas (2 y 5) sin ninguna sesión. | Hay código aceptado (200 `lineas_aceptadas`, `tasa_aceptacion` 0,55 a 0,65), pero casi no se declara en los PR: 1 de 71 (`pr_con_cc`/`pr_totales` = 1 %). |
| Bodega | 2 · Uso individual | Uso masivo: los 7 `usuarios_activos` todas las semanas y la mayor cantidad de `sesiones` de la BU (921; 105 a 124 por semana). | Ese uso entrega poco: `tasa_aceptacion` de 0,11 a 0,17, la más baja de la BU, y Claude Code en solo 30 de 109 PR (28 %). Cada persona lo usa a su manera, sin reglas comunes que mejoren las propuestas. |
| Reportes | 2 · Uso individual | Uso intermitente: las semanas impares tienen 3 a 4 `usuarios_activos` y 38 a 47 `sesiones`; las pares, 0 a 1 usuario y 0 a 6 sesiones (la semana 4, sin sesiones). | Cuando se usa, la `tasa_aceptacion` es media (0,50 a 0,58), y los PR con Claude Code (18 de 49, 37 %) solo aparecen en las semanas impares: en las pares `pr_con_cc` = 0. Depende de quién esté, no de un proceso. |

> Ningún indicador solo: mira el uso, la tasa de aceptación y los PR juntos. Una celda vacía en `tasa_aceptacion` significa que esa semana no hubo sesiones.

**Por qué no un nivel más alto (o más bajo):**

- **Pedidos Web no llega a 4 ni a 5.** 21 de sus 107 PR (20 %) todavía no usan Claude Code, así que no es aún un proceso AI-native completo, y su `tasa_aceptacion` no muestra una tendencia de mejora en las 8 semanas (0,76 la semana 1, 0,77 la semana 8): el CSV no evidencia que se esté midiendo y ajustando.
- **Bodega no está en 1.** Su uso es visible y de todo el equipo; el problema no es que se oculte, sino que rinde poco: 5.110 `lineas_aceptadas` con 921 sesiones, frente a 17.570 de Pedidos Web con 674.
- **Reportes no está en 3.** Tiene una tasa razonable, pero el uso cae casi a cero semana por medio; un equipo con reglas comunes no tendría ese vaivén.
- **Facturacion no está en 2.** El código aceptado existe, pero no queda registrado en los PR (1 de 71); eso es lo que define el uso a escondidas.

**Observación sobre los datos:** en la semana 8, Facturacion registra 0 `usuarios_activos` pero 1 sesión y 10 líneas aceptadas. Es una inconsistencia del CSV; no cambia el nivel asignado, pero conviene revisarla con quien genera el reporte.

## 3. Objetivo por equipo para el próximo trimestre

| Equipo | Objetivo medible | Indicador que lo muestra |
|---|---|---|
| | | |

## 4. El playbook

<Qué trae `pedidos-playbook` y para qué sirve cada pieza. Cómo lo instala otro equipo (los comandos están en el README del taller).>

## 5. Hoja de ruta a 6 y 12 meses

| Hito | Mes | Equipo | Champion | Cómo sabemos que se cumplió |
|---|---|---|---|---|
| | | | | |

<El ciclo: estandarizar → medir → ajustar → escalar.>

## 6. Indicadores que vamos a seguir

<Cuáles, cada cuánto y quién los revisa.>

## 7. Riesgos y próximos pasos

- <riesgo> → <cómo lo mitigamos>
