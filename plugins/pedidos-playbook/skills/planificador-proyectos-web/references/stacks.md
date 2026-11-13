# Stacks: recomendación, reglas y permisos

Cómo se elige el stack en la entrevista (los criterios de la sección 2 son los que trae la skill: puedes editarlos para dejar los tuyos), qué reglas técnicas y comandos lleva cada uno a `AGENTS.md` y cómo se arma `.claude/settings.json` con los fragmentos de `assets/permisos/`.

## Índice

1. Cómo preguntar y recomendar
2. Criterios de recomendación
3. Experiencia del desarrollador y riesgo
4. Fichas por stack
5. Proyectos fullstack y monorrepos
6. Armar `settings.json`

---

## 1. Cómo preguntar y recomendar

La pregunta del stack (bloque 1 de `entrevista.md`) tiene tres partes, en un solo mensaje:

1. **Qué stack prefiere el desarrollador** para este proyecto, si ya tiene uno en mente.
2. **Con qué stacks tiene experiencia**, de los que están en juego.
3. **Tu recomendación**, según el tipo de producto que describió, con su razón en una línea.

Ejemplo:

```text
Para el stack: ¿tienes uno en mente para el backend? ¿Y con cuáles tienes experiencia
(Python con Django o FastAPI, Java con Spring o Quarkus, Node.js con Express)?
Mi recomendación: frontend con Astro, Tailwind y PNPM, y backend con Django. Es un
sistema con varios módulos (pedidos, clientes, inventario) y un panel de administración,
y quieres sumar agentes de IA más adelante: el ecosistema de Python te deja eso a mano.
```

Si el desarrollador ya dijo el stack, no recomiendes otro: confirma versiones y gestor de paquetes. Si elige uno distinto al recomendado, no discutas; si ves un riesgo concreto, dilo una vez. El stack acordado se registra como RDA en `decisiones.md`, con la alternativa recomendada si no se eligió.

## 2. Criterios de recomendación

### Frontend

**Astro, Tailwind CSS y PNPM**, siempre que haya interfaz web: sitios, landings, sitios con contenido y paneles. Astro entrega HTML estático por defecto y suma interactividad solo donde se necesita (islas), y Tailwind mantiene los tokens de `DESIGN.md` en un solo lugar. Si el desarrollador pide otro framework, se usa ese y se adaptan las reglas y los permisos a mano a partir de `astro-pnpm.json`.

### Backend

El backend depende del tipo de proyecto:

| Si el proyecto… | Ecosistema | Opciones |
| :-------------- | :--------- | :------- |
| Es un sistema monolítico con varios módulos, se necesita rápido, y en el futuro puede sumar servicios en la nube o agentes de IA | **Python** | **Django** para un monolito modular con base de datos, autenticación y panel de administración incluidos. **FastAPI** para una API o un servicio acotado (por ejemplo, un servicio de IA) con validación automática y documentación OpenAPI generada |
| Exige mucha seguridad: sector público, banca, finanzas, salud, o datos sensibles en volumen | **Java** | **Spring Boot** por defecto: ecosistema maduro y Spring Security. **Quarkus** si la prioridad es correr en contenedores o en la nube con arranque rápido y poca memoria |
| Es chico y necesita un backend pequeño, conciso y minimalista (un formulario, pocas rutas, un intermediario hacia un servicio externo) | **JavaScript** | **Node.js con Express** y PNPM |

Cuando un proyecto calza en dos filas, manda el factor de mayor riesgo: un sistema modular que maneja datos financieros va a Java aunque sea pequeño. Dilo en la recomendación.

### Gestor de paquetes y herramientas

- **Node.js:** PNPM siempre.
- **Python:** pregunta si usa `uv` o `pip`. Si no tiene preferencia, recomienda `uv` (más rápido y con archivo de bloqueo). Los fragmentos de permisos cubren los dos.
- **Java:** Maven o Gradle, siempre con el *wrapper* del proyecto (`./mvnw`, `./gradlew`), para que todos usen la misma versión. Pregunta cuál usa.

## 3. Experiencia del desarrollador y riesgo

La experiencia entra al diagnóstico como el factor **riesgo técnico** de `niveles-complejidad.md`:

- Stack que el desarrollador domina: riesgo bajo.
- Una pieza nueva (por ejemplo, conoce Django pero no FastAPI): riesgo medio.
- Un ecosistema completo que no conoce (por ejemplo, nunca trabajó con Java): riesgo alto.

Si lo que recomiendas por tipo de proyecto choca con la experiencia del desarrollador, preséntale las dos opciones con lo que gana y lo que arriesga cada una. Por ejemplo: «Para un sistema bancario recomiendo Spring Boot, pero no lo has usado: el nivel sube a complejo y conviene una iteración de prueba de concepto al inicio. Con Django, que conoces, el riesgo baja, pero tendrás que reforzar tú la seguridad». Decide él.

## 4. Fichas por stack

Cada ficha dice qué va a `AGENTS.md` (comandos y reglas técnicas típicas), qué entra en la definición de terminado y qué fragmento de permisos usa. Los comandos exactos se confirman con el desarrollador o se leen de los archivos del proyecto (`package.json`, `pyproject.toml`, `pom.xml`): no inventes scripts que el proyecto no tiene.

### Astro, Tailwind y PNPM · `astro-pnpm`

