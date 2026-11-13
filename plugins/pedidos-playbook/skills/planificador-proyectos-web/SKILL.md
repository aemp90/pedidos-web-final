---
name: planificador-proyectos-web
description: "Planifica y documenta proyectos web (frontend, backend o API, fullstack) en el idioma que elijas (por defecto, español), con un proceso proporcional a su complejidad: diagnostica si el proyecto es pequeño, mediano o complejo y activa solo lo que aporta (AGENTS.md, DESIGN.md, README.md, .claude/, y en _planificacion/ el registro con backlog, decisiones, especificaciones, épicas, iteraciones y bitácoras; OpenAPI si expone una API REST). Úsala siempre que el desarrollador quiera planificar, documentar o arrancar un proyecto web (nuevo o con código ya escrito), elegir el stack, planificar la siguiente épica, especificar una funcionalidad nueva, gestionar un cambio o una corrección con trazabilidad, rediagnosticar la complejidad, preparar las reglas para agentes de IA (Claude Code, Antigravity, Codex) o revisar y optimizar un DESIGN.md o README.md existente, aunque no nombre la skill ni los archivos."
---

# Planificador de proyectos web

Esta skill arma y mantiene la documentación con la que un desarrollador y sus agentes de IA trabajan en un proyecto web: qué se construye, con qué reglas, en qué orden y qué se hizo. Combina dos ideas:

- **Spec-Driven Development (SDD):** lo que se construye se especifica antes de programarlo, y se verifica contra esa especificación.
- **Prácticas útiles de Scrum, sin Scrum formal:** un backlog priorizado, iteraciones con objetivo y una revisión al cerrar que orienta lo siguiente. No hay sprints de duración fija ni ceremonias.

Funciona en dos lugares:

- **En el chat:** entrevistas al desarrollador, generas los archivos y los entregas en un `.zip` listo para descomprimir en la raíz del repositorio.
- **En Claude Code:** escribes los archivos directamente en el repositorio y, más adelante, ayudas a evolucionarlos (funcionalidades nuevas, cambios, correcciones).

El modelo viene de proyectos reales de su autor. Varias reglas de abajo existen porque en esos proyectos algo salió mal (documentos que parecían borradores, IDs de auditoría duplicados, historiales de 15 líneas por fila, bitácoras sin nombre claro). Cuando una regla parezca rígida, la razón está a su lado: úsala para decidir en los casos que la regla no cubre.

## Reglas que no se negocian

1. **Un solo idioma y una sola variante regional en todo**, los que acuerdes con el desarrollador en la entrevista. Por defecto, español latinoamericano neutro con trato de **tú**; si pide otra variante (español de Chile, de México o de Argentina, portugués, inglés), esa decide la ortografía, el vocabulario y el formato de números, monedas, teléfonos y fechas. Tus explicaciones en el chat van en el mismo idioma.
2. **Todo en Markdown**, salvo que el desarrollador pida otro formato al inicio. Las únicas excepciones son `.claude/settings.json`, que Claude Code exige en JSON, y el contrato `docs/api/openapi.yaml` de las APIs REST, que sigue el formato OpenAPI.
3. **Nombres de archivo y carpeta en ASCII**: minúsculas, guiones, sin tildes ni ñ (`epica-02-sistema-diseno/`, `bitacora-01-01.md`). Los caracteres no ASCII dan problemas en Git entre Windows y Linux, en URLs de GitHub y en terminales. El *contenido* sí lleva tildes y ñ.
4. **No inventes datos.** Lo que no confirmó el desarrollador o el cliente va con un marcador explícito: `PENDIENTE_CLIENTE` (datos del negocio) o `PENDIENTE_DESARROLLADOR` (decisiones técnicas o de diseño), y se registra en `registro-pendientes.md`. Un dato de relleno que parece real termina publicado. Lo mismo vale para resultados: no declares una verificación, una prueba o un estado de implementación que no ocurrió.
5. **Cuatro documentos inmutables**: `AGENTS.md`, `DESIGN.md`, `README.md` (raíz) y `_planificacion/README.md`. Ningún agente los edita: solo sugiere cambios y explica qué y por qué; los aplica el desarrollador. Por eso se escriben como versión única y consolidada (ver «Documentos inmutables»).
6. **Proporcionalidad.** El proceso crece solo cuando la complejidad, el riesgo o el mantenimiento del proyecto lo justifican. Ningún documento se crea por obligación: si no le sirve al desarrollador ni a un agente, no existe. Ante dos alternativas válidas, elige la que conserve la estructura existente, evite duplicar información y requiera menos trabajo administrativo.

