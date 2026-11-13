# _planificacion · Guía de la documentación del proyecto

Esta carpeta contiene todo lo que un agente de IA o una persona necesita para saber **qué se construye, por qué, en qué orden y qué se hizo**. No contiene código ni se publica.

> Si este archivo discrepa con `00_producto/registro-pendientes.md` sobre el estado del trabajo, prevalece el registro. Sobre las reglas de la carpeta, prevalece este archivo.

<!-- PLANTILLA: este archivo se escribe solo con los componentes activos del proyecto. Las secciones marcadas con un comentario PLANTILLA se borran completas si su componente no está activo (y se renumeran las que siguen dentro del mismo grupo; las secciones condicionales están al final de cada grupo para que los números que citan otros documentos no cambien); si está activo, se borra solo el comentario. Borra también este comentario. -->

---

## 1. Estructura

```text
_planificacion/
├── README.md                          Esta guía, sus reglas y las plantillas
├── 00_producto/
│   ├── producto.md                    Qué es el producto, para quién y qué no es
│   ├── registro-pendientes.md         Backlog y fuente única de verdad del trabajo pendiente
│   ├── decisiones.md                  Registro de Decisiones Arquitectónicas (RDA)
│   └── auditoria-tecnica.md           Auditorías acumulativas y sus hallazgos
├── 10_epicas/
│   └── epica-XX-nombre/
│       ├── definicion-epica-XX.md
│       ├── iteracion-XX-YY-descripcion.md
│       └── evidencia-XX-YY.md         Solo si hizo falta pedir datos de producción
├── 20_especificaciones/
│   └── spec-funcionalidad.md          Comportamiento esperado de cada funcionalidad
└── 99_bitacora/
    └── epica-XX-nombre/
        ├── bitacora-XX-YY.md
        └── bitacora-XX-YY-sufijo.md   Bitácoras adicionales de la misma iteración
```

## 2. Contenido de cada carpeta

### 2.1 `00_producto/`

Documentos vivos que describen el producto completo.

1. **`producto.md`:** propósito, qué es, qué no es, público, idioma y tono, indicadores de éxito e hitos. Cambia poco; se consulta ante dudas de alcance.
2. **`registro-pendientes.md`:** estado de épicas e iteraciones, backlog priorizado, datos pendientes del cliente, decisiones por tomar, propuestas fuera de alcance e historial. Es lo primero que se lee y lo último que se actualiza en cada trabajo.
3. **`decisiones.md`:** cada decisión técnica relevante como RDA numerada. `RDA-001` registra el nivel de proceso del proyecto.
4. **`auditoria-tecnica.md`:** auditorías acumulativas con sus hallazgos.

### 2.2 `10_epicas/`

Una subcarpeta por épica, con nombre descriptivo en ASCII (`epica-01-fundacion/`). Cada una contiene su definición, sus iteraciones y, solo si fue necesario, los archivos de evidencia de producción.

Si el proyecto ya tenía código cuando se adoptó esta documentación, existe `epica-00-linea-base/`: describe lo que ya estaba construido, está `Terminada` y no lleva iteraciones ni bitácoras. No se modifica; sirve de punto de partida para el backlog y la primera auditoría.

### 2.3 `99_bitacora/`

Una subcarpeta por épica, con el mismo nombre que la carpeta de la épica en `10_epicas/`. Dentro, las bitácoras de sus iteraciones.

<!-- PLANTILLA: la sección 2.4 va solo si el proyecto usa especificaciones (nivel mediano o complejo). -->

### 2.4 `20_especificaciones/`

Una especificación por funcionalidad que tiene lógica, reglas de negocio, varios caminos posibles o endpoints de API. Describe cómo debe comportarse el producto, no cuándo se construyó: varias iteraciones pueden implementar o cambiar la misma especificación.

<!-- PLANTILLA: la sección 2.5 va solo si el proyecto expone una API REST con contrato OpenAPI. -->

