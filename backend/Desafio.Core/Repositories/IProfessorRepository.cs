using Desafio.Core.Models;

namespace Desafio.Core.Repositories;

public interface IProfessorRepository
{
    IReadOnlyList<Professor> ObterTodos();
}