## Modo arranque: planificar un proyecto nuevo

### 1. Entrevista (o análisis del código existente)

**Si el proyecto ya tiene código** (un repositorio abierto en Claude Code, una carpeta o un `.zip` adjunto) pero no tiene `AGENTS.md` ni `_planificacion/`, empieza por `references/proyecto-existente.md`: deduce del código el stack, la estructura, lo construido y los riesgos; muéstralo en el chat para que el desarrollador lo confirme, y pregunta solo lo que el código no responde. Lo construido queda como **Épica 00 · Línea base** (`Terminada`, sin iteraciones ni bitácoras) y la revisión inicial del código como **Auditoría 01**. En el chat, pide el proyecto como `.zip`.

Antes de generar nada, lee `references/entrevista.md` y haz las preguntas que falten. Pregunta en bloques cortos (no más de cinco preguntas por mensaje) y empieza por lo que decide la estructura: tipo de proyecto, stack, alcance y si hay frontend. Si el desarrollador adjunta documentos previos (propuesta comercial, prototipo, brief, un DESIGN.md o README.md antiguo) o si trabajas en un repositorio que ya tiene documentación, sácales las respuestas antes de preguntar: no repitas preguntas cuya respuesta ya está escrita.

No hace falta tener todo para empezar: lo que siga sin respuesta se marca como pendiente (regla 4). Lo que sí necesitas antes de generar es el tipo de proyecto, el stack principal y el alcance, porque cambian qué archivos existen.

**El stack se pregunta con una recomendación.** En la misma pregunta: qué stack prefiere el desarrollador, con cuáles tiene experiencia y cuál recomiendas tú según el tipo de producto, con su razón en una línea. Los criterios están en `references/stacks.md`: frontend con Astro, Tailwind y PNPM; backend en Python (Django o FastAPI) para sistemas modulares que deben salir rápido y pueden sumar nube o agentes de IA, en Java (Spring Boot o Quarkus) cuando la seguridad es crítica (sector público, banca, finanzas) y en Node.js con Express para backends pequeños y minimalistas. La experiencia del desarrollador entra al diagnóstico como riesgo técnico. Si el proyecto se ve pequeño desde el primer mensaje, basta con el bloque 1 de la entrevista y lo esencial del bloque 2.

### 2. Diagnóstico de complejidad

Lee `references/niveles-complejidad.md` y clasifica el proyecto como **pequeño**, **mediano** o **complejo** según sus factores (alcance, integraciones, persistencia, autenticación, seguridad, riesgos, despliegue, mantenimiento). El nivel decide qué componentes del proceso se activan; no es una plantilla rígida y se pueden combinar componentes con una razón.

Presenta en el chat, en un solo mensaje y en pocas líneas: el nivel propuesto con los factores que lo justifican, qué se activa y qué queda fuera, los archivos que vas a generar, las épicas previstas y los supuestos. Espera el visto bueno: el desarrollador puede confirmar o cambiar el nivel y cualquier componente. Rehacer quince archivos por un supuesto equivocado cuesta más que una confirmación.

El nivel acordado se registra como `RDA-001 · Nivel de proceso` en `decisiones.md`, con sus factores y los componentes activos.

### 3. Elegir la variante

Lee `references/tipos-proyecto.md` y aplica la variante que corresponde:

- **Frontend** (sitio, landing, SPA): con `DESIGN.md`.
- **Backend o API sin interfaz**: sin `DESIGN.md`; las convenciones generales de la API van en una sección propia de `AGENTS.md` y, si expone una API REST, el contrato vive en `docs/api/openapi.yaml` según el nivel.
- **Fullstack**: `DESIGN.md` cubre la interfaz; las convenciones de la API van en `AGENTS.md` y el contrato en `docs/api/openapi.yaml`.

Aplica también la ficha del stack acordado (`references/stacks.md`, sección 4): comandos, reglas técnicas propias y definición de terminado. Arma `.claude/settings.json` con el script, nunca a mano:

```bash
python scripts/combinar_permisos.py --stack <fragmento>[:carpeta] [--stack ...] [--api-rest] --salida <directorio>/.claude/settings.json
```

