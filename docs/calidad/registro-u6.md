# Registro de calidad · Unidad 6

## Preparación

- **Tests en `Skip` (pendientes de unidades anteriores):** ninguno.

## Etapa 1 · Tests que sí prueban

- **Módulo elegido:** `CalculoTotales`.
- **Tests que propuso Claude:** 5 (8 casos con los `InlineData`).
- **Cambio que hice a mano para probarlos:** cambié un valor de `CalculoTotales.cs`; creo que fue el límite del 5 % (de 500.000 a 599.999), pero no recuerdo el valor exacto.
- **Tests que me quedé:** los 5, en `CalculoTotalesTests.cs` (commit `0ee070c`): `Calcular_BaseUnPesoBajoElUmbral_NoSubeDeTramo`, `Calcular_CategoriaOfeConOtraEscritura_NoEntraEnLaBase`, `Calcular_LineaSinCategoria_EntraEnLaBaseSinFallar`, `Calcular_LineaNula_LanzaArgumentExceptionConElNumeroDeLinea` y `Calcular_ListaNula_LanzaArgumentNullException`.
- **Tests que borré y por qué:** ninguno; todos fallaron con el cambio a mano.

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
