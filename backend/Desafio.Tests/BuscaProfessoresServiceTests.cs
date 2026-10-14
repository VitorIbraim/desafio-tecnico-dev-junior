using Desafio.Core.Models;
using Desafio.Core.Services;

namespace Desafio.Tests;

public class BuscaProfessoresServiceTests
{
    [Fact]
    public void Buscar_ProfessorQueNaoEnsinaOIdioma_FicaNaoElegivel()
    {
        var professor = CriarProfessor(1, "Ana", ["Inglês"], disponibilidade: [Bloco("segunda", "18:00", "22:00")]);

        var resultado = Buscar(CriarFiltro("Espanhol", ["segunda"], "18:00", "20:00", 60), professor);

        Assert.Empty(resultado.Professores);
        var naoElegivel = Assert.Single(resultado.NaoElegiveis);
        Assert.Equal(1, naoElegivel.Id);
        Assert.Equal("Não ensina o idioma", naoElegivel.Motivo);
    }

    [Fact]
    public void Buscar_ProfessorComAgendaBloqueada_FicaNaoElegivel()
    {
        var professor = CriarProfessor(1, "Ana", ["Inglês"], disponibilidade: [Bloco("segunda", "18:00", "22:00")], agendaBloqueada: true);

        var resultado = Buscar(CriarFiltro("Inglês", ["segunda"], "18:00", "20:00", 60), professor);

        Assert.Empty(resultado.Professores);
        var naoElegivel = Assert.Single(resultado.NaoElegiveis);
        Assert.Equal(1, naoElegivel.Id);
        Assert.Equal("Agenda bloqueada", naoElegivel.Motivo);
    }

    [Fact]
    public void Buscar_VariosDias_RetornaSomenteIniciosLivresEmTodosOsDias()
    {
        var professor = CriarProfessor(
            1,
            "Ana",
            ["Inglês"],
            disponibilidade: [Bloco("segunda", "18:00", "21:00"), Bloco("quarta", "18:00", "20:00")],
            aulas: [Aula("segunda", "18:00", 60)]);

        var resultado = Buscar(CriarFiltro("Inglês", ["segunda", "quarta"], "18:00", "21:00", 60), professor);

        var elegivel = Assert.Single(resultado.Professores);
        Assert.Equal(new[] { "19:00" }, elegivel.Horarios);
    }

    [Fact]
    public void Buscar_DiasSemInicioEmComum_FicaNaoElegivel()
    {
        var professor = CriarProfessor(
            1,
            "Ana",
            ["Inglês"],
            disponibilidade: [Bloco("segunda", "18:00", "19:00"), Bloco("quarta", "20:00", "21:00")]);

        var resultado = Buscar(CriarFiltro("Inglês", ["segunda", "quarta"], "18:00", "21:00", 60), professor);

        Assert.Empty(resultado.Professores);
        var naoElegivel = Assert.Single(resultado.NaoElegiveis);
        Assert.Equal("Sem horário em comum nos dias pedidos", naoElegivel.Motivo);
    }

    [Theory]
    [InlineData("18:30", "19:30")]
    [InlineData("19:00", "20:00")]
    [InlineData("19:30", "20:30")]
    public void Buscar_HorarioQueSobrepoeAulaMarcada_NaoEhRetornado(string inicioFaixa, string fimFaixa)
    {
        var professor = CriarProfessor(
            1,
            "Ana",
            ["Inglês"],
            disponibilidade: [Bloco("segunda", "18:00", "22:00")],
            aulas: [Aula("segunda", "19:00", 60)]);

        var resultado = Buscar(CriarFiltro("Inglês", ["segunda"], inicioFaixa, fimFaixa, 60), professor);

        Assert.Empty(resultado.Professores);
        var naoElegivel = Assert.Single(resultado.NaoElegiveis);
        Assert.Equal("Sem horário em comum nos dias pedidos", naoElegivel.Motivo);
    }

