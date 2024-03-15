using Stoquei.Application.Interfaces;
using Stoquei.Application.Services;
using Stoquei.Infra.Interfaces;
using Stoquei.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

// Add services to the container.

var services = builder.Services;

services.AddControllers();
services.AddCors(options =>
{
    options.AddPolicy(name: "MyPolicy",
                      policy =>
                      {
                          policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                      });
});

// Dependency Injection
services.AddSingleton<IUsuariosService, UsuariosService>();
services.AddSingleton<IProdutoService, ProdutoService>();
services.AddSingleton<IFornecedorService, FornecedorService>();

var connectionString = configuration["ConnectionStrings:DB_Stoquei"];
services.AddSingleton<IUsuarioRepository, UsuarioRepository>(x => new UsuarioRepository(connectionString));
services.AddSingleton<IProdutoRepository, ProdutoRepository>(x => new ProdutoRepository(connectionString));
services.AddSingleton<IFornecedorRepository, FornecedorRepository>(x => new FornecedorRepository(connectionString));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseCors("MyPolicy");

app.MapControllers();

app.Run();