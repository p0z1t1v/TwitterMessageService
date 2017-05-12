using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwittApiTest
{
    public class ReceivedMessage
    {

        public string SenderId { get; set; }
        public string SenderScreenName { get; set; }
        public string MessageText { get; set; }
        public bool IsIncome { get; set; }


        public ReceivedMessage(string message)
        {
            IsIncome = false;
            var jsonData = JObject.Parse(message);
            SenderId = jsonData.SelectToken("direct_message.sender_id_str").ToString();
            SenderScreenName = jsonData.SelectToken("direct_message.sender_screen_name").ToString();
            
            if(SenderId != "832143668987437056")
            {
                IsIncome = true;
            }
            MessageText = jsonData.SelectToken("direct_message.text").ToString();
        }

    }
}
