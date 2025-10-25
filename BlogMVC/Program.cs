using BlogMVC.Configuraciones;
using BlogMVC.Datos;
using BlogMVC.Entidades;
using BlogMVC.Jobs;
using BlogMVC.Servicios;
using BlogMVC.Utilidades;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// IA
builder.Services.AddOptions<ConfiguracionesIA>() // Configuración de opciones para IA
    .Bind(builder.Configuration.GetSection(ConfiguracionesIA.Seccion)) // Vinculación de la sección de configuración
    .ValidateDataAnnotations() // Validación de datos anotados
    .ValidateOnStart(); // Validación al iniciar la aplicación
builder.Services.AddScoped(sp =>
{
    var configuracionesIA = sp.GetRequiredService<IOptions<ConfiguracionesIA>>();
    return new OpenAIClient(configuracionesIA.Value.LlaveOpenAI);
});

// Blazor
builder.Services.AddServerSideBlazor();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>(); // Inyección de dependencia para el servicio de almacenamiento de archivos
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>(); // Inyección de dependencia para el servicio de usuarios
builder.Services.AddTransient<IServicioChat, ServicioChatOpenAI>(); // Inyección de dependencia para el servicio de chat IA
builder.Services.AddTransient<IServicioImagenes, ServicioImagenesOpenAI>(); // Inyección de dependencia para el servicio de generación de imágenes IA
builder.Services.AddScoped<IAnalisisSentimientos, AnalisisSentimientosOpenAI>(); // Inyección de dependencia para el servicio de análisis de sentimientos IA. Se usa Scoped porque se usa ApplicationDbContext en el servicio.

builder.Services.AddHttpClient(); // Cliente HTTP para llamadas externas

// Tarea de fondo que se aplicará de forma recurrente para el análisis de sentimientos.
builder.Services.AddHostedService<AnalisisSentimientosRecurrente>();

builder.Services.AddDbContextFactory<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection")
            .UseSeeding(Seeding.Aplicar)
            .UseAsyncSeeding(Seeding.AplicarAsync)
);
builder.Services.AddIdentity<Usuario, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, opciones =>
{
    opciones.LoginPath = "/usuarios/login";
    opciones.AccessDeniedPath = "/usuarios/login";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapBlazorHub(); // Mapea el hub de Blazor

app.Run();
