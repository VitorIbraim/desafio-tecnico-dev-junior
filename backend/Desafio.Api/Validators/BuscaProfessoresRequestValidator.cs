using System.Globalization;
using System.Text.RegularExpressions;
using Desafio.Api.Contracts;

namespace Desafio.Api.Validators;

public static class BuscaProfessoresRequestValidator
{
    private static readonly string[] DiasValidos = ["segunda", "terca", "quarta", "quinta", "sexta", "sabado", "domingo"];
    private static readonly Regex FormatoHorario = new(@"^([01][0-9]|2[0-3]):[0-5][0-9]\z");

    public static List<string> Validar(BuscaProfessoresRequest request)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Idioma))
        {
            erros.Add("O idioma é obrigatório.");
        }

        if (request.Dias is not { Count: > 0 })
        {
            erros.Add("Informe pelo menos um dia.");
        }
        else
        {
            foreach (var dia in request.Dias.Where(dia => !DiasValidos.Contains(dia)))
            {
                erros.Add($"Dia inválido: '{dia}'. Use: {string.Join(", ", DiasValidos)}.");
            }
        }

        var erroInicio = ValidarHorario(request.InicioFaixa, "início da faixa");
        var erroFim = ValidarHorario(request.FimFaixa, "fim da faixa");

        if (erroInicio is not null)
        {
            erros.Add(erroInicio);
        }

        if (erroFim is not null)
        {
            erros.Add(erroFim);
        }

        if (erroInicio is null && erroFim is null && ParaHorario(request.FimFaixa!) <= ParaHorario(request.InicioFaixa!))
        {
            erros.Add("O fim da faixa deve ser posterior ao início.");
        }

        if (request.DuracaoMinutos is not (30 or 60 or 90))
        {
            erros.Add("A duração deve ser de 30, 60 ou 90 minutos.");
        }

        return erros;
    }

    private static string? ValidarHorario(string? valor, string campo)
    {
        if (string.IsNullOrEmpty(valor))
        {
            return $"O {campo} é obrigatório.";
        }

        return FormatoHorario.IsMatch(valor) ? null : $"O {campo} deve estar no formato HH:mm.";
    }

    private static TimeOnly ParaHorario(string valor) =>
        TimeOnly.ParseExact(valor, "HH:mm", CultureInfo.InvariantCulture);
}
