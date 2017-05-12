using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwittApiTest.OAuth;

namespace TwittApiTest
{
    public class OnMessageRecivedEventArgs : EventArgs
    {
        public string MessageText { get; set; }
        

        public OnMessageRecivedEventArgs(string messageText)
        {
            MessageText = messageText;
        }
        
    }
}
