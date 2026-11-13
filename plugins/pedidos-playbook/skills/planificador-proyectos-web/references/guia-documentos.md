# Guía documento por documento

Qué lleva cada archivo, qué no lleva y por qué. Las plantillas correspondientes están en `assets/plantillas/`.

## Índice

1. AGENTS.md
2. DESIGN.md
3. README.md (raíz)
4. .claude/settings.json y .claude/README.md
5. _planificacion/README.md
6. 00_producto/producto.md
7. 00_producto/registro-pendientes.md
8. 00_producto/decisiones.md
9. 00_producto/auditoria-tecnica.md
10. Épicas: definicion-epica-XX.md
11. Iteraciones: iteracion-XX-YY-descripcion.md
12. Bitácoras
13. Evidencia de producción y fases
14. Especificaciones: spec-funcionalidad.md
15. Contrato de la API: docs/api/openapi.yaml
16. Cambios y correcciones: qué documentos se tocan
17. Proyecto con código existente: épica 00 y auditoría inicial

---

## 1. AGENTS.md

Es la entrada principal para cualquier agente de IA (Claude Code lo lee de forma nativa desde su versión 2.1.277; también Antigravity y Codex). No se genera `CLAUDE.md`.

Ojo con una trampa de Claude Code: si en la carpeta del proyecto o más arriba existe un `CLAUDE.md` o un `CLAUDE.local.md`, en el modo por defecto Claude Code lee ese archivo y **deja de cargar `AGENTS.md`**. Por eso `.claude/README.md` lo advierte, y quien necesite instrucciones personales debe importar las reglas del proyecto con una primera línea `@AGENTS.md` en su `CLAUDE.local.md`.

**Lleva, en este orden:**

1. **Contexto en 30 segundos**: cliente (o «proyecto propio»), objetivo, alcance, desarrollador responsable. Corto: el agente lo lee en cada sesión.
2. **Documentos protegidos**: `AGENTS.md`, `DESIGN.md`, `README.md` y `_planificacion/README.md` no se editan. Si el agente cree que conviene cambiarlos, entrega al desarrollador una propuesta con qué cambiaría, dónde y por qué, y él decide y edita. Esto va arriba y explícito, porque un agente que «mejora» las reglas por su cuenta deja de seguirlas.
3. **Orden de lectura obligatorio**: `registro-pendientes.md` (backlog, qué está pendiente y cuál es la iteración activa), la iteración activa, su especificación y el contrato OpenAPI si los tiene, `decisiones.md`, `DESIGN.md`, `README.md` (raíz), `_planificacion/README.md` y el resto de `_planificacion/` según la tarea. Si algo está en conflicto, el agente se detiene y pregunta.
4. **Stack y versiones**, con las restricciones que no se deducen de los archivos (por ejemplo «no actualices TypeScript a la 7: rompe `astro check`»).
5. **Comandos**: aquí y solo aquí. El README raíz no lleva comandos.
6. **Límites de actuación** en tres grupos: prohibido, requiere consulta, permitido sin consulta. Coherentes con `.claude/settings.json`.
7. **Reglas de contenido**: idioma, no inventar datos ni resultados, marcadores de pendientes.
8. **Reglas técnicas** del tipo de proyecto (ver `tipos-proyecto.md`).
9. **Convenciones de la API**, solo en backend y fullstack: reglas generales y dónde está el contrato, no el detalle de cada ruta.
10. **Flujo de trabajo por iteración**: leer, comprobar que la especificación esté aprobada, proponer rama, implementar solo lo de los criterios, verificar, escribir la bitácora con su revisión, actualizar el registro, entregar el resumen y el commit sugerido. Incluye la regla de autonomía de la sección 13 de esta guía y, como último paso, que la última iteración de una épica termina con el cierre de la épica y la propuesta de iteraciones de la siguiente (sección 10 de esta guía).
11. **Cambios y correcciones**: el flujo corto de la sección 16 de esta guía.
12. **Definición de preparada y de terminado.** La **preparada** es la lista de chequeo antes de empezar una iteración: objetivo en una o dos frases, criterios verificables, especificación `Aprobada` si la tiene, dependencias terminadas, datos del cliente disponibles (o acuerdo de trabajar con el marcador) y tamaño dentro del máximo (sección 11 de esta guía). Si falla una condición, el agente no empieza y dice qué falta. La **de terminado** son tres a cinco condiciones que valen para todo trabajo (compilación si aplica, la aplicación se ejecuta, criterios comprobados con evidencia, documentos afectados al día, bitácora o línea de historial), más los comandos que el desarrollador declaró obligatorios. Aparte, en una lista separada, las verificaciones **recomendadas**: se escriben como recomendación y el agente no las presenta como requisito. No se confunde con los criterios de aceptación, que son de cada funcionalidad o iteración.
13. **Cuándo detenerse y preguntar**, incluido cuando una tarea introduce un factor que cambia la complejidad del proyecto.

