using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Opus_ChatBot_HealthCare_8.Helpers
{
    public class ApiCall
    {
        public static async Task<T> GetApiResponseAsync<T>(string url, string bearerToken)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Set up the client
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                    // Send the GET request
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    // Read the response content
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Deserialize JSON response into the specified type
                    T result = JsonConvert.DeserializeObject<T>(responseBody);

                    return result;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                    throw; // Re-throw the exception to handle it where the function is called
                }
            }
        }
    }
}
