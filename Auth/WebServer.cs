using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ChefPlusPlus.Auth
{
    public class WebServer
    {
        private readonly HttpListener listener;

        public WebServer(string uri)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(uri);
        }

        public async Task<Authorization> Listen()
        {
            listener.Start();
            return await onRequest();
        }

        private async Task<Authorization> onRequest()
        {
            while (listener.IsListening)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                using (StreamWriter writer = new StreamWriter(resp.OutputStream))
                {
                    writer.WriteLine("<h1 id=\"code\"></h1><script>var location = document.location.hash;var token = String(location).split(\"=\")[1].split(\"&\")[0];document.getElementById(\"code\").innerHTML = \"Your Token : \" + token;</script><br><br>You may now close this window.");
                    writer.Flush();
                }
            }

            return null;
        }
    }
}