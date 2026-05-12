<!--
  Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
  Este ficheiro faz parte de software proprietario. Ver LICENSE na raiz.
  Reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
-->

<p align="left">
  <img src="https://img.shields.io/badge/Licen%C3%A7a-Propriet%C3%A1ria-critical?style=for-the-badge" alt="Licenca proprietaria" />
  <img src="https://img.shields.io/badge/C%C3%B3digo%20fechado-111111?style=for-the-badge" alt="Codigo fechado" />
</p>

**Licença:** proprietária. Este software é de **código fechado**. O uso, modificação ou redistribuição do código-fonte **não é permitido** sem consentimento expresso por escrito dos titulares. Ver o ficheiro **LICENSE** na raiz.

# ChronosPoint

Plataforma **SaaS multi-tenant** para automatizar a gestão, correção e apuração de **pontos eletrônicos**, com foco em escritórios de contabilidade e departamentos de RH que hoje fecham o mês em planilhas.

> Repositório de produto em evolução: documentação pensada para **portfólio** e visão de produto, sem expor credenciais, ambientes internos ou procedimentos de implantação.

## Visão do produto

O **ChronosPoint** recebe batidas e jornadas vindas de sistemas legados (tipicamente exportações em planilhas), **padroniza** os dados, associa colaboradores de forma **determinística por identificador**, aplica regras de carga horária e apuração, e produz **relatórios auditáveis** e artefatos de impressão alinhados ao modelo operacional do cliente.

Objetivos centrais:

- Reduzir erro humano na associação nome, matrícula e batidas.
- Acelerar o **fechamento mensal** com conferência guiada e histórico.
- Manter **rastreabilidade** de importações, correções, exportações e pagamentos de horas.

## Principais funcionalidades (roadmap / escopo)

| Área | Descrição |
|------|-----------|
| **Engine de importação** | Leitura de planilhas padronizadas (layout configurável por tenant), mapeamento de colunas e vínculo automático a colaboradores e jornadas cadastradas. |
| **Painel de conferência (Check-and-Go)** | Fluxo para inconsistências: batidas faltantes, intervalos, extras não autorizadas, ajustes com registro de quem alterou e quando. |
| **Calendário e apuração** | Preenchimento de período (início/fim da apuração), fechamento por competência, cálculo de variáveis e **banco de horas** conforme políticas do tenant. |
| **Exportação** | Geração de relatórios no formato esperado pelo cliente (incl. cabeçalhos com dias no padrão XLS quando aplicável), armazenamento versionado. |
| **CRM operacional** | Cadastro enriquecido: nome amigável, vínculo estável por **ID** (a mesma chave usada na importação elimina ambiguidade de nomes abreviados na planilha). |
| **Remuneração e freelancer** | Salário ou valor dia ou mês, **calculadora de valor-hora**, pagamento de horas que **zera** saldo pago no período mantendo histórico (horas pagas, valores). |
| **Armazenamento por colaborador** | Artefatos e preenchimentos organizados em estrutura lógica (ex.: pastas por funcionário na infraestrutura do tenant), adequado a auditoria e backup. |

## Stack tecnológica (logos)

<p align="left">
  <a href="https://dotnet.microsoft.com/apps/aspnet" title="ASP.NET Core"><img src="https://img.shields.io/badge/ASP.NET%20Core%209-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core 9" /></a>
  <a href="https://learn.microsoft.com/dotnet/csharp/" title="C#"><img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#" /></a>
  <a href="https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor" title="Blazor WebAssembly"><img src="https://img.shields.io/badge/Blazor%20WebAssembly-512BD4?style=for-the-badge&logo=blazor&logoColor=white" alt="Blazor WebAssembly" /></a>
  <a href="https://learn.microsoft.com/ef/core/" title="Entity Framework Core"><img src="https://img.shields.io/badge/Entity%20Framework%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="Entity Framework Core" /></a>
  <a href="https://www.oracle.com/database/" title="Oracle Database"><img src="https://img.shields.io/badge/Oracle-F80000?style=for-the-badge&logo=oracle&logoColor=white" alt="Oracle" /></a>
  <a href="https://www.hangfire.io/" title="Hangfire"><img src="https://img.shields.io/badge/Hangfire-59388A?style=for-the-badge" alt="Hangfire" /></a>