**No lleva:** historial, fechas, iteraciones ni RDA como justificación (es inmutable). Las excepciones a una regla se escriben como parte de la regla, con su razón técnica. Conviene que no pase de unas 200 líneas: es lo que el agente carga en cada sesión.

## 2. DESIGN.md

Fuente única de verdad del diseño. Solo en frontend y fullstack.

**Lleva:** concepto visual y principios; color (tokens, uso y tabla de contrastes medidos, con la regla práctica que sale de ella); tipografía y escala; espaciado, retícula y forma; componentes con sus reglas; botones; iconografía; imágenes; movimiento; implementación de los tokens en el stack; voz y textos.

**Reglas al escribirlo:**

- Valores concretos y medibles (hex, px, rem, ratios), no adjetivos. «Contraste suficiente» no le sirve a un agente; «6,09:1, usar en botones» sí.
- Cada excepción vive dentro de la regla a la que pertenece, con su razón técnica, sin fecha ni iteración. En vez de «Excepciones (iteración 04-04, decisiones del 2026-10-02): …», escribe en la sección de botones: «Los botones de la barra móvil llegan a los bordes de la pantalla: su anillo de foco se dibuja hacia adentro».
- Si el diseño no está definido, no lo inventes: propónlo en el chat, espera aprobación y recién entonces escríbelo. Lo que quede sin decidir va con `PENDIENTE_DESARROLLADOR` en su sección y se registra en `registro-pendientes.md`.
- Al optimizar un `DESIGN.md` existente: consolida, elimina rastros de tiempo, quita duplicados con `AGENTS.md` (las reglas técnicas generales van allá; las de diseño, aquí) y explica en el chat qué cambió.
- **Optimizar no es completar.** Las secciones de la plantilla que el documento original no cubría quedan con `PENDIENTE_DESARROLLADOR`, no con valores que parezcan razonables. Sugerencias, solo en el chat. Si un valor existente rompe una regla (contraste, objetivos táctiles), se escribe la restricción que lo neutraliza, se mide y documenta el problema en la tabla de contraste o en la auditoría, y el cambio de valor queda como decisión pendiente del desarrollador.
- Recorre al final el documento optimizado y compara cada valor concreto (hex, px, nombres de familias, librerías, umbrales) contra el original: todo valor nuevo que no venga del original, del desarrollador o de una norma citada sobra.
- **En nivel pequeño**, versión breve: concepto, color con su tabla de contraste, tipografía, componentes clave con sus reglas y voz. Las secciones que el proyecto no necesita (escala tipográfica detallada, movimiento, iconografía) se omiten en vez de quedar con `PENDIENTE_DESARROLLADOR`; si después hacen falta, el desarrollador las agrega.

## 3. README.md (raíz)

La vitrina del repositorio en GitHub. Sus lectores llegan sin contexto (personas que evalúan tu trabajo, colegas, estudiantes), así que se explica de forma conceptual y ordenada.

**Lleva:** qué es el proyecto y para quién; qué problema resuelve; secciones o funciones principales; stack con una frase por pieza explicando para qué sirve; arquitectura explicada en palabras; estructura del repositorio (se permite un árbol de carpetas en un bloque `text`, porque no es código ni comandos); cómo se despliega, en concepto; metodología de trabajo (backlog, épicas, iteraciones, especificaciones y contrato de la API si el proyecto los usa, RDA, bitácoras, agentes de IA y quién hace los commits); autoría y licencia (en un proyecto propio, sin cliente).

**No lleva:** código fuente, comandos (ni de instalación ni de uso), historial, fechas, iteraciones ni estados («aún no existe», «se publica en…»). Si un estudiante quiere ejecutar el proyecto, el README le dice en palabras qué necesita (por ejemplo «Node.js 24 y PNPM») y que los comandos están en `AGENTS.md`.

