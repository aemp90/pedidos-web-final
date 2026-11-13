# Variantes por tipo de proyecto

La estructura base es la misma para todos los proyectos. Lo que cambia según el tipo es si existe `DESIGN.md`, qué secciones lleva `AGENTS.md`, qué reglas técnicas se escriben, si hay contrato OpenAPI y qué permisos tiene `.claude/settings.json`. Lo que cambia según el nivel de complejidad está en `niveles-complejidad.md`, y lo que cambia según el stack (recomendación, comandos, reglas técnicas propias y permisos) está en `stacks.md`: las tres dimensiones se combinan.

## Frontend (sitio, landing, SPA)

- **`DESIGN.md`: sí.** Fuente única de verdad del diseño: concepto, color con contrastes medidos, tipografía, espaciado, componentes, botones, iconografía, imágenes, movimiento, implementación de los tokens y voz. En nivel pequeño, versión breve (ver `guia-documentos.md`).
- **`AGENTS.md`**, reglas técnicas típicas: HTML semántico, un solo `h1`, cero JavaScript por defecto, sin recursos de terceros en tiempo de ejecución, accesibilidad (WCAG 2.2 AA, foco visible, objetivos táctiles de 44 × 44 px, movimiento reducido), metas de rendimiento (Lighthouse móvil, LCP, CLS, INP), imágenes optimizadas con dimensiones explícitas, nombres de componentes.
- **Definición de terminado**: compilación sin errores ni advertencias nuevas, revisión en móvil (360 px) y escritorio, y los criterios de aceptación. Formato y revisión de tipos entran como obligatorios si el stack trae esos comandos y el desarrollador lo confirma.
- **`producto.md`**: pone peso en el público y en las condiciones de uso (dispositivo, señal, apuro), porque de ahí salen las decisiones de diseño.

## Backend o API sin interfaz

- **`DESIGN.md`: no se genera.** No hay interfaz que documentar, y un archivo vacío confunde a los agentes.
- **`AGENTS.md` suma la sección «Convenciones de la API»**, que reemplaza lo que en un frontend haría `DESIGN.md`. Lleva las reglas generales, que valen para todas las rutas, en pocas líneas:
  - Estilo de la API (REST, GraphQL, RPC) y convención de rutas (plural, kebab-case, anidación máxima).
  - Versionado, si lo hay.
  - Autenticación y autorización.
  - Formato de las respuestas y de los errores (estructura, códigos HTTP por caso, mensajes en el idioma del proyecto si llegan al usuario).
  - Validación de entradas, paginación, filtros y orden.
  - Seguridad: secretos solo en variables de entorno, nunca en el repositorio; límites de tasa; CORS.
  - Dónde está el contrato (`docs/api/openapi.yaml`) y la regla de alineación (ver abajo).

  El detalle de cada ruta no va en `AGENTS.md`: lo lee el agente en cada sesión y conviene que no supere unas 200 líneas. Va en el contrato OpenAPI.
- **Reglas técnicas típicas**: migraciones de base de datos versionadas y nunca editadas una vez aplicadas, registro de errores sin datos personales, y cada ruta nueva comprobada contra sus criterios con una petición reproducible.
- **Definición de terminado**: compilación (si el stack compila), la API arranca, los criterios de aceptación y las pruebas de API pertinentes (ver abajo). Linter, revisión de tipos y pruebas automatizadas son obligatorios solo si el desarrollador lo decide; si no, quedan como recomendación.
- **`README.md` raíz**: explica conceptualmente qué recursos expone la API y para qué, sin listar rutas con su sintaxis.
- **`settings.json`**: suma a `deny` la ejecución de migraciones contra entornos que no sean el local y cualquier comando que toque bases de datos remotas. Por ejemplo, según el stack: `Bash(pnpm prisma migrate deploy *)`, `Bash(pnpm drizzle-kit push *)`, `Bash(psql *)`, `Bash(mysql *)`, la CLI de la base de datos gestionada. Suma a `allow` la edición del contrato: `Edit(/docs/api/**)`.

## Fullstack

- **`DESIGN.md`: sí**, solo para la interfaz.
- **`AGENTS.md`** lleva las reglas de frontend y la sección «Convenciones de la API», y explica cómo se comunican las dos partes (contrato, tipos compartidos o generados desde el OpenAPI, dónde vive cada cosa en el repositorio).
- **Épicas**: conviene separar la fundación (repositorio, entorno, base de datos), el sistema de diseño, el núcleo de la API y las funciones que cruzan ambas capas. Cada iteración dice qué capa toca.
- **`settings.json`**: une los permisos de ambas variantes.

