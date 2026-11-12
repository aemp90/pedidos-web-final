# Registro de calidad · Unidad 6

<!--
Lo completas tú durante el taller. Frases cortas: lo que hiciste y por qué.
Borra estos comentarios al terminar.
-->

## Preparación

- **Tests en `Skip` (pendientes de unidades anteriores):** <ninguno, o cuáles y su motivo>

## Etapa 1 · Tests que sí prueban

- **Módulo elegido:** <CalculoTotales, ClientesService o FolioFormatter>
- **Tests que propuso Claude:** <cuántos>
- **Cambio que hice a mano para probarlos:** <qué línea cambié>
- **Tests que me quedé:** <cuántos y cuáles>
- **Tests que borré y por qué:** <el que pasaba con y sin el cambio>

## Etapa 2 · Pipeline seguro

- **`ci.yml`:** https://github.com/aemp90/pedidos-web-final/actions/runs/43214122612
- **`claude.yml` en `main`:** sí (PR #1: https://github.com/aemp90/pedidos-web-final/pull/1)
- **Revisé:** permisos mínimos ☑ · timeout ☑ · versiones fijas ☑ · token solo como secreto ☑

## Etapa 3 · La falla

- **PR:** https://github.com/aemp90/pedidos-web-final/pull/2
- **Qué rompí (a mano):** `src/PedidosWeb.Core/Totales/CalculoTotales.cs:20`, `LimiteVolumenBajo` de `500_000m` a `599_999m` (commit `f3f42bf`); fallaron 16 de 189 tests.
- **Diagnóstico de `@claude` (en una línea):** el umbral del 5 % subió a 599.999 y contradice D1 (base ≥ 500.000); propone volver a `500_000m`.
- **Mi decisión:** Aceptar ☑ · Ajustar ☐ · Rechazar ☐
- **Por qué:** la causa y la corrección eran correctas y bastaban para arreglar la falla. Citó mal algunas líneas y no listó todos los tests en rojo (no leyó el log ni corrió los tests), pero eso no cambió la corrección.
- **Corrección y merge:** `9f0080e`; CI en verde ☑ ([run](https://github.com/aemp90/pedidos-web-final/actions/runs/43222882253)); merge ☑ (`165d3e6`)
