using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp.Net.Http;
using Crestron.SimplSharp;

namespace Sony_RestAPI
{
    
    internal class HTTPClient : HttpClient
    {
        private string IPaddress, PSK;





        /*
         * Constructor
         */


        public HTTPClient(string address, string psk)
        {
            this.IPaddress = address;
            this.PSK = psk;

        }

       

        private HttpClientRequest CreateRequest(RequestType type, string message, string body = null, HttpHeader[] headers = null)
        {
            HttpClientRequest request = new HttpClientRequest
            {
                Url = new UrlParser("http://" + IPaddress + "/sony" + message),
                RequestType = type
            };
           
            //Create the authentication Header
            request.Header.SetHeaderValue("X-Auth-PSK", PSK);
            

            //If any Headers are specified, include them as well
            if (headers != null)
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    request.Header.AddHeader(headers[i]);
                }
            }

            //If a body is specified, include it
            if (body != null)
            {
                request.ContentString = body;
            }

            return request;
        }


        private HttpClientResponse DispatchRequest(HttpClientRequest request, HTTPClientResponseCallback callback = null)
        {
            HttpClientResponse response;
            HTTP_CALLBACK_ERROR error;

            try
            {
                GlobalFunctions.PrintDeb("Headers {{\n" + request.Header + "\n}}");
                response = Dispatch(request);
                GlobalFunctions.PrintDeb("Success Header " + request.Header);
                error = HTTP_CALLBACK_ERROR.COMPLETED;
            }
            catch(Exception e)
            {
                GlobalFunctions.PrintDeb("Error sending the request");
                GlobalFunctions.PrintDeb("Error Type \n {{" + e + "\n}}");
                GlobalFunctions.PrintDeb("Error Inner Exception " + e.InnerException);
                GlobalFunctions.PrintDeb("Error Hresult " + e.HResult);
                GlobalFunctions.PrintDeb("Error sending the request " + e.Message);
                GlobalFunctions.PrintDeb("Error Header " + request.Header);



                response = null;
                error = HTTP_CALLBACK_ERROR.UNKNOWN_ERROR;
                
            }

            callback?.Invoke(response, error);
            return response;
        }



        public HttpClientResponse SendHttpRequest(RequestType type, string message, string body = null, HttpHeader[] headers = null)
        {

    
            
            HttpClientRequest request = CreateRequest(type, message, body, headers);
            
            
            
            

            return DispatchRequest(request);
        }


   





    }
}
