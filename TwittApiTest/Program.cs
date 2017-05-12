using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwittApiTest;
using TwittApiTest.Enum;
using TwittApiTest.OAuth;

namespace TwittApiTest
{
    class Program
    {
        private const string _consumerKey = "UK49L81OxOuLTTVFOqchYMgvy";
        private const string _consumerSecret = "sO2xnvWNmU3RhD6UAOKPHqSoGMx8aIYfWZ0ZWl9Mo4XkBRohdS";
        private const string _tokenKey = "832143668987437056-qpMYR3F03gC7dCsOdHcu7LHa1pK1KzE";
        private const string _tokenSecret = "NfbS3DHKnu03U6vA3qqnyNRoaotGjzSMDidKrVuBBA4bb";
        static void Main(string[] args)
        {
            OAuthUser user = new OAuthUser(_consumerKey, _consumerSecret, _tokenKey, _tokenSecret);
            Task.Run(async () =>
            {
                TwitterMessageService service = new TwitterMessageService(_consumerKey, _consumerSecret, _tokenKey, _tokenSecret);
                await service.RunService();
            });
            
            Console.ReadLine();
        }
    }
}
