using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;

namespace Neudesic.SmartOffice.RoomService.Tests.Integration {
    internal static class AuthenticationHelper {        

        public static async Task<string> GetAuthenticationToken(String userId, string password, string scopes = null) {
            var urlString = ConfigurationManager.AppSettings["oauth_token_endpoint"];

            var client = new OAuth2Client(new Uri(urlString), ConfigurationManager.AppSettings["oauth_client_id"], ConfigurationManager.AppSettings["oauth_key"]);

            var cts = new CancellationTokenSource();

            cts.CancelAfter(new TimeSpan(0, 0, 60));

            var result = await client.RequestResourceOwnerPasswordAsync(ConfigurationManager.AppSettings["known_good_user_id"],
                                                                        ConfigurationManager.AppSettings["known_good_user_password"],
                                                                        scopes ?? ConfigurationManager.AppSettings["oauth_client_scopes"], null, cts.Token);

            if (result.IsError == false) {
                return result.AccessToken;
            } else {
                return null;
            }
        }
        public static async Task<string> GetKnownAuthenticationToken() {
            var urlString = ConfigurationManager.AppSettings["oauth_token_endpoint"];

            var client = new OAuth2Client(new Uri(urlString), ConfigurationManager.AppSettings["oauth_client_id"], ConfigurationManager.AppSettings["oauth_key"]);

            var cts = new CancellationTokenSource();

            cts.CancelAfter(new TimeSpan(0, 0, 60));

            var result = await client.RequestResourceOwnerPasswordAsync(ConfigurationManager.AppSettings["known_good_user_id"],
                                                                        ConfigurationManager.AppSettings["known_good_user_password"],
                                                                        ConfigurationManager.AppSettings["oauth_client_scopes"], null, cts.Token);

            if (result.IsError == false) {
                return result.AccessToken;
            } else {
                return null;
            }
        }
    }
}