    [Theory]
    [InlineData("18:00", "19:00")]
    [InlineData("20:00", "21:00")]
    public void Buscar_HorarioQueApenasEncostaEmAulaMarcada_EhRetornado(string inicioFaixa, string fimFaixa)
    {
        var professor = CriarProfessor(
            1,
            "Ana",
            ["Inglês"],
            disponibilidade: [Bloco("segunda", "18:00", "22:00")],
            aulas: [Aula("segunda", "19:00", 60)]);

        var resultado = Buscar(CriarFiltro("Inglês", ["segunda"], inicioFaixa, fimFaixa, 60), professor);

        var elegivel = Assert.Single(resultado.Professores);
        Assert.Equal(new[] { inicioFaixa }, elegivel.Horarios);
    }

    [Fact]
    public void Buscar_FaixaComecandoForaDaGrade_RetornaSomenteIniciosEmHoraCheiaOuMeiaHora()
    {
        var professor = CriarProfessor(1, "Ana", ["Inglês"], disponibilidade: [Bloco("segunda", "18:15", "20:00")]);

        var resultado = Buscar(CriarFiltro("Inglês", ["segunda"], "18:15", "20:00", 30), professor);

        var elegivel = Assert.Single(resultado.Professores);
        Assert.Equal(new[] { "18:30", "19:00", "19:30" }, elegivel.Horarios);
    }

    [Fact]
    public void Buscar_AulaQueNaoCabeInteiraEmUmBloco_NaoEhRetornada()
    {
        var professor = CriarProfessor(
            1,
            "Ana",
            ["Inglês"],
            disponibilidade: [Bloco("segunda", "18:00", "19:00"), Bloco("segunda", "20:00", "21:30")]);

        var resultado = Buscar(CriarFiltro("Inglês", ["segunda"], "18:00", "22:00", 60), professor);

        var elegivel = Assert.Single(resultado.Professores);
        Assert.Equal(new[] { "18:00", "20:00", "20:30" }, elegivel.Horarios);
    }

    [Fact]
    public void Buscar_AulaQueUltrapassaOFimDaFaixa_NaoEhRetornada()
    {
        var professor = CriarProfessor(1, "Ana", ["Inglês"], disponibilidade: [Bloco("segunda", "18:00", "22:00")]);

        var resultado = Buscar(CriarFiltro("Inglês", ["segunda"], "18:00", "19:30", 90), professor);

        var elegivel = Assert.Single(resultado.Professores);
        Assert.Equal(new[] { "18:00" }, elegivel.Horarios);
    }

    [Fact]
    public void Buscar_VariosElegiveis_OrdenaPorMenosAulasEDepoisPorNome()
    {
        var segunda = new[] { Bloco("segunda", "18:00", "19:00") };
        var umaAula = new[] { Aula("terca", "08:00", 30) };
        var duasAulas = new[] { Aula("terca", "08:00", 30), Aula("terca", "09:00", 30) };

        var resultado = Buscar(
            CriarFiltro("Inglês", ["segunda"], "18:00", "19:00", 60),
            CriarProfessor(1, "Carla", ["Inglês"], segunda, umaAula),
            CriarProfessor(2, "Aline", ["Inglês"], segunda, duasAulas),
            CriarProfessor(3, "Bruna", ["Inglês"], segunda, umaAula),
            CriarProfessor(4, "Davi", ["Inglês"], segunda),
            CriarProfessor(5, "Álvaro", ["Inglês"], segunda, umaAula));

        Assert.Equal(new[] { "Davi", "Álvaro", "Bruna", "Carla", "Aline" }, resultado.Professores.Select(professor => professor.Nome));
    }

    [Fact]
    public void Buscar_ProfessorComMaisDeUmMotivo_RetornaPrimeiroMotivoPelaPrioridade()
    {
        var resultado = Buscar(
            CriarFiltro("Inglês", ["segunda"], "18:00", "20:00", 60),
            CriarProfessor(1, "Ana", ["Francês"], agendaBloqueada: true),
            CriarProfessor(2, "Bruno", ["Inglês"], agendaBloqueada: true),
            CriarProfessor(3, "Carla", ["Inglês"]));

        Assert.Empty(resultado.Professores);
        Assert.Equal(
            new[]
            {
                (1, "Não ensina o idioma"),
                (2, "Agenda bloqueada"),
                (3, "Sem horário em comum nos dias pedidos")
            },
            resultado.NaoElegiveis.Select(professor => (professor.Id, professor.Motivo)));
    }

