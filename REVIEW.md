# Instrucciones de revisión · pedidos-web

## Qué es Importante aquí
- Lo que cambia un monto cobrado al cliente (totales, descuentos, impuestos) o se aleja
  del legado o de la spec sin una decisión escrita.
- Dinero que no es `decimal` en pesos chilenos sin decimales (`double` o `float` es Importante siempre).
- Fechas de pedido que no son `DateOnly`, o límites de rango mal tratados (inclusive/exclusive).
- Un criterio de aceptación de la spec sin su test xUnit, o un test sin valor esperado concreto.
- Funcionalidad nueva sin spec en `specs/<historia>/spec.md`.
- Secretos, claves o cadenas de conexión en el código o en archivos versionados.
- Dependencias de infraestructura en `src/PedidosWeb.Core`, o lógica de dominio en `Program.cs`
  en vez de delegar en Core.
- Paquetes NuGet nuevos, o cambios en `legado/`, `.idea/`, `AGENTS.md` o `REVIEW.md`.

Estilo, nombres y patrones de diseño son Detalle como máximo.

## Tope de detalles
Máximo 3 detalles por revisión.

## No reportar
- Lo que ya valida el CI (compilación y tests en verde).
- Todo lo que está en `legado/` y en `migracion/` (golden master), salvo que el PR los modifique.

## Revisar siempre
1. Montos en `decimal`, nunca `double` ni `float`.
2. Fechas de pedido en `DateOnly`; los límites de un rango coinciden con la spec.
3. Un test por criterio de aceptación, en `tests/PedidosWeb.Core.Tests/<Namespace>/<Clase>Tests.cs`,
   con nombre `Metodo_Escenario_Resultado`.
4. Entradas inválidas: qué pasa con `null`, listas vacías o rangos invertidos.
5. Clases, métodos y propiedades del dominio en español.

## Evidencia
Cada hallazgo cita `archivo:línea`, su severidad (Importante o Detalle) y por qué importa.
Si no hay nada Importante, se dice en una línea: no se inventan hallazgos.
