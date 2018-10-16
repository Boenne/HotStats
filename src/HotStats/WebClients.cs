using System.Net;
using System.Net.Http;

namespace HotStats
{
    public static class WebClients
    {
        public static readonly HttpClient HttpClient = new HttpClient();
        public static readonly WebClient WebClient = new WebClient();

        static WebClients()
        {
            //This is needed to be able to download portraits if the protocol is https
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
    }
}