</p>

<p align="left">
  <img src="https://img.shields.io/badge/Clean%20Architecture-111111?style=for-the-badge" alt="Clean Architecture" />
  <img src="https://img.shields.io/badge/DDD-004880?style=for-the-badge" alt="Domain-Driven Design" />
  <img src="https://img.shields.io/badge/Multi--tenant-2C3E50?style=for-the-badge" alt="Multi-tenant" />
</p>

As imagens são badges (shields.io) e carregam no GitHub. Links levam à documentação oficial.

## Arquitetura e stack (diretriz do projeto)

| Camada | Tecnologia |
|--------|------------|
| **Backend** | ASP.NET Core 9, Web API RESTful |
| **Frontend** | Blazor WebAssembly |
| **Persistência** | Entity Framework Core + Oracle |
| **Jobs / filas** | Hangfire (apuração e importações pesadas) |
| **Arquitetura** | Clean Architecture com orientação a **DDD** |
| **SaaS** | **Multi-tenant** com isolamento forte de dados por empresa cliente |

Diagrama conceitual (alto nível):

```mermaid
flowchart LR
  subgraph ingest["Ingestão"]
    XLS[Planilhas brutas]
    MAP[Mapeamento e validação]
  end
  subgraph core["Domínio"]
    CAL[Apuração e calendário]
    BH[Banco de horas]
    PAY[Pagamento de horas]
  end
  subgraph surface["Superfícies"]
    API[Web API]
    UI[Blazor WASM]
    BG[Hangfire]
  end
  XLS --> MAP --> API
  API --> CAL
  CAL --> BH
  CAL --> PAY
  BG --> CAL
  UI --> API
```

## Multi-tenant e segurança (visão)

- Isolamento de dados **por tenant** (empresa), com políticas de acesso por perfil.
- Dados sensíveis (connection strings, chaves de API, wallets Oracle, segredos de Hangfire) **não** devem ser versionados: ver [`docs/HANDOFF.md`](docs/HANDOFF.md) e `.gitignore`.
- Logs e exportações podem conter dados pessoais e trabalhistas: tratamento conforme LGPD e política interna do operador do sistema.

## O que este repositório não inclui (de propósito)

Para manter o repositório seguro e adequado a **portfólio público**:

- Credenciais, connection strings reais, URLs de ambientes internos.
- Scripts ou runbooks que reproduzam produção na íntegra.
- Planilhas com dados reais de colaboradores (quando houver exemplos, devem ser **anonimizados**).

Contribuições e evolução do código devem respeitar a mesma linha.

## Estrutura esperada do código (quando o monorepo for criado)

```
/src
  /ChronosPoint.Domain
  /ChronosPoint.Application
  /ChronosPoint.Infrastructure
  /ChronosPoint.Api
  /ChronosPoint.Web
/tests
/docs
```

Em `docs/`: [`HANDOFF.md`](docs/HANDOFF.md) e [`CHANGELOG.md`](docs/CHANGELOG.md).

## Licença e uso

**Proprietary software. All rights reserved. No redistribution allowed.** Texto completo: ficheiro **LICENSE** na raiz. O `package.json` declara `"license": "UNLICENSED"` e `"private": true` para tooling Node não assumir Open Source. Substitua "ChronosPoint" no cabeçalho dos ficheiros e no LICENSE pelo titular legal (nome ou empresa) quando aplicável.

## Contato / portfólio

**Jefferson Lima**

- **Contato:** [(35) 98404-7078](tel:+5535984047078)
- **Email:** [jeffersonlima@jeffersonlima.net.br](mailto:jeffersonlima@jeffersonlima.net.br)
- **Portfolio:** [www.jeffersonlima.net.br](https://www.jeffersonlima.net.br)
- **GitHub:** [https://github.com/Jefferson23br](https://github.com/Jefferson23br)

*ChronosPoint: controle de ponto com propósito, menos planilha frágil, mais apuração confiável.*
