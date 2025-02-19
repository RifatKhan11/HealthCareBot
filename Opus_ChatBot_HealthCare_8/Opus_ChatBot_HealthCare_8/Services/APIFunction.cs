using Opus_ChatBot_HealthCare_8.Services.IServices;
using RestSharp;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class APIFunction: IAPIFunction
    {
        #region Global Declaration

        #region Single_Sms
        public string Single_Sms_Url = "https://smsplus.sslwireless.com/api/v3/send-sms";
        public string Single_Sms_Sid = "EVERCAREMASK";
        public string Single_Sms_msisdn = "8801735224039";
        public string Single_Sms_sms = "Single Sms Test";
        public string Single_Sms_csms_id = "1234569";
        public string Single_Sms_api_token = "evqrtuny-k3piumsr-2j8u68uo-ru7cbnnp-nsdvpaba";
        #endregion

        #region Bulk_Sms
        public string Bulk_Sms_Url = "https://smsplus.sslwireless.com/api/v3/send-sms/bulk";
        public string Bulk_Sms_api_token = "e97a085c-***-4527-91aa-e7aac4508ec5";
        public string Bulk_Sms_Sid = "***";
        public string Bulk_Sms_msisdn1 = "0173****0109";
        public string Bulk_Sms_msisdn2 = "0191***6340";
        public string Bulk_Sms_sms = "Bulk Test Sms";
        public string Bulk_Sms_batch_csms_id = "2934fe343";

        //{
        //  "api_token": "e97a085c-***-4527-91aa-e7aac4508ec5",
        //  "sid": "***",
        //  "msisdn": [
        //    "0191***340",
        //    "017****0109"
        //  ],
        //  "sms": "Test",
        //  "batch_csms_id": "2934fe343"
        //}

        #endregion

        #region Dynamic_Sms

        public string Dynamic_Sms_Url = "https://smsplus.sslwireless.com/api/v3/send-sms/dynamic";
        public string Dynamic_Sms_api_token = "e97a085c-***-4527-91aa-e7aac4508ec5";
        public string Dynamic_Sms_sid = "***";

        public string Dynamic_Sms_msisdn1 = "0191***6340";
        public string Dynamic_Sms_text1 = "Dynamic Sms Test1";
        public string Dynamic_Sms_cms_id1 = "92334034";

        public string Dynamic_Sms_msisdn2 = "0173***0109";
        public string Dynamic_Sms_text2 = "Dynamic Sms Test2";
        public string Dynamic_Sms_cms_id2 = "92340333";

        //{
        //  "api_token": "e97a085c-***-4527-91aa-e7aac4508ec5",
        //  "sid": "***",
        //  "sms": [
        //    {
        //      "msisdn": "0191***76340",
        //      "text": "SMS 1",
        //      "csms_id": "92334034"
        //    },
        //    {
        //      "msisdn": "0173***0109",
        //      "text": "SMS 2",
        //      "csms_id": "92340333"
        //    }
        //  ]
        //}
        #endregion

        #endregion

        #region Single_Sms
        //public string Single_Sms(string phone, string msg)
        //{
        //    var client = new RestClient(Single_Sms_Url);
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        //    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        //    request.AddParameter("api_token", Single_Sms_api_token);
        //    request.AddParameter("sid", Single_Sms_Sid);
        //    request.AddParameter("msisdn", phone);
        //    request.AddParameter("sms", msg);
        //    request.AddParameter("csms_id", Single_Sms_csms_id);
        //    IRestResponse response = client.Execute(request);
        //    return response.Content;
        //}
        public string Single_Sms(string phone, string msg)
        {
            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(msg))
            {
                throw new ArgumentException("Phone number or message cannot be empty.");
            }

            // Create RestClientOptions and set Timeout
            var options = new RestClientOptions(Single_Sms_Url)
            {
                Timeout = TimeSpan.FromMilliseconds(30000) // Set timeout to 30 seconds
            };

            var client = new RestClient(options);

            // Use the new method for specifying HTTP method
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            // Add parameters to the request
            request.AddParameter("api_token", Single_Sms_api_token);
            request.AddParameter("sid", Single_Sms_Sid);
            request.AddParameter("msisdn", phone);
            request.AddParameter("sms", msg);
            request.AddParameter("csms_id", Single_Sms_csms_id);

            try
            {
                // Send the request and get the response
                RestResponse response = client.Execute(request);

                // Check if the response was successful
                if (response.IsSuccessful)
                {
                    return response.Content;
                }
                else
                {
                    // Log or handle the error as necessary
                    throw new Exception($"Error sending SMS: {response.StatusCode} - {response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                throw new Exception($"Exception while sending SMS: {ex.Message}", ex);
            }
        }

        #endregion

        #region Bulk_Sms
        //public string Bulk_Sms()
        //{
        //    var client = new RestClient(Bulk_Sms_Url);
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddParameter("application/json,application/json", "{\n\"api_token\":\"" + Bulk_Sms_api_token.Trim() + "\",\n\"sid\": \"" + Bulk_Sms_Sid + "\",\n  \"msisdn\":[\n\"" + Bulk_Sms_msisdn1 + "\",\n\"" + Bulk_Sms_msisdn2 + "\"\n],\n\"sms\":\"" + Bulk_Sms_sms + "\",\n\"batch_csms_id\":\"" + Bulk_Sms_batch_csms_id + "\"\n}", ParameterType.RequestBody);
        //    IRestResponse response = client.Execute(request);
        //    return response.Content;

        //}
        public string Bulk_Sms()
        {
            var clientOptions = new RestClientOptions(Bulk_Sms_Url)
            {
                Timeout = TimeSpan.FromMilliseconds(30000) // Set a reasonable timeout, e.g., 30 seconds
            };

            var client = new RestClient(clientOptions);

            // Create request body as JSON object (avoid string concatenation)
            var requestBody = new
            {
                api_token = Bulk_Sms_api_token.Trim(),
                sid = Bulk_Sms_Sid,
                msisdn = new[] { Bulk_Sms_msisdn1, Bulk_Sms_msisdn2 },
                sms = Bulk_Sms_sms,
                batch_csms_id = Bulk_Sms_batch_csms_id
            };

            // Creating the RestRequest with POST method
            var request = new RestRequest()
            {
                Method = Method.Post // Use Method.Post for POST request
            };

            request.AddHeader("Content-Type", "application/json");

            // Add the request body by serializing the object to JSON
            request.AddJsonBody(requestBody);

            try
            {
                // Execute the request and get the response
                var response = client.Execute(request);

                // Check if the response was successful
                if (response.IsSuccessful)
                {
                    return response.Content;
                }
                else
                {
                    // Log or handle the error as necessary
                    throw new Exception($"Error sending bulk SMS: {response.StatusCode} - {response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                throw new Exception($"Exception while sending bulk SMS: {ex.Message}", ex);
            }
        }

        #endregion

        #region Dynamic_Sms
        //public string Dynamic_Sms()
        //{
        //    var client = new RestClient(Dynamic_Sms_Url);
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddParameter("application/json,application/json", "{\n\"api_token\":\"" + Dynamic_Sms_api_token + "\",\n\"sid\":\"" + Dynamic_Sms_sid + "\",\n\"sms\": [\n{\n\"msisdn\": \"" + Dynamic_Sms_msisdn1 + "\",\n\"text\": \"" + Dynamic_Sms_text1 + "\",\n\"csms_id\": \"" + Dynamic_Sms_cms_id1 + "\"\n},\n{\n\"msisdn\":\"" + Dynamic_Sms_msisdn2 + "\",\n\"text\":\"" + Dynamic_Sms_text2 + "\",\n\"csms_id\": \"" + Dynamic_Sms_cms_id2 + "\"\n}\n]\n}", ParameterType.RequestBody);
        //    RestResponse response = client.Execute(request);
        //    return response.Content;

        //}
        public string Dynamic_Sms()
        {
            var clientOptions = new RestClientOptions(Dynamic_Sms_Url)
            {
                Timeout = TimeSpan.FromMilliseconds(30000) // Set timeout (30 seconds)
            };

            var client = new RestClient(clientOptions);

            // Create the request body object (avoid string concatenation)
            var requestBody = new
            {
                api_token = Dynamic_Sms_api_token,
                sid = Dynamic_Sms_sid,
                sms = new[]
                {
            new { msisdn = Dynamic_Sms_msisdn1, text = Dynamic_Sms_text1, csms_id = Dynamic_Sms_cms_id1 },
            new { msisdn = Dynamic_Sms_msisdn2, text = Dynamic_Sms_text2, csms_id = Dynamic_Sms_cms_id2 }
        }
            };

            // Create the request with POST method
            var request = new RestRequest()
            {
                Method = Method.Post
            };

            // Set content type to application/json
            request.AddHeader("Content-Type", "application/json");

            // Add the request body (will automatically serialize to JSON)
            request.AddJsonBody(requestBody);

            try
            {
                // Execute the request and get the response
                var response = client.Execute(request);

                // Check if the response was successful
                if (response.IsSuccessful)
                {
                    return response.Content;
                }
                else
                {
                    // Log or handle the error
                    throw new Exception($"Error sending dynamic SMS: {response.StatusCode} - {response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception($"Exception while sending dynamic SMS: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
