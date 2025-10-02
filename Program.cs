using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Repositories;
using AGAMinigameApi.Services;
using AGAMinigameApi.Services.EmailSender;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using SlotsApi.Services;
using SmptOptions;

var builder = WebApplication.CreateBuilder(args);

// Add Smtp
builder.Services.AddOptions<SmtpOptions>()
    .BindConfiguration("Smtp")
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add App
builder.Services.AddOptions<AppOptions>()
    .BindConfiguration("App")
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add services to the container.
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

//custom error validations for invalid modelstate
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(AGAMinigameApi.Filters.ValidateModelStateFilter));
}).ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// For Authentication and Authorization
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var keyBytes = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],

            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
         options.Events = new JwtBearerEvents
        {
            // Don't write a response here — let OnChallenge produce the JSON 401.
            OnAuthenticationFailed = context =>
            {
                context.NoResult(); // suppress default
                return Task.CompletedTask;
            },

            OnChallenge = async context =>
            {
                // Prevent the default WWW-Authenticate writer
                context.HandleResponse();

                if (context.Response.HasStarted)
                    return;

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json; charset=utf-8";

                var response = new ApiResponse<object>(
                    false,
                    "Unauthorized",
                    string.IsNullOrWhiteSpace(context.ErrorDescription)
                        ? "Invalid or expired access token."
                        : context.ErrorDescription,
                   StatusCodes.Status401Unauthorized
                );
                                
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            },

            OnForbidden = async context =>
            {
                if (context.Response.HasStarted)
                    return;

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json; charset=utf-8";

                var response = new ApiResponse<object>(
                    true,
                    "Forbidden",
                    "Forbidden. You do not have permission to access this resource.",
                    StatusCodes.Status403Forbidden
                );

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        };
    });

builder.Services.AddHttpContextAccessor(); // for handler
builder.Services.AddSingleton<IAuthorizationHandler, OwnerOrAdminHandler>();
builder.Services.AddOwnerOrAdminAuthorization();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthResultHandler>();

builder.Services.AddScoped<IBannerRepository, BannerRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IChargeRepository, ChargeRepository>();
builder.Services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IBettingRepository, BettingRepository>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IForgotPasswordRepository, ForgotPasswordRepository>();
builder.Services.AddScoped<IPlayRepository, PlayRepository>();
builder.Services.AddScoped<IGamePreviewRepository, GamePreviewRepository>();

builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<IChargeService, ChargeService>();
builder.Services.AddScoped<IEmailVerificationService, EmailVerificationService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IBettingService, BettingService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddScoped<IPlayService, PlayService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((doc, ctx, ct) =>
    {
        doc.Components ??= new();
        doc.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        doc.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Paste ONLY the JWT. The UI will send 'Authorization: Bearer <token>'."
        };

        // Apply Bearer to all operations by default
        doc.SecurityRequirements ??= new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [ new OpenApiSecurityScheme
                    { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }
                ] = Array.Empty<string>()
            }
        };

        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "AGAMinigame API";
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
