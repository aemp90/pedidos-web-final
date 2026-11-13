# Checklist de revisión · pedidos-web

## Antes de pedir revisión
- [ ] `dotnet test` en verde en local.
- [ ] Cada criterio de aceptación de la spec tiene su test.
- [ ] Leí mi propio diff línea por línea, incluido lo que generó la IA.

## Al revisar (persona)
- [ ] ¿Los tests prueban algo? ¿Fallarían si el código estuviera mal?
- [ ] ¿Entiendo cada línea? ¿Podría explicarla y mantenerla sin la IA?
- [ ] ¿Afecta a otros? ¿Rompe algo de contabilidad, del legado o de otro equipo que la IA no conoce?

## Con los hallazgos de la IA
- [ ] Cada hallazgo tiene respuesta: acepto, discutir o descarto, con argumento.
- [ ] Si el hallazgo es un bug, pido o escribo el test que falla antes de corregirlo.

## Merge
- [ ] CI en verde.
- [ ] Todos los hallazgos de la IA respondidos.
- [ ] Aprobación de al menos una persona.
- [ ] La descripción del PR trae la línea de uso de IA (`docs/gobernanza/politica-ia.md`, sección 5).

## Mi PR de HU-03
- **PR:** https://github.com/aemp90/pedidos-web-final/pull/5
- **Hallazgo de la IA que más discutimos:** ninguno. La revisión de `@claude` (2026-11-12) concluyó «no hay nada Importante»: `EntreFechas` usa `DateOnly`, incluye los límites del rango y rechaza un rango invertido, hay un test por criterio (CA-01 a CA-04) y no se tocan `AGENTS.md`, `REVIEW.md`, `legado/` ni paquetes NuGet.
- **Mi hallazgo humano:** ninguno registrado. El PR no tiene revisiones de personas ni descripción (tampoco la línea de uso de IA): lo que hoy pide la sección «Merge» de este checklist.
