using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwittApiTest.Enum;

namespace TwittApiTest.OAuth
{
    public class OAuthRequestHandler : DelegatingHandler
    {
        private OAuthUser User;
        private string _contentTypeHeader;

        private BaseOAuth _oAuth = new BaseOAuth();

        public OAuthRequestHandler(HttpMessageHandler handler, OAuthUser user) : base(handler)
        {
            User = user;
        }
        public OAuthRequestHandler(HttpMessageHandler handler, string contentType, OAuthUser user) : base(handler)
        {
            User = user;
            _contentTypeHeader = contentType;
        }



        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string outUrl = "";
            string nonce = _oAuth.GenerateNonce();
            string timeStamp = _oAuth.GenerateTimeStamp();
            string signature = _oAuth.GenerateSignatureKey(request.RequestUri, request.Method.Method, 
                    User.ConsumerKey, User.ConsumerSecret, User.Token, User.TokenSecret, nonce, timeStamp);

            if (request.Method.Method == RequestMethod.POST.ToString())
            {
                request.Headers.Add("ContentType", _contentTypeHeader);
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", _oAuth.GetAuthorizationHeader());

            return base.SendAsync(request, cancellationToken);
        }
    }
}
