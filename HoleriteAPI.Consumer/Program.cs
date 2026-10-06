using HoleriteAPI.Core.Application;
using HoleriteAPI.Core.Application.Interfaces;
using HoleriteAPI.Core.Domain.Ports;
using HoleriteAPI.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IHoleriteRepository, HoleriteRepository>();
builder.Services.AddScoped<IHoleriteService, HoleriteService>();

// API pública, só de leitura e sem dados de usuário: qualquer origem pode consultar (GET).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", policy =>
{
  policy.AllowAnyOrigin()
        .WithMethods("GET")
        .AllowAnyHeader();
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyPolicy");

app.MapControllers();

app.Run();

// Permite que os testes de integração usem WebApplicationFactory<Program>.
public partial class Program { }
