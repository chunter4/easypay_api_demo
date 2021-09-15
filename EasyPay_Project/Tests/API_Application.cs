using NUnit.Framework;
using System.Net;
using EasyPay_API.Classes;

namespace EasyPay_API
{
    [TestFixture]
    public class API_Application
    {
        [TestCase("/api/Application/all", HttpStatusCode.OK)]
        public void Get_Application_All(string resource, HttpStatusCode expReturnCode)
        {
            var common = new Common();
            common.getAPI(resource, expReturnCode);
        }

        [TestCase("/api/Application", "{ \"applicationId\": 123, \"name\": \"test\", \"age\": \"18\", \"amount\": 18.0 }", HttpStatusCode.OK, "")]
        [TestCase("/api/Application", "{ \"applicationId\": 123, \"name\": \"test\", \"age\": \"17\", \"amount\": 18.0 }", HttpStatusCode.BadRequest, "The field Age must be between 18 and 30.")]
        [TestCase("/api/Application", "{ \"applicationId\": 123, \"name\": \"test\", \"age\": \"31\", \"amount\": 18.0 }", HttpStatusCode.BadRequest, "The field Age must be between 18 and 30.")]
        [TestCase("/api/Application", "{ \"applicationId\": 123, \"name\": \"test\", \"age\": \"18\", \"amount\": \"test\" }", HttpStatusCode.BadRequest, "The JSON value could not be converted to System.Decimal. Path: $.amount")]
        [TestCase("/api/Application", "{ \"applicationId\": 123, \"name\": 123, \"age\": \"18\", \"amount\": 18.0 }", HttpStatusCode.BadRequest, "The JSON value could not be converted to System.String. Path: $.name")]
        [TestCase("/api/Application", "{ \"applicationId\": 123, \"name\": \"test\", \"age\": 18, \"amount\": 18.0 }", HttpStatusCode.BadRequest, "The JSON value could not be converted to System.String. Path: $.age")]
        public void Post_Application(string resource, string body, HttpStatusCode expReturnCode, string expErrorMsg)
        {
            var common = new Common();
            common.postAPI(resource, body, expReturnCode, expErrorMsg);
        }
    }
}