### 2.5 Fuera de esta carpeta: `docs/api/openapi.yaml`

El contrato técnico de la API REST: endpoints, métodos, parámetros, modelos de solicitud y respuesta, códigos HTTP, errores y seguridad. Vive fuera de `_planificacion/` porque también lo usan herramientas. Las especificaciones lo citan por `operationId` y no copian sus esquemas.

## 3. Convenciones

- **Numeración:** épicas `01`–`99`; iteraciones `XX-YY`, donde `XX` es la épica e `YY` el correlativo.
- **Nombres de archivo y carpeta:** minúsculas, guiones y solo caracteres ASCII, sin tildes ni ñ (`epica-02-sistema-diseno/`). El contenido sí lleva tildes y ñ.
- **Iteraciones:** `iteracion-XX-YY-descripcion-corta.md`.
- **Tamaño máximo de una iteración:** un solo objetivo, un resultado que el desarrollador revisa en unos 30 minutos y **8 tareas como máximo**. Una iteración que se pasa se parte en dos o más antes de empezar, cada una con su propio objetivo y sus criterios.
- **Detalle de las iteraciones:** al arrancar el proyecto se escriben en detalle solo las iteraciones de la primera épica; las de las demás épicas quedan nombradas en la tabla de su definición. Las iteraciones de cada épica siguiente se generan todas juntas al cerrar la anterior (sección 4.4).
- **Especificaciones:** `spec-nombre-de-la-funcionalidad.md`. Criterios de aceptación con ID corto (`CA-01`), que las iteraciones citan.
- **Bitácoras:** `bitacora-XX-YY.md` para la primera de la iteración; las siguientes llevan un sufijo descriptivo (`bitacora-XX-YY-correccion.md`, `bitacora-XX-YY-produccion.md`) y, si el sufijo se repite, un número (`bitacora-XX-YY-ajustes-2.md`). La fecha va dentro del archivo, no en el nombre.
- **Estados de trabajo:** `Pendiente`, `En curso`, `Bloqueada`, `En revisión`, `Terminada`, `Descartada`.
- **Estados de una especificación:** `Borrador`, `Aprobada`, `Vigente`, `Reemplazada`.
- **Fechas:** formato ISO `AAAA-MM-DD`, {{ZONA_HORARIA}}.
- **Idioma:** {{IDIOMA_Y_VARIANTE}} en toda la documentación.
- **Ramas:** `iteracion/XX-YY-descripcion-corta`, o `correccion/descripcion-corta` para una corrección fuera de iteración. Su creación requiere aprobación del desarrollador.
- **Commits:** `Épica N - Iteración NN-NN: descripción`, o `Corrección: descripción` fuera de iteración; una línea de máximo 200 caracteres. Los hace solo el desarrollador.
- **Marcadores de datos faltantes:** `PENDIENTE_CLIENTE` (datos del negocio) y `PENDIENTE_DESARROLLADOR` (decisiones técnicas o de diseño). Todo marcador se registra en `registro-pendientes.md`.

## 4. Ciclos de trabajo

### 4.1 Una iteración

Una iteración es una unidad de trabajo que se puede revisar de una vez: una sesión, un hito o un bloque funcional. No tiene duración fija.

