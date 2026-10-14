namespace Desafio.Core.Models;

public class Professor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public List<string> Idiomas { get; set; } = [];
    public bool AgendaBloqueada { get; set; }
    public List<BlocoDisponibilidade> Disponibilidade { get; set; } = [];
    public List<AulaMarcada> AulasMarcadas { get; set; } = [];
}
