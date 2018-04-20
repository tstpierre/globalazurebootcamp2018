using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neudesic.SmartOffice.RoomService {
    internal static class AuthenticationHelper {

        public static async Task<string> GetAuthenticationToken(TestContext context, String userId, string password, string scopes = null) {
            var urlString = (string)context.Properties["oauth_token_endpoint"];

            var client = new OAuth2Client(new Uri(urlString), (string)context.Properties["oauth_client_id"], (string)context.Properties["oauth_key"]);

            var cts = new CancellationTokenSource();

            cts.CancelAfter(new TimeSpan(0, 0, 60));

            var result = await client.RequestResourceOwnerPasswordAsync((string)context.Properties["known_good_user_id"],
                                                                        (string)context.Properties["known_good_user_password"],
                                                                        scopes ?? (string)context.Properties["oauth_client_scopes"], null, cts.Token);

            if (result.IsError == false) {
                return result.AccessToken;
            } else {
                return null;
            }
        }
        public static async Task<string> GetKnownAuthenticationToken(TestContext context) {
            var urlString = (string)context.Properties["oauth_token_endpoint"];

            var client = new OAuth2Client(new Uri(urlString), (string)context.Properties["oauth_client_id"], (string)context.Properties["oauth_key"]);

            var cts = new CancellationTokenSource();

            cts.CancelAfter(new TimeSpan(0, 0, 60));

            var result = await client.RequestResourceOwnerPasswordAsync((string)context.Properties["known_good_user_id"],
                                                                        (string)context.Properties["known_good_user_password"],
                                                                        (string)context.Properties["oauth_client_scopes"], null, cts.Token);

            if (result.IsError == false) {
                return result.AccessToken;
            } else {
                return null;
            }
        }
    }
}
