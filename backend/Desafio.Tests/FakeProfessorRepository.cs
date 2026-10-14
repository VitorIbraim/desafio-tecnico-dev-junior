using Desafio.Core.Models;
using Desafio.Core.Repositories;

namespace Desafio.Tests;

public class FakeProfessorRepository : IProfessorRepository
{
    private readonly IReadOnlyList<Professor> _professores;

    public FakeProfessorRepository(params Professor[] professores)
    {
        _professores = professores;
    }

    public IReadOnlyList<Professor> ObterTodos() => _professores;
}
