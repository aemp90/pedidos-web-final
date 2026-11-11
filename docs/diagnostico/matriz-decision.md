# Matriz de decisión de Pedidos Legado

<!--
Etapa 3 · Decidir (modo por defecto).
1. Reparte los pesos entre los 5 criterios. Deben sumar 100 %.
2. Puntúa cada estrategia de 1 a 5 para cada módulo (5 = la mejor opción para ese criterio)
   y calcula el puntaje ponderado. Pídele a Claude que haga las cuentas, y revísalas.
3. La recomendación de cada módulo debe salir de la matriz, no de la intuición.
Punto de control E3: pesos que suman 100 %, al menos 4 módulos puntuados,
recomendación que se desprende de la matriz y resumen de 5 líneas.
-->

> **Estado: APROBADA (2026-11-10).** Pesos y puntajes propuestos por Claude a partir de `inventario.md`, `reglas-negocio.md`, `riesgos.md` y `dependencias.md`, y aprobados por el equipo (ver «Decisiones aprobadas»).

## Qué significa cada estrategia

| Estrategia | En este proyecto significa… |
|---|---|
| Mantener | Dejar el módulo en Delphi + DBISAM tal como está; solo correcciones urgentes. |
| Refactorizar | Corregirlo **dentro** de Delphi (parámetros SQL, transacción, unificar cálculos), sin cambiar de tecnología. |
| Modernizar | Portarlo a PedidosWeb (.NET 10, `PedidosWeb.Core` + API), conservando las reglas documentadas y aplicando las decisiones D1–D7, con tests xUnit contra el histórico. |
| Reemplazar | Sustituirlo por un producto o servicio estándar (ERP, CRM, BI, proveedor de identidad, motor de base de datos) en vez de construirlo. |

Escala: **5 = la mejor opción para ese criterio**. En Costo y Complejidad, 5 = barato / simple. En Riesgo, 5 = menos riesgo **neto**: lo que puede salir mal al ejecutar la estrategia **más** los riesgos RS-xx que deja vivos (si no se cuentan estos últimos, «Mantener» saldría siempre como la opción menos riesgosa, lo que es falso con RS-01 a RS-07 abiertos).

## Pesos

| Criterio | Qué mide | Peso |
|---|---|---|
| Costo | Cuánto cuesta en horas y dinero | 15 % |
| Complejidad | Qué tan difícil es técnicamente | 15 % |
| Riesgo | Qué puede salir mal en el camino (y qué riesgos quedan vivos) | 30 % |
| Impacto | Cuánto valor entrega al negocio | 25 % |
| Personas | Conocimiento disponible en el equipo | 15 % |
| **Total** |  | **100 %** |

Por qué estos pesos:
- **Riesgo 30 %:** el diagnóstico encontró 7 riesgos de prioridad Alta (RS-01 a RS-07): inyección SQL, clave en el código, sin autenticación, totales distintos en pantalla y en base, sin transacción, folios duplicados. Es lo que más diferencia a las estrategias.
- **Impacto 25 %:** la toma de pedidos y los totales son el corazón del negocio; un error ahí es plata mal cobrada (folio 1011) o mal informada a contabilidad (RS-08).
- **Costo, Complejidad y Personas 15 % cada uno:** importan, pero el sistema es chico (≈2.400 líneas en `legado/src/`), así que el costo no es lo que define la decisión.
- **Personas (confirmado el 2026-11-10):** el equipo domina .NET (ya existen HU-01, HU-02, `Clientes`, `TotalPedido` y sus tests) y tiene poco conocimiento de Delphi/DBISAM.

## Matriz

Puntaje ponderado = Σ (puntaje 1–5 × peso). Máximo posible: 5,00.

| Módulo | Mantener | Refactorizar | Modernizar | Reemplazar | Recomendación |
|---|---|---|---|---|---|
| Cálculo de totales (`UCalculoTotales` + cálculo de `UFrmPedidos`) | 2,35 | 2,85 | **4,40** | 2,10 | **Modernizar** |
| Toma de pedidos (`UFrmPedidos`) | 2,35 | 2,55 | **3,80** | 2,35 | **Modernizar** |
| Stock (`UStock`) | 2,35 | 3,00 | **4,00** | 2,20 | **Modernizar** |
| Clientes (`UFrmClientes`) | 2,65 | 2,60 | **4,00** | 2,70 | **Modernizar** |
| Productos (`UFrmProductos`) | 2,95 | 3,20 | **3,75** | 2,70 | **Modernizar** |
| Informes (`UFrmInformes`) | 2,35 | 3,15 | **3,85** | 3,25 | **Modernizar** |
| Datos, sesión y configuración (`UDMPrincipal`, `UGlobales`, `UFrmPrincipal`) | 2,35 | 2,45 | 3,10 | **3,95** | **Reemplazar** |
| Utilidades (`UUtilidades`: RUT, fechas, redondeo) | 2,65 | 2,90 | **3,90** | 3,45 | **Modernizar** |

### Puntajes por criterio (para revisar)