Fragmentos: `astro-pnpm`, `django`, `fastapi`, `spring`, `quarkus`, `express-pnpm` (`--listar` los muestra). En un monorrepo fullstack, cada fragmento va con su carpeta (`--stack astro-pnpm:frontend --stack django:backend`). Después revisa el resultado contra el proyecto real.

### 4. Generar

Lee `references/guia-documentos.md` antes de escribir: ahí está, documento por documento, qué contiene, qué no contiene y por qué. Las plantillas están en `assets/plantillas/`, con la misma estructura de carpetas que el entregable. Cópialas a un directorio de trabajo, rellénalas con los datos del proyecto y adapta lo que el proyecto pida: las plantillas son un punto de partida, no un formulario. Las secciones marcadas con un comentario `<!-- PLANTILLA: … -->` se conservan o se borran según el nivel y el tipo, y el comentario se borra siempre.

Estructura del entregable (lo marcado como condicional depende del nivel y del tipo):

```text
<raiz-del-proyecto>/
├── AGENTS.md
├── DESIGN.md                     (solo si hay interfaz)
├── README.md
├── .claude/
│   ├── settings.json             (combinado con scripts/combinar_permisos.py)
│   ├── README.md
│   └── commands/siguiente-epica.md
├── docs/api/openapi.yaml         (API REST: obligatorio desde nivel mediano)
└── _planificacion/
    ├── README.md
    ├── 00_producto/
    │   ├── producto.md
    │   ├── registro-pendientes.md   (incluye el backlog)
    │   ├── decisiones.md
    │   └── auditoria-tecnica.md     (desde nivel mediano, o si hay insumos que auditar)
    ├── 10_epicas/
    │   ├── epica-00-linea-base/     (solo si el proyecto ya tenía código)
    │   │   └── definicion-epica-00.md
    │   └── epica-01-<nombre>/
    │       ├── definicion-epica-01.md
    │       └── iteracion-01-01-<descripcion>.md
    ├── 20_especificaciones/         (desde nivel mediano)
    │   └── spec-<funcionalidad>.md
    └── 99_bitacora/
        └── epica-01-<nombre>/       (vacía al inicio, con un .gitkeep)
```

Genera las épicas que el proyecto necesite con su definición, pero escribe en detalle solo las iteraciones de la primera épica (y las de la segunda si ya están claras). Las iteraciones lejanas, escritas hoy, se reescriben casi siempre: basta con nombrarlas en la tabla de su `definicion-epica-XX.md`. Las de cada épica siguiente se generan **todas juntas al cerrar la anterior** (ver «Cierre de épica» en el modo evolución). El backlog de `registro-pendientes.md` sí lista desde el inicio todo lo conocido, priorizado.

Cada iteración que escribas respeta el **tamaño máximo** (un objetivo, revisable en unos 30 minutos, 8 tareas como máximo; si se pasa, pártela) y nace **preparada**: la revisas contra la definición de preparada de `AGENTS.md` y registras como pendiente lo que falte por causas externas. Detalle en `references/guia-documentos.md`, secciones 1 y 11.

Escribe especificaciones (`20_especificaciones/`) solo para las funcionalidades de las primeras iteraciones que las justifican (ver `references/niveles-complejidad.md`). Nacen en estado `Borrador` y el desarrollador las aprueba. Las demás funcionalidades quedan en el backlog y se especifican cuando se acerque su turno.

Las bitácoras no se generan al inicio: las escribe el agente al terminar cada iteración. La carpeta de la primera épica va con un `.gitkeep` para que Git la versione.

Si el desarrollador trae un `DESIGN.md` o `README.md` existente, no lo copies: optimízalo. Consolida lo vigente, quita los rastros de tiempo (ver abajo) y, en el chat, lista qué quitaste o moviste y a qué documento vivo fue a parar.

**Optimizar no es completar.** Al reordenar un documento existente contra la estructura de la plantilla, aparecen secciones vacías (concepto, escala tipográfica, espaciado, íconos, hover, movimiento) y la tentación es rellenarlas con valores razonables. No lo hagas: un valor razonable inventado se lee igual que uno decidido, y un agente lo va a implementar como verdad. Lo que el documento original no decía se deja con `PENDIENTE_DESARROLLADOR`, se registra en «Decisiones por tomar» de `registro-pendientes.md` y, si tienes una sugerencia, la das en el chat. Lo mismo vale para corregir: si un valor existente rompe una regla (por ejemplo, un color de texto que falla el contraste AA), escribe en el documento la restricción que evita el problema («solo indicadores, nunca texto»), registra el cambio como decisión pendiente del desarrollador y explícalo en el chat; no reemplaces el valor por uno que él no eligió.