- **Comandos típicos:** `pnpm dev`, `pnpm build`, `pnpm preview`, `pnpm astro check`, `pnpm format`.
- **Reglas técnicas:** las de frontend de `tipos-proyecto.md`, más: componentes `.astro` por defecto y una isla interactiva solo con justificación; tokens de `DESIGN.md` en la configuración de Tailwind (o en `@theme`, según la versión), nunca valores sueltos en las clases; imágenes con el componente de imagen de Astro y dimensiones explícitas.
- **Terminado:** `pnpm build` sin errores ni advertencias nuevas y `pnpm astro check` sin errores.

### Django · `django`

- **Comandos típicos:** `python manage.py runserver`, `check`, `test`, `makemigrations`, `migrate` (local); con `uv`, antecedidos de `uv run`.
- **Reglas técnicas:** una app de Django por módulo del negocio; lógica de negocio fuera de las vistas (en servicios o en los modelos); migraciones versionadas y nunca editadas una vez aplicadas; configuración sensible solo desde variables de entorno; `DEBUG` desactivado fuera de desarrollo.
- **Terminado:** `python manage.py check` sin errores, pruebas pertinentes en verde y migraciones creadas para todo cambio de modelo.
- **Permisos:** `migrate` se consulta, porque depende de a qué base apunta el entorno; `flush` y `dbshell` están prohibidos.

### FastAPI · `fastapi`

- **Comandos típicos:** `fastapi dev app/main.py` o `uvicorn app.main:app --reload`, `pytest`, `alembic revision --autogenerate` y `alembic upgrade head` (local).
- **Reglas técnicas:** modelos de entrada y salida con Pydantic; rutas agrupadas por recurso con `APIRouter`; dependencias para autenticación y sesión de base de datos; el OpenAPI que genera FastAPI se compara con `docs/api/openapi.yaml`, que manda.
- **Terminado:** `pytest` en verde y las pruebas de API pertinentes de `tipos-proyecto.md`.
- **Permisos:** `alembic upgrade` y `downgrade` se consultan.

### Spring Boot · `spring`

- **Comandos típicos:** `./mvnw spring-boot:run`, `./mvnw test`, `./mvnw verify` (o `./gradlew bootRun`, `test`, `build`).
- **Reglas técnicas:** capas separadas (controlador, servicio, repositorio); DTO en las fronteras, nunca entidades expuestas; validación con Bean Validation; seguridad con Spring Security y la regla de menor privilegio; migraciones con Flyway o Liquibase, nunca editadas una vez aplicadas; secretos fuera de `application.properties`.
- **Terminado:** `./mvnw verify` (o `./gradlew build`) sin errores.
- **Permisos:** editar `application*` y las migraciones se consulta; `deploy`, `release` y la limpieza de la base están prohibidos.

### Quarkus · `quarkus`

- **Comandos típicos:** `./mvnw quarkus:dev`, `./mvnw test`, `./mvnw package` (o `./gradlew quarkusDev`, `build`).
- **Reglas técnicas:** las de Spring Boot adaptadas (recursos REST, servicios, Panache o repositorios); extensiones de Quarkus antes que librerías sueltas; configuración por perfiles (`%dev`, `%prod`).
- **Terminado:** `./mvnw verify` (o `./gradlew build`) sin errores.
- **Permisos:** `quarkus:deploy` y `quarkus:image-push` se consultan.

### Node.js, Express y PNPM · `express-pnpm`

- **Comandos típicos:** `pnpm dev`, `pnpm start`, `pnpm test`, `pnpm lint`.
- **Reglas técnicas:** rutas, controladores y servicios separados aunque sean pocos archivos; validación de entradas en cada ruta; un manejador de errores central con el formato de error común; `helmet` y CORS configurados si la API es pública.
- **Terminado:** `pnpm test` en verde y las pruebas de API pertinentes.
- **Permisos:** las migraciones contra la base remota (`prisma migrate deploy`, `drizzle-kit push`) están prohibidas.

## 5. Proyectos fullstack y monorrepos

La estructura por defecto de un fullstack es un monorrepo con dos carpetas: `frontend/` (Astro) y `backend/` (el stack elegido). Si el desarrollador usa otra estructura, se respeta.

- `AGENTS.md` dice en «Comandos» desde qué carpeta se ejecuta cada uno.
- Con Django, pregunta si la interfaz va en Astro aparte (consume la API) o dentro del monolito (plantillas de Django con Tailwind). Lo segundo convierte el proyecto en uno solo, sin `frontend/`.

## 6. Armar `settings.json`

No escribas `settings.json` a mano. Combínalo con el script, que parte de `base.json` (Git, sistema de archivos, planificación, documentos protegidos, secretos y hosting) y suma un fragmento por pieza del stack:

```bash
# Ver los fragmentos disponibles
python scripts/combinar_permisos.py --listar

# Un solo stack en la raíz
python scripts/combinar_permisos.py --stack astro-pnpm --salida <dir>/.claude/settings.json

# Monorrepo fullstack con API REST
python scripts/combinar_permisos.py --stack astro-pnpm:frontend --stack django:backend \
    --api-rest --salida <dir>/.claude/settings.json
```

Después, revisa el resultado contra el proyecto real: carpetas de código que el fragmento no conoce, scripts que el proyecto no tiene, archivos de configuración propios. Los criterios de qué va en `allow`, `ask` y `deny` están en `tipos-proyecto.md`, «Cómo adaptar `settings.json` al stack», y `.claude/README.md` explica cada grupo de reglas.
