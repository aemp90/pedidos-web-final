---
name: pdf-con-estilo
description: "Convierte un documento Markdown del repo (planes, políticas, diagnósticos, specs) en un PDF con el diseño estándar de pedidos-web: encabezado de color, tablas legibles, notas destacadas y numeración de páginas. Úsala cuando pidan generar, exportar o imprimir un .md como PDF, o entregar un documento «para la gerencia», aunque no nombren la Skill."
---

# PDF con estilo

Convierte un `.md` en un `.pdf` que siempre se ve igual, sea cual sea el documento. El diseño vive solo en `assets/plantilla.html`; el script no tiene CSS.

## Requisitos
- Python 3 con Playwright y Chromium, y la librería `markdown`:
  - Windows: `py -m pip install playwright markdown` y `py -m playwright install chromium`
  - macOS, Linux o Git Bash: lo mismo con `python3`
- Si falta alguno, el script lo dice y termina. No instales nada sin avisarle a la persona.

## Pasos
1. Confirma que el `.md` de entrada existe y léelo. Si tiene marcadores sin completar (`<tu nombre>`, `<fecha>`, celdas vacías), avísale a la persona antes de generar: el PDF los mostrará tal cual.
2. Desde la raíz del repo, ejecuta:
   ```bash
   py .claude/skills/pdf-con-estilo/scripts/html_a_pdf.py <entrada.md> <salida.pdf>
   ```
   Por defecto, el PDF va junto al `.md`, con el mismo nombre.
3. Comprueba que el PDF existe y que su tamaño no es 0. Si la persona quiere revisar el diseño, agrega `--html <vista.html>` (fuera del repo, por ejemplo en una carpeta temporal) para guardar el HTML intermedio.
4. Informa la ruta del PDF y cualquier advertencia del paso 1.

## Reglas
- **No toques `assets/plantilla.html` para un documento puntual.** Todos los PDF comparten el mismo diseño; si hay que cambiarlo, se cambia para todos y en un commit aparte.
- No modifiques el `.md` de entrada para «que se vea mejor» sin pedirlo: el PDF refleja el documento tal cual.
- El título del PDF es el primer `# ` del Markdown; usa `--titulo` solo si no tiene uno.
- El HTML crudo dentro del Markdown se muestra como texto (así `<tu nombre>` no desaparece); los comentarios `<!-- -->` no salen en el PDF.
- Las sublistas indentadas con 2 espacios (estilo GitHub) se anidan bien: el script ajusta la indentación.
- No dejes archivos intermedios (`.html`) dentro del repo.
- Escribe los mensajes en español de Chile.
