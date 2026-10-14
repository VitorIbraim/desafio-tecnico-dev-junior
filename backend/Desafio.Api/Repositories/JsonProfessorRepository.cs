using System.Text.Json;
using Desafio.Core.Models;
using Desafio.Core.Repositories;

namespace Desafio.Api.Repositories;

public class JsonProfessorRepository : IProfessorRepository
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IReadOnlyList<Professor> _professores;

    public JsonProfessorRepository(string caminhoArquivo)
    {
        var json = File.ReadAllText(caminhoArquivo);
        var dados = JsonSerializer.Deserialize<DadosProfessores>(json, Opcoes);

        if (dados?.Professores is not { Count: > 0 })
        {
            throw new InvalidOperationException($"O arquivo '{caminhoArquivo}' não contém uma lista de professores.");
        }

        _professores = dados.Professores;
    }

    public IReadOnlyList<Professor> ObterTodos() => _professores;
}
