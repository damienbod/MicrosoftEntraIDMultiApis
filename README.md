# Microsoft Entra ID and Azure AD B2C Multi tenant APIs

[![.NET](https://github.com/damienbod/AadMutliApis/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/AadMutliApis/actions/workflows/dotnet.yml)

[Using multi-tenant Microsoft Entra ID delegated APIs from different tenants](https://damienbod.com/2023/01/30/using-multi-tenant-aad-delegated-apis-from-different-tenants/)

## Create the service principal for the API in your tenant

## History

- 2025-05.10 .NET 9
- 2024-10-30 Added Microsoft Entra ID API and an Azure AD B2C UI multi-tenant application
- 2024-10-25 Updated packages
- 2024-10-06 Updated security headers
- 2024-10-05 Updated packages
- 2024-06-22 Updated packages
- 2024-01-14 Updated packages
- 2023-11-22 Updated .NET 8
- 2023-11-03 Updated packages, fixed security headers
- 2023-08-27 Updated packages
- 2023-06-08 Updated packages
- 2023-04-29 Updated packages
- 2023-03-02 Updated packages

```powershell

# Connect-AzureAD -TenantId '<UI-tenantId>' 

# New-AzureADServicePrincipal -AppId 'ClientId-from-multi-tenant-api'

Connect-AzureAD -TenantId 'e8b4665e-8ad9-4e12-8c3f-0d48ddb58d16'                                            

New-AzureADServicePrincipal -AppId 'ca8dc6a9-c0de-4dfb-8e42-758ef311d8ab'
```

## Give Consent in your tenant to the Enterprise applications

1. Open the Enterprise Applications blade
2. Find your enterprise application using the guid ObjectId from the powershell script
3. Open the permissions blade
4. Grant Admin consent if you require to use local tenant permissions

## Microsoft Entra ID Permissions API

Permissions used in the AAD API

- none

## Microsoft Entra ID Permissions UI 

- multi-tenant-api

## Note

Validate the UI client ID in the API to only allow ATs from known Azure AD B2C tenants.

## Links

https://damienbod.com/2023/01/02/azure-ad-multi-tenant-azure-app-registration-consent/

https://stackoverflow.com/questions/60929155/how-to-create-service-principal-of-multi-tenant-application

https://learn.microsoft.com/en-us/azure/active-directory-b2c/access-tokens

https://stackoverflow.com/questions/74121290/allow-azure-b2c-app-registration-access-to-regular-organization-ad-app-registrat