**Por qué**: un README con comandos se desactualiza en cuanto cambia el gestor de paquetes, y el que tiene fechas parece abandonado a los seis meses. El README conceptual dura lo que dura el proyecto.

## 4. .claude/settings.json y .claude/README.md

**`settings.json`**: se arma con `scripts/combinar_permisos.py` a partir de los fragmentos por stack de `assets/permisos/` (ver `stacks.md`, sección 6) y después se ajusta al proyecto. JSON limpio, sin comentarios (JSON no los admite y el archivo dejaría de cargarse) y sin claves inventadas para comentar (generan advertencias de esquema). Se ordena en tres bloques, `allow`, `ask` y `deny`, y dentro de cada uno se agrupan las reglas por tema en un orden fijo: comandos del proyecto, Git, sistema de archivos, edición, lectura, web. Los cuatro documentos inmutables van en `deny` de edición, no en `ask`, porque el desarrollador decidió que ningún agente los edite.

**`.claude/commands/siguiente-epica.md`**: el comando `/siguiente-epica` de Claude Code. Es un atajo corto que remite a la sección 4.4 de `_planificacion/README.md`; el flujo completo vive allá para que los agentes sin este atajo hagan lo mismo. Se copia tal cual de la plantilla.

**`.claude/README.md`**: explica cada bloque y cada grupo de reglas en una tabla «regla · por qué», cómo se resuelven los conflictos (`deny` gana sobre `ask` y `ask` sobre `allow`) y que el archivo `settings.local.json`, si existe, es personal y no se versiona. Este README también es documentación del desarrollador: el agente no lo edita (está dentro de `.claude/`).

## 5. _planificacion/README.md

Guía de la carpeta, inmutable. Lleva: para qué sirve la carpeta, estructura, contenido de cada subcarpeta, convenciones (numeración, nombres en ASCII, estados, fechas, idioma, ramas, commits), los ciclos de trabajo (iteración, funcionalidad nueva, cambio o corrección), reglas de mantenimiento de los documentos vivos (secciones 7 a 16 de esta guía) y las plantillas de iteración, bitácora, evidencia, RDA, hallazgo y especificación.

Las reglas de mantenimiento van escritas aquí porque los agentes que trabajan en el proyecto no tienen esta skill: lo que no quede en el repositorio no lo van a seguir.

**Se escribe solo con los componentes activos.** Las secciones de la plantilla que dependen del nivel (especificaciones, auditorías, contrato de la API) llevan un comentario `<!-- PLANTILLA: … -->`: si el componente no está activo, se borra la sección completa y se renumera; si lo está, se borra solo el comentario. Un agente que lee reglas de componentes que el proyecto no usa intenta aplicarlas.

## 6. 00_producto/producto.md

Documento vivo que cambia poco. Lleva: datos del documento (cliente, desarrollador, documento base, última revisión), propósito, qué es, qué no es, público objetivo (tabla perfil · situación · qué necesita), condiciones de uso, idioma y tono, indicadores de éxito e hitos comprometidos. «Qué no es» es tan importante como «qué es»: es la sección que el agente consulta cuando una tarea huele a fuera de alcance.

## 7. 00_producto/registro-pendientes.md

Fuente única de verdad del trabajo pendiente: si discrepa con cualquier otro documento de planificación, prevalece. Lleva: encabezado con última actualización, iteración activa y próximo hito; estado de épicas e iteraciones (tabla con enlace a la bitácora); **backlog**; datos pendientes del cliente; decisiones por tomar; propuestas fuera de alcance; historial.

**Backlog:** una tabla ordenada por prioridad con las funcionalidades, mejoras, correcciones y trabajo técnico relevante que el proyecto ya comprometió. Columnas: prioridad (`Alta`, `Media`, `Baja`), elemento (una frase), tipo (`Funcionalidad`, `Mejora`, `Corrección`, `Técnico`), especificación (enlace, si tiene), iteración (si ya está asignada) y estado. No lleva IDs, historias de usuario, puntos ni estimaciones: si un proyecto complejo necesita dependencias entre elementos, se agrega una columna «Depende de» y nada más.

Para que no haya dos fuentes de verdad:

