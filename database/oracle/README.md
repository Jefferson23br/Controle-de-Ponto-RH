# Oracle (ChronosPoint)

## 1. Criar utilizador da aplicacao

1. Copie `01-create-user.sql.example` para um ficheiro **local** (fora do Git) ou descomente e edite no servidor.
2. Ligue-se como **DBA** (SQL*Plus, SQL Developer, DBeaver).
3. Crie o utilizador dedicado (ex.: `CHRONOSPOINT_APP`) na **PDB** onde a app vai ligar (ex.: `XEPDB1`), com password forte e `QUOTA` no tablespace acordado.

Nao grave passwords no repositorio.

## 1.1 ORA-01031 (privilegios insuficientes) no DBeaver

O utilizador da sessao nao pode executar o comando (ex.: `CREATE USER`). Use `SYSTEM` ou `SYS` como **SYSDBA**, ou peca ao DBA para criar o utilizador da aplicacao.

```sql
SELECT USER AS usuario_atual FROM DUAL;
```

## 2. Connection string (referencia)

Easy Connect (exemplo):

```text
User Id=CHRONOSPOINT_APP;Password=***;Data Source=HOST:1521/XEPDB1
```

Com TNS (`tnsnames.ora`): `Data Source=ALIAS_TNS;`

## 3. User Secrets (desenvolvimento)

Trabalhe em `src/ChronosPoint.Api`. **Escolha uma forma so** (evite duplicar):

| Forma | Chaves |
|--------|--------|
| **A** | `ConnectionStrings:Oracle` (string completa) |
| **B** | `ORACLE_HOST`, `ORACLE_PORT`, `ORACLE_SERVICE_NAME`, `ORACLE_USER`, `ORACLE_PASSWORD` |

Se **A** existir, o codigo **ignora B**. Password com `;`, `@`, espacos: prefira **B** (o codigo usa `OracleConnectionStringBuilder`).

### 3.0 PowerShell: leia isto para nao se cansar

- **Mensagens de erro em ingles** (ex.: `Could not find a MSBuild project file...`) sao **saida** de um comando que correu mal. **Nao** as cole de volta no PowerShell: o PS tenta executar a primeira palavra (`Could`) como comando e da `CommandNotFoundException`.
- `dotnet user-secrets remove ...` a dizer **Cannot find ... in the secret store** significa que **essa chave ja nao existe** (foi apagada ou nunca foi gravada). **Nao e falha:** pode ignorar e seguir.
- Comandos `dotnet user-secrets` precisam do projeto: ou `cd src\ChronosPoint.Api` antes, ou `--project src\ChronosPoint.Api` na **raiz** do repo.

```bash
cd src/ChronosPoint.Api

# Forma B (recomendada com passwords especiais)
dotnet user-secrets set "ORACLE_HOST" "HOST"
dotnet user-secrets set "ORACLE_PORT" "1521"
dotnet user-secrets set "ORACLE_SERVICE_NAME" "XEPDB1"
dotnet user-secrets set "ORACLE_USER" "CHRONOSPOINT_APP"
dotnet user-secrets set "ORACLE_PASSWORD" "SUA_PASSWORD"

# Remover a forma A se ja nao quiser usa-la (use --project se nao estiver na pasta da API)
dotnet user-secrets remove "ConnectionStrings:Oracle" --project src/ChronosPoint.Api

dotnet user-secrets list --project src/ChronosPoint.Api
```

**Seguranca:** nao partilhe passwords em chat nem em capturas. Se expuser, altere no Oracle e atualize os segredos.

### 3.1 ORA-01017 (invalid username/password)

O listener respondeu, mas o servidor **rejeitou** user/password, ou o utilizador **nao existe nessa PDB**.

1. No DBeaver, ligue com **os mesmos** host, porta, service name, user e password.
2. Confirme que o user foi criado **na PDB** do service (ex.: `XEPDB1`), nao so no `CDB$ROOT`.
3. Remova `ConnectionStrings:Oracle` se estiver a usar **B**, para nao haver duas fontes em conflito.
4. No PowerShell, veja se ha **variaveis de ambiente** antigas a sobrepor os secrets: `Get-ChildItem Env:ConnectionStrings*`, `Get-ChildItem Env:ORACLE_*`. Remova sessao com `Remove-Item Env:NOME` se for o caso.

### 3.2 Ainda ORA-01017 (checklist no servidor Oracle)

Ligue como **SYS ou SYSTEM** na **mesma PDB** do service name (ex.: `XEPDB1`) e confirme se o utilizador existe e esta desbloqueado:

```sql
ALTER SESSION SET CONTAINER = XEPDB1;
SELECT username, account_status FROM dba_users WHERE username = 'CHRONOSPOINT_APP';
```

Se nao aparecer linha, o user **nao existe nesta PDB**: crie-o aqui ou ligue com o service name da PDB onde o user foi criado.

Se existir com `LOCKED` ou password errada, redefina (ajuste a password):

```sql
ALTER USER CHRONOSPOINT_APP IDENTIFIED BY nova_password ACCOUNT UNLOCK;
```

