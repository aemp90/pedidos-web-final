# Registro de pendientes

**Backlog y fuente única de verdad del trabajo pendiente.** Si discrepa con cualquier otro documento de planificación sobre el estado del trabajo, prevalece este archivo.

- **Última actualización:** {{FECHA}}
- **Iteración activa:** {{ITERACION_ACTIVA}}
- **Próximo hito:** {{PROXIMO_HITO}}
- **Entrega comprometida:** {{FECHA_ENTREGA}}
- **Flujo de ramas vigente:** {{FLUJO_DE_RAMAS}}

---

## 1. Estado de épicas e iteraciones

| Iteración | Nombre | Épica | Estado | Depende de | Bitácoras |
| :-------- | :----- | :---- | :----- | :--------- | :-------- |
| 01-01 | {{NOMBRE}} | 01 | Pendiente | — | — |

## 2. Backlog

Lo comprometido que falta construir, ordenado por prioridad. Un elemento asignado a una iteración muestra su número; al cerrar la iteración se actualizan su fila y las de este backlog. Los terminados se quedan al final.

| Prioridad | Elemento | Tipo | Especificación | Iteración | Estado |
| :-------- | :------- | :--- | :------------- | :-------- | :----- |
| {{PRIORIDAD}} | {{ELEMENTO}} | {{FUNCIONALIDAD_MEJORA_CORRECCION_O_TECNICO}} | {{SPEC_O_GUION}} | {{ITERACION_O_GUION}} | Pendiente |

## 3. Datos pendientes del cliente

Mientras falten, se usa `PENDIENTE_CLIENTE` donde corresponda.

| Dato | Hallazgo | Necesario para | Estado |
| :--- | :------- | :------------- | :----- |
| {{DATO}} | {{AUD_O_GUION}} | {{ITERACION}} | Pendiente |

## 4. Decisiones por tomar

Decisiones del desarrollador que bloquean o condicionan trabajo. Mientras falten, se usa `PENDIENTE_DESARROLLADOR`.

| Tema | RDA o hallazgo | Responsable |
| :--- | :------------- | :---------- |
| {{TEMA}} | {{RDA_O_GUION}} | Desarrollador |

## 5. Propuestas fuera de alcance

Ideas detectadas durante el trabajo que no forman parte de lo comprometido. {{REGLA_DE_PROPUESTAS}} Si se aprueba una, pasa al backlog con una fila de historial.

| Propuesta | Origen | Fecha |
| :-------- | :----- | :---- |
| — | — | — |

## 6. Historial

Una fila por cambio, en una sola línea, con enlace a la bitácora que tiene el detalle. Nada se borra: si un cambio queda sin efecto, se agrega una fila que lo dice.

| Fecha | Cambio |
| :---- | :----- |
| {{FECHA}} | Creación de la documentación del proyecto: nivel {{NIVEL}}, {{N}} épicas y {{M}} iteraciones previstas. |
