using HomeBankingV9.Models;
using HomeBankingV9.Repositories;
using HomeBankingV9.Repositories.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Add swagger to the container.
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// Add context to the container.
builder.Services.AddDbContext<HomeBankingContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDBConnection"))
    );

//Add repositories to the container.
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped <ITransactionRepository, TransactionRepository>();

//Add authentication to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(au =>
    {
        au.ExpireTimeSpan = TimeSpan.FromMinutes(1);
        au.LoginPath = new PathString("/index.html");
    });

//Add authorization to the container. PREGUNTAR SOBRE LA LAMBDA EN ESTA PARTE, LAMBDA ADENTRO DE OTRA LAMBDA?
builder.Services.AddAuthorization(aut =>
{
    aut.AddPolicy("Client Only", po => po.RequireClaim("Client"));
});

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
        logger.LogError(ex, "Ocurrió un error creando la DB.");
    }
}

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
    } else
    {
        app.UseSwagger();
        app.UseSwaggerUI(sw => sw.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeBankingV9 v1"));
    }

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