Depois atualize `ORACLE_PASSWORD` nos User Secrets e volte a correr `dotnet ef database update`.

### 3.3 ORA-01918 (o usuario nao existe)

O comando (ex.: `ALTER USER`) foi corrido num contentor onde esse user **ainda nao foi criado**. Crie o user **dentro da PDB** do teu `SERVICE_NAME` (ex.: `XEPDB1`), como `SYSTEM` ou `SYS` com **SYSDBA**:

```sql
ALTER SESSION SET CONTAINER = XEPDB1;

CREATE USER CHRONOSPOINT_APP IDENTIFIED BY "EscolhaUmaPasswordForte123"
  DEFAULT TABLESPACE USERS
  TEMPORARY TABLESPACE TEMP
  QUOTA UNLIMITED ON USERS;

GRANT CREATE SESSION TO CHRONOSPOINT_APP;
GRANT CREATE TABLE TO CHRONOSPOINT_APP;
GRANT CREATE SEQUENCE TO CHRONOSPOINT_APP;
GRANT CREATE VIEW TO CHRONOSPOINT_APP;
GRANT CREATE PROCEDURE TO CHRONOSPOINT_APP;
GRANT CREATE TRIGGER TO CHRONOSPOINT_APP;
GRANT UNLIMITED TABLESPACE TO CHRONOSPOINT_APP;
```

Se `USERS` ou `TEMP` nao existirem nesta PDB, ajuste com o que o DBA indicar (`SELECT tablespace_name FROM dba_tablespaces;`). Veja tambem `database/oracle/01-create-user.sql.example`.

### 3.4 ORA-01031 ao correr `dotnet ef database update` (apos login OK)

O EF Core Oracle cria `__EFMigrationsHistory` com um **bloco PL/SQL anonimo** (`EXECUTE IMMEDIATE 'CREATE TABLE ...'`). Nesses blocos o Oracle **desactiva roles**: privilegios que existem **so** atraves de `GRANT RESOURCE` / `CONNECT` **nao** aplicam-se ao DDL dinamico. Resultado: **ORA-01031** mesmo que o user “pareca” ter `RESOURCE`.

**Correcao:** conceder privilegios de sistema **directos** ao utilizador (na PDB do service name), **alem** de quota no tablespace:

```sql
ALTER SESSION SET CONTAINER = XEPDB1;

GRANT CREATE TABLE TO CHRONOSPOINT_APP;
GRANT CREATE SEQUENCE TO CHRONOSPOINT_APP;
GRANT CREATE VIEW TO CHRONOSPOINT_APP;
GRANT CREATE PROCEDURE TO CHRONOSPOINT_APP;
GRANT CREATE TRIGGER TO CHRONOSPOINT_APP;
GRANT UNLIMITED TABLESPACE TO CHRONOSPOINT_APP;
```

Alternativa a `UNLIMITED TABLESPACE` (quota no tablespace por omissao do user, muitas vezes `USERS`):

```sql
ALTER USER CHRONOSPOINT_APP QUOTA UNLIMITED ON USERS;
```

**Verificar privilegios directos (como DBA, na mesma PDB):**

```sql
ALTER SESSION SET CONTAINER = XEPDB1;
SELECT privilege FROM dba_sys_privs
WHERE grantee = 'CHRONOSPOINT_APP'
ORDER BY privilege;
```

Tem de constar `CREATE TABLE` (e idealmente `UNLIMITED TABLESPACE` ou quota em `USER_TS_QUOTAS`).

**Verificar como o proprio user (sessao normal):**

```sql
SELECT privilege FROM USER_SYS_PRIVS ORDER BY privilege;
```

Se `CREATE TABLE` nao aparecer, os `GRANT` directos ainda nao foram aplicados **nesta PDB** ou foram dados a outro utilizador.

Volte a correr `dotnet ef database update` depois disto.

## 4. Migracoes EF Core

Na **raiz** do repositorio:

```bash
dotnet tool restore
dotnet ef database update --project src/ChronosPoint.Infrastructure --startup-project src/ChronosPoint.Api
```

Sem User Secrets nesta maquina: pode passar a string uma vez com `--connection "User Id=...;Password=...;Data Source=..."`.

**Migracoes sem servidor Oracle** (so gerar ficheiros C#):

```powershell
$env:CHRONOSPOINT_EF_USE_PLACEHOLDER="1"
dotnet ef migrations add NomeDaMigracao --project src/ChronosPoint.Infrastructure --startup-project src/ChronosPoint.Api
```

### 4.1 ORA-12541 / ORA-50201

Ninguem escuta em `HOST:PORTA` (listener inactivo, IP/porta errados, firewall, ou placeholder `127.0.0.1` sem Oracle local). Confirme conectividade com DBeaver a partir deste PC.

## 5. Ambientes publicos (referencia)

- API: `https://api7.auctusconsultoria.com.br`
- Frontend: `https://rh.auctusconsultoria.com.br`

O CORS da API inclui a origem do frontend em `appsettings.json`.
