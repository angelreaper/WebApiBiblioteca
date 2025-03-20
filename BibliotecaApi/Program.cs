using BibliotecaApi.Data;
using BibliotecaApi.Entities;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();

var allowedOrigin = builder.Configuration.GetSection("AllowedOrigin").Get<string[]>()!;//sacamos los origenes permitidos
builder.Services.AddCors(options => 
    options.AddDefaultPolicy(optionCORS =>
    {
        //permitimos cualquier origen, cualquie invocación de metodo y cualquier header
        optionCORS.WithOrigins(allowedOrigin)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithExposedHeaders("mi-cabecera");// también permitimos la cabecera personalizada para que cors no la bloqueé
    })
);
//área de servicios
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers().AddNewtonsoftJson();// agregamos el newtonsoft
//builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);// agregamos el newtonsoft
//builder.Services.AddControllers().AddJsonOptions(options => 
//options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);// para que ignore los ciclos

builder.Services.AddDbContext<ApplicationDBContext>(options => 
    options.UseSqlServer("name=DefaultConnection"));//llamamos el servicio para el contexto de base de datos y para que pueda ser usado dentro de toda la app
//creamos el servicio de sistema de usuarios
builder.Services.AddIdentityCore<User>()
             .AddEntityFrameworkStores<ApplicationDBContext>()
             .AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<User>>();//manejador de usuarios
builder.Services.AddScoped<SignInManager<User>>();// para autenticar usuarios
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

builder.Services.AddSwaggerGen(options =>
{ 
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Biblioteca Api",
        Description = "Este es un web api para una biblioteca",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Email = "pepito@hotmail.com",
            Name = "Pepito Perez",
            Url = new Uri("http://pagina.prueba")
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        { 
            Name="MIT",
            Url= new Uri("https://opensource.org/license/mit/")
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement( new OpenApiSecurityRequirement {
        { 
            new OpenApiSecurityScheme
            { 
                Reference = new OpenApiReference
                { 
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{ }
        }
    });
});


var app = builder.Build();

//área de middelwares

app.UseSwagger();
app.UseSwaggerUI();
//agregamos una cabecera personalizada

app.UseCors();// indicamos que use cors
app.MapControllers();

app.Run();
