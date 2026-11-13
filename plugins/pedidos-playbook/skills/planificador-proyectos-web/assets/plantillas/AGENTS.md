# AGENTS.md · {{NOMBRE_PROYECTO}}

Instrucciones de trabajo para cualquier agente de IA (Claude Code, Antigravity, Codex u otro) que colabore en este repositorio. Este archivo es la entrada principal.

---

## 1. Contexto en 30 segundos

- **Cliente:** {{CLIENTE_Y_RUBRO_O_PROYECTO_PROPIO}}.
- **Objetivo:** {{OBJETIVO_EN_UNA_FRASE}}.
- **Alcance:** {{ALCANCE}}. {{REGLA_DE_ALCANCE}}
- **Dominio:** {{DOMINIO}}.
- **Desarrollador responsable:** {{DESARROLLADOR_Y_ENLACE}}.

## 2. Documentos protegidos

`AGENTS.md`, `DESIGN.md`, `README.md` (raíz) y `_planificacion/README.md` son la referencia estable del proyecto y **no los edites**.

Si crees que alguno debería cambiar, entrega al desarrollador una propuesta con:

1. El archivo y la sección.
2. El texto actual y el texto que propones.
3. Por qué conviene el cambio y qué pasa si no se hace.

El desarrollador decide y hace la edición. Mientras tanto, sigue el documento tal como está.

## 3. Orden de lectura obligatorio antes de trabajar

1. `_planificacion/00_producto/registro-pendientes.md`: backlog, qué está pendiente y cuál es la iteración activa. Es la fuente única de verdad del trabajo.
2. El archivo de la iteración activa en `_planificacion/10_epicas/<epica>/`.
3. La especificación que la iteración implementa, en `_planificacion/20_especificaciones/`, y el contrato `docs/api/openapi.yaml` si la tarea toca la API.
4. `_planificacion/00_producto/decisiones.md`: RDA vigentes. No contradigas una RDA aceptada.
5. `DESIGN.md`: fuente de verdad del diseño. Léelo siempre que la tarea toque la interfaz.
6. `README.md` (raíz): descripción del proyecto y de su arquitectura.
7. `_planificacion/README.md`: estructura de la planificación, ciclos de trabajo, convenciones, reglas de mantenimiento y plantillas.
8. El resto de `_planificacion/` según la tarea: `producto.md` ante dudas de alcance o tono, `auditoria-tecnica.md` si la iteración cierra hallazgos, y las bitácoras anteriores de la misma épica.

Si algo de lo anterior está en conflicto, detente y pregunta.

<!-- PLANTILLA: en el punto 3, deja solo lo que existe en el proyecto (especificaciones desde nivel mediano, contrato si hay API REST con OpenAPI); si no existe nada, borra el punto y renumera. Quita también DESIGN.md en un backend. Borra este comentario. -->

## 4. Stack y versiones

- {{STACK_LINEA_1}}
- {{STACK_LINEA_2}}
- Hosting: {{HOSTING}}.

## 5. Comandos

```bash
{{COMANDOS_CON_COMENTARIO}}
```

Usa siempre {{GESTOR_PAQUETES}}. Nunca otro gestor de paquetes. No dejes servidores de desarrollo corriendo al terminar.

## 6. Límites de actuación

**Prohibido (lo hace solo el desarrollador):**

- `git commit`, `git push`, `git merge`, `git pull`, `git rebase`, `git reset`, `git tag` y cualquier operación que reescriba o publique historial.
- Desplegar o modificar la configuración del hosting, del DNS o de servicios externos ({{SERVICIOS_EXTERNOS}}).
- Editar los documentos protegidos (sección 2), `.claude/`, `.git/` o archivos `.env`.

**Requiere consulta previa:**

- Crear o cambiar de rama. Propón el nombre con el formato `iteracion/XX-YY-descripcion-corta` (por ejemplo `iteracion/02-01-tokens-diseno`), o `correccion/descripcion-corta` para una corrección fuera de iteración.
- Editar archivos de configuración de la raíz ({{ARCHIVOS_CONFIGURACION}}).
- Instalar, actualizar o eliminar dependencias. Cada dependencia nueva requiere justificar su peso, la alternativa sin dependencia y, si es estructural, una RDA.
- Eliminar o mover archivos.

