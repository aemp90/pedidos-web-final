# Diagnóstico de complejidad y componentes por nivel

El nivel no mide el prestigio del proyecto: decide cuánto proceso necesita. Un sitio de cinco secciones con un formulario no gana nada con especificaciones formales y un contrato OpenAPI; una API con autenticación, pagos y tres consumidores pierde mucho sin ellos.

## 1. Cómo diagnosticar

Evalúa cada factor con lo que sabes de la entrevista, de los documentos adjuntos y, si el proyecto ya tiene código, del análisis de `proyecto-existente.md`. El riesgo técnico se mide contra la experiencia que declaró el desarrollador en la pregunta del stack (`stacks.md`, sección 3). Si un factor no se conoce, no lo supongas alto ni bajo: dilo en el diagnóstico.

| Factor | Bajo | Medio | Alto |
| :----- | :--- | :---- | :--- |
| Alcance funcional | Pocas secciones o recursos, casi todo contenido | Varias funcionalidades con lógica propia | Muchas funcionalidades que se cruzan entre sí |
| Diversidad de funcionalidades | Un solo tipo (mostrar información) | Formularios, búsqueda, paneles | Flujos de varios pasos, roles distintos, procesos de negocio |
| Integraciones externas | Ninguna, o un servicio de formularios o analítica | Una o dos APIs externas | Varias, o una crítica (pagos, facturación, identidad) |
| Persistencia y datos | Sin base de datos, o datos en archivos del repositorio | Base de datos con pocas entidades | Modelo con muchas relaciones, migraciones frecuentes, datos históricos |
| Autenticación y autorización | No hay | Inicio de sesión con un rol | Varios roles, permisos finos, sesiones de terceros |
| Seguridad y datos sensibles | Nada sensible | Datos personales básicos (correo, teléfono) | Datos personales sensibles, pagos, salud, menores |
| Arquitectura | Estático o un solo servicio | Frontend y API separados | Varios servicios, colas, tareas programadas, tiempo real |
| Riesgo técnico | Stack que el desarrollador domina | Una pieza nueva para el desarrollador | Un ecosistema que el desarrollador no conoce, tecnología inmadura, rendimiento exigente, migración de un sistema existente |
| Despliegue e infraestructura | Hosting estático con despliegue automático | Servidor o plataforma con base de datos | Varios entornos, contenedores, infraestructura propia |
| Mantenimiento previsto | Se publica y cambia poco | Evoluciona por etapas | Producto de largo plazo, otras personas o agentes lo mantendrán |

**Regla de clasificación (orientativa):**

- **Pequeño:** casi todos los factores en «bajo» y ninguno en «alto».
- **Mediano:** dos o más factores en «medio», o uno en «alto».
- **Complejo:** tres o más factores en «alto», o uno en «alto» en seguridad y datos sensibles junto con integraciones o autenticación en «medio» o más.

La regla ayuda a proponer; no reemplaza el criterio. Si un factor pesa más que los demás en este proyecto (por ejemplo, un sitio pequeño que procesa pagos), dilo y propón el nivel que corresponde al riesgo real.

## 2. Cómo presentar el diagnóstico

En el chat, corto y comprensible para alguien que no conoce la tabla:

```text
Diagnóstico propuesto: mediano.
Por qué: API REST con base de datos (persistencia media), inicio de sesión con un rol
(autenticación media) y una integración de correo. Sin datos sensibles ni pagos.
Qué activa: backlog priorizado, especificaciones para las funcionalidades con lógica,
plan técnico en las iteraciones, contrato OpenAPI y auditoría técnica.
Qué queda fuera: contract testing automático y versionado de la API (se propondrán si
aparece un segundo consumidor).
¿Lo confirmas o lo ajustas? Puedes cambiar el nivel o cualquier componente.
```

Si el desarrollador cambia el nivel o un componente, no discutas: ajusta, y si ves un riesgo concreto, menciónalo una vez. El nivel acordado se registra como `RDA-001 · Nivel de proceso` en `decisiones.md`, con los factores, los componentes activos y los que quedaron fuera a propósito.

