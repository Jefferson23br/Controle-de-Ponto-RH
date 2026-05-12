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

- Ficheiro **LICENSE** na raiz (software proprietário, sem modelo MIT).
- **package.json** com `"license": "UNLICENSED"` e `"private": true` (evita classificação automática como Open Source em ferramentas Node).
- Aviso de licença no **README** (badges e texto em negrito).
- Cabeçalho de copyright no topo de ficheiros de configuração (`.gitignore`, `.gitattributes`) e comentário HTML no topo dos `.md` principais.
- Regra em **HANDOFF**: todo ficheiro de código-fonte novo deve incluir o cabeçalho de licença (ver `docs/HANDOFF.md`).

### ALTERADO

- Changelog saiu de `docs/Historico/` e passou para `docs/CHANGELOG.md`.
- Removidos índice `docs/README.md`, pasta `Historico` e pasta `arquitetura` da documentação (só ficam handoff + changelog em `docs/`).

### CORRIGIDO

- README na raiz em UTF 8 (evita caracteres estranhos se o ficheiro for gravado em UTF 16).

### SEGURANÇA

- Não publicar planilhas com dados pessoais em remoto público. Opcional no gitignore: ignorar `*.xls` e `*.xlsx` se quiser bloquear planilhas reais.