1. El desarrollador marca la iteración como `En curso` en `registro-pendientes.md` (o pide al agente que lo haga).
2. El agente lee `AGENTS.md`, el registro, la iteración, su especificación, el contrato si toca la API y las RDA relacionadas.
3. El agente comprueba la **definición de preparada** de `AGENTS.md`: objetivo claro, criterios verificables, especificación `Aprobada` si la tiene, dependencias terminadas, datos del cliente disponibles y tamaño dentro del máximo. Si algo falla, no empieza: avisa qué falta y propone cómo resolverlo (pedir la aprobación, registrar el dato pendiente, partir la iteración).
4. El agente ejecuta **la iteración completa**: sigue el plan técnico y las tareas, implementa, verifica según la definición de terminado de `AGENTS.md`, incluso contra producción si tiene cómo, y escribe la bitácora con su revisión.
5. Solo si no pudo obtener un dato de producción después de intentarlo por todos los medios, crea un archivo de evidencia (sección 5.4).
6. El agente actualiza el registro (estado, backlog, historial) y, si corresponde, la especificación (a `Vigente` cuando se cumplen todos sus criterios), las RDA y las auditorías.
7. El agente deja la iteración en `En revisión` y propone el mensaje de commit.
8. Si es la última iteración de su épica, el agente sigue con el cierre de la épica y la propuesta de la siguiente (sección 4.4), en la misma entrega.
9. El desarrollador revisa, hace commit y push, y marca la iteración como `Terminada`.

### 4.2 Una funcionalidad nueva

1. Se agrega al backlog con su prioridad.
2. Cuando se acerca su turno, si la funcionalidad lo justifica, el agente redacta su especificación en `Borrador` y, si expone endpoints, los agrega al contrato. Si no lo justifica, basta con el objetivo y los criterios de la iteración.
3. El desarrollador aprueba la especificación (`Aprobada`).
4. Se crea la iteración que la implementa, con plan técnico, tareas y los criterios que cita.
5. Se sigue el ciclo de la sección 4.1.

### 4.3 Un cambio o una corrección

1. **Entender** el pedido. Si es un error, reproducirlo antes de corregirlo.
2. **Identificar** las funcionalidades, componentes y documentos afectados.
3. **Clasificar** con una pregunta: ¿después del cambio, la especificación, el contrato y las RDA siguen describiendo bien el sistema?
   - **Menor (sí):** el código no cumplía lo especificado, un texto, un ajuste dentro de `DESIGN.md`. No se toca la especificación. Fila de tipo `Corrección` en el backlog, implementación, verificación contra los criterios existentes y una línea de historial. Bitácora breve solo si hubo mediciones o decisiones que conviene conservar.
   - **Significativo (no):** cambia el comportamiento esperado, una regla de negocio, el contrato, el modelo de datos o la arquitectura. Primero se actualiza la especificación (vuelve a `Borrador` hasta que el desarrollador la aprueba) o el contrato, o se propone una RDA; si afecta un documento protegido, se entrega la propuesta al desarrollador. Después se trabaja como una iteración, con plan, tareas y bitácora.
   - **Ante la duda,** se pregunta antes de tocar el código.
4. **Actualizar** solo los documentos afectados.
5. **Implementar, verificar y registrar.** Una corrección que no pertenece a la iteración activa usa la rama `correccion/descripcion-corta` y el commit `Corrección: descripción`.

### 4.4 Cierre de una épica y planificación de la siguiente

Se activa en dos casos: como paso final de la última iteración de una épica, o cuando el desarrollador lo pide («planifica la siguiente épica», o el comando `/siguiente-epica` en Claude Code). Si la épica todavía tiene iteraciones sin terminar, el agente lo dice y no la cierra.

