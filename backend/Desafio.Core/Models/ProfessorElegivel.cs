namespace Desafio.Core.Models;

public class ProfessorElegivel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int AulasNaSemana { get; set; }
    public List<string> Horarios { get; set; } = [];
}
