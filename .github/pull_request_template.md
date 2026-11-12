<!-- Completa cada sección. Si una no aplica, escribe "No aplica" y por qué. -->

## Historia y spec
<!-- Ej.: HU-03 · specs/HU-03-pedidos-por-fecha/spec.md -->
- Historia:
- Spec:

## Qué cambia
<!-- 1 a 3 líneas: qué hace ahora el sistema que antes no hacía. -->

## Criterios de aceptación → tests
| CA | Test (`Metodo_Escenario_Resultado`) | Valor esperado |
|----|-------------------------------------|----------------|
| CA-01 |  |  |

## Fuera de alcance
<!-- Lo que la spec excluye y este PR respeta; lo que queda para otro PR. -->

## Decisiones que no están en la spec
<!-- Elecciones del autor que el revisor debe validar
     (ej.: lanzar excepción vs. acumular errores, orden del resultado). -->

## Entradas inválidas
<!-- Qué pasa con null, listas vacías, rangos invertidos, montos negativos… -->

## Checklist del autor
- [ ] Hay un test xUnit por CA, con valor esperado concreto.
- [ ] El dinero está en `decimal` CLP sin decimales; no hay `double` ni `float`.
- [ ] Las fechas de pedido son `DateOnly`, y los límites (inclusive o exclusive) coinciden con la spec.
- [ ] No hay lógica de dominio en `Program.cs`, y Core no depende de infraestructura.
- [ ] No hay secretos, claves ni cadenas de conexión.
- [ ] No agregué paquetes NuGet ni toqué `legado/`, `.idea/`, `AGENTS.md` ni `REVIEW.md` (si lo hice, explico por qué arriba).
- [ ] Ejecuté `dotnet test` en local y pasa en verde.
