using BibliotecaApi.Data;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
//área de servicios
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers().AddNewtonsoftJson();// agregamos el newtonsoft
//builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);// agregamos el newtonsoft
//builder.Services.AddControllers().AddJsonOptions(options => 
//options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);// para que ignore los ciclos

builder.Services.AddDbContext<ApplicationDBContext>(options => 
    options.UseSqlServer("name=DefaultConnection"));//llamamos el servicio para el contexto de base de datos y para que pueda ser usado dentro de toda la app
//creamos el servicio de sistema de usuarios
builder.Services.AddIdentityCore<IdentityUser>()
             .AddEntityFrameworkStores<ApplicationDBContext>()
             .AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<IdentityUser>>();//manejador de usuarios
builder.Services.AddScoped<SignInManager<IdentityUser>>();// para autenticar usuarios
builder.Services.AddTransient<IUserServices,UserServices>();//agregamos el servicio para sacar el usuario
builder.Services.AddHttpContextAccessor();// para poder acceder al contexto http desde cualquier clase
// creamos los servicios de autenticación
builder.Services.AddAuthentication().AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;// para evitar el cambio de un claim por otro de forma automatica desde aspnetcore
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer= false,// no voy a validar el emisor del token
            ValidateAudience= false,// no se valida la audiencia
            ValidateLifetime=true,// si se valida el tiempo de vida del token
            ValidateIssuerSigningKey=true,// validamos la llave secreta
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["llaveJwt"]!)),// indicamos de donde va a salir la llave secreta
            ClockSkew = TimeSpan.Zero// eso es para que no tengamos tiempos de discrepancia temporal 
        };

    });

builder.Services.AddAuthorization (options => {
    options.AddPolicy("isadmin", politic => politic.RequireClaim("isadmin"));
});

var app = builder.Build();

//área de middelwares
app.MapControllers();

app.Run();