1. **Revisión al cierre.** El agente completa esa sección en la definición de la épica que termina: si se cumplió el criterio de término, qué se aprendió y qué cambia en el backlog o en las épicas siguientes. Si cambia algo, lo aplica en el registro en la misma pasada. Si lo aprendido justifica cambiar un documento protegido, entrega la propuesta.
2. **Lectura.** Lee la definición de la épica siguiente, el backlog, las especificaciones relacionadas y las RDA vigentes.
3. **Propuesta.** Presenta en el chat las iteraciones de la épica siguiente: número, nombre y objetivo en una línea, elementos del backlog que cubre, y qué especificación o dato del cliente necesita cada una. Toda iteración respeta el tamaño máximo (sección 3); la que no cabe ya viene partida en la propuesta.
4. **Iteraciones que ya existen.** Si la épica siguiente ya tiene iteraciones escritas en detalle, no se rehacen: el agente las revisa contra la revisión al cierre y propone ajustes solo si algo cambió. Si nada cambió, lo dice.
5. **Aprobación.** El agente espera la aprobación del desarrollador. Si este ajusta la lista, el agente la corrige.
6. **Generación.** Con la aprobación, el agente genera de una vez todos los archivos de iteración de la épica siguiente con la plantilla de la sección 6.1, y en la misma pasada actualiza la tabla de iteraciones de la definición de la épica, la tabla de estado y el backlog de `registro-pendientes.md` (iteración asignada a cada elemento) y una línea de historial.
7. **Preparación.** El agente revisa cada iteración generada contra la definición de preparada de `AGENTS.md`. Lo que falta por causas externas (una especificación por aprobar, un dato del cliente) se registra en «Decisiones por tomar» o «Datos pendientes del cliente», y la iteración queda `Pendiente` hasta que se resuelva.

Si no hay épica siguiente, el agente revisa el backlog: si quedan elementos comprometidos, propone una épica nueva con su definición; si no, lo dice para que el desarrollador cierre el proyecto o lo pase a mantenimiento.

## 5. Reglas de mantenimiento

### 5.1 Registro de pendientes y backlog

- El backlog dice **qué** falta y en qué orden; la tabla de estado dice **cómo va** cada iteración. Un elemento asignado a una iteración muestra su número, y al cerrar la iteración se actualizan en la misma pasada su fila y las del backlog que cubrió.
- Los elementos del backlog no llevan IDs, puntos ni estimaciones. Los terminados se quedan al final de la tabla con estado `Terminada`.
- Una propuesta fuera de alcance que se aprueba se mueve al backlog, con una fila de historial.
- El historial lleva **una fila por cambio y una sola línea**, con enlace a la bitácora que tiene el detalle. Las mediciones, decisiones y salvedades van en la bitácora, no en el historial.
- Nada se borra. Si un cambio anterior queda sin efecto, se agrega una fila nueva que lo dice.
- La tabla de estado enlaza todas las bitácoras de cada iteración.

### 5.2 Decisiones (RDA)

- Numeración correlativa: `RDA-001`, `RDA-002`… Antes de crear una, revisa el último número usado.
- Una RDA aceptada no se contradice: se reemplaza con una nueva que la declare «Reemplazada por RDA-XXX» (o «Reemplazada parcialmente»), y la antigua recibe una línea que remite a la nueva.
- Las actualizaciones van al final de la RDA como «**Actualización AAAA-MM-DD:** …», breves. Si una RDA acumula varias actualizaciones de fondo, se reemplaza con una nueva.
- La tabla índice del inicio del archivo se mantiene al día.
- Un cambio de nivel de proceso se registra como una RDA nueva que reemplaza a `RDA-001` (o a la que la haya reemplazado).

### 5.3 Bitácoras

- Se escriben al terminar cada tramo de trabajo, con la plantilla de la sección 6.2.
- No se editan después de entregadas: las correcciones van en una bitácora nueva con sufijo.
- Registran mediciones reales. «Verificado» sin decir cómo y dónde no cuenta.
- Una casilla de criterio se marca solo si se midió.
- Terminan con una revisión breve: resultado frente al objetivo, qué se aprendió y qué cambia para lo que sigue. Si cambia algo (el backlog, una especificación, el orden de las iteraciones), se aplica en la misma pasada.

### 5.4 Evidencia de producción

**Por defecto, una iteración se ejecuta completa y sin fases.** El agente resuelve todo por su cuenta, incluidas las verificaciones en producción que pueda hacer con las herramientas que tiene permitidas.

Pedir datos al desarrollador es **la excepción**, porque cada pedido le suma trabajo. Solo corresponde cuando se cumplen las dos condiciones:

