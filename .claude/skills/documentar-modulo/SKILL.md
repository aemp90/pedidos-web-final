---
name: documentar-modulo
description: "Genera o actualiza la documentación de un módulo de PedidosWeb.Core (Pedidos, Clientes, Lineas, Folios) en docs/modulos/<Modulo>.md. Úsala cuando pidan documentar, explicar o describir un módulo, carpeta o namespace del dominio, aunque no nombren la Skill."
---

# Documentar un módulo

## Pasos
1. Identifica el módulo: es una carpeta de `src/PedidosWeb.Core/<Modulo>/` (= namespace `PedidosWeb.Core.<Modulo>`).
2. Lee todas sus clases, interfaces y records; marca qué es `public` (eso es la API pública).
3. Lee sus tests en `tests/PedidosWeb.Core.Tests/<Modulo>/` para entender el comportamiento esperado y sacar ejemplos reales.
4. Revisa `src/PedidosWeb.Api/Program.cs` para ver qué endpoints usan el módulo.
5. Escribe o actualiza `docs/modulos/<Modulo>.md` con las secciones de abajo.
6. Ejecuta `dotnet build` para confirmar que los nombres citados existen.

## Qué debe tener docs/modulos/<Modulo>.md
- Propósito
- API pública
- Ejemplo de uso
- Diagrama Mermaid

## Reglas
- Escribe en español de Chile.
- Documenta solo lo que existe en el código; no inventes métodos ni comportamientos.
- El ejemplo de uso debe compilar y basarse en un test existente cuando sea posible.
- Montos en pesos chilenos como `decimal` sin decimales; fechas de pedido como `DateOnly`.
- No modifiques `src/` ni `tests/` al documentar: solo escribe en `docs/modulos/`.
- El diagrama Mermaid usa `classDiagram` para las clases o `flowchart` para el flujo con la Api.
