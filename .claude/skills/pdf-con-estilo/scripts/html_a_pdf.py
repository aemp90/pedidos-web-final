#!/usr/bin/env python3
"""Convierte un documento Markdown en un PDF con el diseño de la Skill pdf-con-estilo.

Uso:
    py .claude/skills/pdf-con-estilo/scripts/html_a_pdf.py <entrada.md> <salida.pdf>
    py .claude/skills/pdf-con-estilo/scripts/html_a_pdf.py <entrada.md> <salida.pdf> --html <vista.html>

Pasos:
1. Lee el Markdown (UTF-8) y lo convierte a HTML con la librería `markdown`
   (tablas, bloques de código y listas). El HTML crudo del Markdown se muestra como
   texto: así los marcadores de plantilla como `<tu nombre>` no desaparecen.
2. Inserta el resultado en assets/plantilla.html, reemplazando {{titulo}} y {{contenido}}.
   El diseño vive solo en la plantilla: este script no tiene CSS.
3. Imprime el HTML con Chromium (Playwright) en A4, con fondos y numeración de páginas.

Requisitos: `py -m pip install playwright markdown` y `py -m playwright install chromium`.
"""

import argparse
import html
import re
import sys
from pathlib import Path

try:
    import markdown
    from markdown.extensions import Extension
    from playwright.sync_api import sync_playwright
except ImportError as error:
    sys.exit(
        f"Falta una dependencia ({error.name}). Instala con:\n"
        "  py -m pip install playwright markdown\n"
        "  py -m playwright install chromium"
    )

RAIZ_SKILL = Path(__file__).resolve().parent.parent
PLANTILLA_POR_DEFECTO = RAIZ_SKILL / "assets" / "plantilla.html"

PIE_DE_PAGINA = (
    '<div style="width:100%;font-size:8px;color:#6b7280;padding:0 18mm;'
    'display:flex;justify-content:space-between;font-family:Segoe UI,Arial,sans-serif;">'
    "<span>{titulo}</span>"
    '<span>Página <span class="pageNumber"></span> de <span class="totalPages"></span></span>'
    "</div>"
)


class SinHtmlCrudo(Extension):
    """Desactiva el HTML crudo en el Markdown: se escapa y se muestra como texto."""

    def extendMarkdown(self, md):
        md.preprocessors.deregister("html_block")
        md.inlinePatterns.deregister("html")


def extraer_titulo(texto_md: str, respaldo: str) -> str:
    """Toma el primer encabezado de nivel 1 como título del documento."""
    coincidencia = re.search(r"^#\s+(.+?)\s*#*\s*$", texto_md, flags=re.MULTILINE)
    return coincidencia.group(1).strip() if coincidencia else respaldo


def markdown_a_html(texto_md: str, plantilla: str, titulo: str) -> str:
    contenido = markdown.markdown(
        texto_md,
        extensions=["tables", "fenced_code", "sane_lists", SinHtmlCrudo()],
        output_format="html5",
    )
    # Se reemplaza el contenido al final para que un "{{titulo}}" dentro del documento no se toque.
    return plantilla.replace("{{titulo}}", html.escape(titulo)).replace("{{contenido}}", contenido)


def html_a_pdf(documento_html: str, salida: Path, titulo: str) -> None:
    with sync_playwright() as p:
        navegador = p.chromium.launch()
        try:
            pagina = navegador.new_page()
            pagina.set_content(documento_html, wait_until="load")
            pagina.emulate_media(media="print")
            pagina.pdf(
                path=str(salida),
                format="A4",
                print_background=True,  # sin esto se pierden los colores de fondo
                prefer_css_page_size=True,
                display_header_footer=True,
                header_template="<div></div>",
                footer_template=PIE_DE_PAGINA.format(titulo=html.escape(titulo)),
            )
        finally:
            navegador.close()


def main() -> int:
    parser = argparse.ArgumentParser(description="Markdown → PDF con el diseño de pdf-con-estilo.")
    parser.add_argument("entrada", type=Path, help="Archivo Markdown de entrada")
    parser.add_argument("salida", type=Path, help="Archivo PDF de salida")
    parser.add_argument("--plantilla", type=Path, default=PLANTILLA_POR_DEFECTO,
                        help="Plantilla HTML (por defecto, assets/plantilla.html de la Skill)")
    parser.add_argument("--titulo", help="Título del documento (por defecto, el primer '# ' del Markdown)")
    parser.add_argument("--html", type=Path, help="Guarda también el HTML intermedio, para revisarlo")
    args = parser.parse_args()

    if not args.entrada.is_file():
        print(f"No existe el archivo de entrada: {args.entrada}", file=sys.stderr)
        return 1
    if not args.plantilla.is_file():
        print(f"No existe la plantilla: {args.plantilla}", file=sys.stderr)
        return 1

    texto_md = args.entrada.read_text(encoding="utf-8")
    titulo = args.titulo or extraer_titulo(texto_md, args.entrada.stem)
    documento_html = markdown_a_html(texto_md, args.plantilla.read_text(encoding="utf-8"), titulo)

    if args.html:
        args.html.write_text(documento_html, encoding="utf-8")

    args.salida.parent.mkdir(parents=True, exist_ok=True)
    html_a_pdf(documento_html, args.salida, titulo)
    print(f"PDF generado: {args.salida}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
