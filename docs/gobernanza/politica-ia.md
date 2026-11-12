# Política de uso de IA · pedidos-web

<!--
BORRADOR. Las celdas «PENDIENTE» esperan las notas de la etapa 2.
No marques nada como probado sin haberlo intentado violar.
Borra este comentario al terminar.
-->

## 1. Datos que nunca entran a Claude

- **Secretos y claves** (`.env`, `infra/.env`, certificados `*.pem`, contraseñas, tokens). Si entran al contexto, salen del equipo y no se pueden recuperar.
- **Cadenas de conexión y claves de base de datos**, incluida la del legado (`CLAVE_BD`, ver AU-01 en `auditoria-legado.md`). AGENTS.md prohíbe escribirlas en código o en archivos versionados.
- **Datos reales de clientes** (RUT, nombres, saldos, deudas). En el taller solo se usan datos de ejemplo en memoria y valores falsos.

## 2. Controles activos

Están en `.claude/settings.json` y `.claude/hooks/guardian.sh`.

| Regla | Control | ¿Probado? (fecha) | Resultado |
|---|---|---|---|
| No modificar el legado | `deny: Edit(legado/**)` | PENDIENTE | PENDIENTE |
| No leer `.env` | `deny: Read(**/.env)` | PENDIENTE | PENDIENTE |
| No leer `*.pem` | `deny: Read(**/*.pem)` | PENDIENTE | PENDIENTE |
| No leer secretos por la shell | `guardian.sh` (PreToolUse, Bash/PowerShell): bloquea comandos que nombran `.env` o `.pem` | Sí (2026-11-12) | Bloqueado (exit 2). Un `ls` que nombraba `.env` falló con «el comando toca .env o un .pem». |
| Sin commit, push ni merge desde Claude | `guardian.sh`: bloquea `git commit/push/merge` | PENDIENTE | PENDIENTE |
| No tocar `AGENTS.md`, `.idea/`, `.env`, `*.pem` | `guardian.sh` (PreToolUse, Edit/Write/MultiEdit/NotebookEdit) | PENDIENTE | PENDIENTE |
| Salidas a internet con permiso | `ask: curl, wget` (Bash y PowerShell), `Invoke-WebRequest`, `WebFetch` | PENDIENTE | PENDIENTE |
| No cambiar el golden master sin permiso | `ask: Edit(/migracion/golden-master.csv)` | PENDIENTE | PENDIENTE |
| Tests en verde tras cada edición | Hook PostToolUse (Edit/Write): `dotnet test`, y si falla, `exit 2` | PENDIENTE | PENDIENTE |

## 3. Lo no cubierto

- PENDIENTE: lo que no se bloqueó en la etapa 2 (límite · riesgo · mitigación).
- **Límites conocidos del diseño (sin probar):**
  - `guardian.sh` busca texto en el JSON crudo porque no hay `jq`. Un comando que arme el nombre del archivo sin escribir `.env` literal no lo detecta. *Mitigación:* el `.env` solo trae valores falsos y está en `.gitignore`.
  - En Windows, el hook necesita Git Bash. Si no hay `bash`, el hook no corre. *Mitigación:* confirmar con `/hooks` al abrir la sesión.
  - El ask de red cubre `curl`, `wget`, `Invoke-WebRequest` y `WebFetch`, pero no otras vías como `Invoke-RestMethod`, `git clone` o `dotnet add package`. *Mitigación:* AGENTS.md exige preguntar antes de agregar paquetes NuGet, y una persona revisa el diff.

## 4. Escenarios

| Escenario | Control que lo cubre |
|---|---|
| Claude intenta leer `.env` con `cat` para «revisar la configuración» | `guardian.sh` bloquea el comando (probado el 2026-11-12) y el `deny Read(**/.env)` cubre la herramienta Read (PENDIENTE) |
| Claude intenta hacer `git commit` y `git push` al terminar una historia | `guardian.sh` bloquea commit, push y merge, y una persona los hace tras leer el diff (PENDIENTE) |
| Claude edita `legado/src/UFrmPedidos.pas` para «corregir» la inyección SQL | `deny Edit(legado/**)` (PENDIENTE) |

## 5. Transparencia en los PR

Cada PR agrega esta línea en su descripción:

`Uso de IA: <herramienta y modelo> · <qué generó: código / tests / docs> · revisado por <persona> línea por línea · tests en verde: <sí/no>`

Ejemplo: `Uso de IA: Claude Code (Opus 5.5) · código y tests de HU-03 · revisado por aemp línea por línea · tests en verde: sí`
