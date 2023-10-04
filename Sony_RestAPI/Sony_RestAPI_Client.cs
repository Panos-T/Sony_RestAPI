using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sony_RestAPI
{
    public class Sony_RestAPI_Client
    {
        private HTTPClient client ;
        private int retryFactor;



        public abstract class SonyBraviaIPMethod<T>
        {
            public string method { get; set; }
            public int id { get; set; }
            public T[] @params { get; set; }
            public string version { get; set; }
        }


        internal class Uri
        {
            public string uri;
        }

        internal sealed class setActiveApp : SonyBraviaIPMethod<Uri>
        {
            public setActiveApp()
            {
                method = "setActiveApp";
                id = 601;
                @params = new Uri[1];
                @params[0] = new Uri();
                version = "1.0";
            }
        }


        /*
         * Function called at SIMPL+ startup
         */

        public void Initialize(string iptxt, string psktxt)
        {
            client = new HTTPClient(iptxt, psktxt);
            setDebugLevel(0);
            
        }


        public void selectApp(string Uri)
        {

            setActiveApp requestBody = new setActiveApp();

            requestBody.@params[0].uri = Uri;


            GlobalFunctions.PrintDeb("requestBody: " + JsonConvert.SerializeObject(requestBody));
            //client.SendHttpRequestAsync(RequestType.Post, "/appControl", JsonConvert.SerializeObject(requestBody), new HttpHeader[1] { new HttpHeader("Content-Type", "application/json;") });
            HttpClientResponse response = client.SendHttpRequest(RequestType.Post, "/appControl", JsonConvert.SerializeObject(requestBody), new HttpHeader[1] { new HttpHeader("Content-Type", "application/json; charset=UTF-8") });


            
            if (response == null && retryFactor < 5)
            {
                retryFactor++;
                GlobalFunctions.PrintDeb("Http Request retry attempt No " + retryFactor);
                this.selectApp(Uri);
            }
            else
            {
                GlobalFunctions.PrintDeb("Http Request Succeeded after " + retryFactor + " retries");
                GlobalFunctions.PrintDeb("Response Content " + response.ContentString);
                GlobalFunctions.PrintDeb("Response Code " + response.Code);
                retryFactor = 0;

            }
        }

        public void setDebugLevel(ushort lvl)
        {
            GlobalFunctions.DebugLevel = lvl;
        }



    }
    public static class GlobalFunctions
    {
        public static int DebugLevel;
        public static void PrintDeb(string msg)
        {
            if (DebugLevel != 0)
            {
                CrestronConsole.PrintLine(msg);
            }
        }
    }
   
}