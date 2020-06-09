using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CompanyApi.Tests.Services
{
    public class HttpClientHelper
    {
		public HttpClientHelper(HttpClient httpHttpClient)
		{
			HttpClient = httpHttpClient;
		}

		public HttpClient HttpClient { get; }

	    public async Task<T> DeleteAsync<T>(string path)
		{
			var response = await HttpClient.DeleteAsync(path).ConfigureAwait(false);
			return await GetContentAsync<T>(response);
		}

		public async Task<System.Net.HttpStatusCode> DeleteAsync(string path)
		{
			var response = await HttpClient.DeleteAsync(path);
			return response.StatusCode;
		}

		public async Task<T> GetAsync<T>(string path)
		{
			var response = await HttpClient.GetAsync(path);
			return await GetContentAsync<T>(response);
		}

		public async Task<T> PostAsync<T>(string path, T content)
		{
            return await PostAsync<T, T>(path, content);
        }

		public async Task<TOut> PostAsync<TIn, TOut>(string path, TIn content)
		{
			var json = content == null ?
				null :
				new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
			var response = await HttpClient.PostAsync(path, json).ConfigureAwait(false);
			return await GetContentAsync<TOut>(response);
		}

	    private async Task<T> GetContentAsync<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(responseString);
        }
    }
}
