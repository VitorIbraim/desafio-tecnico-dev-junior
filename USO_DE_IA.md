# Uso de IA

## Ferramenta

Claude Code (Anthropic), utilizado pela extensão do VS Code como ferramenta auxiliar de revisão e validação durante o desenvolvimento.

## Como a IA foi usada

A IA foi utilizada principalmente como apoio durante o desenvolvimento, especialmente para revisão de decisões técnicas, organização das etapas e validação do comportamento da solução.

A arquitetura do projeto, a separação entre API, regra de negócio e testes, a estratégia de leitura do `professores.json`, a lógica de disponibilidade e a estrutura do frontend foram definidas por mim antes da implementação.

A maior parte do código foi escrita manualmente. A IA foi usada principalmente para:

1. Revisar decisões de arquitetura e apontar possíveis inconsistências.
2. Ajudar a dividir o desenvolvimento em etapas menores e verificáveis.
3. Revisar regras de negócio, principalmente casos de borda relacionados a horários e sobreposição.
4. Sugerir cenários de teste que poderiam revelar falhas na implementação.
5. Revisar a organização dos arquivos e a clareza da solução.
6. Apoiar a validação do comportamento da API e do frontend.
7. Revisar a documentação final.

As sugestões da IA não foram aplicadas automaticamente. Cada decisão relevante foi revisada antes de ser incorporada ao projeto.

## Como as sugestões foram verificadas

Nenhuma etapa foi considerada concluída sem validação prática.

- **Build e testes:** `dotnet build` e `dotnet test` foram executados ao longo do desenvolvimento. Na validação final do backend, os 15 testes automatizados passaram.
- **Desserialização dos dados:** o `professores.json` foi carregado de forma real, verificando os 8 professores e conferindo valores concretos para garantir que a desserialização não tivesse produzido objetos vazios ou incorretos.
- **Regra de negócio:** cenários de disponibilidade, interseção entre dias, colisão de horários e limites dos blocos foram comparados com resultados calculados manualmente.
- **API:** o endpoint `POST /api/professores/busca` foi executado de verdade. O exemplo do enunciado retornou `200`, e diferentes requests inválidos retornaram `400`.
- **Frontend:** foram executados `npm run build` e `npm run lint`, além da aplicação em navegador. Foram verificados os estados inicial, carregando, sucesso, nenhum professor encontrado, erros de validação e falha de comunicação com a API.

A IA foi tratada como uma segunda camada de revisão. O fato de uma sugestão parecer correta não foi considerado suficiente para aceitá-la sem teste.

## Caso em que a IA errou

Durante a modelagem do `professores.json`, foi sugerido inicialmente usar `TimeOnly` diretamente para representar os campos de horário, como `inicio` e `fim`.

A hipótese era que o `System.Text.Json` conseguiria converter valores como:

```json
"18:00"

diretamente para TimeOnly.
Como o erro foi percebido
O projeto compilava normalmente, então o problema não aparecia no build.
Ao testar a desserialização real do arquivo com .NET 8, ocorreu uma falha logo no primeiro campo de horário:
System.Text.Json.JsonException: The JSON value could not be converted to System.TimeOnly.
Path: $.professores[0].disponibilidade[0].inicio
---> System.FormatException: The JSON value is not in a supported TimeOnly format.

Isso mostrou que o conversor padrão de TimeOnly do System.Text.Json não aceitava diretamente o formato "HH:mm" usado no arquivo.
Correção
Uma alternativa seria criar um conversor JSON específico para TimeOnly, mas isso adicionaria complexidade sem necessidade para o escopo do desafio.
Por isso, os horários foram mantidos como string nos modelos, preservando exatamente o formato recebido no JSON.
Durante os cálculos da regra de negócio, essas strings são convertidas internamente para TimeSpan, permitindo realizar operações de duração, comparação e detecção de sobreposição sem alterar o formato dos dados de entrada.
Depois dessa alteração, o arquivo foi desserializado corretamente e os 8 professores foram carregados como esperado.
Esse caso reforçou a importância de validar sugestões da IA com execução real, em vez de considerar uma solução correta apenas porque ela compila.
```