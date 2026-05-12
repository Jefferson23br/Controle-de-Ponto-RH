# Oracle (ChronosPoint)

## 1. Criar utilizador da aplicacao

1. Copie `01-create-user.sql.example` para um ficheiro **local** (fora do Git) ou descomente e edite no servidor.
2. Ligue-se como **DBA** (SQL*Plus, SQL Developer, DBeaver, etc.).
3. Defina um utilizador dedicado (ex.: `CHRONOSPOINT_APP`) e password forte.
4. Garanta `QUOTA` no tablespace onde as tabelas vao residir (normalmente `USERS` ou tablespace da empresa).

Nao grave passwords no repositorio.

## 1.1 Erro ORA-01031 (privilegios insuficientes) no DBeaver

Significa: o **utilizador com que esta ligado** nao tem permissao para o comando que executou (por exemplo `CREATE USER`).

**O que fazer:**

1. **Criar utilizador (`CREATE USER`)** so pode ser feito por contas com privilegio de administracao, em geral:
   - `SYS` como **SYSDBA**, ou
   - `SYSTEM`, ou
   - outro utilizador a quem o DBA tenha dado `CREATE USER` / papel **DBA**.

   Contas “normais” da aplicacao (ex.: utilizador so para consultar dados) **nao** criam utilizadores.

2. **No DBeaver** (ligacao Oracle):
   - Edite a ligacao: **Driver properties** ou separador **Oracle** (conforme versao do DBeaver).
   - Para ligar como `SYS`: defina o papel **SYSDBA** (opcao “Connection type” / “Role” = `SYSDBA`). Sem isto, `SYS` liga-se sem privilegios suficientes e falha o mesmo tipo de operacao.
   - Confirme que esta na **PDB correta** (Oracle 12c+): o utilizador da aplicacao deve ser criado **dentro da PDB** onde a aplicacao vai trabalhar. Se estiver so no `CDB$ROOT` sem permissoes adequadas, tambem pode dar erro.

3. **Se a base for gerida por terceiros (hosting / DBA da empresa):** peça a criacao do utilizador `CHRONOSPOINT_APP` (ou o nome acordado) e os **GRANTs** necessarios. Voce recebe utilizador + password + `SERVICE_NAME` e nao precisa de `CREATE USER` localmente.

4. **Confirmar quem e a sessao atual** (com a ligacao que esta a usar):

```sql
SELECT USER AS usuario_atual FROM DUAL;
```

Se nao for `SYSTEM` nem `SYS` (com SYSDBA), e normal nao conseguir `CREATE USER`.

## 2. Connection string para a API (.NET)

Formatos comuns (ajuste `HOST`, `PORT`, `SERVICE_NAME` ou `SID` ao seu ambiente):

**Easy Connect (recomendado para testes):**

```text
User Id=CHRONOSPOINT_APP;Password=SUA_PASSWORD;Data Source=HOST:1521/NOME_DO_SERVICE;
```

**TNS (se tiver `tnsnames.ora` configurado no servidor da API):**

```text
User Id=CHRONOSPOINT_APP;Password=SUA_PASSWORD;Data Source=ALIAS_TNS;
```

## 3. User Secrets (desenvolvimento na sua maquina)

**Nao** coloque passwords em `appsettings.json` versionado. Use [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) na pasta `src/ChronosPoint.Api`.

**Opcao A — uma connection string (Easy Connect):**

```bash
cd src/ChronosPoint.Api
dotnet user-secrets set "ConnectionStrings:Oracle" "User Id=CHRONOSPOINT_APP;Password=SUA_PASSWORD;Data Source=HOST:1521/XEPDB1"
```

**Opcao B — variaveis ORACLE_* (a API e o `dotnet ef` montam a string):**

```bash
cd src/ChronosPoint.Api
dotnet user-secrets set "ORACLE_HOST" "HOST"
dotnet user-secrets set "ORACLE_PORT" "1521"
dotnet user-secrets set "ORACLE_SERVICE_NAME" "XEPDB1"
dotnet user-secrets set "ORACLE_USER" "CHRONOSPOINT_APP"
dotnet user-secrets set "ORACLE_PASSWORD" "SUA_PASSWORD"
```

Se `ConnectionStrings:Oracle` estiver preenchida, ela **tem prioridade** sobre `ORACLE_*`.

**Variaveis de ambiente (PowerShell, sessao atual):** `ConnectionStrings__Oracle` ou as chaves `ORACLE_HOST`, etc. (underscore duplo so na connection string.)

**Seguranca:** se a password foi exposta (chat, email, captura de ecra), **altere-a** no Oracle (`ALTER USER CHRONOSPOINT_APP IDENTIFIED BY nova_senha REPLACE antiga_senha;`) e atualize os segredos.

## 4. Migracoes EF Core

O projeto inclui `ChronosPointDbContextFactory` para o `dotnet ef` carregar a mesma configuracao (appsettings + User Secrets + ambiente).

A partir da raiz do repositorio (com [.NET 9 SDK](https://dotnet.microsoft.com/download) e `dotnet` no PATH):

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/ChronosPoint.Infrastructure --startup-project src/ChronosPoint.Api
dotnet ef database update --project src/ChronosPoint.Infrastructure --startup-project src/ChronosPoint.Api
```

A connection string (ou `ORACLE_*`) tem de estar definida antes do `database update`.

Depois de subir a API em desenvolvimento, teste a ligacao: `GET /api/health/database` (deve responder `ok` com `database: oracle`).

## 5. Ambientes publicos (referencia)

- API (planeado): `https://api7.auctusconsultoria.com.br`
- Frontend Blazor (planeado): `https://rh.auctusconsultoria.com.br`

O CORS da API ja inclui a origem do frontend em `appsettings.json`. Se usar mais origens (ex.: preview), acrescente em configuracao.
