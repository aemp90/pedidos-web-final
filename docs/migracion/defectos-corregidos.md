# Defectos corregidos · migración del cálculo de totales

<!--
Una sección por defecto que corriges y una por cada hallazgo que NO corriges.
Regla: si un valor esperado de migracion/golden-master.csv cambia, ese caso aparece aquí.
Borra estos comentarios al terminar.
-->

## D1 · <id de la regla> · <nombre corto> (CORREGIDO)

- **Qué hace el legado:** <comportamiento> (`legado/src/<archivo>.pas:<línea>`)
- **Corrección:** <qué hace ahora el C#>
- **Por qué:** <impacto para el cliente o el negocio>
- **Casos del golden master que cambian:** <cuántos y cuáles>
- **Suma de la columna total:** <antes> → <después>
- **Commit:** <mensaje del commit, separado del «igual al legado»>
- **Convivencia:** <qué debe conciliar el negocio mientras conviven ambos sistemas>

## H1 · <nombre corto> (HALLAZGO, NO CORREGIDO)

- **Qué encontraste:** <diferencia> (`archivo:línea` de cada lado)
- **Efecto:** <qué pedidos y cuánto>
- **Quién decide:** <área del negocio>. El C# replica <qué versión>.
