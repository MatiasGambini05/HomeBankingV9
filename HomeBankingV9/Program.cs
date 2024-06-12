using HomeBankingV9.Models;
using HomeBankingV9.Repositories;
using HomeBankingV9.Repositories.Implementations;
using HomeBankingV9.Services;
using HomeBankingV9.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// Add context to the container.
builder.Services.AddDbContext<HomeBankingContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDBConnection"))
    );

//Add repositories to the container.
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IClientLoansRepository, ClientLoansRepository>();

//Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ILoanService, LoanService>();

//Add authentication to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(au =>
    {
        au.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        au.LoginPath = new PathString("/index.html");
    });

//Add authorization to the container.
builder.Services.AddAuthorization(aut =>
{
    aut.AddPolicy("Client Only", po => po.RequireClaim("Client"));
    aut.AddPolicy("AdminOnly", po => po.RequireClaim("Admin"));
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
    }

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
