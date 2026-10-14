namespace Desafio.Core.Models;

public class ResultadoBusca
{
    public List<ProfessorElegivel> Professores { get; set; } = [];
    public List<ProfessorNaoElegivel> NaoElegiveis { get; set; } = [];
}
