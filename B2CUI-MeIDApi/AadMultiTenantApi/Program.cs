using MeIDMultiTenantApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
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

builder.Services.AddOpenApi(options =>
{
    //options.UseTransformer((document, context, cancellationToken) =>
    //{
    //    document.Info = new()
    //    {
    //        Title = "My API",
    //        Version = "v1",
    //        Description = "API for Damien"
    //    };
    //    return Task.CompletedTask;
    //});
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
IdentityModelEventSource.ShowPII = true;

app.UseSecurityHeaders();

app.MapOpenApi("/openapi/v1/openapi.json");

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1/openapi.json", "v1");
    });
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();