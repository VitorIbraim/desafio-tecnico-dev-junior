# Uso de IA

## Ferramenta

Claude Code (Anthropic), utilizado pela extensão do VS Code, com acesso ao repositório e ao terminal.

## Como a IA foi usada

A IA foi utilizada como ferramenta de apoio ao desenvolvimento, principalmente na estruturação das etapas, revisão técnica, criação e expansão dos testes e validação da solução.

A maior parte do código da aplicação foi escrita por mim. A IA teve participação mais significativa nas partes relacionadas à organização da estrutura do projeto, definição e revisão de cenários de teste, validação de casos de borda e revisão das decisões tomadas durante a implementação.

Antes de cada etapa, defini o que deveria ser desenvolvido, quais restrições deveriam ser respeitadas e quais critérios seriam utilizados para considerar aquela etapa concluída.

O desenvolvimento foi dividido em:

1. Estrutura inicial da solution .NET.
2. Modelagem dos dados de `professores.json`.
3. Leitura e desserialização do arquivo.
4. Implementação da regra de busca.
5. Testes automatizados da regra de negócio.
6. Endpoint `POST /api/professores/busca` e validação do request.
7. Frontend em React + TypeScript.
8. Refinamento visual.
9. Documentação e revisão final.

### Minha participação

Ficaram sob minha responsabilidade:

1. Definir a arquitetura geral da solução e a separação entre API, regra de negócio e testes.
2. Definir a estrutura das etapas e a ordem de implementação.
3. Escrever a maior parte do código da solução.
4. Definir como a regra de disponibilidade deveria funcionar.
5. Definir as restrições técnicas do projeto, evitando dependências e abstrações desnecessárias.
6. Determinar os cenários que precisavam ser cobertos pelos testes.
7. Revisar as implementações e os resultados das validações.
8. Executar e revisar os commits antes da publicação.
9. Revisar a documentação e o comportamento final da aplicação.

### Participação da IA

A IA foi utilizada principalmente para:

1. Revisar a estrutura do projeto e sugerir formas de manter a regra de negócio desacoplada da camada HTTP.
2. Auxiliar na organização das etapas de desenvolvimento.
3. Apoiar a criação e expansão dos testes automatizados.
4. Levantar casos de borda relacionados a horários, interseção de dias e colisões entre aulas.
5. Revisar trechos da implementação e apontar possíveis inconsistências.
6. Auxiliar em partes relacionadas à estrutura dos projetos e organização dos arquivos.
7. Executar validações auxiliares e relatar os resultados.
8. Apoiar a revisão do frontend e da documentação.

A IA não foi tratada como fonte de verdade. As sugestões foram analisadas antes de serem incorporadas, e cada etapa relevante foi validada por compilação, testes ou execução real da aplicação.

## Como as sugestões foram verificadas

Nenhuma etapa foi considerada concluída apenas porque o código compilava ou porque uma sugestão parecia correta.

Foram utilizadas diferentes formas de validação:

- **Build e testes:** `dotnet build` e `dotnet test` foram executados durante o desenvolvimento. Na validação final, os 15 casos automatizados passaram.
- **Desserialização dos dados:** o `professores.json` foi carregado de forma real, verificando os 8 professores e alguns de seus valores.
- **Regra de negócio:** cenários de disponibilidade, interseção entre dias, colisão entre horários e limites dos blocos foram comparados com resultados calculados manualmente.
- **API:** o endpoint `POST /api/professores/busca` foi executado diretamente. O exemplo fornecido no desafio retornou `200`, enquanto requests inválidos retornaram `400`.
- **Frontend:** foram executados `npm run build` e `npm run lint`, além de testes manuais no navegador.
- **Estados da interface:** foram verificados carregamento, sucesso, ausência de professores elegíveis, erros de validação e falha de comunicação com a API.

O objetivo do uso de IA foi aumentar a capacidade de revisão e reduzir a chance de deixar casos importantes sem validação, e não substituir o entendimento da solução.

## Caso em que a IA errou

Durante a modelagem do `professores.json`, foi sugerido inicialmente utilizar `TimeOnly` diretamente para representar os campos de horário, como `inicio` e `fim`.

A hipótese era que o `System.Text.Json` conseguiria converter diretamente valores como:

```json
"18:00"

para TimeOnly.
Como o erro foi percebido
O projeto compilava normalmente, portanto o problema não era detectado pelo build.
Ao testar a desserialização real do arquivo com .NET 8, ocorreu uma falha já no primeiro campo de horário:
System.Text.Json.JsonException: The JSON value could not be converted to System.TimeOnly.
Path: $.professores[0].disponibilidade[0].inicio
---> System.FormatException: The JSON value is not in a supported TimeOnly format.

Isso mostrou que o conversor padrão de TimeOnly utilizado pelo System.Text.Json não aceitava diretamente o formato HH:mm presente no arquivo.
Correção
Uma alternativa seria implementar um conversor JSON específico para TimeOnly, mas isso adicionaria complexidade que não era necessária para o escopo do desafio.
A solução adotada foi manter os horários como string nos modelos, preservando o formato original de professores.json.
Durante os cálculos da regra de negócio, esses valores são convertidos para TimeSpan, permitindo realizar comparações de horário, adicionar durações e verificar sobreposição entre intervalos.
Depois da alteração, a desserialização foi executada novamente e os 8 professores foram carregados corretamente.
Esse caso foi um exemplo concreto da importância de não aceitar automaticamente uma sugestão de IA: mesmo uma solução tecnicamente plausível precisa ser confirmada por testes e execução real.