### 5. Validar

Ejecuta el validador sobre el directorio generado:

```bash
python scripts/validar_documentacion.py <directorio> --tipo frontend|backend|fullstack --nivel pequeno|mediano|complejo [--api-rest]
```

Marca como **error**: archivos obligatorios del nivel faltantes, nombres no ASCII o fuera de convención, `settings.json` inválido, reglas de `allow` o `ask` anuladas por una de `deny`, marcadores de plantilla sin rellenar, código en el README raíz, `DESIGN.md` en un backend, un registro sin backlog, iteraciones o especificaciones sin criterios de aceptación, una API REST de nivel mediano o complejo sin `docs/api/openapi.yaml` y marcadores `PENDIENTE_` sin su sección correspondiente en `registro-pendientes.md`. Corrige todos los errores antes de entregar.

Marca como **aviso**: posibles rastros de tiempo en los documentos inmutables, criterios de aceptación vagos y la lista de archivos con pendientes, para que confirmes que cada uno tiene su fila en el registro. Los avisos pueden ser falsos positivos (por ejemplo, «historial de cambios del esquema» en un README que explica las migraciones): léelos uno por uno, corrige los reales y deja los otros.

### 6. Entregar

- **En el chat:** empaqueta con el mismo script (`--zip <ruta-del-zip>`) y entrega el `.zip` con la herramienta de archivos que tengas.
- **En Claude Code:** genera en un directorio de trabajo, valida y recién entonces copia al repositorio. Escribe `.claude/settings.json` al final: una vez en su lugar, bloquea la edición de los documentos inmutables, también para ti. No hagas commits.

En ambos casos, en el chat, después de entregar:

1. El diagnóstico acordado en una línea.
2. Una explicación breve **por documento** de por qué quedó así: las decisiones de fondo, no un resumen del contenido. El desarrollador necesita entender el porqué, y es lo que le permite corregir un criterio en vez de corregir archivo por archivo.
3. La lista de pendientes (`PENDIENTE_CLIENTE` y `PENDIENTE_DESARROLLADOR`) que quedaron, para que sepa qué conseguir.
4. Un mensaje de commit sugerido para el commit inicial, con el formato del proyecto.

## Modo evolución: un proyecto que ya tiene la documentación

Cuando el repositorio ya tiene `AGENTS.md` y `_planificacion/`, no regeneres nada: lee el registro, las RDA y los documentos que toque la solicitud, y trabaja sobre lo que existe. Los flujos están escritos dentro de `_planificacion/README.md` del proyecto (sección 4), porque los agentes que trabajan en el repositorio no tienen esta skill; síguelos tal como están ahí:

- **Funcionalidad nueva:** fila en el backlog → especificación si el nivel y la funcionalidad lo justifican → iteración con plan técnico y tareas → implementación → validación contra los criterios.
- **Cambio o corrección:** entender el pedido → identificar lo afectado → clasificarlo como menor o significativo → actualizar solo los documentos afectados → tareas → implementación → verificación → registro. Una corrección menor no lleva especificación nueva; un cambio que altera el comportamiento esperado, el contrato de la API o la arquitectura actualiza primero la especificación, el contrato o la RDA, y después se implementa.
- **Cierre de épica y planificación de la siguiente:** es el paso final de la última iteración de cada épica, y el desarrollador también lo puede pedir («planifica la siguiente épica», o `/siguiente-epica` en Claude Code). Revisión al cierre de la épica que termina → propuesta en el chat de las iteraciones de la siguiente (número, nombre, objetivo en una línea, qué cubre del backlog y qué necesita), todas dentro del tamaño máximo → aprobación del desarrollador → generación de **todas** esas iteraciones de una vez, con el registro y el backlog al día. Si la épica siguiente ya tenía iteraciones detalladas, no se rehacen: se proponen ajustes solo si la revisión al cierre cambió algo.
- **Rediagnóstico:** si el proyecto ganó factores de complejidad (autenticación, una integración nueva, datos personales, más consumidores de la API) o los perdió, propón el nivel nuevo con sus razones. Si el desarrollador lo acepta, registra una RDA que reemplaza a la anterior y entrega la propuesta de cambios a los documentos inmutables para que él la aplique.

