# .claude · Permisos de Claude Code

Esta carpeta configura qué puede hacer Claude Code en este repositorio. `settings.json` no lleva comentarios porque JSON no los admite; la explicación de cada regla está aquí.

Los agentes de IA no editan esta carpeta: la mantiene el desarrollador.

## 1. Cómo se leen los permisos

`settings.json` tiene tres listas dentro de `permissions`:

| Lista | Qué significa |
| :---- | :------------ |
| `allow` | Claude Code lo hace sin pedir permiso. |
| `ask` | Claude Code pide aprobación cada vez. |
| `deny` | Claude Code no lo puede hacer, aunque se le pida. |

Si una acción calza con reglas de más de una lista, gana `deny`, luego `ask` y al final `allow`. Por eso `git branch *` puede estar en `ask` y `git branch -d *` en `deny`: listar o crear ramas se consulta, borrarlas está prohibido.

Dentro de cada lista, las reglas van agrupadas por tema y separadas por una línea en blanco, siempre en este orden: comandos del proyecto, Git, sistema de archivos, edición, lectura y web.

`defaultMode: "default"` significa que todo lo que no esté en ninguna lista también se consulta.

## 2. Lo que se permite sin preguntar (`allow`)

| Reglas | Por qué |
| :----- | :------ |
| {{COMANDOS_PERMITIDOS}} | Son los comandos de trabajo diario (desarrollo, compilación, revisión, formato, pruebas). Pedir permiso para cada uno solo frena la iteración. |
| `git status`, `git diff`, `git log`, `git show` | Solo leen el historial; no cambian nada. |
| `ls` | Solo lista archivos. |
| Edición de {{CARPETAS_EDITABLES}} | Es donde vive el código que el agente construye. |
| Edición de `_planificacion/00_producto/`, `10_epicas/`, `20_especificaciones/` y `99_bitacora/` | El agente actualiza el registro y su backlog, las RDA, las auditorías y las especificaciones, y escribe las bitácoras. `_planificacion/README.md` queda fuera: es un documento protegido. |
| `WebFetch` a {{DOMINIOS_DE_DOCUMENTACION}} | Documentación oficial del stack, para verificar sintaxis vigente en vez de recordarla. |

## 3. Lo que se consulta (`ask`)

| Reglas | Por qué |
| :----- | :------ |
| Agregar, quitar o actualizar dependencias | Cada dependencia tiene peso, mantenimiento y riesgo; el desarrollador decide. |
| `dlx` y `npx` | Descargan y ejecutan paquetes que no están en el proyecto. |
| `git branch`, `checkout`, `switch`, `stash`, `add`, `restore`, `fetch` | Cambian el estado de la copia de trabajo o preparan un commit. Las ramas siguen el formato `iteracion/XX-YY-descripcion-corta`. |
| `rm`, `mv` | Borrar o mover archivos no se deshace fácil. |
| Edición de {{ARCHIVOS_CONFIGURACION}} | Cambian cómo se instala, compila o formatea el proyecto. |

## 4. Lo que está prohibido (`deny`)

| Reglas | Por qué |
| :----- | :------ |
| Gestores de paquetes distintos de {{GESTOR_PAQUETES}} y cualquier `publish` | Otro gestor crea un archivo de bloqueo distinto y rompe la instalación; publicar paquetes no es parte del proyecto. |
| `git commit`, `push`, `merge`, `pull`, `rebase`, `cherry-pick`, `revert`, `reset`, `tag`, `clean`, `remote`, `config`, borrar ramas y `git -C` | Publican o reescriben historial. Los commits, pushes y merges los hace solo el desarrollador. `git -C` se bloquea porque permite ejecutar Git en otra carpeta y saltarse estas reglas. |
| `gh pr merge`, `gh repo`, `gh release` | Las mismas operaciones, hechas desde la CLI de GitHub. |
| `sudo`, `rm -rf` | Pueden dañar el sistema o borrar carpetas completas. |
| CLI de hosting y nube (`wrangler`, `vercel`, `netlify`, `railway`, `flyctl`, `heroku`, `gcloud`, `aws`, `az`, `kubectl`, `docker push`), también con `npx` o `pnpm dlx` | Desplegar y tocar la infraestructura lo hace solo el desarrollador. |
| Edición de `AGENTS.md`, `DESIGN.md`, `README.md` y `_planificacion/README.md` | Son los documentos protegidos del proyecto. El agente propone cambios y el desarrollador los aplica. |
| Edición de `.claude/`, `.git/`, `.env` y `.env.*` | El agente no cambia sus propios permisos, no toca la base de Git y no maneja secretos. Como `.env.*` incluye `.env.example` y `deny` gana sobre `ask`, ese archivo también lo edita el desarrollador: el agente le propone qué variables agregar. |
| Lectura de `.env` y `.env.*` | Contienen secretos que no deben llegar al contexto de un modelo. |
{{FILAS_DENY_ADICIONALES}}

## 5. Ajustes personales

Si necesitas permisos distintos solo en tu equipo, usa `.claude/settings.local.json`. Ese archivo es personal, no se versiona (debe estar en `.gitignore`) y se suma a las reglas de `settings.json`.

## 6. Instrucciones del proyecto: AGENTS.md

Claude Code carga `AGENTS.md` como instrucciones del proyecto solo si no encuentra un `CLAUDE.md` ni un `CLAUDE.local.md` en esta carpeta o más arriba. Si creas uno de esos archivos para instrucciones personales, su primera línea debe ser `@AGENTS.md`; si no, Claude Code deja de leer las reglas del proyecto sin avisar.

## 7. Comandos del proyecto

La carpeta `commands/` tiene atajos de Claude Code para los flujos de trabajo del proyecto. Cada comando es corto y remite a `_planificacion/README.md`, que es donde están los flujos completos: así los agentes que no usan estos atajos (Codex, Antigravity) siguen exactamente el mismo proceso.

| Comando | Qué hace |
| :------ | :------- |
| `/siguiente-epica` | Cierra la épica activa (revisión al cierre) y propone las iteraciones de la siguiente. Con tu aprobación, las genera todas de una vez. Sigue la sección 4.4 de `_planificacion/README.md`. |
