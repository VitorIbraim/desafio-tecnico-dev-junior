using Desafio.Api.Contracts;
using Desafio.Api.Repositories;
using Desafio.Api.Validators;
using Desafio.Core.Models;
using Desafio.Core.Repositories;
using Desafio.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProfessorRepository>(
    new JsonProfessorRepository(Path.Combine(AppContext.BaseDirectory, "professores.json")));
builder.Services.AddSingleton<BuscaProfessoresService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/api/professores/busca", (BuscaProfessoresRequest request, BuscaProfessoresService service) =>
{
    var erros = BuscaProfessoresRequestValidator.Validar(request);

    if (erros.Count > 0)
    {
        return Results.BadRequest(new { erros });
    }

    var filtro = new FiltroBusca
    {
        Idioma = request.Idioma!,
        Dias = request.Dias!,
        InicioFaixa = request.InicioFaixa!,
        FimFaixa = request.FimFaixa!,
        DuracaoMinutos = request.DuracaoMinutos!.Value
    };

    return Results.Ok(service.Buscar(filtro));
});

app.Run();
