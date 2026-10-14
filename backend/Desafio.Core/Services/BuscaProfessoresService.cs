using System.Globalization;
using Desafio.Core.Models;
using Desafio.Core.Repositories;

namespace Desafio.Core.Services;

public class BuscaProfessoresService
{
    private const string FormatoHorario = @"hh\:mm";
    private const int MinutosEntreInicios = 30;

    private readonly IProfessorRepository _professorRepository;

    public BuscaProfessoresService(IProfessorRepository professorRepository)
    {
        _professorRepository = professorRepository;
    }

    public ResultadoBusca Buscar(FiltroBusca filtro)
    {
        var duracao = TimeSpan.FromMinutes(filtro.DuracaoMinutos);
        var inicios = GerarInicios(ParaHorario(filtro.InicioFaixa), ParaHorario(filtro.FimFaixa), duracao);

        var elegiveis = new List<ProfessorElegivel>();
        var naoElegiveis = new List<ProfessorNaoElegivel>();

        foreach (var professor in _professorRepository.ObterTodos())
        {
            if (!professor.Idiomas.Contains(filtro.Idioma, StringComparer.OrdinalIgnoreCase))
            {
                naoElegiveis.Add(NaoElegivel(professor, "Não ensina o idioma"));
                continue;
            }

            if (professor.AgendaBloqueada)
            {
                naoElegiveis.Add(NaoElegivel(professor, "Agenda bloqueada"));
                continue;
            }

            var horarios = inicios
                .Where(inicio => filtro.Dias.All(dia => EstaLivre(professor, dia, inicio, duracao)))
                .ToList();

            if (horarios.Count == 0)
            {
                naoElegiveis.Add(NaoElegivel(professor, "Sem horário em comum nos dias pedidos"));
                continue;
            }

            elegiveis.Add(new ProfessorElegivel
            {
                Id = professor.Id,
                Nome = professor.Nome,
                AulasNaSemana = professor.AulasMarcadas.Count,
                Horarios = horarios.Select(horario => horario.ToString(FormatoHorario)).ToList()
            });
        }

        return new ResultadoBusca
        {
            Professores = elegiveis
                .OrderBy(professor => professor.AulasNaSemana)
                .ThenBy(professor => professor.Nome, StringComparer.InvariantCulture)
                .ToList(),
            NaoElegiveis = naoElegiveis
        };
    }

    private static List<TimeSpan> GerarInicios(TimeSpan inicioFaixa, TimeSpan fimFaixa, TimeSpan duracao)
    {
        var inicios = new List<TimeSpan>();
        var primeiroInicio = TimeSpan.FromMinutes(
            Math.Ceiling(inicioFaixa.TotalMinutes / MinutosEntreInicios) * MinutosEntreInicios);

        for (var inicio = primeiroInicio; inicio + duracao <= fimFaixa; inicio += TimeSpan.FromMinutes(MinutosEntreInicios))
        {
            inicios.Add(inicio);
        }

        return inicios;
    }

    private static bool EstaLivre(Professor professor, string dia, TimeSpan inicio, TimeSpan duracao)
    {
        var fim = inicio + duracao;

        var cabeEmBloco = professor.Disponibilidade.Any(bloco =>
            bloco.Dia == dia
            && ParaHorario(bloco.Inicio) <= inicio
            && fim <= ParaHorario(bloco.Fim));

        var conflitaComAula = professor.AulasMarcadas.Any(aula =>
        {
            var inicioAula = ParaHorario(aula.Inicio);
            var fimAula = inicioAula + TimeSpan.FromMinutes(aula.DuracaoMinutos);
            return aula.Dia == dia && inicio < fimAula && inicioAula < fim;
        });

        return cabeEmBloco && !conflitaComAula;
    }

    private static ProfessorNaoElegivel NaoElegivel(Professor professor, string motivo) => new()
    {
        Id = professor.Id,
        Nome = professor.Nome,
        Motivo = motivo
    };

    private static TimeSpan ParaHorario(string valor) =>
        TimeSpan.ParseExact(valor, FormatoHorario, CultureInfo.InvariantCulture);
}
