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
        public string Single_Sms(string phone, string msg)
        {
            var client = new RestClient(Single_Sms_Url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("api_token", Single_Sms_api_token);
            request.AddParameter("sid", Single_Sms_Sid);
            request.AddParameter("msisdn", phone);
            request.AddParameter("sms", msg);
            request.AddParameter("csms_id", Single_Sms_csms_id);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
        #endregion

        #region Bulk_Sms
        public string Bulk_Sms()
        {
            var client = new RestClient(Bulk_Sms_Url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json,application/json", "{\n\"api_token\":\"" + Bulk_Sms_api_token.Trim() + "\",\n\"sid\": \"" + Bulk_Sms_Sid + "\",\n  \"msisdn\":[\n\"" + Bulk_Sms_msisdn1 + "\",\n\"" + Bulk_Sms_msisdn2 + "\"\n],\n\"sms\":\"" + Bulk_Sms_sms + "\",\n\"batch_csms_id\":\"" + Bulk_Sms_batch_csms_id + "\"\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;

        }
        #endregion

        #region Dynamic_Sms
        public string Dynamic_Sms()
        {
            var client = new RestClient(Dynamic_Sms_Url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json,application/json", "{\n\"api_token\":\"" + Dynamic_Sms_api_token + "\",\n\"sid\":\"" + Dynamic_Sms_sid + "\",\n\"sms\": [\n{\n\"msisdn\": \"" + Dynamic_Sms_msisdn1 + "\",\n\"text\": \"" + Dynamic_Sms_text1 + "\",\n\"csms_id\": \"" + Dynamic_Sms_cms_id1 + "\"\n},\n{\n\"msisdn\":\"" + Dynamic_Sms_msisdn2 + "\",\n\"text\":\"" + Dynamic_Sms_text2 + "\",\n\"csms_id\": \"" + Dynamic_Sms_cms_id2 + "\"\n}\n]\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;

        }
        #endregion
    }
}
