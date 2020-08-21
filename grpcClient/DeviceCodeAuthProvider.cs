using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

public class DeviceCodeAuthProvider
{
    private IPublicClientApplication msalClient;
    private IAccount userAccount;

    public DeviceCodeAuthProvider(IConfiguration config)
    {
        msalClient = PublicClientApplicationBuilder
            .Create(config["AzureAd:ClientId"])
            .WithAuthority(AadAuthorityAudience.AzureAdMyOrg, true)
            .WithTenantId(config["AzureAd:TenantId"])
            .Build();
    }

    public async Task<string> GetAccessToken(string[] scopes)
    {
        if (userAccount == null)
        {
            try
            {
                var result = await msalClient.AcquireTokenWithDeviceCode(scopes, callback => {
                    Console.WriteLine(callback.Message);
                    return Task.FromResult(0);
                }).ExecuteAsync();

                userAccount = result.Account;
                return result.AccessToken;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error getting access token: {exception.Message}");
                return null;
            }
        }
        else
        {
            var result = await msalClient
                .AcquireTokenSilent(scopes, userAccount)
                .ExecuteAsync();

            return result.AccessToken;
        }
    }
}