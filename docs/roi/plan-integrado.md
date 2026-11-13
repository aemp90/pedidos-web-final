# Plan integrado de adopción de IA · Pedidos Web

**Para:** Gerencia general · **Autor:** Andres · **Fecha:** 2026-11-13

## Página 1 · Informe ejecutivo

### 1. Problema
Hay que decidir si el equipo Pedidos Web (6 devs) sigue trabajando con IA durante los próximos 12 meses. El piloto de 12 semanas terminó y ya hay datos para decidir con números y no por intuición.

### 2. Qué se hizo
Durante 6 semanas el equipo trabajó como siempre y durante otras 6 trabajó con un asistente de IA (Claude Code). Se compararon las mismas mediciones en ambos períodos.

### 3. Resultados
- **Más entregas y más rápidas:** de 2,0 a 3,0 despliegues por semana; de 6,6 a 5,2 días desde el cambio hasta producción.
- **Menos errores:** los despliegues con falla bajaron de 41,7 % a 33,3 %, y los defectos que llegan a producción, de 2,0 a 1,5 por semana.
- **ROI a 12 meses: entre 183 % y 318 %**, sobre un costo de USD 3.900 (asientos, capacitación y curva de aprendizaje).

Lo que más mueve el ROI son las horas ahorradas: las reporta el propio equipo y son el 74 % del beneficio. Con la mitad de ese ahorro, el ROI sigue en 78 %. El costo se cubre si cada dev ahorra 2,7 h al mes; el piloto muestra 5,7.

### 4. Recomendación
Aprobar Claude Team para los 6 devs de Pedidos Web por 12 meses, con un costo total de USD 3.900.

### 5. Próximo paso
Andres, junto con RR. HH., valida el valor hora y las horas reales de capacitación (hoy son estimados) antes del 2026-12-15, y presenta a la gerencia el modelo actualizado y la medición de horas ahorradas a los 3 meses.

*Preparado con apoyo de IA; cifras verificadas por Andres.*

---

## Anexo

No repite los entregables: los cita.

### Roadmap
- **3 meses:** reemplazar los supuestos estimados por datos reales (valor hora, capacitación, costo de un defecto) y comprobar que las horas ahorradas siguen sobre el punto de equilibrio.
- **6 meses:** con el modelo confirmado, ofrecer el playbook de Pedidos Web a un segundo equipo.
- **12 meses:** repetir el modelo de ROI con los datos del año y decidir la renovación.

FALTA: la hoja de ruta con hitos, equipos y champions de `plan-evolucion.md` no está completa. · Respaldo: `docs/adopcion/plan-evolucion.md` (U9)

### Indicadores
Se siguen las mismas mediciones del piloto, cada mes, con estas metas:

| Indicador | Hoy (con IA) | Meta |
|---|---|---|
| Horas ahorradas por dev al mes | 5,67 | No bajar de 2,7 (punto de equilibrio) |
| Despliegues por semana | 3,0 | 3,0 o más |
| Lead time (días) | 5,2 | 5,2 o menos |
| Change fail rate | 33,3 % | 33,3 % o menos |
| Defectos escapados por semana | 1,5 | 1,5 o menos |

Cada cambio pasa por tests comprobados a mano y por el checklist de revisión antes del merge, que exige la aprobación de una persona además de la revisión de la IA. · Respaldo: `docs/calidad/registro-u6.md` (U6) · `docs/checklist-revision.md` (U7)

### Gobernanza
Se llevan a los otros equipos la política de datos que nunca entran a la IA (secretos, cadenas de conexión, datos reales de clientes), las reglas de `.claude/settings.json` y el hook `guardian.sh`. Hoy solo 1 de 9 controles está probado; los otros 8 deben probarse antes de escalar. · Respaldo: `docs/gobernanza/politica-ia.md` (U8)

### ROI
- **Supuestos clave:** asiento de USD 25 por dev al mes (oficial); horas ahorradas y defectos evitados (medidos en el piloto); valor hora de USD 20, 16 h de capacitación por dev y USD 120 por defecto (estimados).
- **Costo:** USD 3.900 en 12 meses (1.800 en asientos, 1.920 en capacitación, 180 de curva de adopción).
- **Escenarios:** conservador 183 % (beneficio de USD 11.040); optimista 318 % (USD 16.320).
- **Sensibilidad:** con el ahorro a la mitad, 78 %; con el ahorro y los defectos evitados a la mitad, 42 %; con un valor hora de USD 15, 145 %.

Además, la migración corrigió 2 defectos del sistema legado. · Respaldo: `docs/roi/modelo-roi.md` · `docs/migracion/defectos-corregidos.md` (U5)

### Punto de equilibrio
Es donde el beneficio apenas cubre el costo: ROI = 0.

- **Qué se despeja:** las horas ahorradas por dev al mes. **Fijos:** costo de USD 3.900, valor hora de USD 20, 6 devs y 12 meses. Los defectos evitados no se cuentan.
- **Despeje:** 0 = (H × 20 − 3.900) / 3.900 ⇒ H × 20 = 3.900 ⇒ H = 195 h al año para el equipo ⇒ 195 / 6 / 12 = **2,7 h por dev al mes**.
- **Contra el piloto:** se midieron 5,67 h por dev al mes, unas 2 veces el equilibrio.

**Para la gerencia:** basta que cada dev ahorre 2,7 h al mes para cubrir el costo; el piloto muestra 5,7. · Respaldo: `docs/roi/modelo-roi.md`, sección 6

### Evolución
Nivel 3 (Equipo con reglas) → meta: nivel 4 (Proceso AI-native), con la IA en todos los cambios del equipo y las mediciones de este plan revisadas cada mes. · Respaldo: `docs/adopcion/plan-evolucion.md` (U9)
