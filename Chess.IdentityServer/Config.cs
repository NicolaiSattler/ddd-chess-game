using Duende.IdentityServer.Models;

namespace Chess.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
    {
        new IdentityResources.OpenId()
    };

    public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
    {
        new ApiScope(name: "PlayerApi", displayName: "Chess.PlayerApi")
    };

    public static IEnumerable<Client> Clients => new Client[]
    {
        new()
        {
            ClientId = "client", //Replace with id of Chess.Web (has to be determined)
            // no interactive user, use the clientid/secret for authentication
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets =
            {
                new("secret".Sha256())
            },
            AllowedScopes = { "PlayerApi" }

            //TODO: https://docs.duendesoftware.com/identityserver/v6/quickstarts/1_client_credentials/#add-jwt-bearer-authentication
        }
    };
}