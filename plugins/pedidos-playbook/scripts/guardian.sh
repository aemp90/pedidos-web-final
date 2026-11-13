#!/usr/bin/env bash
# guardian.sh · hook PreToolUse de pedidos-web
# Bloquea (exit 2) lo que las reglas de permisos de settings.json no alcanzan a cubrir:
#   1. Leer secretos por la shell: el deny de Read(.env / *.pem) no cubre `cat .env`.
#   2. git commit / push / merge: según AGENTS.md los hace una persona tras leer el diff.
#   3. Escribir AGENTS.md, .idea/, .env o *.pem.
# jq no está instalado, así que se busca sobre el JSON crudo que llega por stdin.

entrada="$(cat)"

bloquear() {
  echo "guardian.sh: $1" >&2
  exit 2
}

campo() {
  # Valor de texto de un campo del JSON (primera aparición; respeta \" y \\).
  printf '%s' "$entrada" \
    | grep -oE '"'"$1"'"[[:space:]]*:[[:space:]]*"([^"\\]|\\.)*"' \
    | head -n1 \
    | sed -E 's/^"[^"]*"[[:space:]]*:[[:space:]]*"//; s/"$//'
}

herramienta="$(campo tool_name)"

case "$herramienta" in
  Bash|PowerShell)
    comando="$(campo command)"
    if printf '%s' "$comando" | grep -Eiq '(^|[^[:alnum:]_])\.env([^[:alnum:]_.]|$)|\.pem([^[:alnum:]_]|$)'; then
      bloquear "el comando toca .env o un .pem; los secretos no se leen desde Claude."
    fi
    if printf '%s' "$comando" | grep -Eiq '\bgit\b[^|;&]*[[:space:]](commit|push|merge)\b'; then
      bloquear "commit, push y merge los hace una persona después de leer el diff (AGENTS.md)."
    fi
    ;;
  Edit|Write|MultiEdit|NotebookEdit)
    ruta="$(campo file_path)"
    [ -z "$ruta" ] && ruta="$(campo notebook_path)"
    # Las rutas de Windows llegan como C:\\a\\b en el JSON: se normalizan a C:/a/b.
    ruta="$(printf '%s' "$ruta" | sed 's#\\\\#/#g; s#\\#/#g')"
    case "$ruta" in
      AGENTS.md|*/AGENTS.md|.idea/*|*/.idea/*)
        bloquear "AGENTS.md y .idea/ no se tocan (AGENTS.md, reglas de trabajo)." ;;
      .env|*/.env|*.pem)
        bloquear "no se escriben secretos ni claves en archivos del repo." ;;
    esac
    ;;
esac

exit 0
