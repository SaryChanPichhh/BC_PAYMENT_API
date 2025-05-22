using System.Data;
using BC.PAYMENT.API.Helper;
using log4net.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using BC.PAYMENT.INFRASTRUCTURE;
using Microsoft.Data.SqlClient;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()  // Set default log level
    .Enrich.FromLogContext()  // Include contextual info like request ID
    .WriteTo.Console()  // Log to console
    .WriteTo.Seq("http://localhost:5341") // Optional: log to Seq
    .CreateLogger();

builder.Host.UseSerilog();
//Configure Log4net.
XmlConfigurator.Configure(new FileInfo("log4net.config"));
// Inject Connection 
builder.Services.AddTransient<IDbConnection>(x=>new SqlConnection(builder.Configuration.GetConnectionString("DBConnection")));

//Injecting services.
builder.Services.RegisterServices();

// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Configure enums globally to serialize as strings
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("SharedConfig/appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.ApplicationName}.json", optional: true, reloadOnChange: true)  // Keep API-specific settings
    .AddEnvironmentVariables();

// Enforce lowercase URLs for all generated routes.
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:PYS_KEY"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

//});

// Add Swagger Auth Scheme.
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.UseInlineDefinitionsForEnums();
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
        {
            // Fluent API
            options
                .WithTitle("TD PAYMENT API")
                .WithSidebar(true);
            options.Title = "TD PAYMENT API";
        })
        ;
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });

    app.UseSwaggerUI();
}
// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

//Correct Middleware order

app.UseRouting();
// custom jwt auth middleware
app.UseMiddleware<JwtMiddlewares>();
//app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();


app.Run();
