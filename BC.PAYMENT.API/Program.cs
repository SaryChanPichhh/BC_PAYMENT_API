using System.Data;
using log4net.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;
using BC.PAYMENT.INFRASTRUCTURE;
using Serilog;
using System.Threading.RateLimiting;
using BC.PAYMENT.CORE;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();
XmlConfigurator.Configure(new FileInfo("log4net.config"));
builder.Services.AddTransient<IDbConnection>(x =>
    new SqlConnection(builder.Configuration.GetConnectionString("DBConnection")));
builder.Services.RegisterServices();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("SharedConfig/appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.ApplicationName}.json", true, true) // Keep API-specific settings
    .AddJsonFile("secrets.json", true, true)
    .AddEnvironmentVariables();

builder.Services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
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

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.CustomSchemaIds(type =>
    {
        if (!type.IsGenericType)
            return type.FullName!.Replace("+", ".");
        var genericBase = type.GetGenericTypeDefinition().FullName!
            .Split('`')[0]
            .Replace("+", ".");
        var genericArgs = string.Join("_", type.GetGenericArguments().Select(t =>
            t.IsGenericType
                ? t.GetGenericTypeDefinition().FullName!.Split('`')[0].Split('.').Last() + "_" +
                  string.Join("_", t.GetGenericArguments().Select(a => a.FullName ?? a.Name))
                : t.FullName ?? t.Name));
        return $"{genericBase}[{genericArgs}]";
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
Singleton.Instance.Role = builder.Configuration["AdminRole"];
Singleton.Instance.Username = builder.Configuration["AdminUsername"];
Singleton.Instance.UserPassword = builder.Configuration["AdminPassword"];
Singleton.Instance.AppCode = builder.Configuration["AppCode"];
var enableApiDocs = builder.Configuration.GetValue<bool>("EnableApiDocs");

if (app.Environment.IsDevelopment() || enableApiDocs)
{
    app.UseSwagger(options => options.RouteTemplate = "openapi/{documentName}.json");
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("TD PAYMENT API")
            .WithSidebar(true)
            .WithOpenApiRoutePattern("/openapi/{documentName}.json");
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtMiddlewares>();
app.MapControllers();
app.Run();