using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwittApiTest.OAuth
{
    public class OAuthUser
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string UserScreenName { get; set; }
        public string Id { get; set; }

        public OAuthUser(string consumerKey, string consumerSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;

        }

        public OAuthUser(string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            Token = token;
            TokenSecret = tokenSecret;
        }
    }
}