- El backlog dice **qué** falta y en qué orden; la tabla de estado dice **cómo va** cada iteración. Un elemento asignado a una iteración muestra su número, y al cerrar la iteración el agente actualiza en la misma pasada la fila de la iteración y las filas del backlog que cubrió.
- Los elementos terminados se quedan en el backlog con estado `Terminada`, al final de la tabla, para ver lo completado sin buscar en las bitácoras.
- «Propuestas fuera de alcance» es otra cosa: ideas que nadie comprometió. Si el desarrollador (o el cliente) aprueba una, se mueve al backlog y se borra de las propuestas con una fila de historial.
- En nivel pequeño, el backlog puede ser una lista corta con prioridad y estado; las otras columnas se omiten.

**Historial corto:** una fila por cambio, una sola línea, con el enlace a la bitácora que tiene el detalle. Ejemplo: `| 2026-10-06 | Iteración 05-04 en revisión. [Bitácora](../99_bitacora/epica-05-publicacion/bitacora-05-04.md) |`. El detalle (mediciones, decisiones, salvedades) va en la bitácora; repetirlo aquí deja un historial de cientos de líneas que nadie lee.

**Reversiones:** nada se borra. Si un cambio anterior queda sin efecto, se agrega una fila nueva que lo dice («La fila del 2026-09-30 que da por terminada la Épica 02 queda sin efecto: …»).

## 8. 00_producto/decisiones.md

Registro de Decisiones Arquitectónicas (RDA): tabla índice (número, título, estado) y una sección por RDA con fecha, estado, contexto, decisión, alternativas consideradas y consecuencias.

- Numeración correlativa sin huecos (`RDA-001`, `RDA-002`…).
- Una RDA aceptada no se contradice: se reemplaza con una nueva que la declare «Reemplazada por RDA-XXX» (o «Reemplazada parcialmente»), y la antigua recibe una línea que remite a la nueva.
- Las actualizaciones de una RDA van como párrafos «**Actualización AAAA-MM-DD:** …» al final de la RDA, breves. Si una RDA acumula más de dos o tres actualizaciones de fondo, conviene una RDA nueva que la reemplace: es más fácil de leer que una cadena de parches.
- Al inicio del proyecto, `RDA-001 · Nivel de proceso` registra el diagnóstico de complejidad acordado: nivel, factores que lo justifican, componentes activos y los que quedaron fuera a propósito. Después, registra como RDA las decisiones estructurales ya tomadas en la entrevista (stack, hosting, fuente de datos, estructura de páginas o de la API), con el estado que corresponda.

## 9. 00_producto/auditoria-tecnica.md

Registro acumulativo: las auditorías se agregan al final y nunca se borran. Cada auditoría lleva fecha, auditor, alcance, resumen y una tabla de hallazgos con ID, severidad, área, hallazgo, acción recomendada y estado.

- **Numeración**: antes de agregar una auditoría, busca el último número usado en el archivo y suma uno. IDs con el formato `AUD-NN-MMM` (número de auditoría y correlativo del hallazgo). Nunca se reutiliza un número.
- **Cierre**: un hallazgo se marca «Resuelto en XX-YY» o «Resuelto en RDA-XXX», con la evidencia mínima («medido en…»). «Riesgo aceptado» solo si el desarrollador lo aceptó y consta dónde.
- **Severidad**: Alta (bloquea la publicación o afecta al usuario o al negocio), Media (afecta calidad o rendimiento), Baja (mejora).
- Al inicio del proyecto, si hay insumos que auditar (prototipo, propuesta, sitio anterior, o código existente según la sección 17), genera la Auditoría 01 con sus hallazgos. Si no los hay, el archivo nace solo con el encabezado y las reglas.
- **En nivel pequeño**, el archivo no se genera salvo que haya insumos que auditar. Si más adelante se hace una auditoría, se crea con la plantilla.

## 10. Épicas: definicion-epica-XX.md

Carpeta `10_epicas/epica-XX-<nombre-corto>/`. La definición lleva: estado, objetivo, alcance, tabla de iteraciones (número y nombre), decisiones de la épica si las hay y criterio de término verificable (qué se mide para darla por terminada).

