using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using AadMultiTenantApi;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

// API info => multi tenant App registration V2
// "TenantId": "7ff95b15-dc21-4ba6-bc92-824856578fc1",
// "ClientId": "fd88c6e8-e790-4b1e-afab-3a9df8726a80"
// jwt validate, should be in the configuration
var issuert1 = "https://b2cdamienbod.b2clogin.com/f611d805-cf72-446f-9a7f-68f2746e4724/v2.0/";
var aud = "ca8dc6a9-c0de-4dfb-8e42-758ef311d8ab";
var azpClientId = "8cbb1bd3-c190-42d7-b44e-42b20499a8a1";
var aadMetadataAddress = "https://b2cdamienbod.b2clogin.com/b2cdamienbod.onmicrosoft.com/B2C_1_signup_signin/v2.0/.well-known/openid-configuration";

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.MetadataAddress = aadMetadataAddress;
        //options.Authority = issuert1;
        options.Audience = aud;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidAudiences = new List<string> { aud },
            ValidIssuers = new List<string> { issuert1 }
        };
    });

services.AddSingleton<IAuthorizationHandler, ValidTenantsAndClientsHandler>();

services.AddAuthorization(policies =>
{
    policies.AddPolicy("ValidTenantsAndClients", p =>
    {
        // only delegated trusted  known clients allowed to use the API
        p.Requirements.Add(new ValidTenantsAndClientsRequirement());

        // Validate id of application for which the token was created
        p.RequireClaim("azp", azpClientId);

        // client secret = 1, 2 if certificate is used
        p.RequireClaim("azpacr", "1");
    });
});

services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    // add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>() }
            });
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1",
        Description = "Exam Manager API"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // c.IncludeXmlComments(xmlPath);
});
var app = builder.Build();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
IdentityModelEventSource.ShowPII = true;

app.UseSecurityHeaders(
    SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();