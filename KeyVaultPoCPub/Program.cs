using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;

namespace KeyVaultPoCPub    
{
    public class Program
    {
        static void Main()
        {
            try
            {
                var prog = new Program();
                // Start
                Task<string> callTask = Task.Run(() => prog.GetSecret("<KeyVault URL/DNSName>", "<Secret Name>"));
                // Wait
                callTask.Wait();
                // Get result
                string dbconnstring = callTask.Result;
                // Write to console
                Console.WriteLine(dbconnstring);
                Console.ReadKey();
            }
            catch (Exception ex)  // Exceptions here or in the function will be caught here
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        public async Task<string> GetSecret(string vaultUrl, string vaultKey)
        {
            var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken), new HttpClient());
            var secret = await client.GetSecretAsync(vaultUrl, vaultKey);

            return secret.Value;
        }

        private async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            // AAD App ID, Hidden Key
            var appCredentials = new ClientCredential("<AAD App ID>", "<Generated App Hidden Key>");
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);

            var result = await context.AcquireTokenAsync(resource, appCredentials);

            return result.AccessToken;
        }
    }
}