## Contrato de una API REST

Aplica a backend y fullstack que exponen endpoints REST. Si la API no es REST, o el proyecto no expone API, nada de esta sección se exige.

**Qué es cada documento, para no duplicar:**

| Documento | Responde | No repite |
| :-------- | :------- | :-------- |
| `AGENTS.md`, «Convenciones de la API» | Las reglas que valen para todas las rutas | El detalle de cada ruta |
| `docs/api/openapi.yaml` | El contrato técnico: endpoints, métodos, parámetros, modelos de solicitud y respuesta, códigos HTTP, errores, esquemas de seguridad, paginación y filtros | Las reglas de negocio ni el porqué |
| `spec-<funcionalidad>.md` | El comportamiento esperado, las reglas de negocio, los escenarios y los criterios de aceptación | Los esquemas: nombra las operaciones por su `operationId` |

**Por nivel:**

- **Pequeño:** OpenAPI recomendado, no obligatorio. Si no hay contrato, las rutas de cada iteración (método, ruta, entrada, respuesta y errores principales) se escriben en la iteración.
- **Mediano:** `docs/api/openapi.yaml` (OpenAPI 3.1) obligatorio desde la primera iteración que expone un endpoint. Al arrancar, se genera con `info`, `servers`, esquemas de seguridad, el formato de error común y los endpoints de las primeras iteraciones que ya estén especificados; el resto se agrega cuando se especifica.
- **Complejo:** lo mismo, con ejemplos de solicitud y respuesta y todos los casos de error documentados.

**Regla de alineación:** el contrato cambia antes que el código. Si una tarea cambia una ruta, primero se actualiza el OpenAPI (y la especificación si cambia el comportamiento), después se implementa, y al cerrar se comprueba que la implementación responde lo que dice el contrato. Si el agente encuentra una diferencia entre el contrato y el código, se detiene y pregunta cuál de los dos está bien: no ajusta el contrato para que calce con un error.

**Pruebas de API pertinentes (obligatorias desde nivel mediano):** cada endpoint nuevo o modificado se ejercita contra sus criterios con peticiones reproducibles: el caso exitoso, al menos un error de validación, y el acceso sin autorización si la ruta está protegida. Pueden ser pruebas automatizadas o un archivo de peticiones versionado; la bitácora registra cuáles se ejecutaron y su resultado.

**Lo que no se impone:** contract testing automático, versionado avanzado o validación automática de compatibilidad. Se proponen con una RDA cuando hay razones concretas (consumidores de terceros, API pública, varios equipos).

## Cómo adaptar `settings.json` al stack

`settings.json` se arma con `scripts/combinar_permisos.py` a partir de los fragmentos de `assets/permisos/` (uno por stack: Astro, Django, FastAPI, Spring Boot, Quarkus, Express), como explica `stacks.md`, sección 6. Después se ajusta al proyecto real. Si el stack no tiene fragmento, parte del más parecido y cambia los comandos. En cualquier caso, conserva siempre:

- En **allow**: los comandos de desarrollo, compilación, revisión, formato y pruebas; lectura de Git (`status`, `diff`, `log`, `show`); edición de las carpetas de código y de `_planificacion/` (incluida `20_especificaciones/`), y de `docs/api/` si hay contrato.
- En **ask**: instalar, actualizar o quitar dependencias; crear o cambiar ramas; mover o borrar archivos; editar archivos de configuración de la raíz.
- En **deny**: toda operación de Git que publique o reescriba historial; la CLI del hosting (`wrangler`, `vercel`, `railway`, etc.), también invocada con `pnpm dlx`, `pnpm exec` o `npx`; la CLI de la base de datos remota; edición de `.claude/`, `.git/`, `.env*` y de los cuatro documentos inmutables; lectura de `.env*`.

**Cuidado con los conflictos entre listas.** `deny` gana siempre, así que una regla en `ask` o `allow` que calce con una de `deny` no sirve. El caso típico: `Edit(/.env.*)` en `deny` también bloquea `.env.example`. No lo pongas en `ask`; deja el archivo a cargo del desarrollador y explícalo en `.claude/README.md`. Antes de entregar, revisa que ninguna regla de `allow` o `ask` quede anulada por una de `deny` sin que lo sepas.

Si el gestor de paquetes del proyecto es uno, deniega los otros (por ejemplo, con PNPM se deniegan `npm install` y `yarn`), para que el agente no genere un archivo de bloqueo distinto.
