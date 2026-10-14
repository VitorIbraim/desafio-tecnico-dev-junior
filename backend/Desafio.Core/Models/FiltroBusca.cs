namespace Desafio.Core.Models;

public class FiltroBusca
{
    public string Idioma { get; set; } = string.Empty;
    public List<string> Dias { get; set; } = [];
    public string InicioFaixa { get; set; } = string.Empty;
    public string FimFaixa { get; set; } = string.Empty;
    public int DuracaoMinutos { get; set; }
}
