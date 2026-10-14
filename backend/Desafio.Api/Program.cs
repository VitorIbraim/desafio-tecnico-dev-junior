using Desafio.Api.Repositories;
using Desafio.Core.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProfessorRepository>(
    new JsonProfessorRepository(Path.Combine(AppContext.BaseDirectory, "professores.json")));

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
