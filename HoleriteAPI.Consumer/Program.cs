using HoleriteAPI.Core.Application;
using HoleriteAPI.Core.Application.Interfaces;
using HoleriteAPI.Core.Domain.Ports;
using HoleriteAPI.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configuração do serviço de autenticação Google
builder.Services.AddAuthentication(options =>
{
  options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
  options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
  options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddScoped<IHoleriteRepository, HoleriteRepository>();
builder.Services.AddScoped<IHoleriteService, HoleriteService>();

// Configuração do Swagger e CORS
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
  builder.WithOrigins("*")
         .AllowAnyMethod()
         .AllowAnyHeader();
}));

var app = builder.Build();

// Configuração do pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyPolicy");

app.UseAuthentication();  // Certifique-se de adicionar a autenticação
app.UseAuthorization();

app.MapControllers();

app.Run();