Al cerrar la épica, el agente completa **«Revisión al cierre»** en pocas líneas: si se cumplió el criterio de término, qué se aprendió y qué cambia en el backlog o en las épicas siguientes. Es lo único que se conserva de una retrospectiva: lo que orienta el trabajo que viene. En nivel pequeño, la sección puede ser de una o dos líneas.

**Cierre de épica y planificación de la siguiente.** Al arrancar se detallan solo las iteraciones de la primera épica; las de cada épica siguiente se generan **todas juntas** al cerrar la anterior, para no planificar iteración por iteración. Es el último paso de la última iteración de la épica (y el desarrollador puede pedirlo en cualquier momento con «planifica la siguiente épica» o `/siguiente-epica`):

1. Revisión al cierre de la épica que termina.
2. Propuesta en el chat de las iteraciones de la épica siguiente: número, nombre, objetivo en una línea, elementos del backlog que cubre y qué necesita cada una. Toda iteración respeta el tamaño máximo (sección 11).
3. Si la épica siguiente ya tenía iteraciones detalladas, no se rehacen: se proponen ajustes solo si la revisión al cierre cambió algo.
4. Con la aprobación del desarrollador, se generan de una vez todos los archivos de iteración y se actualizan la tabla de la épica, el registro y el backlog.
5. Cada iteración generada se revisa contra la definición de preparada; lo que falte por causas externas se registra como pendiente.

Como los agentes que trabajan en el repositorio no tienen esta skill, el flujo queda escrito en `_planificacion/README.md` (sección 4.4), resumido en el paso final del flujo por iteración de `AGENTS.md` y con el atajo `.claude/commands/siguiente-epica.md` para Claude Code.

## 11. Iteraciones: iteracion-XX-YY-descripcion.md

Nombre con descripción corta en ASCII: `iteracion-01-01-andamiaje.md`. Una unidad de trabajo acotada, sin duración fija: puede ser una sesión, un hito o un bloque funcional, mientras se pueda revisar de una vez.

**Tamaño máximo:** un solo objetivo, un resultado que el desarrollador revisa en unos 30 minutos y **8 tareas como máximo**. Si una iteración se pasa, se parte antes de escribirla en detalle, cada parte con su objetivo y sus criterios. No es una estimación (el backlog sigue sin puntos): es la regla para cortar el trabajo en trozos que un agente termina y una persona revisa de una vez. El validador avisa si una iteración tiene más de 8 tareas.

**Nace preparada:** al escribir una iteración, revísala contra la definición de preparada de `AGENTS.md` (sección 1 de esta guía, punto 12). Lo que falte por causas externas se registra como pendiente y queda anotado en la iteración. Lleva: épica, estado, rama sugerida, dependencias, especificación y criterios que implementa (si tiene), RDA relacionadas, hallazgos que cierra, objetivo, alcance y entregables, plan técnico (cuando aplica), tareas, criterios de aceptación verificables (casillas), fuera de alcance y datos requeridos del cliente.

- **Objetivo:** una o dos frases sobre lo que la iteración deja funcionando. Es lo que se revisa al cerrar.
- **Alcance y entregables:** qué existe al terminar (una página publicada, un endpoint respondiendo, un componente en el sistema de diseño). Sirve para revisar el resultado sin leer el código.
- **Plan técnico:** cómo se aborda la solución dentro de la arquitectura y las convenciones del proyecto: componentes o módulos que se tocan, cambios en el modelo de datos o en el contrato de la API, decisiones de implementación y riesgos. Se escribe cuando la iteración toca arquitectura, datos, API o integraciones (desde nivel mediano) o siempre (nivel complejo). Para una iteración simple se borra la sección: las tareas bastan.
- **Tareas:** concretas, ordenadas y ejecutables por un agente sin preguntar qué significan. En nivel complejo, cada tarea que depende de otra lo dice.
- **Criterios de aceptación:** si la iteración implementa una especificación, se citan sus criterios (`CA-01`, `CA-02`) en vez de copiarlos, y se agregan solo los técnicos propios de la iteración. Si no hay especificación, la iteración trae sus propios criterios, breves.

Un buen criterio de aceptación se puede comprobar con una medición o un comando: «`pnpm build` termina sin errores» sí; «el diseño se ve bien» no. Si hay que medir en producción, el criterio dice qué se mide y dónde.

## 12. Bitácoras

