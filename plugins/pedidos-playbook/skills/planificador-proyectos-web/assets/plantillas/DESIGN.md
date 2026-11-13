# DESIGN.md · Sistema de diseño de {{NOMBRE_PROYECTO}}

Fuente única de verdad del diseño del proyecto. {{ORIGEN_DEL_DISENO}}

---

## 1. Concepto visual

**{{CONCEPTO_EN_UNA_LINEA}}.** {{DESCRIPCION_DEL_CONCEPTO}}

Principios:

1. **{{PRINCIPIO_1}}.** {{RAZON_1}}
2. **{{PRINCIPIO_2}}.** {{RAZON_2}}
3. **{{PRINCIPIO_3}}.** {{RAZON_3}}

## 2. Color

### 2.1 Superficies

| Token | Hex | Uso |
| :---- | :-- | :-- |
| {{TOKEN}} | {{HEX}} | {{USO}} |

### 2.2 Acento y texto

| Token | Hex | Uso |
| :---- | :-- | :-- |
| {{TOKEN}} | {{HEX}} | {{USO}} |

### 2.3 Contraste

Contrastes medidos según WCAG 2.2:

| Combinación | Ratio | Resultado |
| :---------- | :---- | :-------- |
| {{TEXTO}} sobre {{FONDO}} | {{RATIO}} | {{AA_AAA_O_FALLA_Y_REGLA}} |

Regla práctica: {{REGLA_DE_CONTRASTE}}.

## 3. Tipografía

| Rol | Familia | Pesos | Uso |
| :-- | :------ | :---- | :-- |
| Display | {{FAMILIA}} | {{PESOS}} | {{USO}} |
| Texto | {{FAMILIA}} | {{PESOS}} | {{USO}} |

Fallbacks: {{FALLBACKS}}. Origen de las fuentes: {{ORIGEN_FUENTES}}.

### 3.1 Escala tipográfica

| Token | Tamaño / interlínea | Tracking | Peso | Familia |
| :---- | :------------------ | :------- | :--- | :------ |
| {{TOKEN}} | {{TAMANO}} | {{TRACKING}} | {{PESO}} | {{FAMILIA}} |

Reglas:

1. {{REGLA_TIPOGRAFICA}}

## 4. Espaciado, retícula y forma

| Token | Valor |
| :---- | :---- |
| {{TOKEN}} | {{VALOR}} |

- Contenedor: {{CONTENEDOR}}.
- Retícula: {{RETICULA}}.
- Radios: {{RADIOS}}.
- Sombras: {{SOMBRAS}}.

## 5. Componentes

| Componente | Descripción y reglas |
| :--------- | :------------------- |
| {{COMPONENTE}} | {{REGLAS_CON_SUS_EXCEPCIONES_Y_RAZONES}} |

### 5.1 Botones

| Variante | Fondo | Texto | Hover |
| :------- | :---- | :---- | :---- |
| Principal | {{FONDO}} | {{TEXTO}} | {{HOVER}} |
| Secundario | {{FONDO}} | {{TEXTO}} | {{HOVER}} |

Todos: {{REGLAS_COMUNES_DE_BOTONES_Y_FOCO}}.

## 6. Iconografía

{{LIBRERIA_Y_FORMATO}}

| Uso | Ícono |
| :-- | :---- |
| {{USO}} | {{ICONO}} |

Íconos decorativos con `aria-hidden="true"`; los que actúan solos como botón llevan `aria-label` en el elemento interactivo.

## 7. Imágenes

1. {{ORIGEN_DE_LAS_IMAGENES}}
2. {{FORMATOS_Y_OPTIMIZACION}}
3. Todas con texto alternativo descriptivo en español y dimensiones explícitas.

## 8. Movimiento

- Permitido: {{MOVIMIENTO_PERMITIDO}}.
- No se usa: {{MOVIMIENTO_PROHIBIDO}}.
- Con `prefers-reduced-motion: reduce`, {{COMPORTAMIENTO_REDUCIDO}}.

## 9. Implementación de los tokens

{{DONDE_VIVEN_LOS_TOKENS_Y_COMO_SE_USAN}}

## 10. Voz y textos

- {{IDIOMA_Y_VARIANTE}}, trato de {{TRATO}}, frases cortas. Primero la acción, después la explicación.
- {{REGLAS_DE_VOZ}}
