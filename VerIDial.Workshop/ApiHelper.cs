using System;
using System.Net;
using System.Runtime.Serialization.Json;

namespace VerIDial.Workshop
{
        public class ApiHelper
        {
            /// <summary>
            /// Make an api request and return the HttpWebResponse
            /// </summary>
            /// <param name="uri">the uri to append to the api url (sms/app/voice etc)</param>
            /// <param name="postdata">any post data to send</param>
            /// <param name="authHeader">Populated if the request requires authentication</param>
            /// <returns></returns>
            public static HttpWebResponse GetResponse(string uri, string postdata, string authHeader = null, string method = "POST")
            {
                byte[] byteData = System.Text.Encoding.UTF8.GetBytes(postdata);
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("https://api.veridial.co.uk/" + uri);
                webRequest.Method = method;
                webRequest.ContentType = "application/json; charset=UTF-8";
                webRequest.Accept = "application/json";
                webRequest.ContentLength = byteData.Length;
                if (authHeader != null)
                {
                   webRequest.Headers.Add("Authorization", authHeader);
                }
                if (method == "POST")
                {
                    using (var stream = webRequest.GetRequestStream())
                    {
                        stream.Write(byteData, 0, byteData.Length);
                    }
                }
                try
                {
                    HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                    return webResponse;
                }
                catch (WebException ex)
                {
                    return (HttpWebResponse)ex.Response;
                }
            }

            /// <summary>
            /// Helper function to deserialise a json object
            /// </summary>
            /// <typeparam name="T">The type of the object to deserialise</typeparam>
            /// <param name="response">the HttpWebResponse containing the object to deserialise</param>
            /// <returns>an instance of type T</returns>
            public static T DeseraliseObject<T>(HttpWebResponse response)
            {
                using (var stream = response.GetResponseStream())
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                    object value = ser.ReadObject(stream);
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }
        }
}
