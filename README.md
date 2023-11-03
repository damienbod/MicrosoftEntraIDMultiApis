# AAD and AD B2C Multi tenant APIs

[![.NET](https://github.com/damienbod/AadMutliApis/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/AadMutliApis/actions/workflows/dotnet.yml)

[Using multi-tenant AAD delegated APIs from different tenants](https://damienbod.com/2023/01/30/using-multi-tenant-aad-delegated-apis-from-different-tenants/)

## Create the service principal for the API in your tenant

## History

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

## Azure AD Permissions API

Permissions used in the AAD API

- none

## Azure AD Permissions UI 

- multi-tenant-api

## Note

Validate the UI client ID in the API to only allow ATs from known B2C tenants.

## Links

https://damienbod.com/2023/01/02/azure-ad-multi-tenant-azure-app-registration-consent/

https://stackoverflow.com/questions/60929155/how-to-create-service-principal-of-multi-tenant-application
