using Microsoft.EntityFrameworkCore;
using RFAW.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 1. Criando o Passe VIP chamado "PermitirReact"
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirReact", policy =>
    {
        policy.AllowAnyOrigin()  // Permite qualquer site (seu React)
              .AllowAnyHeader()  // Permite qualquer tipo de dado
              .AllowAnyMethod(); // Permite GET, POST, PUT, etc.
    });
});

var app = builder.Build();


app.UseCors("PermitirReact");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/reservar-cota/{numeroDesejado}", (int numeroDesejado, string nomeCliente, string telCliente, RFAW.Data.AppDbContext db) =>
{
    var cota = db.Cotas.FirstOrDefault(c => c.Numero == numeroDesejado);

    if (cota == null)
        return "Erro esse numero nao existe";

    if (cota.Status != "Disponivel")
        return "Ops! Esse numero ja foi selecionado";

    cota.Status = "Pendente";
    cota.Nome = nomeCliente;
    cota.Tel = telCliente;

    db.SaveChanges();

    return $"Sucesso, cauan! O número {numeroDesejado} foi reservado para {nomeCliente}. Agora é só aguardar o PIX!";
});

app.MapGet("/aprovar-cota/{numeroparaaprovar}", (int numeroparaaprovar, RFAW.Data.AppDbContext db) =>
{
    var cota = db.Cotas.FirstOrDefault(c => c.Numero == numeroparaaprovar);

    if (cota == null)
        return "Nulo";

    if (cota.Status != "Pendente")
        return "Essa cota ainda não foi reservada";

    cota.Status = "Vendido";

    db.SaveChanges();

    return $"Rifa aprovada";

});

app.MapGet("/cotas-disponiveis/{iddarifa}", (int iddarifa, RFAW.Data.AppDbContext db) =>
{
    // O banco vai filtrar a rifa certa E os bilhetes que estão livres
    var cotasLivres = db.Cotas
                        .Where(c => c.RifaId == iddarifa && c.Status == "Disponivel")
                        .ToList();

    return cotasLivres;
});

app.MapGet("/cotas-vendidas/{iddarifa}", (int iddarifa, RFAW.Data.AppDbContext db) =>
{
    var cotasVendidas = db.Cotas
                          .Where(c => c.RifaId == iddarifa && c.Status == "Vendido")
                          .ToList();

    return cotasVendidas;
});

app.MapGet("/criar-rifa/{quantidade}", (int quantidade, string titulo, float preco, RFAW.Data.AppDbContext db) =>
{
    var novarifa = new RFAW.Models.NovaPasta.Rifa
    {
        Titulo = titulo,
        QuantidadeCotas = quantidade,
        Preço = preco

    };

    for (int i = 1; i <= novarifa.QuantidadeCotas; i++) 
    {
        var novaCota = new RFAW.Models.NovaPasta.Cota
        {
            Numero = i
        };

        novarifa.Cotas.Add(novaCota);
    }

    db.Rifas.Add(novarifa);
    db.SaveChanges();
    return $"Sucesso total! A rifa '{titulo}' foi criada no banco com {quantidade} cotas a R$ {preco}. O ID dela é {novarifa.Id}!";
});

app.Run();