1. El trabajo exige contrastar lo hecho en local con información de producción (un panel de un servicio externo, una medición en un dispositivo real, una cuenta a la que el agente no tiene acceso).
2. El agente intentó obtener esa información por todos los medios a su alcance y no pudo.

En ese caso, el agente:

1. Termina y registra en la bitácora todo lo que sí pudo hacer.
2. Crea `10_epicas/<epica>/evidencia-XX-YY.md` con la plantilla de la sección 6.3, que incluye **qué intentó y por qué no pudo**. Sin esa sección el pedido no es válido.
3. Pide solo los datos que faltan, con pasos exactos y el menor esfuerzo posible para el desarrollador.
4. Deja la iteración en `En revisión` y avisa.
5. Cuando el desarrollador responde en el archivo de evidencia, consolida en una bitácora nueva (`bitacora-XX-YY-produccion.md`).

<!-- PLANTILLA: la sección 5.5 va si el proyecto tiene auditoria-tecnica.md. -->

### 5.5 Auditorías

- Las auditorías se agregan al final y nunca se borran ni se reescriben.
- **Antes de agregar una auditoría, busca el último número usado en el archivo y suma uno.** Los IDs de hallazgo tienen el formato `AUD-NN-MMM` (auditoría y correlativo) y nunca se reutilizan.
- Un hallazgo se cierra indicando dónde: «Resuelto en XX-YY» o «Resuelto en RDA-XXX», con la evidencia mínima. «Riesgo aceptado» solo si el desarrollador lo aceptó y consta dónde.
- Severidad: Alta (bloquea la publicación o afecta al usuario o al negocio), Media (afecta calidad o rendimiento), Baja (mejora).

<!-- PLANTILLA: la sección 5.6 va solo si el proyecto usa especificaciones. -->

### 5.6 Especificaciones

- Nacen en `Borrador`. El agente puede redactarlas; el desarrollador las aprueba. Nada se implementa desde una especificación en `Borrador`.
- Pasan a `Vigente` cuando la implementación cumple todos sus criterios, y vuelven a `Borrador` si un cambio significativo las modifica.
- Describen el comportamiento actual, sin tabla de historial: la trazabilidad está en las bitácoras y en el registro.
- Las secciones que no aportan se borran. Lo mínimo es problema, comportamiento esperado y criterios de aceptación.
- Una funcionalidad rehecha de raíz deja la especificación antigua en `Reemplazada`, con enlace a la nueva.

<!-- PLANTILLA: la sección 5.7 va solo si el proyecto expone una API REST con contrato OpenAPI. -->

### 5.7 Contrato de la API

- El contrato cambia antes que el código. Si una tarea cambia una ruta, primero se actualiza `docs/api/openapi.yaml` y después se implementa.
- Cada endpoint nuevo o modificado se ejercita contra sus criterios con peticiones reproducibles: el caso exitoso, al menos un error de validación y el acceso sin autorización si la ruta está protegida. La bitácora registra cuáles se ejecutaron y su resultado.
- Si el contrato y el código no coinciden, el agente se detiene y pregunta cuál de los dos está bien.

## 6. Plantillas

### 6.1 Iteración

```markdown
# Iteración XX-YY · Nombre

- **Épica:** XX · Nombre
- **Estado:** Pendiente
- **Rama sugerida:** iteracion/XX-YY-descripcion-corta
- **Depende de:** XX-YY
- **Especificación:** spec-funcionalidad.md (CA-01, CA-02) o —
- **RDA relacionadas:** RDA-XXX
- **Hallazgos que cierra:** AUD-NN-MMM

## Objetivo

Una o dos frases: qué deja funcionando la iteración.

## Alcance y entregables

- …

## Plan técnico

Solo si la iteración toca arquitectura, datos, API o integraciones; si no, borra la sección.
Componentes que se tocan, cambios en el modelo de datos o el contrato, decisiones de implementación y riesgos.

## Tareas

1. …

## Criterios de aceptación

- [ ] CA-01 de la especificación (se cita, no se copia)
- [ ] … (criterio técnico propio, verificable con una medición o un comando)

## Fuera de alcance

- …

## Datos requeridos del cliente

- …
```