    [Fact]
    public void Buscar_ExemploDoReadme_RetornaResultadoEsperado()
    {
        var ana = CriarProfessor(
            1,
            "Ana",
            ["Inglês", "Espanhol"],
            disponibilidade: [Bloco("segunda", "18:00", "22:00"), Bloco("quarta", "18:00", "22:00")],
            aulas: [Aula("segunda", "19:00", 60)]);
        var bruno = CriarProfessor(
            2,
            "Bruno",
            ["Inglês"],
            disponibilidade:
            [
                Bloco("segunda", "18:00", "21:00"),
                Bloco("terca", "18:00", "20:00"),
                Bloco("quarta", "18:00", "19:00"),
                Bloco("quarta", "20:00", "22:00"),
                Bloco("quinta", "18:00", "20:00")
            ],
            aulas: [Aula("terca", "18:00", 60), Aula("quinta", "18:00", 60)]);
        var diego = CriarProfessor(
            4,
            "Diego",
            ["Espanhol"],
            disponibilidade: [Bloco("segunda", "18:00", "22:00"), Bloco("terca", "08:00", "12:00"), Bloco("quarta", "18:00", "22:00")],
            aulas: [Aula("terca", "08:30", 30)]);

        var resultado = Buscar(CriarFiltro("Espanhol", ["segunda"], "18:00", "20:00", 60), ana, bruno, diego);

        Assert.Collection(
            resultado.Professores,
            elegivel =>
            {
                Assert.Equal((1, "Ana", 1), (elegivel.Id, elegivel.Nome, elegivel.AulasNaSemana));
                Assert.Equal(new[] { "18:00" }, elegivel.Horarios);
            },
            elegivel =>
            {
                Assert.Equal((4, "Diego", 1), (elegivel.Id, elegivel.Nome, elegivel.AulasNaSemana));
                Assert.Equal(new[] { "18:00", "18:30", "19:00" }, elegivel.Horarios);
            });
        var naoElegivel = Assert.Single(resultado.NaoElegiveis);
        Assert.Equal((2, "Bruno", "Não ensina o idioma"), (naoElegivel.Id, naoElegivel.Nome, naoElegivel.Motivo));
    }

    private static ResultadoBusca Buscar(FiltroBusca filtro, params Professor[] professores) =>
        new BuscaProfessoresService(new FakeProfessorRepository(professores)).Buscar(filtro);

    private static FiltroBusca CriarFiltro(string idioma, string[] dias, string inicioFaixa, string fimFaixa, int duracaoMinutos) => new()
    {
        Idioma = idioma,
        Dias = dias.ToList(),
        InicioFaixa = inicioFaixa,
        FimFaixa = fimFaixa,
        DuracaoMinutos = duracaoMinutos
    };

    private static Professor CriarProfessor(
        int id,
        string nome,
        string[] idiomas,
        BlocoDisponibilidade[]? disponibilidade = null,
        AulaMarcada[]? aulas = null,
        bool agendaBloqueada = false) => new()
    {
        Id = id,
        Nome = nome,
        Idiomas = idiomas.ToList(),
        AgendaBloqueada = agendaBloqueada,
        Disponibilidade = disponibilidade?.ToList() ?? [],
        AulasMarcadas = aulas?.ToList() ?? []
    };

    private static BlocoDisponibilidade Bloco(string dia, string inicio, string fim) => new()
    {
        Dia = dia,
        Inicio = inicio,
        Fim = fim
    };

    private static AulaMarcada Aula(string dia, string inicio, int duracaoMinutos) => new()
    {
        Dia = dia,
        Inicio = inicio,
        DuracaoMinutos = duracaoMinutos
    };
}
