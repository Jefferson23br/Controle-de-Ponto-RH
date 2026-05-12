# CHANGELOG

Alterações relevantes do repositório. Atualize quando fechar um bloco de trabalho.

Formato inspirado em Keep a Changelog: https://keepachangelog.com/pt-BR/1.0.0/

## [NÃO PUBLICADO]

### ADICIONADO

- README na raiz do ChronosPoint (portfólio).
- `docs/HANDOFF.md` para passagem de bastão entre máquinas.
- `docs/CHANGELOG.md` neste caminho (junto do handoff).
- `.gitignore` para .NET e segredos comuns.

### ALTERADO

- Changelog saiu de `docs/Historico/` e passou para `docs/CHANGELOG.md`.
- Removidos índice `docs/README.md`, pasta `Historico` e pasta `arquitetura` da documentação (só ficam handoff + changelog em `docs/`).

### CORRIGIDO

- README na raiz em UTF 8 (evita caracteres estranhos se o ficheiro for gravado em UTF 16).

### SEGURANÇA

- Não publicar planilhas com dados pessoais em remoto público. Opcional no gitignore: ignorar `*.xls` e `*.xlsx` se quiser bloquear planilhas reais.
