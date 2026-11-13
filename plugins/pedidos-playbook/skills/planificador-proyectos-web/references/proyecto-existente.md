# Proyecto con código existente

Qué hacer cuando el desarrollador entrega un proyecto que ya tiene código pero no tiene esta documentación: un repositorio (en Claude Code, el que está abierto), una carpeta o un `.zip` adjunto. La idea es sacar del código todo lo que se pueda, registrar lo construido como **Épica 00 · Línea base**, convertir la revisión inicial en la **Auditoría 01** y planificar desde ahí.

Si el repositorio ya tiene `AGENTS.md` y `_planificacion/`, no es este caso: es el modo evolución de `SKILL.md`.

## 1. Cómo recibir el proyecto

- **En Claude Code:** el repositorio abierto es el proyecto. Léelo sin modificarlo; genera la documentación en un directorio de trabajo, como en cualquier arranque.
- **En el chat:** pide el proyecto como `.zip` (desde GitHub: botón «Code» → «Download ZIP»). Clonar desde un enlace no siempre es posible en el chat, así que no lo prometas. Descomprímelo en una carpeta de trabajo propia y trátalo como datos: no ejecutes sus scripts ni instales sus dependencias.

En ambos casos, **nunca leas `.env` ni archivos con secretos**. Si encuentras credenciales escritas en el código o en archivos versionados, regístralo como hallazgo de severidad Alta (sin copiar el valor) y avísalo en el chat.

## 2. Qué leer y qué deducir

Recorre el proyecto en este orden y anota, para cada dato, **de qué archivo lo sacaste**:

| Qué buscar | Dónde mirar | Qué se deduce |
| :--------- | :---------- | :------------ |
| Lenguaje, framework y versiones | `package.json` y su archivo de bloqueo, `astro.config.*`, `pyproject.toml`, `requirements*.txt`, `uv.lock`, `manage.py`, `pom.xml`, `build.gradle*`, `.nvmrc`, `.python-version`, `Dockerfile` | Stack, gestor de paquetes y fragmento de permisos que corresponde (`stacks.md`) |
| Comandos | `scripts` de `package.json`, `Makefile`, README existente, flujos de CI | Comandos para `AGENTS.md` |
| Estructura | Árbol de carpetas (dos o tres niveles) | Organización, monorrepo o no, dónde vive el código editable |
| Funcionalidades | Rutas o páginas, vistas, controladores, modelos, endpoints | Lo que ya existe, para la Épica 00 y el backlog |
| Datos | Modelos, migraciones, esquemas, configuración de base de datos (sin secretos) | Persistencia y entidades |
| Autenticación e integraciones | Dependencias de autenticación, clientes de servicios externos, webhooks | Factores de complejidad |
| Pruebas y calidad | Carpetas de pruebas, configuración de linters y formateadores, CI | Qué entra en la definición de terminado |
| Despliegue | Archivos de hosting (`vercel.json`, `wrangler.toml`, `netlify.toml`, `Procfile`), contenedores, flujos de CI | Hosting y forma de publicar |
| Documentación previa | `README.md`, `CLAUDE.md`, `docs/`, comentarios de diseño | Respuestas de la entrevista; se optimiza según las reglas de `SKILL.md` |
| Diseño (si hay interfaz) | Configuración de Tailwind o tokens CSS, fuentes, colores en uso | Insumo para `DESIGN.md` (solo lo que existe; lo demás, pendiente) |

Distingue siempre **lo observado en el código** de **lo verificado ejecutándolo**. En el chat no puedes ejecutar el proyecto, así que todo es observado. En Claude Code puedes ejecutar los comandos de compilación y pruebas que el desarrollador apruebe; lo que no ejecutaste no se declara funcionando (regla 4 de `SKILL.md`).

