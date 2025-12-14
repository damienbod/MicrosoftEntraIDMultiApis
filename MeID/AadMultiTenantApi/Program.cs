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

// API info => multi tenant App registration V2
// "TenantId": "7ff95b15-dc21-4ba6-bc92-824856578fc1",
// "ClientId": "fd88c6e8-e790-4b1e-afab-3a9df8726a80"
// jwt validate, should be in the configuration
var issuert1 = "https://login.microsoftonline.com/5698af84-5720-4ff0-bdc3-9d9195314244/v2.0";
var aud = "fd88c6e8-e790-4b1e-afab-3a9df8726a80";
var azpClientId = "63aa66a6-9a50-464b-a37f-f624365b5926";
var aadMetadataAddress = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";

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