using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace EasyPay_API.Classes
{
    public class Common
    {
        public string baseUrl = "https://code-api-staging.easypayfinance.com";
        public string username = "user";
        public string password = "pass";

        public RestClient getAuthClient()
        {
            //create new rest client instance
            RestClient client = new RestClient(baseUrl);

            //create new rest request instance
            RestRequest request = new RestRequest("/api/Authentication/login");
            
            //accept all headers
            request.AddHeader("Accept", "*/*");
            
            //set expected content type returned
            request.AddHeader("Content-Type", "application/json");
            
            //create the post request json body
            object jsonData = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
            
            //add the post body
            request.AddParameter("text/json", jsonData, ParameterType.RequestBody);
            request.AddParameter("grant_type", "client_credentials");

            //send the request
            IRestResponse response = client.Post(request);

            //parse the content for the token to create the bearer token
            string responseJson = response.Content;
            dynamic data = JObject.Parse(responseJson);

            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(data.token.ToString(), "Bearer");

            //return the authenticated rest client object
            return client;
        }

        public void getAPI(string resource, HttpStatusCode expReturnCode)
        {
            //get an authenticated rest client
            RestClient client = getAuthClient();

            //create the request with the resource point
            RestRequest request = new RestRequest(resource);
            request.AddHeader("Accept", "*/*");

            //execute the request
            IRestResponse response = client.Get(request);

            //verify return code and content type
            Assert.That(response.StatusCode, Is.EqualTo(expReturnCode));
            Assert.That(response.ContentType, Is.EqualTo("application/json; charset=utf-8"));
        }

        public void postAPI(string resource, string body, HttpStatusCode expReturnCode, string expErrorMsg)
        {
            //get an authenticated rest client
            RestClient client = getAuthClient();

            //create the request with the resource point
            RestRequest request = new RestRequest(resource);
            request.AddHeader("Accept", "*/*");

            //add the post body
            request.AddParameter("text/json", body, ParameterType.RequestBody);

            //execute the request
            IRestResponse response = client.Post(request);

            //verify return code
            Assert.That(response.StatusCode, Is.EqualTo(expReturnCode));

            //if expected error message is supplied, verify it
            string responseJson = response.Content;
            dynamic data = JObject.Parse(responseJson);

            if (expErrorMsg != "")
            {
                TestContext.Out.WriteLine("Testname: " + TestContext.CurrentContext.Test.Name + " -> Error data returned: " + data.errors.ToString());
                string actErrorMsg_clean = Regex.Replace(data.errors.ToString(), @"\t|\n|\r|\s", "");
                string expErrorMsg_clean = Regex.Replace(expErrorMsg, @"\t|\n|\r|\s", "");
                StringAssert.Contains(expErrorMsg_clean, actErrorMsg_clean);
            }
        }

        public void deleteAPI(string resource, string item_id, HttpStatusCode expReturnCode)
        {
            //get an authenticated rest client
            RestClient client = getAuthClient();

            //create the request with the resource point
            RestRequest request = new RestRequest(resource + "/{id}");
            request.AddHeader("Accept", "*/*");

            //add the item id to delete
            request.AddParameter("id", item_id);

            //execute the request
            IRestResponse response = client.Delete(request);

            //verify return code
            Assert.That(response.StatusCode, Is.EqualTo(expReturnCode));
        }
    }
}