Carpeta por épica, con el mismo nombre que la carpeta de la épica: `99_bitacora/epica-01-fundacion/`.

- La primera bitácora de una iteración: `bitacora-01-01.md`.
- Las siguientes de la misma iteración llevan un sufijo descriptivo en ASCII: `bitacora-01-01-correccion.md`, `bitacora-01-01-ajustes.md`, `bitacora-01-01-produccion.md`. Si se repite el sufijo, se numera: `bitacora-01-01-ajustes-2.md`.
- La fecha va dentro del archivo, no en el nombre.
- Se escribe al terminar, con la plantilla de `_planificacion/README.md`, y no se edita después de entregada: las correcciones van en una bitácora nueva.
- Registra mediciones reales. «Verificado» sin decir cómo no cuenta.
- Termina con una sección **«Revisión»** de dos a cuatro líneas: el resultado frente al objetivo, qué se aprendió y qué cambia para lo que sigue (una fila nueva en el backlog, una especificación que ajustar, una iteración que reordenar). Es la parte útil de una retrospectiva, sin la ceremonia. Si la revisión cambia algo, el agente lo aplica en el registro en la misma pasada.
- En nivel pequeño, la bitácora puede ser breve: resumen, verificación con evidencia, criterios y una o dos líneas de revisión.

## 13. Evidencia de producción y fases

**Por defecto, una iteración se ejecuta completa, sin fases.** El agente resuelve todo por su cuenta.

Una iteración se separa en dos partes **solo** cuando el trabajo exige contrastar lo que el agente hace en local con datos de producción **y** el agente, después de intentarlo por todos los medios a su alcance (comandos permitidos, herramientas web, documentación pública, registros del proyecto), no logra obtener esos datos. Recién entonces:

1. Termina y registra todo lo que sí pudo hacer, en la bitácora de la iteración.
2. Crea `10_epicas/epica-XX-<nombre>/evidencia-XX-YY.md` con la plantilla de evidencia: qué dato necesita, para qué criterio, **qué intentó para obtenerlo y por qué no pudo**, y los pasos exactos para que el desarrollador lo consiga en el menor tiempo posible.
3. Deja la iteración «En revisión» y avisa al desarrollador.
4. Cuando el desarrollador responde en el archivo de evidencia, el agente consolida en una bitácora nueva con sufijo (`bitacora-XX-YY-produccion.md`).

Pedir evidencia es la excepción, no un paso del proceso: cada pedido carga al desarrollador con trabajo. El archivo de evidencia lleva una sección obligatoria «Qué intentó el agente», y sin ella el pedido no es válido.

## 14. Especificaciones: spec-funcionalidad.md

Carpeta `_planificacion/20_especificaciones/`, desde nivel mediano. Una especificación por funcionalidad que la justifique (criterios en `niveles-complejidad.md`, sección 4), con nombre en ASCII: `spec-reserva-de-horas.md`.

**Por qué una carpeta propia y no dentro de la épica:** la especificación describe cómo se comporta el producto, no cuándo se construyó. Una funcionalidad la tocan varias iteraciones de épicas distintas a lo largo del proyecto; si la especificación viviera en la carpeta de la épica donde nació, los cambios posteriores quedarían repartidos. Las iteraciones (lo temporal) apuntan a la especificación (lo vigente). Tampoco se separan `spec.md`, `plan.md` y `tasks.md` por funcionalidad, como hacen otras herramientas de SDD: el plan técnico y las tareas cambian en cada iteración, y la iteración ya es su lugar natural.

**Lleva, según la necesidad** (lo mínimo son problema, comportamiento y criterios; el resto se borra si no aporta):

1. **Estado** (`Borrador`, `Aprobada`, `Vigente`, `Reemplazada`), última revisión e iteraciones que la implementan.
2. **Problema o necesidad** que resuelve, en dos o tres líneas.
3. **Comportamiento esperado**, descrito desde quien usa la funcionalidad.
4. **Requisitos funcionales**, numerados.
5. **Reglas de negocio**, numeradas: cálculos, validaciones, estados, permisos.
6. **Escenarios** relevantes en formato Dado / Cuando / Entonces: el camino principal y los casos borde que cambian el resultado. No todos los imaginables.
7. **Criterios de aceptación** verificables, con ID corto (`CA-01`, `CA-02`) para que las iteraciones los citen.
8. **Restricciones o consideraciones técnicas** que el plan debe respetar.
9. **Contrato de API**, solo si expone o consume endpoints: las operaciones del OpenAPI por su `operationId`, sin copiar esquemas.
10. **Fuera de alcance.**
11. **Preguntas abiertas**, con sus marcadores de pendiente.

