using Microsoft.EntityFrameworkCore;
using RFAW.Data;
using RFAW.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirReact", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHostedService<SorteioBackgroundService>();

var app = builder.Build();

app.UseCors("PermitirReact");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/reservar-cota/{numeroDesejado}", (int numeroDesejado, string nomeCliente, string telCliente, AppDbContext db) =>
{
    var cota = db.Cotas.FirstOrDefault(c => c.Numero == numeroDesejado);

    if (cota == null) return "Erro esse numero nao existe";
    if (cota.Status != "Disponivel") return "Ops! Esse numero ja foi selecionado";

    cota.Status = "Pendente";
    cota.Nome = nomeCliente;
    cota.Tel = telCliente;

    db.SaveChanges();

    return $"Sucesso, cauan! O número {numeroDesejado} foi reservado para {nomeCliente}. Agora é só aguardar o PIX!";
});

app.MapGet("/aprovar-cota/{numeroparaaprovar}", (int numeroparaaprovar, AppDbContext db) =>
{
    var cota = db.Cotas.FirstOrDefault(c => c.Numero == numeroparaaprovar);

    if (cota == null) return "Nulo";
    if (cota.Status != "Pendente") return "Essa cota ainda não foi reservada";

    cota.Status = "Vendido";

    db.SaveChanges();

    return $"Rifa aprovada";
});

app.MapGet("/cotas-disponiveis/{iddarifa}", (int iddarifa, AppDbContext db) =>
{
    return db.Cotas.Where(c => c.RifaId == iddarifa && c.Status == "Disponivel").ToList();
});

app.MapGet("/cotas-vendidas/{iddarifa}", (int iddarifa, AppDbContext db) =>
{
    return db.Cotas.Where(c => c.RifaId == iddarifa && c.Status == "Vendido").ToList();
});

app.MapGet("/criar-rifa/{quantidade}", (int quantidade, string titulo, float preco, AppDbContext db) =>
{
    var novarifa = new RFAW.Models.Rifa
    {
        Titulo = titulo,
        QuantidadeCotas = quantidade,
        Preço = preco
    };

    for (int i = 1; i <= novarifa.QuantidadeCotas; i++)
    {
        novarifa.Cotas.Add(new RFAW.Models.Cota { Numero = i });
    }

    db.Rifas.Add(novarifa);
    db.SaveChanges();

    return $"Sucesso total! A rifa '{titulo}' foi criada no banco com {quantidade} cotas a R$ {preco}. O ID dela é {novarifa.Id}!";
});

app.Run();