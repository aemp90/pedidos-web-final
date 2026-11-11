---
name: postgres
description: "Sube, baja o revisa el contenedor PostgreSQL local de Pedidos Web (infra/docker-compose.yml). Uso: /postgres subir | bajar | estado | logs."
argument-hint: "subir | bajar | estado | logs"
disable-model-invocation: true
allowed-tools: Bash(docker compose *), Bash(docker info *), PowerShell(docker compose *), PowerShell(docker info *), PowerShell(Copy-Item *), PowerShell(Test-Path *), Read
---

# Contenedor PostgreSQL local

Acción pedida: `$ARGUMENTS`

Todos los comandos se ejecutan con `-f infra/docker-compose.yml` desde la raíz del repo (o `cd infra`).

## Pasos
1. Si `$ARGUMENTS` viene vacío o no es una de las acciones de abajo, muestra el uso (`/postgres subir | bajar | estado | logs`) y detente.
2. Verifica que Docker esté corriendo con `docker info --format "{{.ServerVersion}}"`. Si falla, pide abrir Docker Desktop y detente.
3. Ejecuta la acción:

| Acción  | Comando |
|---------|---------|
| `subir`  | `docker compose -f infra/docker-compose.yml up -d --wait` |
| `bajar`  | `docker compose -f infra/docker-compose.yml down` |
| `estado` | `docker compose -f infra/docker-compose.yml ps` |
| `logs`   | `docker compose -f infra/docker-compose.yml logs --tail 50 postgres` |

## Reglas
- Antes de `subir`: si no existe `infra/.env`, cópialo desde `infra/.env.example` y pide a la persona que escriba `POSTGRES_PASSWORD`. **Nunca** inventes, escribas ni muestres la contraseña.
- `bajar` nunca usa `-v` (borraría los datos) salvo que la persona lo pida explícitamente en el argumento (ej. `/postgres bajar borrando datos`); en ese caso confirma antes de ejecutarlo.
- Al terminar, informa en una o dos líneas el estado del contenedor `pedidos-pg` (corriendo/saludable, detenido, o el error exacto).