Orden de cada celda: **Costo / Complejidad / Riesgo / Impacto / Personas**.

| Módulo | Mantener | Refactorizar | Modernizar | Reemplazar |
|---|---|---|---|---|
| Cálculo de totales | 5/5/1/1/2 | 3/3/3/3/2 | 4/4/4/5/5 | 1/2/2/3/2 |
| Toma de pedidos | 5/5/1/1/2 | 2/2/3/3/2 | 2/2/4/5/5 | 1/2/2/4/2 |
| Stock | 5/5/1/1/2 | 4/3/3/3/2 | 4/3/4/4/5 | 1/1/2/4/2 |
| Clientes | 5/5/2/1/2 | 3/3/3/2/2 | 3/4/4/4/5 | 2/2/3/3/3 |
| Productos | 5/5/3/1/2 | 4/4/4/2/2 | 3/4/4/3/5 | 2/2/3/3/3 |
| Informes | 5/5/1/1/2 | 4/4/3/3/2 | 3/3/4/4/5 | 3/3/3/4/3 |
| Datos, sesión y configuración | 5/5/1/1/2 | 3/2/3/2/2 | 2/2/3/4/4 | 3/3/4/5/4 |
| Utilidades | 5/5/2/1/2 | 4/4/3/2/2 | 4/4/4/3/5 | 4/4/3/3/4 |

Patrones comunes a todas las filas:
- **Mantener** siempre tiene Costo y Complejidad 5 (no se hace nada) e Impacto 1 (no mejora nada); su Riesgo es bajo solo donde los riesgos abiertos son menores.
- **Refactorizar** tiene Personas 2 (Delphi) y, además, es trabajo que se bota cuando el módulo se modernice.
- **Modernizar** tiene Personas 5 (el equipo trabaja en .NET y ya hay base en Core).

### Por qué cada puntaje

**1. Cálculo de totales → Modernizar (4,40; margen 1,55).**
Es una unidad pura (`dependencias.md`: solo usa `SysUtils`), con las dudas ya resueltas (D1 límites `>=`, D3 sin navidad, D7 redondeo `Trunc(x + 0.49)`) y `TotalPedido.cs` ya existe en Core: Costo y Complejidad 4. Riesgo 4: se cierra RS-05 (dos totales distintos) con una sola función y tests contra folios reales como el 1011; no es 5 porque el redondeo y el `Double` (RS-09) exigen cuadrar con el histórico. Impacto 5: es lo que se cobra. Mantener tiene Riesgo 1 porque deja vivo RS-05. Reemplazar (motor de precios de un ERP) obliga a reconfigurar 11 reglas: Costo 1, Riesgo 2.

**2. Toma de pedidos → Modernizar (3,80; margen 1,25).**
Es el archivo más grande (420 + 226 líneas) y concentra RS-01, RS-05, RS-06, RS-07 y RS-13: Costo y Complejidad 2 en cualquier estrategia que lo toque. Modernizar tiene Riesgo 4 porque corrige por diseño la inyección SQL, la falta de transacción y el folio `MAX + 1`; no es 5 por la migración en sí y por las diferencias intencionales (D5a, D5b, D6, RS-18). Refactorizar en Delphi corrige lo mismo pero con Personas 2 y es trabajo desechable. Reemplazar por un ERP tiene Impacto 4, pero Riesgo 2: se pierden reglas propias (pedido mínimo fuera de la RM, bloqueo por deuda, margen VIP).

**3. Stock → Modernizar (4,00; margen 1,00).**
Unidad chica (93 líneas): Costo 4. Complejidad 3 porque el descuento debe ser atómico (RS-07: lee y escribe valor absoluto) y hay que crear de verdad el registro de movimientos (RS-12, `MOV_STOCK` no existe). Riesgo 4: con D6 se valida antes de grabar y se cierra RS-13 para pedidos nuevos. Reemplazar (WMS/inventario de ERP) es Costo y Complejidad 1: sobredimensionado para 40 productos.

**4. Clientes → Modernizar (4,00; margen 1,30).**
Ya existe `PedidosWeb.Core/Clientes` (Complejidad 4). Riesgo 4: se elige **un** validador de RUT (RS-04, hoy hay dos incompatibles) y se elimina el ID `MAX + 1` (RS-07). Impacto 4: un RUT mal validado bloquea pedidos. Mantener tiene Riesgo 2 (no 1): los RS de clientes son serios, pero menos que los de pedidos. Reemplazar por un CRM puntúa un poco más alto que Mantener, pero exige integrar la regla VIP y el saldo vencido.

**5. Productos → Modernizar (3,75; margen 0,55).**
Es el módulo más sano: CRUD simple, solo RS-01 en el filtro. Por eso Mantener tiene Riesgo 3 y Refactorizar Riesgo 4 / Costo 4 (3,20, el segundo mejor). Modernizar gana por Personas e Impacto, pero **con poco margen**: es el candidato natural a dejar para el final.

