using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwittApiTest
{
    public class TwitterMessageService : TwitterHttpClient
    {
        private const string _consumerKey = "UK49L81OxOuLTTVFOqchYMgvy";
        private const string _consumerSecret = "sO2xnvWNmU3RhD6UAOKPHqSoGMx8aIYfWZ0ZWl9Mo4XkBRohdS";
        private const string _tokenKey = "832143668987437056-qpMYR3F03gC7dCsOdHcu7LHa1pK1KzE";
        private const string _tokenSecret = "NfbS3DHKnu03U6vA3qqnyNRoaotGjzSMDidKrVuBBA4bb";

        public TwitterMessageService(string consumerKey, string consumerSecret, string token, string tokenSecret)
            : base(consumerKey, consumerSecret, token, tokenSecret)
        {
        }
        public async Task RunService()
        {
            //TwitterHttpClient streamClient = new TwitterHttpClient(_consumerKey, _consumerSecret, _tokenKey, _tokenSecret);
            //streamClient.onReciveNewMessage += ReciveMessage;
            onReciveNewMessage += ReciveMessage;
            await RunStreamAsync();
        }

        private async Task RunStreamAsync()
        {
            await Task.Run(async () =>
            {
                await StartUserStreamAsync();
            });
        }

        private async void ReciveMessage(OnMessageRecivedEventArgs arg)
        {
            ReceivedMessage message = new ReceivedMessage(arg.MessageText);

            if (message.IsIncome)
            {
                string result = await SendMessageAsync(message.SenderId, message.MessageText);
                Console.WriteLine(result.ToString());
            }
            

            //TwitterHttpClient client = new TwitterHttpClient(_consumerKey, _consumerSecret, _tokenKey, _tokenSecret);
            //await SendAnswer(message);
        }

        //private async Task<string> SendAnswer(ReceivedMessage message)
        //{
        //    TwitterHttpClient client = new TwitterHttpClient(_consumerKey, _consumerSecret, _tokenKey, _tokenSecret);
        //    string result = await client.SendMessageAsync(message.SenderId, message.MessageText);
        //    return result;
        //}
    }
}
