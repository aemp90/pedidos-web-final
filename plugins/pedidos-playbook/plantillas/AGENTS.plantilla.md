# AGENTS.md · <Nombre del proyecto>

<!--
Plantilla del playbook de Pedidos Web. Cópiala a la raíz de tu repo como AGENTS.md y
reemplaza todo lo que está entre < >. Escríbela tú: el guardián del plugin bloquea
que Claude Code edite AGENTS.md.
-->

## Proyecto
<Qué es el sistema, para quién y qué reemplaza o moderniza, en 2 o 3 líneas.>

## Comandos
- Compilar: `<comando>`
- Tests: `<comando>`
- Ejecutar: `<comando>` (<URL o forma de comprobar que levantó>)

## Estilo de código
- Clases, métodos y propiedades del dominio en español.
- Dinero: `decimal`; nunca `double` ni `float`. <Moneda y decimales.>
- <Tipos de fecha, capas del proyecto y qué depende de qué.>

## Tests
- <Framework> en `<carpeta de tests>`: una carpeta por namespace, un archivo `<Clase>Tests` por clase.
- Nombre: `Metodo_Escenario_Resultado`.
- Ejecuta los tests al terminar cada tarea de implementación, antes de darla por terminada.

## Seguridad
- Nunca escribas secretos, claves ni cadenas de conexión en el código ni en archivos versionados.
- `.env` y `*.pem` no se leen desde Claude Code (lo bloquea el guardián del playbook).

## Reglas de trabajo
- Responde, comenta el código y escribe en <idioma>.
- Toda funcionalidad parte de una spec aprobada en `specs/<historia>/spec.md`.
- Un test por criterio de aceptación.
- Nunca hagas commit, push ni merge: lo hace una persona después de leer el diff.
- Antes de abrir un PR, usa el subagente `revisor` del playbook.
- Solo trabajas en <carpetas permitidas>; no toques <carpetas prohibidas> ni este `AGENTS.md`.
- No agregues dependencias sin preguntar primero.