**6. Informes → Modernizar (3,85; margen 0,60).**
Hoy entrega a contabilidad un neto incorrecto (RS-08: IVA fijo y flete dentro del neto) y fechas `dd/mm/yy` (RS-10): Mantener tiene Riesgo 1. Reemplazar por una herramienta de BI queda segundo (3,25, Impacto 4), pero necesita primero los datos en una base moderna, así que depende del módulo 7. Si contabilidad pide tableros además del CSV, vale la pena reevaluar esta fila.

**7. Datos, sesión y configuración → Reemplazar (3,95; margen 0,85).**
Aquí «Reemplazar» significa usar piezas estándar en vez de construirlas: un motor de base de datos en lugar de DBISAM, un proveedor de identidad para el login (RS-03: hoy quien escribe «ADMIN» es nivel 9) y la configuración/secretos de .NET para la clave (RS-02) y las rutas (RS-16). Riesgo 4 e Impacto 5: cierra los tres riesgos de seguridad. Modernizar (construir nuestro propio login y acceso) queda en 3,10 por Costo y Complejidad 2: no conviene escribir autenticación propia. Ojo: elegir motor de base y proveedor de identidad es una decisión aparte (¿y agregar paquetes NuGet? hay que preguntar primero, según `AGENTS.md`).

**8. Utilidades → Modernizar (3,90; margen 0,45).**
Funciones puras (Costo y Complejidad 4). Riesgo 4: al portarlas se elige el RUT correcto (RS-04) y se usa `DateOnly` en vez de años de 2 dígitos (RS-10). Reemplazar por bibliotecas (BCL de .NET, paquete de RUT, calendario de feriados) queda cerca (3,45): en la práctica la modernización **usará** la BCL para fechas, y solo el RUT y los feriados serían candidatos a paquete externo.

### Sensibilidad: ¿cambia la recomendación si cambian los pesos?

| Escenario de pesos (C / X / R / I / P) | ¿Cambia alguna recomendación? | Módulos con margen más estrecho |
|---|---|---|
| Propuesta: 15 / 15 / 30 / 25 / 15 | — | Utilidades (0,45), Productos (0,55) |
| Iguales: 20 / 20 / 20 / 20 / 20 | No | Utilidades (0,40), Productos (0,60) |
| Costo primero: 30 / 20 / 20 / 15 / 15 | No | Toma de pedidos y Productos (0,15) |

La recomendación es estable con los tres juegos de pesos. Si priorizas costo, Toma de pedidos queda casi empatado con Refactorizar: es la fila que más conviene discutir.

## Decisiones aprobadas (2026-11-10)

| # | Pregunta | Decisión |
|---|---|---|
| M1 | Pesos (Costo 15 / Complejidad 15 / Riesgo 30 / Impacto 25 / Personas 15) | Aprobados |
| M2 | Supuesto de Personas (equipo .NET, poco Delphi) | Confirmado |
| M3 | Módulo 7: Reemplazar la base de datos DBISAM y el login propio por productos estándar | Aprobado. Motor de base de datos: **PostgreSQL** (cambiado desde MySQL el 2026-11-10). Proveedor de identidad: **ASP.NET Core Identity**, con los usuarios guardados en PostgreSQL (ambos decididos el 2026-11-10). Los paquetes NuGet necesarios se piden antes de agregarlos. Roles aprobados (reemplazan los niveles del legado, regla 24): **Vendedor** (nivel 1: toma pedidos y mantiene clientes normales), **Jefatura** (nivel 2, que el legado menciona pero nunca asigna: además crea clientes VIP y ve informes) y **Administrador** (nivel 9: todo, incluido borrar productos) |
| M4 | Productos y Utilidades, con margen bajo | Se modernizan, igual que el resto |

Resultado: **7 módulos se modernizan en PedidosWeb y 1 (datos, sesión y configuración) se reemplaza.** Ningún módulo se mantiene ni se refactoriza en Delphi; el legado sigue operando sin cambios hasta que cada módulo pase a PedidosWeb.

## Resumen ejecutivo

<!-- Exactamente 5 líneas, para alguien que no leerá el resto. -->

1. **Qué es y su estado:** Pedidos Legado es una aplicación Delphi + DBISAM de ≈2.400 líneas que funciona, pero con 7 riesgos de prioridad Alta y reglas duplicadas que no coinciden.
2. **Hallazgo principal (con evidencia):** el total que ve el cliente y el que se graba se calculan en dos lugares con reglas distintas (`UCalculoTotales.pas:77` vs. `UFrmPedidos.pas:296`; folio 1011).
3. **Riesgo más urgente:** seguridad: SQL concatenado (RS-01), clave de la base en el código (RS-02) y permisos según el nombre que se escribe (RS-03).
4. **Estrategia y por dónde empezar:** modernizar 7 de 8 módulos en PedidosWeb y reemplazar base de datos y login por productos estándar; empezar por Cálculo de totales y Utilidades (puras, puntaje 4,40 y 3,90).
5. **Próximo paso que se pide aprobar:** la spec de la historia «Cálculo de totales» con D1, D3 y D7, validada contra los totales históricos (PostgreSQL y ASP.NET Core Identity ya están decididos para datos y login).
