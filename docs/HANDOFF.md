# HANDOFF CHRONOSPOINT

Passagem de bastão entre duas máquinas (Cursor). Leia também `CHANGELOG.md` na mesma pasta `docs/` antes de retomar trabalho.

## ENCODING

Grave ficheiros `.md` em UTF 8 (sem BOM se possível). Não use UTF 16 no Git: o editor pode mostrar lixo no início do ficheiro.

## 1 ANTES DE TROCAR DE MAQUINA

1. Commit com mensagem clara.
2. Push para o remoto no branch que estiver a usar (`main`, `develop`, etc.).
3. Se ficou trabalho a meio: branch `wip/` ou `git stash` e escreva uma linha no **REGISTRO RAPIDO** no fim deste ficheiro.

### CHECKLIST ANTES DE GIT PUSH

- [ ] Sem connection string real em `appsettings*.json` no Git.
- [ ] Sem chaves no código (API keys, JWT, Hangfire, Oracle).
- [ ] Sem planilhas reais de ponto (nomes, CPF, salário, batidas).
- [ ] Sem `.env`, sem `*.pfx`, sem wallet Oracle real no índice.
- [ ] Não adicionar `bin/`, `obj/`, nem user secrets por engano.

Se tiver dúvida, não faça push. Se já subiu segredo: rode a credencial e trate o histórico com `git filter-repo` ou apoio do GitHub.

## 2 NA OUTRA MAQUINA

1. `git pull`.
2. Abrir a raiz do projeto no Cursor.
3. Voltar a pôr User Secrets e variáveis de ambiente à mão (não vão no Git).
4. Ler `docs/CHANGELOG.md` e o **REGISTRO RAPIDO** em baixo.

### CURSOR

- Um clone, uma raiz de workspace.
- Chat longo: ao voltar, resuma o que estava a fazer e diga quais ficheiros mexia.
- Se criar `.cursor/rules` ou `AGENTS.md`, anote no registro.

## 3 CHANGELOG

O ficheiro é `docs/CHANGELOG.md` (ao lado deste handoff). Atualize no fim de entregas úteis ou antes de merge em `main`. Secções: ADICIONADO, ALTERADO, CORRIGIDO, REMOVIDO, SEGURANCA. Datas em ISO.

## 4 GITHUB E DADOS SENSIVEIS

- Oracle e strings de conexão: User Secrets, CI privada ou cofre. Nada disso em repo público.
- Planilhas com dados pessoais: não commitar. Exemplos só anonimizados.
- Salários e pagamentos: só em ambiente controlado.

Ficheiros `.xls` ou `.xlsx` com dados reais: não envie para remoto público. Use `.gitignore` ou `git rm --cached` e troque por amostra limpa.

## 5 BRANCHES

- `main`: linha estável.
- `develop`: opcional.
- `feature/`, `fix/`, `wip/`: como precisar.

Tags tipo `v0.1.0` alinhadas ao changelog.

## 6 REGISTRO RAPIDO

Edite no fim de cada sessão importante (fale consigo mesmo aqui).

- 2026 05 12: Docs em `docs/` reduzidos a `HANDOFF.md` + `CHANGELOG.md`. UTF 8 no README da raiz.

## 7 LAYOUTS (SEM DADOS REAIS)

Importação: aba Relógio em planilhas tipo matriz de atendimento. Impressão: modelo tipo controle de ponto eletrónico. Descreva colunas em texto se precisar, não suba ficheiros com PII.

Ultima revisão: 2026 05 12.
