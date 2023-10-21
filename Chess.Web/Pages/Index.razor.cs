using IdentityModel.Client;
using Microsoft.AspNetCore.Components;

namespace Chess.Web.Pages;

public partial class Index: ComponentBase
{
    [Inject]
    private IHttpClientFactory? HttpClientFactory { get; set;}

    public string? AccessToken { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (HttpClientFactory != null)
        {
            var httpClient = HttpClientFactory.CreateClient();
            var doc = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");

            if (doc.IsError)
            {
                Console.WriteLine("Oeps");
            }

            var response = await httpClient.RequestClientCredentialsTokenAsync(new()
            {
                Address = doc.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",
                Scope = "PlayerApi"
            });

            if (response.IsError)
            {
                Console.WriteLine("Cannot retrieve token");

                return;
            }

            AccessToken = response.AccessToken ?? string.Empty;
        }
    }
}