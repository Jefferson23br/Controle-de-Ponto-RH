<!--
  Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
  Este ficheiro faz parte de software proprietario. Ver LICENSE na raiz.
  Reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
-->

# CHANGELOG

Alterações relevantes do repositório. Atualize quando fechar um bloco de trabalho.

Formato inspirado em Keep a Changelog: https://keepachangelog.com/pt-BR/1.0.0/

## [NÃO PUBLICADO]

### ADICIONADO

- **`OracleConnectionResolver`** em `ChronosPoint.Infrastructure`: monta a connection string a partir de `ConnectionStrings:Oracle` **ou** das chaves `ORACLE_HOST`, `ORACLE_PORT`, `ORACLE_SERVICE_NAME`, `ORACLE_USER`, `ORACLE_PASSWORD` (User Secrets, ambiente ou configuracao).
- **`ChronosPointDbContextFactory`** (`IDesignTimeDbContextFactory`): `dotnet ef` usa appsettings da API, User Secrets e variaveis de ambiente sem depender apenas do registo em runtime.
- Documentacao Oracle: secao **ORA-01031** (DBeaver / privilegios), **User Secrets** com opcao de string unica ou variaveis `ORACLE_*`, aviso de seguranca sobre rotacao de password, teste **`GET /api/health/database`** apos migracoes.
- Comentarios em `database/oracle/01-create-user.sql.example` sobre **SYSTEM** / **SYS AS SYSDBA** e ORA-01031.
- Solução **ChronosPoint.sln** com backend .NET 9: `ChronosPoint.Domain`, `ChronosPoint.Application`, `ChronosPoint.Infrastructure` (Oracle EF Core), `ChronosPoint.Api` (health, OpenAPI dev, CORS para `https://rh.auctusconsultoria.com.br`).
- Pasta **database/oracle/** com `01-create-user.sql.example` e **README.md** (connection string, User Secrets, `dotnet ef`, referência a `api7` e `rh`).
- Ficheiro **LICENSE** na raiz (software proprietário, sem modelo MIT).
- **package.json** com `"license": "UNLICENSED"` e `"private": true` (evita classificação automática como Open Source em ferramentas Node).
- Aviso de licença no **README** (badges e texto em negrito).
- Cabeçalho de copyright no topo de ficheiros de configuração (`.gitignore`, `.gitattributes`) e comentário HTML no topo dos `.md` principais.
- Regra em **HANDOFF**: todo ficheiro de código-fonte novo deve incluir o cabeçalho de licença (ver `docs/HANDOFF.md`).

### ALTERADO

- **`AddInfrastructure`**: leitura da connection string Oracle via **`OracleConnectionResolver`** (mantem `ConnectionStrings:Oracle` vazio no Git com suporte a `ORACLE_*`).
- **`ChronosPoint.Infrastructure.csproj`**: referencias a `Microsoft.Extensions.Configuration.Json`, `EnvironmentVariables`, `UserSecrets` e `Microsoft.EntityFrameworkCore.Design` (ferramentas EF no projeto de persistencia).
- Changelog saiu de `docs/Historico/` e passou para `docs/CHANGELOG.md`.
- Removidos índice `docs/README.md`, pasta `Historico` e pasta `arquitetura` da documentação (só ficam handoff + changelog em `docs/`).

### CORRIGIDO

- **`docs/HANDOFF.md`** regravado em **UTF-8** (o ficheiro estava ilegivel por encoding incorreto).
- README na raiz em UTF 8 (evita caracteres estranhos se o ficheiro for gravado em UTF 16).

### SEGURANÇA

- Não publicar planilhas com dados pessoais em remoto público. Opcional no gitignore: ignorar `*.xls` e `*.xlsx` se quiser bloquear planilhas reais.
