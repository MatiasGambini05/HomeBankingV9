using HomeBankingV9.Models;
using HomeBankingV9.Repositories;
using HomeBankingV9.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add context to the container.
builder.Services.AddDbContext<HomeBankingContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDBConnection"))
    );

//Add repositories to the container.
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped <ITransactionRepository, TransactionRepository>();

var app = builder.Build();

// Crear scope para inicializar base de datos.
using (var scope = app.Services.CreateScope())
{
    try
    {
        var service = scope.ServiceProvider;
        var context = service.GetRequiredService<HomeBankingContext>();
        DBInitializer.Initialize(context);
    } catch (Exception ex) {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<HomeBankingContext>>();
        logger.LogError(ex, "Ocurri� un error creando la DB.");
    }
}

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
    }
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

