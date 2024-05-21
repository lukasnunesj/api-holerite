using HoleriteAPI.Core.Application;
using HoleriteAPI.Core.Application.Interfaces;
using HoleriteAPI.Core.Domain.Ports;
using HoleriteAPI.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IHoleriteRepository, HoleriteRepository>();
builder.Services.AddScoped<IHoleriteService, HoleriteService>();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
  builder.WithOrigins("*")
         .AllowAnyMethod()
         .AllowAnyHeader();

  // U Can Filter Here
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
