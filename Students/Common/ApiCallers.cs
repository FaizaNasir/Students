using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.Net.Http.Headers;
using System.Text;

namespace EmployeeRegistration_Ui.CommonMethods
{
    public class ApiCallers
    {
        public static async Task<IList<T>> GetList<T>(string ApiUrl)
        {

            List<T> ListOfTypeT = new List<T>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(ApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    ListOfTypeT = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(data);
                }
            }
            return ListOfTypeT;
        }

        public static async Task<T> GetById<T>(string ApiUrl) where T : new()
        {
            T DataOfTypeT = new T();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
                HttpResponseMessage response = await client.GetAsync(ApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    DataOfTypeT = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
                }
            }
            return DataOfTypeT;
        }

        public static async Task<bool> Save_Update<T>(string ApiUrl, T EntityData, string Token = "")
        {
            string EntityDataInJson = JsonConvert.SerializeObject(EntityData);
            bool Status = false;
            using (HttpClient client = new HttpClient())
            {
                var EntityDataInString = new StringContent(EntityDataInJson, UnicodeEncoding.UTF8, "application/json");
                client.BaseAddress = new Uri(ApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage response = await client.PostAsJsonAsync(ApiUrl, EntityData);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    Status = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(data);
                }
                return Status;
            }
        }
        public static async Task<bool> Delete(string ApiUrl)
        {
            bool Status = false;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
                HttpResponseMessage response = await client.DeleteAsync(ApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    Status = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(data);
                }
            }
            return Status;
        }
    }
}
