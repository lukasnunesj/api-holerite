# HoleriteAPI

[![CI/CD](https://github.com/lukasnunesj/api-hoelerite/actions/workflows/main.yml/badge.svg)](https://github.com/lukasnunesj/api-hoelerite/actions/workflows/main.yml)

API em **C# / .NET 8** que estima o holerite mensal de um empregado CLT: adicional noturno, horas extras, DSR,
INSS, IRRF, vale/adiantamento e o valor líquido.

Nasceu para ajudar uma pessoa a calcular, de forma aproximada, quanto receberia de salário. Está no ar:

- Aplicação: <https://react-holerite.fly.dev> (front em React: [react-holerite](https://github.com/lukasnunesj/react-holerite))
- API: `https://api-holerite.fly.dev/api/holerite`

> **Valores aproximados.** Não substitui o holerite. Não cobre dependentes, 13º, férias, pensão alimentícia nem benefícios.

## Como o cálculo funciona

1. **Valor da hora** = salário ÷ 200.
2. **Adicional noturno** = horas noturnas × valor da hora × 30%.
3. **Horas extras**: 75% = horas × valor da hora × 1,75. 100% = horas × valor da hora × 2.
4. **DSR** = (verba ÷ dias úteis) × domingos e feriados, sobre o adicional noturno e sobre as horas extras.
5. **Proventos** = salário + verbas + DSR.
6. **INSS**, progressivo por faixa, limitado ao teto.
7. **IRRF**, descrito abaixo.
8. **Líquido** = proventos − vale (40% do salário) − INSS − IRRF − plano médico − outros descontos.

Cada verba é arredondada em centavos (meio para cima) antes de somar, como num holerite. Os valores usam `decimal`.

### Tabelas de 2026

**INSS** (empregado, [Portaria Interministerial MPS/MF nº 13/2026](https://www.gov.br/inss/pt-br/direitos-e-deveres/inscricao-e-contribuicao/tabela-de-contribuicao-mensal)).
Cada alíquota incide só sobre a parte do salário dentro da faixa:

| Faixa de salário | Alíquota |
| :--- | :--- |
| até R$ 1.621,00 | 7,5% |
| R$ 1.621,01 a R$ 2.902,84 | 9% |
| R$ 2.902,85 a R$ 4.354,27 | 12% |
| R$ 4.354,28 a R$ 8.475,55 (teto) | 14% |

A contribuição máxima é de R$ 988,09.

**IRRF** ([Receita Federal, tributação de 2026](https://www.gov.br/receitafederal/pt-br/assuntos/meu-imposto-de-renda/tabelas/2026)):

1. A base é o rendimento menos a dedução mais vantajosa: o INSS ou o **desconto simplificado de R$ 607,20**.
2. O imposto vem da tabela progressiva mensal:

   | Base de cálculo | Alíquota | Parcela a deduzir |
   | :--- | :--- | :--- |
   | até R$ 2.428,80 | isento | |
   | R$ 2.428,81 a R$ 2.826,65 | 7,5% | R$ 182,16 |
   | R$ 2.826,66 a R$ 3.751,05 | 15% | R$ 394,16 |
   | R$ 3.751,06 a R$ 4.664,68 | 22,5% | R$ 675,49 |
   | acima de R$ 4.664,68 | 27,5% | R$ 908,73 |

3. Aplica-se a **redução da [Lei 15.270/2025](https://www.planalto.gov.br/ccivil_03/_ato2023-2026/2025/lei/l15270.htm)**,
   calculada sobre o rendimento bruto (não sobre a base) e limitada ao imposto apurado:
   - até R$ 5.000,00: redução de R$ 312,89, o que zera o imposto;
   - de R$ 5.000,01 a R$ 7.350,00: redução de R$ 978,62 − (0,133145 × rendimento);
   - acima de R$ 7.350,00: sem redução.

### Premissas do caso de uso original

Ficam como constantes no `HoleriteService`: jornada de 200 horas mensais, adicional noturno de 30%, horas extras de
75% e 100%, e vale de 40% do salário. Não há hora noturna reduzida (52min30s) nem dependentes.

## API

`GET /api/holerite`

| Parâmetro | Tipo | Regra |
| :--- | :--- | :--- |
| `SalarioBruto` | decimal | maior que 0 |
| `HorasNoturnas`, `HorasExtras75`, `HorasExtras100` | `HH:mm` | opcionais, ex.: `02:30` |
| `DiasUteis` | inteiro | de 1 a 31 |
| `DomingosFeriados` | inteiro | de 0 a 31 |
| `PlanoMedico`, `OutrosDescontos` | decimal | 0 ou mais |

```sh
curl "https://api-holerite.fly.dev/api/holerite?SalarioBruto=6000&HorasNoturnas=00:00&HorasExtras75=00:00&HorasExtras100=00:00&DiasUteis=22&DomingosFeriados=5&PlanoMedico=0&OutrosDescontos=0"
```

```json
{
  "dados": {
    "salarioBruto": 6000, "totalIRRF": 385.10, "totalINSS": 641.51,
    "totalAdicionalNoturno": 0, "totalHorasExtras75": 0, "totalHorasExtras100": 0,
    "totalDSRNoturno": 0, "totalDSRHoraExtra": 0,
    "totalDebitos": 6000, "totalGeral": 2573.39,
    "planoMedico": 0, "outrosDescontos": 0, "valorValeAdiantamento": 2400
  },
  "message": "Holerite calculado com sucesso!",
  "errors": []
}
```

`totalDebitos` é o total de proventos (a base do INSS e do IRRF); o nome é mantido por compatibilidade com o front.
Parâmetros inválidos devolvem `400` com os detalhes. A API é pública e só de leitura: CORS liberado apenas para `GET`.

## Arquitetura

```text
HoleriteAPI.Core/
├── HoleriteAPI.Core.Domain/        regras puras: INSS, IRRF, CargaHoraria, Dinheiro e a porta IHoleriteRepository
└── HoleriteAPI.Core.Application/   HoleriteService, DTOs com validação
HoleriteAPI.Data/                   HoleriteRepository: adaptador com as tabelas de INSS e IRRF
HoleriteAPI.Consumer/               ASP.NET Core: controller e Program
HoleriteAPI.Tests/                  xUnit
```

O domínio não depende de nada. As tabelas ficam atrás da porta `IHoleriteRepository`, então trocá-las (ou lê-las de
outra fonte) não mexe nas regras.

## Testes

```sh
dotnet test HoleriteAPI.sln
```

São 28 testes. As regras de IRRF são conferidas contra os **cinco exemplos oficiais** da Receita
([Exemplos de aplicação da Lei 15.270/2025](https://www.gov.br/receitafederal/pt-br/assuntos/meu-imposto-de-renda/tabelas/exemplos-de-aplicacao-da-lei-15-270-2025)),
e o INSS contra as contribuições desses mesmos exemplos (faixas de 2025). Há ainda testes do serviço, com contas
feitas à mão, e da API (`WebApplicationFactory`), incluindo os casos de `400`.
Um teste de regressão cobre um bug antigo: o INSS e o IRRF saíam trocados na resposta.

## Rodando localmente

```sh
dotnet run --project HoleriteAPI.Consumer
```

Em desenvolvimento, o Swagger fica em `http://localhost:5042/swagger`. Também há um `Dockerfile`.

## Deploy

A cada push na `main`, o GitHub Actions roda os testes e, se passarem, publica no Fly.io
(secret `FLY_API_TOKEN`). Pull requests só rodam os testes.

### Atualizando as tabelas

Todo janeiro: ajustar os valores em `HoleriteAPI.Data/Repositories/HoleriteRepository.cs` e conferir os testes.