Los documentos inmutables tampoco se editan en este modo, aunque los hayas generado tú: entrega la propuesta (archivo, sección, texto actual, texto propuesto y por qué). En Claude Code, además, `.claude/settings.json` lo impide.

## Documentos inmutables

`AGENTS.md`, `DESIGN.md`, `README.md` y `_planificacion/README.md` describen el estado vigente y nada más. Un agente que lee «Excepciones (iteración 04-04, decisiones del 2026-10-02)» entiende que el documento es un borrador en movimiento y empieza a tratarlo como tal. Por eso, en estos cuatro archivos:

- **Sin registro de cambios**, ni formal (tabla de historial) ni encubierto.
- **Sin rastros de tiempo**: fechas, números de iteración o de épica, frases como «se crea en 04-01», «aún no existe», «se publica en la iteración 05-01», «antes era…», «a partir de ahora», «versión 2 de este documento».
- **Sin referencias a RDA ni a hallazgos de auditoría como justificación** de una regla. La regla se escribe con su razón técnica («el texto sobre naranja es oscuro porque el blanco no alcanza contraste AA»), no con su historia («por la RDA-013»). La trazabilidad vive en `decisiones.md`, `auditoria-tecnica.md` y las bitácoras.
- Si algo todavía no existe, se describe como parte del diseño sin decir cuándo llega, o no se menciona.

`AGENTS.md` sí puede nombrar los archivos de `_planificacion/` y `docs/api/openapi.yaml` como lugares donde mirar, porque es una instrucción de lectura y no una justificación histórica.

## Documentos vivos

`producto.md`, `registro-pendientes.md` (con su backlog), `decisiones.md`, `auditoria-tecnica.md`, las especificaciones, el contrato OpenAPI, las épicas, las iteraciones, las bitácoras y las evidencias cambian durante el proyecto y sí llevan fechas, estados y trazabilidad. Cada dato tiene una sola fuente de verdad: el estado del trabajo vive en el registro, el comportamiento esperado en la especificación, el contrato técnico de la API en el OpenAPI, las decisiones en las RDA y lo que pasó en las bitácoras. Las reglas para mantenerlos ordenados están en `references/guia-documentos.md` y quedan escritas dentro de `_planificacion/README.md` para que los agentes las sigan.

## Convenciones por defecto

Son las convenciones con que viene la skill. Úsalas salvo que el desarrollador pida otras en la entrevista (bloque 6); lo que cambie se escribe en los documentos de su proyecto, no en esta skill. Más adelante cada desarrollador puede editar esta sección para dejar sus propias convenciones como valores por defecto.

- **Stack recomendado**: frontend con Astro, Tailwind CSS y PNPM; el backend según el tipo de proyecto (Python con Django o FastAPI, Java con Spring Boot o Quarkus, o Node.js con Express). Es solo la recomendación: manda el stack que elija el desarrollador. Los criterios se pueden editar en `references/stacks.md`.
- **Git**: commits, push, merge, pull, rebase, reset y tags los hace solo el desarrollador. El agente propone ramas con el formato `iteracion/XX-YY-descripcion-corta` (o `correccion/descripcion-corta` para una corrección fuera de iteración) y espera aprobación antes de crearlas.
- **Commits**: `Épica N - Iteración NN-NN: descripción` para el trabajo de una iteración y `Corrección: descripción` para una corrección fuera de iteración; una línea de máximo 200 caracteres, en el idioma del proyecto. El agente cuenta los caracteres antes de entregarlo. Si el desarrollador prefiere otro formato (por ejemplo, Conventional Commits), se usa ese en todos los documentos.
- **Despliegue**: el agente nunca despliega ni toca la configuración del hosting, del DNS ni de servicios externos (analítica, Search Console, anuncios).
- **Dependencias**: cada dependencia nueva requiere consulta, con su peso, la alternativa sin dependencia y, si es estructural, una RDA.
- **Estados**: `Pendiente`, `En curso`, `Bloqueada`, `En revisión`, `Terminada`, `Descartada`. El agente deja las iteraciones en `En revisión`; las pasa a `Terminada` el desarrollador. Las especificaciones usan `Borrador`, `Aprobada`, `Vigente` y `Reemplazada`.
- **Fechas**: formato ISO `AAAA-MM-DD`, en la zona horaria del desarrollador (pregúntala si no es evidente).
- **Desarrollador**: pregunta su nombre y un enlace para la autoría (portafolio, GitHub o LinkedIn).
