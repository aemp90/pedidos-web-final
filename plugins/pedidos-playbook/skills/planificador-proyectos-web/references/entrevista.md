# Entrevista inicial

Preguntas para reunir lo necesario antes de generar la documentación. No las hagas todas de una vez: pregunta en bloques de hasta cinco, en el orden de abajo, y salta lo que ya respondió el desarrollador o lo que sale de los documentos que adjuntó o de los que ya existen en el repositorio.

La profundidad depende del proyecto: si desde el primer mensaje se ve pequeño (un sitio presentacional, una landing, una API de pocos recursos sin usuarios), basta con el bloque 1 y lo esencial del bloque 2; el resto se cubre con los valores por defecto y con marcadores de pendiente. Si se ve mediano o complejo, recorre todos los bloques que apliquen.

Si el desarrollador entrega un proyecto que ya tiene código (un repositorio, una carpeta o un `.zip`), primero analízalo con `proyecto-existente.md`: el stack, la estructura y buena parte del alcance salen del código, y la entrevista se reduce a confirmar lo deducido y preguntar lo que el código no dice (cliente, propósito, público, prioridades).

Si una respuesta falta al final, no la inventes: usa `PENDIENTE_CLIENTE` o `PENDIENTE_DESARROLLADOR` y regístrala en `registro-pendientes.md`.

## Bloque 1 · Lo que decide la estructura (obligatorio antes de generar)

1. **Tipo de proyecto**: ¿frontend (sitio, landing, SPA), backend o API sin interfaz, o fullstack? Si hay API, ¿es REST?
2. **Stack**: qué stack prefiere el desarrollador y con cuáles tiene experiencia, y **en la misma pregunta** tu recomendación según el tipo de producto, con su razón en una línea (criterios y ejemplo en `stacks.md`). Después confirma lo que falte: gestor de paquetes y versión (`uv` o `pip`, Maven o Gradle), base de datos si hay y versión del entorno (Node, Python, Java).
3. **Hosting y despliegue**: dónde se publica y cómo (despliegue automático desde `main`, vistas previas por rama, contenedores, servidor propio).
4. **Alcance comprometido**: qué entra y qué no, y las funcionalidades principales que ya conoces. Si hay una propuesta comercial, pídela.
5. **Lo que sube el riesgo**, en una pregunta: ¿hay inicio de sesión o roles, integraciones con servicios externos, pagos o datos personales?

Con las respuestas de este bloque ya puedes proponer el diagnóstico de complejidad (`niveles-complejidad.md`). Los bloques siguientes lo afinan.

## Bloque 2 · Contexto del producto

1. **Cliente o dueño del producto**: nombre, rubro y ubicación. Si es un proyecto propio o de estudio, dilo así: cambia cómo se escriben el alcance y la autoría.
2. **Propósito**: qué tiene que lograr una persona que usa el sitio o la API, en una frase.
3. **Público objetivo**: quiénes lo usan y en qué condiciones (celular, mala señal, apuro, escritorio en una oficina, otro sistema que consume la API).
4. **Indicadores de éxito**: cómo se va a saber que funciona (conversiones, rendimiento, tiempos de respuesta, adopción).
5. **Hitos y fechas comprometidas**, y el nombre del repositorio y del dominio si ya existen.

## Bloque 3 · Datos y contenido

1. **Datos del negocio confirmados**: contacto, dirección, horarios, precios, capacidades. Lo no confirmado queda como `PENDIENTE_CLIENTE`.
2. **Fuente única de datos**: ¿dónde vivirán (archivo de datos, base de datos, CMS, variables de entorno)?
3. **Textos y fotos**: ¿los entrega el cliente, existen o hay que pedirlos?
4. **Afirmaciones comerciales** que el cliente quiere publicar (tiempos, garantías, años de experiencia): ¿están respaldadas?

## Bloque 4 · Diseño (solo frontend y fullstack)

1. **Referencias visuales**: prototipo, maqueta, marca, logo, paleta o tipografías existentes.
2. **Concepto o tono visual** en pocas palabras.
3. **Modo claro, oscuro o ambos.**
4. **Exigencias de accesibilidad y rendimiento** (por defecto: WCAG 2.2 AA y Lighthouse móvil de 95 o más).
5. **Librería de íconos y origen de las fuentes** (por defecto: SVG en línea y fuentes autoalojadas).

Si no hay referencias visuales, propón un concepto, una paleta y una tipografía en el chat, con la razón de cada elección, y espera la aprobación antes de escribirlo en `DESIGN.md`: ese archivo es inmutable y no debe nacer con un diseño que el desarrollador no eligió.

## Bloque 5 · API (solo backend y fullstack)

1. **Estilo**: REST, GraphQL, RPC u otro.
2. **Autenticación y autorización**: mecanismo y roles.
3. **Recursos principales** y si necesitan paginación, filtros u orden.
4. **Formato de errores y respuestas**, y versionado (en la ruta, en la cabecera o ninguno).
5. **Consumidores**: quién usa la API (tu propio frontend, una app móvil, terceros). Con un solo consumidor propio, el contrato puede ser más liviano; con terceros, conviene más detalle.

## Bloque 6 · Forma de trabajo

Por defecto se usan las convenciones de `SKILL.md` («Convenciones por defecto»). Pregunta solo si hay señales de que este proyecto cambia algo:

1. **Agentes de IA** que trabajarán en el repositorio (Claude Code, Antigravity, Codex u otros).
2. **Comandos del proyecto** (desarrollo, compilación, pruebas, formato, revisión de tipos) y cuáles de ellos son obligatorios antes de cerrar una iteración.
3. **Archivos de configuración** que el agente puede editar sin consultar y cuáles no.
4. **Primeras épicas**: si el desarrollador ya tiene una idea, úsala; si no, propón una división (por ejemplo fundación, sistema de diseño, contenido, SEO y rendimiento, publicación y medición para un sitio; o fundación, primer recurso de punta a punta, autenticación, resto de recursos y publicación para una API) y explícala.
5. **Idioma, autoría y formato**: idioma y variante regional del producto y de la documentación (por defecto, español latinoamericano neutro con trato de tú), zona horaria para las fechas, nombre y enlace del desarrollador, y confirma que todo va en Markdown. Si el desarrollador no tiene preferencia, usa los valores por defecto sin insistir.
