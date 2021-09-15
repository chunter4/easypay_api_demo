using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace EasyPay_Project
{
    [TestFixture]
    public class EasyPay_API
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
            var response = client.Post(request);
            //parse the content for the token to create the bearer token
            var responseJson = response.Content;
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
            var response = client.Get(request);

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
            var response = client.Post(request);

            //verify return code
            Assert.That(response.StatusCode, Is.EqualTo(expReturnCode));

            //if expected error message is supplied, verify it
            var responseJson = response.Content;
            dynamic data = JObject.Parse(responseJson);
            
            if (expErrorMsg != "")
            {
                TestContext.Out.WriteLine("Error returned: " + data.errors.ToString());
                string actErrorMsg_clean = Regex.Replace(data.errors.ToString(), @"\t|\n|\r|\s", "");
                string expErrorMsg_clean = Regex.Replace(expErrorMsg, @"\t|\n|\r|\s", "");
                StringAssert.Contains(expErrorMsg_clean, actErrorMsg_clean);
            }
        }

        [TestCase("/api/Application/all", HttpStatusCode.OK)]
        public void Get_Application_All(string resource, HttpStatusCode expReturnCode)
        {
            getAPI(resource, expReturnCode);
        }

        [TestCase("/api/Application", "{ \"applicationId\": 123, \"name\": \"test\", \"age\": \"18\", \"amount\": 18.0 }", HttpStatusCode.OK, "")]
        [TestCase("/api/Application", "{ \"applicationId\": 123, \"name\": \"test\", \"age\": \"17\", \"amount\": 18.0 }", HttpStatusCode.BadRequest, "The field Age must be between 18 and 30.")]
        [TestCase("/api/Application", "{ \"applicationId\": 123, \"name\": \"test\", \"age\": \"31\", \"amount\": 18.0 }", HttpStatusCode.BadRequest, "The field Age must be between 18 and 30.")]
        [TestCase("/api/Application", "{ \"applicationId\": 123, \"name\": \"test\", \"age\": \"18\", \"amount\": \"test\" }", HttpStatusCode.BadRequest, "The JSON value could not be converted to System.Decimal. Path: $.amount")]
        [TestCase("/api/Application", "{ \"applicationId\": 123, \"name\": 123, \"age\": \"18\", \"amount\": 18.0 }", HttpStatusCode.BadRequest, "The JSON value could not be converted to System.String. Path: $.name")]
        public void Post_Application(string resource, string body, HttpStatusCode expReturnCode, string expErrorMsg)
        {
            postAPI(resource, body, expReturnCode, expErrorMsg);
        }
    }
}
