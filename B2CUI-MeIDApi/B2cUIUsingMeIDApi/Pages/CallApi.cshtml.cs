using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace DifferentTenantUIUseApi.Pages;

[AuthorizeForScopes(Scopes = new string[] { "https://companytwob2c.onmicrosoft.com/ca8dc6a9-c0de-4dfb-8e42-758ef311d8ab/access_as_user" })]
public class CallApiModel : PageModel
{
    private readonly ApiService _apiService;

    public string? DataFromApi { get; set; }

    public CallApiModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        DataFromApi = await _apiService.GetApiDataAsync();
    }
}