# AAD Multi tenant API

[![.NET](https://github.com/damienbod/AadMutliApis/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/AadMutliApis/actions/workflows/dotnet.yml)

[Using multi-tenant AAD delegated APIs from different tenants](https://damienbod.com/2023/01/30/using-multi-tenant-aad-delegated-apis-from-different-tenants/)
## Create the service principal for the API in your tenant

## Hstory

2023-04-29 Updated packages

2023-03-02 Updated packages

```powershell
Connect-AzureAD -TenantId '<UI-tenantId>'                                            

New-AzureADServicePrincipal -AppId 'AppId-from-multi-tenant-api'
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

## Links

https://damienbod.com/2023/01/02/azure-ad-multi-tenant-azure-app-registration-consent/

https://stackoverflow.com/questions/60929155/how-to-create-service-principal-of-multi-tenant-application