**Reglas:**

- Nace en `Borrador`. El agente puede redactarla y proponerla, pero una funcionalidad con especificación no se implementa hasta que el desarrollador la marca `Aprobada`. Cuando la implementación cumple todos sus criterios, pasa a `Vigente`.
- Es un documento vivo: describe el comportamiento actual. Si un cambio altera el comportamiento, la especificación se actualiza **antes** de implementar y vuelve a `Aprobada` cuando el desarrollador revisa el cambio. No lleva tabla de historial: la trazabilidad está en las bitácoras y en el registro.
- Si una funcionalidad se rehace de raíz, la especificación antigua pasa a `Reemplazada` con un enlace a la nueva.

## 15. Contrato de la API: docs/api/openapi.yaml

Solo en backend y fullstack con API REST, obligatorio desde nivel mediano (detalle en `tipos-proyecto.md`, «Contrato de una API REST»). Es la única excepción a «todo en Markdown» junto con `settings.json`.

- Vive en el repositorio fuera de `_planificacion/` porque lo usan herramientas (generadores de tipos, visores, pruebas) y no solo personas.
- Es un documento vivo: el agente lo edita, siempre **antes** que el código de la ruta.
- Al arrancar el proyecto, nace con `info`, `servers`, esquemas de seguridad, el formato de error común y los endpoints ya especificados. Lo que no se sabe se marca con `PENDIENTE_DESARROLLADOR` en la descripción de la operación y se registra en el registro.

## 16. Cambios y correcciones: qué documentos se tocan

El flujo completo queda escrito en `_planificacion/README.md` (sección 4.3) y resumido en `AGENTS.md`. Lo importante para generar esos textos:

| Clase de cambio | Ejemplos | Documentos que se actualizan antes de implementar | Registro al terminar |
| :-------------- | :------- | :------------------------------------------------ | :------------------- |
| **Menor** | Un error en que el código no cumple lo especificado, un texto, un ajuste visual dentro de lo que define `DESIGN.md` | Ninguno | Fila en el backlog (tipo `Corrección`) y una línea de historial. Bitácora breve solo si hubo mediciones o decisiones que conviene conservar |
| **Significativo** | Cambia el comportamiento esperado, una regla de negocio, el contrato de la API, el modelo de datos o la arquitectura | La especificación, el OpenAPI o una RDA, según lo afectado. Si afecta un documento inmutable, propuesta al desarrollador | Iteración con plan y tareas, y bitácora completa |

La pregunta que decide la clase es: **¿después del cambio, la especificación (o el contrato, o la RDA) sigue describiendo bien el sistema?** Si sí, es menor. Si no, es significativo. Ante la duda, el agente pregunta.

Una corrección menor no necesita un archivo de iteración. Si pertenece a la iteración activa, se hace dentro de ella; si no, se trabaja como corrección fuera de iteración, con su rama y su formato de commit.

## 17. Proyecto con código existente: épica 00 y auditoría inicial

Cuando el proyecto ya tiene código pero no esta documentación, el arranque parte por analizar el código (`proyecto-existente.md`). Además de los documentos de siempre, se generan:

- **`10_epicas/epica-00-linea-base/definicion-epica-00.md`**, desde `assets/linea-base/`: lo que ya existe, con su estado observado o verificado, dónde está en el código y el stack detectado con el archivo del que se dedujo. Nace `Terminada`, no lleva iteraciones ni bitácoras (tampoco carpeta en `99_bitacora/`) y no se edita después.
- **Auditoría 01** en `auditoria-tecnica.md`, con la revisión inicial del código, aunque el nivel sea pequeño.
- **RDA de las decisiones previas** (stack, hosting, base de datos), en estado `Aceptada`, sin inventar alternativas ni razones que nadie dejó escritas.
- **Backlog** con lo que está a medio hacer y los hallazgos Altos y Medios, además de lo que salga de la entrevista.

Las épicas planificadas empiezan en la 01. La tabla de estado del registro lleva una primera fila para la línea base.
