using MeIDMultiTenantApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var services = builder.Services;
var configuration = builder.Configuration;

services.AddSecurityHeaderPolicies()
  .SetPolicySelector((PolicySelectorContext ctx) =>
  {
      return SecurityHeadersDefinitions
        .GetHeaderPolicyCollection(builder.Environment.IsDevelopment());
  });

var issuert1 = "https://b2cdamienbod.b2clogin.com/f611d805-cf72-446f-9a7f-68f2746e4724/v2.0/";
var aud = "6e892701-b644-4da8-b70b-26ff89cc1751";
var azpClientId = "54bde571-dfb3-4d8f-aa52-d27a5c039e0e";
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
            ValidAudiences = [aud],
            ValidIssuers = [issuert1]
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

app.UseSecurityHeaders();

if (app.Environment.IsDevelopment())
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