# Encontre um Professor

Solução do desafio técnico: uma API em C#/.NET 8 que encontra professores disponíveis para um novo aluno e uma tela em React + TypeScript para utilizar a busca.

O aluno informa o idioma, os dias da semana, a faixa de horário disponível e a duração da aula. A API retorna os professores elegíveis, os horários de início possíveis e, como diferencial, os professores não elegíveis com o respectivo motivo.

## Tecnologias

- **Backend:** C# com .NET 8, ASP.NET Core (Minimal API) e `System.Text.Json`
- **Testes:** xUnit
- **Frontend:** React 19 com TypeScript, Vite 8 e Oxlint

Nenhuma biblioteca foi adicionada além das dependências trazidas pelos templates do .NET e do Vite.

## Estrutura

```text
backend/
  DesafioTecnico.sln
  Desafio.Api/      endpoint HTTP, validação do request e leitura do professores.json
  Desafio.Core/     modelos e regra de busca, sem dependência de HTTP
  Desafio.Tests/    testes xUnit da regra de negócio

frontend/           aplicação React + TypeScript
professores.json    dados dos professores

Pré-requisitos
- SDK do .NET 8
- Node.js 20.19+ ou 22.12+ e npm
Como executar
API
cd backend
dotnet run --project Desafio.Api

A API será iniciada em:
http://localhost:5239

O professores.json da raiz é copiado para a saída da aplicação durante o build e carregado quando a API inicia.
Frontend
Com a API em execução, abra outro terminal:
cd frontend
npm install
npm run dev

Abra o endereço informado pelo Vite, normalmente:
http://localhost:5173

Durante o desenvolvimento, o Vite encaminha as requisições para /api até a API em http://localhost:5239.
Também estão disponíveis:
npm run build
npm run lint

Testes
cd backend
dotnet test

A suíte possui 15 casos de teste sobre as principais regras da busca.
Os testes utilizam um repositório falso em memória e não dependem da API nem do arquivo professores.json.
Dados
Os professores estão definidos em [`professores.json`](professores.json).
Cada professor possui:
Campo	Descrição
id, nome	Identificação do professor
idiomas	Idiomas que o professor ensina
agendaBloqueada	Indica se pode receber novos alunos
disponibilidade	Blocos semanais disponíveis (dia, inicio, fim)
aulasMarcadas	Aulas já existentes (dia, inicio, duracaoMinutos)


A implementação considera as garantias fornecidas pelo enunciado:
- horários no formato HH:mm;
- nenhum intervalo atravessa a meia-noite;
- blocos de disponibilidade do mesmo dia não se sobrepõem nem se encostam;
- dias válidos: segunda, terca, quarta, quinta, sexta, sabado, domingo.
Endpoint
POST /api/professores/busca
Request
{
  "idioma": "Espanhol",
  "dias": ["segunda"],
  "inicioFaixa": "18:00",
  "fimFaixa": "20:00",
  "duracaoMinutos": 60
}

Resposta 200
{
  "professores": [
    {
      "id": 1,
      "nome": "Ana",
      "aulasNaSemana": 1,
      "horarios": ["18:00"]
    },
    {
      "id": 4,
      "nome": "Diego",
      "aulasNaSemana": 1,
      "horarios": ["18:00", "18:30", "19:00"]
    }
  ],
  "naoElegiveis": [
    {
      "id": 2,
      "nome": "Bruno",
      "motivo": "Não ensina o idioma"
    },
    {
      "id": 3,
      "nome": "Carla",
      "motivo": "Não ensina o idioma"
    },
    {
      "id": 5,
      "nome": "Elisa",
      "motivo": "Não ensina o idioma"
    },
    {
      "id": 6,
      "nome": "Fábio",
      "motivo": "Não ensina o idioma"
    },
    {
      "id": 7,
      "nome": "Gabriela",
      "motivo": "Não ensina o idioma"
    },
    {
      "id": 8,
      "nome": "Heitor",
      "motivo": "Não ensina o idioma"
    }
  ]
}

Resposta 400
Requests inválidos retornam as mensagens de validação aplicáveis:
{
  "erros": [
    "O fim da faixa deve ser posterior ao início."
  ]
}

Validações implementadas:
Situação	Mensagem
Idioma ausente, vazio ou somente espaços	O idioma é obrigatório.
Nenhum dia informado	Informe pelo menos um dia.
Dia inválido	Dia inválido: 'X'. Use: segunda, terca, quarta, quinta, sexta, sabado, domingo.
Início ou fim ausente	O início/fim da faixa é obrigatório.
Horário fora de HH:mm	O início/fim da faixa deve estar no formato HH:mm.
Fim igual ou anterior ao início	O fim da faixa deve ser posterior ao início.
Duração diferente de 30, 60 ou 90	A duração deve ser de 30, 60 ou 90 minutos.


Regras da busca
1. O professor precisa ensinar o idioma solicitado.
2. Professores com agenda bloqueada não são elegíveis.
3. O aluno deseja um horário fixo: o mesmo horário de início precisa estar disponível em todos os dias solicitados.
4. A aula precisa caber integralmente em um bloco de disponibilidade do professor e dentro da faixa solicitada pelo aluno.
5. A aula não pode se sobrepor a uma aula já marcada. Encostar é permitido: uma aula que termina às 19:00 permite outra começando às 19:00.
6. A aula só pode começar em :00 ou :30.
7. Professores elegíveis são ordenados por menor quantidade de aulas na semana e, em caso de empate, por nome. Os horários são retornados em ordem crescente.
Professores não elegíveis recebem apenas o primeiro motivo aplicável, nesta ordem:
1. Não ensina o idioma
2. Agenda bloqueada
3. Sem horário em comum nos dias pedidos
Decisões técnicas
- Regra de negócio separada do HTTP: o Desafio.Core contém os modelos, IProfessorRepository e BuscaProfessoresService, sem dependência de ASP.NET.
- Acesso aos dados: JsonProfessorRepository é responsável pela leitura de professores.json com System.Text.Json.
- Horários: os modelos mantêm os horários como string no formato HH:mm. Durante os cálculos da regra de negócio, os valores são convertidos para TimeSpan. A decisão sobre a desserialização está descrita em [`USO_DE_IA.md`](USO_DE_IA.md).
- Geração de candidatos: os horários são avaliados de 30 em 30 minutos, sempre em :00 ou :30.
- Interseção dos dias: um horário só é válido quando está disponível em todos os dias solicitados.
- Colisão de horários: é considerada sobreposição quando:
inicio < fimAula && inicioAula < fim

As comparações estritas permitem que dois intervalos apenas se encostem.
- Validação: ocorre na camada da API antes da execução da regra de busca.
- Frontend: utiliza apenas React, useState e fetch, sem bibliotecas adicionais de estado, formulário ou HTTP.
Decisões para casos não especificados
- A comparação de idioma não diferencia maiúsculas de minúsculas.
- Os dias devem ser informados usando os valores definidos no enunciado.
- Quando a faixa começa fora de :00 ou :30, o primeiro candidato considerado é o próximo horário válido da grade.
Observação sobre execução
Durante o desenvolvimento, o frontend utiliza o proxy do Vite para encaminhar /api até http://localhost:5239. Por isso, nenhuma configuração adicional de CORS é necessária para executar a solução localmente conforme os passos deste README.