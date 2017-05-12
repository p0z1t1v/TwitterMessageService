using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwittApiTest.Enum;
using TwittApiTest.OAuth;

namespace TwittApiTest
{
    public class TwitterHttpClient
    {
        public delegate void OnRecivedEventHandler(OnMessageRecivedEventArgs message);
        public event OnRecivedEventHandler onReciveNewMessage;


        private const string SEND_MESSAGE_URL = "https://api.twitter.com/1.1/direct_messages/new.json";
        private const string USER_STREAM_URL = "https://userstream.twitter.com/1.1/user.json?stall_warnings=true&replies=all&with=followings";
        private const string USER_SETTINGS_URL = "https://api.twitter.com/1.1/account/settings.json";
        private const string USER_CREDENTIALS = "https://api.twitter.com/1.1/account/verify_credentials.json";
        
        public OAuthUser User { get; set; }

        public TwitterHttpClient(OAuthUser user)
        {
            var currentUser = JObject.Parse(GetUserCredentialsAsync().Result);
            user.UserScreenName = currentUser.SelectToken("screen_name").ToString();
            user.Id = User.Id = currentUser.SelectToken("id_str").ToString();
            User = user;
        }

        public TwitterHttpClient(string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            User = new OAuthUser(consumerKey, consumerSecret, token, tokenSecret);
            var currentUser = JObject.Parse(GetUserCredentialsAsync().Result);
  
            User.UserScreenName = currentUser.SelectToken("screen_name").ToString();
            User.Id = currentUser.SelectToken("id_str").ToString();
        }



        public async Task<string> SendMessageAsync(string userId, string message)
        {
            var baseUrl = new StringBuilder();
            baseUrl.Append(SEND_MESSAGE_URL);
            baseUrl.AppendFormat("?user_id={0}&text={1}", userId, message);
            Uri url = new Uri(baseUrl.ToString());

            HttpClient c = new HttpClient(new OAuthRequestHandler(new HttpClientHandler(),
                "application / x - www - form - urlencoded", User));
            var response = await c.PostAsync(url, new StringContent(url.Query));
            var result = await response.Content.ReadAsStringAsync();
            return result;
       }
        
        public async Task<string> GetUserCredentialsAsync()
        {
            Uri url = new Uri(USER_CREDENTIALS);

            HttpClient client = new HttpClient(new OAuthRequestHandler(new HttpClientHandler(), User));
            
            var response = await client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return result;
               
        } 
        
        public async Task StartUserStreamAsync()
        {
                    await Task.Run(async () => {
                    Uri url = new Uri(USER_STREAM_URL);

                    HttpClient client = new HttpClient(new OAuthRequestHandler(new HttpClientHandler(), User));
                    client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                    var stream = await client.GetStreamAsync(url);

                    using (var reader = new StreamReader(stream))
                    {
                    while (!reader.EndOfStream)
                    {
                            string line = reader.ReadLine();
                            if(line.Contains("direct_message"))
                            {
                                    if(!line.Contains(@"""sender_id_str"": ""832143668987437056"""))
                                    {
                                        onReciveNewMessage(new OnMessageRecivedEventArgs(line));
                                    }
                            }
                        }
                    }
                });

        }
            
    }
}