### 6.2 Bitácora

```markdown
# Bitácora XX-YY · Nombre de la iteración

- **Fecha:** AAAA-MM-DD
- **Agente:** (herramienta y modelo)
- **Rama:** …
- **Estado final:** En revisión | Bloqueada

## Resumen

Qué se hizo, en tres a cinco líneas.

## Archivos creados o modificados

| Archivo | Cambio |
| :------ | :----- |

## Verificación

- (comando o medición): resultado
- Revisión en móvil y escritorio, o pruebas de la API: …

## Criterios de aceptación

- [x] … (con la medición que lo respalda)

## Decisiones tomadas

- … (si es estructural, enlaza la RDA nueva)

## Pendientes y riesgos

- …

## Revisión

- **Resultado frente al objetivo:** …
- **Qué aprendimos:** …
- **Qué cambia para lo que sigue:** … (backlog, especificaciones u orden de las iteraciones; o «nada»)

## Commit sugerido

`Épica N - Iteración NN-NN: descripción`
```

### 6.3 Evidencia de producción

```markdown
# Evidencia XX-YY · Nombre de la iteración

- **Fecha del pedido:** AAAA-MM-DD
- **Pedido por:** (agente)
- **Criterios que dependen de esta evidencia:** …

> Este archivo existe solo porque el agente no pudo obtener estos datos por su cuenta. Responde solo lo que se pide.

## Qué intentó el agente

| Dato | Qué intentó | Por qué no pudo |
| :--- | :---------- | :-------------- |

## Qué necesito del desarrollador

### 1. (dato)

- **Para qué:** criterio o hallazgo que cierra.
- **Cómo obtenerlo:** pasos exactos.
- **Respuesta:**

## Lo que no se pudo verificar

- …
```

### 6.4 RDA

```markdown
## RDA-XXX · Título

- **Fecha:** AAAA-MM-DD
- **Estado:** Propuesta | Aceptada | Reemplazada por RDA-XXX | Descartada

**Contexto:** qué problema o fuerza obliga a decidir.
**Decisión:** qué se hace.
**Alternativas consideradas:** qué se descartó y por qué.
**Consecuencias:** qué se gana, qué se pierde y qué hay que vigilar.
```

<!-- PLANTILLA: la sección 6.5 va si el proyecto tiene auditoria-tecnica.md. -->

### 6.5 Hallazgo de auditoría

```markdown
| ID | Severidad | Área | Hallazgo | Acción recomendada | Estado |
| :- | :-------- | :--- | :------- | :----------------- | :----- |
| AUD-NN-MMM | Alta / Media / Baja | … | … | … | Abierto / Resuelto en XX-YY o RDA-XXX |
```

<!-- PLANTILLA: la sección 6.6 va solo si el proyecto usa especificaciones. -->

### 6.6 Especificación

```markdown
# Especificación · Nombre de la funcionalidad

- **Estado:** Borrador | Aprobada | Vigente | Reemplazada por spec-xxx.md
- **Última revisión:** AAAA-MM-DD
- **Iteraciones:** XX-YY, XX-YY

## Problema o necesidad

## Comportamiento esperado

## Requisitos funcionales

1. …

## Reglas de negocio

1. …

## Escenarios

- **Dado** … **cuando** … **entonces** …

## Criterios de aceptación

- **CA-01:** … (verificable)

## Restricciones técnicas

## Contrato de API

Operaciones de `docs/api/openapi.yaml`: `operationId`, `operationId`.

## Fuera de alcance

## Preguntas abiertas

Lo mínimo es problema, comportamiento esperado y criterios de aceptación; borra las secciones que no aporten.
```