**Permitido sin consulta:** editar {{CARPETAS_EDITABLES}} y `_planificacion/` (salvo `_planificacion/README.md`); ejecutar los comandos de la sección 5; leer el repositorio.

## 7. Reglas de contenido

1. Todo el texto visible y toda la documentación están en **{{IDIOMA_Y_VARIANTE}}**, trato de **{{TRATO}}**, lenguaje simple y directo, sin anglicismos innecesarios.
2. **No inventes datos.** {{DATOS_SENSIBLES}} vienen de {{FUENTE_UNICA_DATOS}} y deben estar confirmados por el cliente.
3. Si falta un dato, usa un marcador explícito: `PENDIENTE_CLIENTE` para datos del negocio y `PENDIENTE_DESARROLLADOR` para decisiones técnicas o de diseño. Regístralo en `registro-pendientes.md`. Nunca publiques un dato de relleno que parezca real.
4. **No inventes resultados.** Una prueba que no ejecutaste, una medición que no hiciste o un estado que no comprobaste no se escriben como hechos.
5. Nombres de archivo y carpeta en ASCII: minúsculas, guiones, sin tildes ni ñ.

## 8. Reglas técnicas

{{REGLAS_TECNICAS_NUMERADAS}}

<!-- PLANTILLA: la sección 9 va solo en backend y fullstack. En un frontend, bórrala y renumera las siguientes. Borra también este comentario. -->

## 9. Convenciones de la API

{{CONVENCIONES_API_GENERALES}}

El contrato de cada ruta (endpoints, parámetros, modelos, códigos HTTP y errores) está en `docs/api/openapi.yaml`. El contrato cambia antes que el código: si una tarea cambia una ruta, actualiza primero el OpenAPI y después implementa. Si encuentras una diferencia entre el contrato y el código, detente y pregunta cuál está bien; no ajustes el contrato para que calce con un error.

## 10. Flujo de trabajo por iteración

1. Lee el registro y la iteración activa (sección 3).
2. Comprueba la definición de preparada (sección 12.1). Si no se cumple, no implementes: avisa al desarrollador qué falta.
3. Si la iteración requiere una rama, propón el nombre y espera aprobación.
4. Sigue el plan técnico y las tareas de la iteración. Implementa solo lo descrito en los criterios de aceptación. Si detectas trabajo fuera de alcance, anótalo como propuesta en `registro-pendientes.md`; no lo hagas.
5. Ejecuta la iteración completa. Resuelve por tu cuenta todo lo que puedas, incluidas las verificaciones contra producción con las herramientas que tengas permitidas. Pide datos al desarrollador solo si, después de intentarlo por todos los medios, no puedes obtenerlos: en ese caso sigue la sección «Evidencia de producción» de `_planificacion/README.md`.
6. Verifica según la definición de terminado (sección 12.2).
7. Escribe la bitácora en `_planificacion/99_bitacora/<epica>/` con la plantilla de `_planificacion/README.md`, incluida su revisión: resultado frente al objetivo, qué se aprendió y qué cambia para lo que sigue.
8. Actualiza en la misma pasada `registro-pendientes.md` (estado de la iteración, filas del backlog que cubrió, historial de una línea con enlace a la bitácora) y, si corresponde, la especificación, `decisiones.md` y `auditoria-tecnica.md`. No toques documentos que el trabajo no afectó.
9. Deja la iteración en `En revisión` y entrega al desarrollador un resumen con los archivos modificados y un mensaje de commit sugerido con el formato `Épica N - Iteración NN-NN: descripción`, en una sola línea de máximo 200 caracteres y en {{IDIOMA_Y_VARIANTE}}; cuenta los caracteres antes de entregarlo. Por ejemplo: `Épica 2 - Iteración 02-01: agrega los tokens de color, tipografía y espaciado`. El commit lo hace el desarrollador.
10. Si era la última iteración de su épica, en la misma entrega cierra la épica y propón las iteraciones de la siguiente, según la sección 4.4 de `_planificacion/README.md`: revisión al cierre, propuesta con nombre y objetivo de cada iteración, y espera de aprobación. Con la aprobación, genera todas las iteraciones de la épica siguiente de una vez. El desarrollador también puede pedirlo en cualquier momento («planifica la siguiente épica» o `/siguiente-epica`).

