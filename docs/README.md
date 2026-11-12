# docs/

Documentación del equipo de Pedidos Web.

| Archivo | Unidad | Qué es |
|---|---|---|
| `mapa-sdlc.md` | U1 | Mapa de uso de IA en el ciclo de vida del software: qué se delega, qué revisa una persona y con qué riesgo. |
| `plantilla-spec-breve.md` | U3 | Plantilla para la spec breve de la HU-02. |
| `plantilla-skill.md` | U3 | Plantilla para la Skill `documentar-modulo`. |
| `modulos/` | U3 | Documentación de cada módulo, generada con tu Skill `documentar-modulo` (por ejemplo, `ClientesService.md`). |
| `diagnostico/` | U4 | Diagnóstico de Pedidos Legado: inventario, dependencias, reglas de negocio, riesgos y matriz de decisión (con el resumen ejecutivo). Trae 5 plantillas. |
| `plantilla-subagente.md` | U4 | Plantilla del subagente `analista-legado`. El archivo final va en `.claude/agents/analista-legado.md`. |
| `migracion/` | U5 | Migración del cálculo de totales: `defectos-corregidos.md` (qué corregiste y qué casos cambian) y `plan-migracion.md` (etapas, convivencia, riesgos y criterio de corte). |
| `gobernanza/` | U8 | Gobernanza del uso de IA: `auditoria-legado.md` (hallazgos del legado validados y descartados) y `politica-ia.md` (datos que nunca entran, controles probados, lo no cubierto y transparencia en los PR). Trae 2 plantillas. |

Las reglas que se aplican solas (permisos y hooks) viven en `.claude/`, no aquí.
Las specs de cada historia van en `specs/`, no aquí.
