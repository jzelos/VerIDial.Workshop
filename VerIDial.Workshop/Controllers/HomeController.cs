using System;
using System.Net;
using System.Web.Mvc;
using VerIDial.Workshop.Models;

namespace VerIDial.Workshop.Controllers
{
    public class HomeController : Controller
    {        
        private static string authHeader = "Basic XXXXXXXXXXXXXXXXXXXX";

        public ActionResult Start()
        {
            return View();
        }

        //
        // POST: /Home/Demo
        /// <summary>
        /// Will process a demo request
        /// </summary>
        /// <param name="submit"></param>
        /// <param name="phonenumber"></param>
        /// <returns></returns>
        [HttpPost]        
        public ActionResult Start(DemoModel model)
        {
            if (model != null && !String.IsNullOrEmpty(model.PhoneNumber))
            {
                string method;
                switch (model.submit)
                {
                    case "CALL":
                        method = "voice";
                        break;

                    case "APP":
                        method = "app";
                        break;

                    default:
                        method = "sms";
                        break;
                }

                HttpWebResponse response = ApiHelper.GetResponse(method, "{ 'Number':'" + model.PhoneNumber + "' }", authHeader);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Get token and redirect to demo                                
                    TokenDTO token = ApiHelper.DeseraliseObject<TokenDTO>(response);
                    return RedirectToAction("PinEntry", new { token = token.Token });
                }
                else
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    StatusDTO status = ApiHelper.DeseraliseObject<StatusDTO>(response);
                    model.ErrorMessage = status.StatusDescription;
                }
            }
            else
            {
                // No model or blank number                
                model.ErrorMessage = "Sorry, you must enter a valid phone number to procees.";
            }

            return View(model);
        }

        /// <summary>
        /// GET: /Home/PinEntry
        /// Shows the demo page with pin plugin ready for data entry
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>        
        public ActionResult PinEntry(string token)
        {
            string baseUrl = Request.Url.GetComponents(UriComponents.Scheme | UriComponents.HostAndPort, UriFormat.Unescaped);
            ViewBag.IFrame = "https://www.veridial.co.uk/plugin?token=" + token + "&pinSuccessUrl=" + String.Concat(baseUrl, "/Home/Success", "?token=", token)
                + "&pinFailureUrl=" + String.Concat(baseUrl, "/Home/Failure", "?token=", token);
            return View();
        }

        /// <summary>
        /// GET: /Home/Failure
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>        
        public ActionResult Failure(string token)
        {
            var model = GetStatus(token);
            return View(model);
        }

        /// <summary>
        /// GET: /Home/Sucess
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>        
        public ActionResult Success(string token)
        {
            var model = GetStatus(token);
            return View(model);
        }

        /// <summary>
        /// Return the current status (as a description)
        /// </summary>
        /// <returns></returns>
        private MessageModel GetStatus(string token)
        {            
            HttpWebResponse response = ApiHelper.GetResponse("status", "{ 'token':'" + token + "' }", authHeader);
            StatusDTO status;
            if (response != null)
            {
                status = ApiHelper.DeseraliseObject<StatusDTO>(response);
                return new MessageModel() { Message = status.StatusDescription };
            }
            else
            {
                return new MessageModel() { Message = "Cannot retrieve status!" };                
            }
        }
    }
}
