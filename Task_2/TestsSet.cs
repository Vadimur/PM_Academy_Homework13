using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    public class TestsSet
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly IJsonReader _jsonReader;

        public TestsSet(IJsonReader jsonReader)
        {
            _jsonReader = jsonReader;
        }

        private bool ConfigureHttpClient()
        {
            string configBaseUrl;
            try
            {
                configBaseUrl = _jsonReader.ReadBaseUrl();
            }
            catch (DataAccessException exception)
            {
                Console.WriteLine($"Exception occured: {exception.Message}");
                return false;
            }

            try
            {
                HttpClient.BaseAddress = new Uri(configBaseUrl);
            }
            catch (Exception exception)
            {
                if (exception is ArgumentNullException ||
                    exception is UriFormatException)
                {
                    Console.WriteLine($"Exception occured: {exception.Message}");
                    return false;
                }

                throw;
            }

            return true;
        }
        public async Task RunAll()
        {
            bool isConfigured = ConfigureHttpClient();
            if (!isConfigured)
            {
                return;
            }

            await Test_RegisterEndpoint_WithValidData();
            
            Task[] tasks =
            {
                Test_RegisterEndpoint_WithInvalidData(),
                Test_RatesEndpoint_WithAuthenticationToken(),
                Test_RatesEndpoint_WithInvalidAuthenticationToken()
            };

            await Task.WhenAll(tasks);
        }
        public async Task Test_RegisterEndpoint_WithValidData()
        {
            await BaseTest(1, 
                "Testing registration endpoint. Trying to register user. It is unknows is user already registered.",
                HttpMethod.Post, 
                "/register",
                requestBody: "{\"login\":\"login123\",\"password\":\"password123\"}");
        }
        
        public async Task Test_RegisterEndpoint_WithInvalidData()
        {
            await BaseTest(2,
                "Testing registration endpoint. Sending invalid data, that will fail validation", 
                HttpMethod.Post, 
                "/register", 
                400, 
                requestBody: "{\"login\":\"login\",\"password\":\"pass\"}");
            
            // not checking response body because it changes
        }
        
        public async Task Test_RatesEndpoint_WithAuthenticationToken()
        {
            await BaseTest(3,
                "Testing rates endpoint. Sending valid request with authentication header", 
                HttpMethod.Get, 
                "/Rates/USD/EUR", 
                200,
                authenticationToken: "Basic bG9naW4xMjM6cGFzc3dvcmQxMjM="); 
            
            // not checking response body because it changes
        }
        
        public async Task Test_RatesEndpoint_WithInvalidAuthenticationToken()
        {
            await BaseTest(4,
                "Testing rates endpoint. Sending valid request withouth authentication header", 
                HttpMethod.Get,
                "/Rates/USD/EUR",
                401, 
                expectedResponseBody: "",
                authenticationToken: "Basic xxxxxxxxxxxxxxxxxxx"); 
            
            // not checking response body because it changes
        }
        
        private async Task BaseTest(
            int testNumber,
            string testSummary,
            HttpMethod method,
            string endpoint,
            int? expectedResponseStatusCode = null,
            string expectedResponseBody = null,
            string requestBody = "",
            string authenticationToken = "Basic xxxxxxxxxxxxxxxxxxx")
        {
            //test info | arrange
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"----------------------Test #{testNumber}----------------------");
            builder.AppendLine(testSummary);
            builder.AppendLine($"Testing '{endpoint}' endpoint");
            if (expectedResponseBody != null)
            {
                builder.AppendLine($"Expected response body: '{expectedResponseBody}'");
            }
            else
            {
                builder.AppendLine("Not checking response body, because it is dynamic");
                
            }

            if (expectedResponseStatusCode.HasValue)
            {
                builder.AppendLine($"Expected response status code: '{expectedResponseStatusCode}'");
            }
            else
            {
                builder.AppendLine("Not checking response status code, because it is dynamic");
            }

            builder.AppendLine();

            //sending request | act
            HttpResponseMessage response;
            try
            {
                response = await SendRequest(method, endpoint, requestBody, authenticationToken:authenticationToken);
            }
            catch (HttpRequestException exception)
            {
                builder.AppendLine($"Exception occured: {exception.Message}");
                ShowTestSummary(builder);

                return;
            }

            string actualResponseBody = await response.Content.ReadAsStringAsync();
            int actualResponseStatusCode = (int)response.StatusCode;

            // checking expected adn actual results | assert
            if (expectedResponseBody == null)
            {
                builder.AppendLine($"Response body: '{actualResponseBody}'");
            }
            else if (expectedResponseBody.Equals(actualResponseBody))
            {
                builder.AppendLine("Response body matches expected value");
            }
            else
            {
                builder.AppendLine("Response body doesn't match expected value");
                builder.AppendLine($"Actual response body: '{actualResponseBody}'");
            }

            if (!expectedResponseStatusCode.HasValue)
            {
                builder.AppendLine($"Response status code: '{actualResponseStatusCode}'");
            }
            else if (expectedResponseStatusCode == actualResponseStatusCode)
            {
                builder.AppendLine("Response status code matches expected value");
            }
            else
            {
                builder.AppendLine("Response status code doesn't match expected value");
                builder.AppendLine($"Actual response status code: '{actualResponseStatusCode}'");
            }

            ShowTestSummary(builder);
        }

        private async Task<HttpResponseMessage> SendRequest(
            HttpMethod method,
            string endpoint,
            string body = "",
            string authenticationToken = "Basic xxxxxxxxxxxxxxxxxxx")
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(HttpClient.BaseAddress, endpoint),
                Headers = { 
                    { HttpRequestHeader.Authorization.ToString(), authenticationToken }
                },
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            
            return await HttpClient.SendAsync(httpRequestMessage);
        }
        
        private void ShowTestSummary(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine();
            string testSummary = stringBuilder.ToString();
            Console.WriteLine(testSummary);
        }
    }
}
