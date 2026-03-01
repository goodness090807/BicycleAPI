using System.Reflection;
using System.Text;
using BicycleAPI.Api.Authorizations.PermissionAuthorization;
using BicycleAPI.Api.Extensions;
using BicycleAPI.Application.Behaviors;
using BicycleAPI.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using static BicycleAPI.Api.OpenApiBearerSecuritySchemeTransformer;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 日誌配置
// ============================================

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// ============================================
// Cookie 與安全性配置
// ============================================

builder.Services.Configure<CookieSettings>(builder.Configuration.GetSection(CookieSettings.SectionName));
var cookieSettings = builder.Configuration.GetSection(CookieSettings.SectionName).Get<CookieSettings>() ?? new CookieSettings();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy.WithOrigins(cookieSettings.AllowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "CSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = cookieSettings.Secure
        ? CookieSecurePolicy.Always
        : CookieSecurePolicy.SameAsRequest;
});

// ============================================
// MediatR 配置
// ============================================

builder.Services.AddMediatR(cfg =>
{
    cfg.LicenseKey = builder.Configuration.GetSection("MediatR:LicenseKey").Value;
    cfg.RegisterServicesFromAssembly(Assembly.Load("BicycleAPI.Application"));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});

// ============================================
// API 版本控制
// ============================================

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Version")
    );
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ============================================
// 授權與權限配置
// ============================================

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"驗證失敗: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = _ =>
            {
                Console.WriteLine("Token 驗證成功");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"權限挑戰: {context.Error}, {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

// ============================================
// 應用程式服務註冊
// ============================================

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddShared(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddOpenApi("v1", options =>
{
    options.AddBearerSecurityScheme();
    options.AddEndpointsHttpSecuritySchemeResolution();
});
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks();

// ============================================
// 建構應用程式
// ============================================

var app = builder.Build();
await app.InitializeDatabaseAsync();

// ============================================
// 中介軟體管線配置
// ============================================

app.UseExceptionHandler();
app.UseCors("AllowWebApp");

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (httpContext, elapsed, ex) => ex != null
        ? Serilog.Events.LogEventLevel.Error
        : Serilog.Events.LogEventLevel.Information;
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    };
});

app.UseAuthentication();
app.UseAuthorization();

// ============================================
// 端點映射
// ============================================

app.MapControllers();
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

// ============================================
// 啟動應用程式
// ============================================

app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information("Application {Application} is shutting down", builder.Environment.ApplicationName);
});

CommandLineExtension.LogStartupInfo(app, builder);

try
{
    await app.RunAsync();
}
finally
{
    await Log.CloseAndFlushAsync();
}

