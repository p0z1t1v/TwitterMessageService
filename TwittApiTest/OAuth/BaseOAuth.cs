using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TwittApiTest.OAuth
{
    public class BaseOAuth
    {
        protected class QueryParameter
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public QueryParameter(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        protected class QueryParameterComparer : IComparer<QueryParameter>
        {
            public int Compare(QueryParameter x, QueryParameter y)
            {
                return x.Name == y.Name ? System.String.CompareOrdinal(x.Value, y.Value) : System.String.CompareOrdinal(x.Name, y.Name);
            }
        }

        private const string O_AUTH_CONSUMER_KEY_KEY = "oauth_consumer_key";
        private const string O_AUTH_SIGNATURE_KEY = "oauth_signature";
        private const string O_AUTH_SIGNATURE_METHOD_KEY = "oauth_signature_method";
        private const string O_AUTH_NONCE_KEY = "oauth_nonce";
        private const string O_AUTH_TIMESTAMP_KEY = "oauth_timestamp";
        private const string O_AUTH_VERSION_KEY = "oauth_version";
        private const string O_AUTH_TOKEN_KEY = "oauth_token";
        private const string O_AUTH_TOKEN_SECRET_KEY = "oauth_token_secret";

        private const string O_AUTH_VERSION = "1.0";
        private const string HMACSHA1_SIGNATURE_TYPE = "HMAC-SHA1";

        private string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZабвгґдеёїжзіийклмнопрстуфхцчшщъыьэєюяАБВГҐДЕЁЇЖЗІИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЄЮЯ0123456789-_.~";

        private List<QueryParameter> Parameters = new List<QueryParameter>();





        public string GenerateSignatureKey(Uri url, string httpMethod, string consumerKey,
            string consumerSecret, string token, string tokenSecret,
            string nonce, string timeStamp)
        {
            if(token == null)
            {
                token = string.Empty;
            }
            if(tokenSecret == null)
            {
                tokenSecret = string.Empty;
            }
            if (string.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }
            if (string.IsNullOrEmpty(httpMethod))
            {
                throw new ArgumentNullException("httpMethod");
            }

            Parameters.AddRange(GetQueryParameters(url.Query));
            Parameters.Add(new QueryParameter(O_AUTH_SIGNATURE_METHOD_KEY, HMACSHA1_SIGNATURE_TYPE));
            Parameters.Add(new QueryParameter(O_AUTH_VERSION_KEY, O_AUTH_VERSION));
            Parameters.Add(new QueryParameter(O_AUTH_TOKEN_KEY, token));
            Parameters.Add(new QueryParameter(O_AUTH_CONSUMER_KEY_KEY, consumerKey));
            Parameters.Add(new QueryParameter(O_AUTH_TIMESTAMP_KEY, timeStamp));
            Parameters.Add(new QueryParameter(O_AUTH_NONCE_KEY, nonce));
            
            string baseUrl = string.Format("{0}://{1}{2}", url.Scheme, url.Host, url.AbsolutePath);

            Parameters.Sort(new QueryParameterComparer());

            string signatureBase = string.Format("{0}&", httpMethod.ToUpper());
            signatureBase += string.Format("{0}&", UrlEncode(baseUrl));
            signatureBase += string.Format("{0}", UrlEncode(GetParameterString(Parameters)));

            var hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), UrlEncode(tokenSecret)));
            string signatureKey = ComputeHash(hmacsha1, signatureBase);
            Parameters.Add(new QueryParameter(O_AUTH_SIGNATURE_KEY, signatureKey));

            return signatureKey;
        }

        public string GenerateSignatureKey(Uri url, string httpMethod, OAuthUser user,
            string nonce, string timeStamp)
        {
            return GenerateSignatureKey(url, httpMethod, user.ConsumerKey,
            user.ConsumerSecret, user.Token, user.TokenSecret,
            nonce, timeStamp);
        }

        private List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?")) { parameters = parameters.Remove(0, 1); }
            var result = new List<QueryParameter>();
            
            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach(string s in p)
                {
                    if(!string.IsNullOrEmpty(s) && !s.StartsWith("oauth_"))
                    {
                        if(s.IndexOf('=') > -1)
                        {
                            string[] t = s.Split('=');
                            result.Add(new QueryParameter(t[0], t[1]));
                        }
                        else { result.Add(new QueryParameter(s, string.Empty)); }
                    }
                }
            }
            return result;
        }

        private string GetParameterString(List<QueryParameter> parameters)
        {
            var result = new StringBuilder();
            for(int i = 0; i < parameters.Count; i++)
            {
                QueryParameter parametr = parameters[i];
                if (parametr.Name.StartsWith("?"))
                {
                    parametr.Name.Remove(0, 1);
                }               
                if(i == 0) { result.AppendFormat("{0}={1}", parametr.Name, parametr.Value); }
                else{ result.AppendFormat("&{0}={1}", parametr.Name, parametr.Value); }                
            }
            return result.ToString();
        }

      
        public string UrlEncode(string value)
        {
            var result = new StringBuilder();
            foreach (char symbol in value)
            {
                if (UnreservedChars.IndexOf(symbol) != -1) { result.Append(symbol); }
                else { result.Append('%' + String.Format("{0:X2}", (int)symbol)); }
            }
            return result.ToString();
        }

        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        public string GetAuthorizationHeader()
        {
            string result = "";
            for(int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Name.StartsWith("oauth"))
                {
                    result += string.Format(@" {0}=""{1}"",", UrlEncode(Parameters[i].Name), UrlEncode(Parameters[i].Value));
                }
            }

            return result.Substring(0, result.Length - 1);
        }

        
        public virtual string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        public virtual string GenerateNonce()
        {
            Random RandomGenerator = new Random();
            string toEncode = "";

            for (int i = 0; i < 31; i++)
            {
                toEncode += ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray())
                    .ElementAt(RandomGenerator.Next(0, 51));
            }

            return Convert.ToBase64String(Encoding.ASCII.GetBytes(toEncode)).Replace("=", "");
        }
    }
}

    