## 11. Cambios y correcciones

Cuando te pidan corregir un error o cambiar algo que ya existe:

1. Entiende el pedido. Si es un error, reprodúcelo antes de corregirlo.
2. Identifica qué funcionalidades, componentes y documentos afecta.
3. Clasifícalo con una pregunta: **¿después del cambio, la especificación, el contrato y las RDA siguen describiendo bien el sistema?**
   - **Sí, es menor** (el código no cumplía lo especificado, un texto, un ajuste dentro de `DESIGN.md`): no toques la especificación. Agrega una fila de tipo `Corrección` al backlog, implementa, verifica contra los criterios existentes y registra una línea de historial. Escribe una bitácora breve solo si hubo mediciones o decisiones que conviene conservar.
   - **No, es significativo** (cambia el comportamiento esperado, una regla de negocio, el contrato, el modelo de datos o la arquitectura): actualiza primero la especificación o el contrato, o propone una RDA; si afecta un documento protegido, entrega la propuesta. Espera la aprobación del desarrollador y trabaja el cambio como una iteración con plan, tareas y bitácora.
   - **Si dudas**, pregunta antes de tocar el código.
4. Actualiza solo los documentos que el cambio afectó.
5. Si la corrección no pertenece a la iteración activa, el commit sugerido es `Corrección: descripción`, con las mismas reglas de largo e idioma.

Los detalles están en `_planificacion/README.md`, sección 4.3.

## 12. Definición de preparada y de terminado

### 12.1 Preparada: antes de empezar una iteración

Una iteración está lista para empezar solo cuando:

1. Su objetivo cabe en una o dos frases.
2. Sus criterios de aceptación se pueden comprobar con un comando o una medición.
3. Si implementa una especificación, está `Aprobada`.
4. Las iteraciones de las que depende están `Terminada`.
5. Los datos del cliente que necesita están confirmados, o está acordado trabajar con `PENDIENTE_CLIENTE` mientras llegan.
6. Cabe en el tamaño máximo: un objetivo, revisable en unos 30 minutos y 8 tareas como máximo. Si no cabe, propón partirla antes de empezar.

Si falla alguna, no empieces: di qué falta y propón cómo resolverlo. Esta lista también se usa al generar las iteraciones de una épica, para que nazcan preparadas.

### 12.2 Terminado: antes de dejarla en revisión

Un trabajo queda listo para revisión (`En revisión`) solo cuando:

1. {{VERIFICACION_OBLIGATORIA}}
2. La aplicación se ejecuta y comprobaste la funcionalidad que tocaste.
3. Cada criterio de aceptación que marcas tiene su evidencia escrita: el comando, la medición o la comprobación que hiciste y su resultado.
4. Los documentos afectados están al día, y solo esos.
5. Escribiste la bitácora (o la línea de historial, si es una corrección menor).

**Verificaciones recomendadas** (no son requisito; propónlas cuando el cambio lo justifique y dilo como recomendación): {{VERIFICACIONES_RECOMENDADAS}}.

## 13. Cuándo detenerse y preguntar

- La iteración no cumple la definición de preparada (sección 12.1).
- Falta un dato del cliente que bloquea la tarea.
- La tarea contradice una RDA, una especificación aprobada, el contrato de la API, `DESIGN.md` o el alcance.
- Una funcionalidad importante no tiene requisitos claros, o su especificación está en `Borrador`.
- Hay que agregar una dependencia, cambiar configuración o crear una rama.
- Conviene cambiar un documento protegido (sección 2).
- La tarea introduce un factor que cambia la complejidad del proyecto (autenticación, una integración nueva, datos personales o pagos, un nuevo consumidor de la API): propón revisar el nivel de proceso.
- Un cambio empeoraría {{METRICAS_CRITICAS}}.