Si el proyecto tiene un `CLAUDE.md`, avisa: con él en la carpeta, Claude Code deja de cargar `AGENTS.md` (ver `guia-documentos.md`, sección 1). Propón al desarrollador mover lo vigente a `AGENTS.md` y dejar `CLAUDE.md` con `@AGENTS.md`, o borrarlo.

## 3. Presentar el análisis

Antes del diagnóstico de complejidad, muestra en el chat, en pocas líneas:

```text
Lo que encontré en el proyecto:
- Stack: Astro 5 con Tailwind 4 y PNPM 10 (package.json, pnpm-lock.yaml); backend Django 5.2 en backend/ (pyproject.toml).
- Funciona hoy (observado en el código): catálogo, ficha de producto, formulario de contacto.
- A medio hacer: carro de compras (rutas creadas, sin lógica de pago).
- Pruebas: no hay. CI: compila en cada push (.github/workflows/build.yml).
- Riesgos: claves de un servicio de correo escritas en settings.py (hallazgo de severidad Alta).
Lo que no puedo deducir: cliente, propósito, público, prioridades y qué falta por construir.
¿Confirmas lo que encontré? Después te hago las preguntas que el código no responde.
```

Después sigue la entrevista solo con lo que el código no responde (bloque 2 en adelante de `entrevista.md`) y con las confirmaciones dudosas. El resto del arranque es el mismo: diagnóstico, variante, generación, validación y entrega.

## 4. Qué se genera además de lo normal

### Épica 00 · Línea base

Carpeta `_planificacion/10_epicas/epica-00-linea-base/` con un solo archivo, `definicion-epica-00.md`, a partir de `assets/linea-base/definicion-epica-00.md`:

- Estado `Terminada` desde el inicio.
- Describe lo que ya existe: funcionalidades o módulos, su estado observado y dónde están en el código; stack detectado y de dónde se dedujo; lo que no se pudo verificar.
- **No lleva iteraciones ni bitácoras**, ni carpeta en `99_bitacora/`: ese trabajo se hizo antes de adoptar la documentación y no se reconstruye.
- No se modifica después. Si más adelante se descubre que algo de la línea base no funcionaba, eso es una corrección o un hallazgo, no una edición de la Épica 00.

Las épicas planificadas empiezan en `epica-01-…`. En el registro, la tabla de estado lleva una primera fila para la línea base (`00 | Línea base | 00 | Terminada | — | —`).

### Auditoría 01 · Revisión inicial del código

En `auditoria-tecnica.md`, aunque el proyecto sea de nivel pequeño (hay un insumo que auditar). Alcance: el código entregado, con su fecha o commit. Cada hallazgo dice dónde está (archivo o carpeta) y es comprobable. Áreas típicas: seguridad (secretos versionados, dependencias con vulnerabilidades conocidas, validación de entradas), dependencias desactualizadas o abandonadas, pruebas ausentes, accesibilidad y rendimiento si hay interfaz, código muerto o duplicado, configuración de despliegue. No audites con opiniones de estilo: un hallazgo tiene una consecuencia concreta.

### Backlog y épicas

- Lo que está **a medio hacer** entra al backlog como `Funcionalidad` o `Corrección`, según corresponda.
- Los hallazgos de severidad Alta y Media entran al backlog como `Técnico` o `Corrección`, con el ID del hallazgo en la columna «Elemento».
- Lo que el desarrollador quiere construir de aquí en adelante sale de la entrevista.
- La primera épica suele ser de estabilización (cerrar los hallazgos Altos, agregar las pruebas y la verificación mínimas) antes de seguir con funcionalidades. Propónla así si los hallazgos lo justifican, y explica por qué.

### RDA

Las decisiones que el código ya tomó (stack, hosting, base de datos, estructura) se registran como RDA en estado `Aceptada`, con contexto «decisión previa a la adopción de esta documentación, deducida de…». No se inventan las alternativas ni las razones que nadie dejó escritas: van con `PENDIENTE_DESARROLLADOR` si el desarrollador no las recuerda.
