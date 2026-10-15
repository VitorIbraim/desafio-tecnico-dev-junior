namespace Desafio.Api.Contracts;

public class BuscaProfessoresRequest
{
    public string? Idioma { get; set; }
    public List<string>? Dias { get; set; }
    public string? InicioFaixa { get; set; }
    public string? FimFaixa { get; set; }
    public int? DuracaoMinutos { get; set; }
}