## 3. Componentes por nivel

| Componente | Pequeño | Mediano | Complejo |
| :--------- | :------ | :------ | :------- |
| Entrevista | Bloque 1 y lo esencial del 2 | Completa | Completa, con detalle de riesgos e integraciones |
| `AGENTS.md`, `README.md`, `.claude/` | Sí; `AGENTS.md` corto | Sí | Sí |
| `DESIGN.md` (si hay interfaz) | Versión breve: concepto, color con contraste, tipografía y componentes clave | Completo | Completo |
| `_planificacion/README.md` | Compacto: solo las reglas de los componentes activos | Completo | Completo |
| `producto.md` | Breve | Sí | Sí |
| Backlog (en `registro-pendientes.md`) | Lista mínima | Priorizado | Priorizado, con dependencias entre elementos |
| `decisiones.md` (RDA) | Solo `RDA-001` y las decisiones estructurales iniciales | Sí | Sí, con seguimiento activo de reemplazos |
| `auditoria-tecnica.md` | Solo si hay insumos que auditar | Sí | Sí |
| Especificaciones (`20_especificaciones/`) | No: cada iteración lleva su descripción breve y sus criterios | Para cada funcionalidad con lógica o reglas de negocio | Para cada funcionalidad, más completas (escenarios y casos borde) |
| Plan técnico en la iteración | Opcional | Cuando la iteración toca arquitectura, datos, API o integraciones | Siempre, con dependencias entre tareas |
| Revisión al cerrar | Una o dos líneas en la bitácora | Sección «Revisión» de la bitácora | Bitácora y revisión al cierre de cada épica |
| Contrato OpenAPI (si expone una API REST) | Recomendado; las rutas pueden ir en la iteración | Obligatorio | Obligatorio y detallado (ejemplos, esquemas de error, seguridad) |
| Verificación obligatoria | Compilación, ejecución y criterios de aceptación | Lo mismo, más pruebas de API pertinentes si hay API REST | Lo mismo |
| Verificaciones recomendadas | Solo si el riesgo lo pide | Pruebas automatizadas de la lógica de negocio y de las rutas | Además, las que el riesgo justifique (contract testing, carga, seguridad), propuestas con una RDA |

Las recomendaciones se escriben como recomendaciones. Una verificación pasa a ser obligatoria solo si el desarrollador lo decide, y entonces se agrega a la «Definición de terminado» de `AGENTS.md`.

## 4. Cuándo una funcionalidad justifica una especificación

En nivel mediano y complejo, escribe `spec-<funcionalidad>.md` cuando se cumple al menos una de estas condiciones:

- Tiene reglas de negocio (cálculos, validaciones, estados, permisos).
- Tiene más de un camino posible (errores, casos borde, roles distintos).
- Expone o consume endpoints de una API.
- Varias iteraciones la van a tocar, o la van a mantener otras personas o agentes.

No la escribas para contenido estático, ajustes visuales que ya define `DESIGN.md`, cambios de configuración o tareas técnicas sin comportamiento visible (andamiaje, dependencias, formato). Para eso basta con el objetivo y los criterios de la iteración.

En nivel pequeño no hay carpeta de especificaciones. Si una funcionalidad puntual la merece (por ejemplo, un formulario que envía datos a un servicio externo), escribe su descripción y criterios dentro de la iteración con un poco más de detalle; si aparecen varias así, es señal de rediagnóstico.

## 5. Rediagnóstico

El nivel se revisa cuando aparece o desaparece un factor que lo decidió: autenticación nueva, una integración, datos personales, un segundo consumidor de la API, más personas en el proyecto, o al revés, un alcance que se recortó. El agente que lo detecta lo propone; el desarrollador decide. Si cambia, se registra una RDA nueva que reemplaza a la anterior, se agregan o retiran componentes desde ese momento (no se reescribe lo ya hecho) y el desarrollador ajusta `_planificacion/README.md` y `AGENTS.md` con la propuesta que le entrega el agente.
