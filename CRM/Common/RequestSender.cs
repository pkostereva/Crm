using System.Threading.Tasks;
using RestSharp;

namespace CRM.API.Common
{
	public static class RequestSender
	{
		public static async ValueTask<IRestResponse<T>> SendRequest<T>(string host, string URI, Method method = Method.GET, object body = null)
		{
			var client = new RestClient(host);
			var request = new RestRequest(URI, method, DataFormat.Json);
			if (method == Method.POST)
			{
				request.AddJsonBody(body);
				var response = await client.ExecuteAsync<T>(request);
				return response;
			}
			else
			{
				var response = await client.ExecuteAsync<T>(request);
				return response;
			}
		}
    }
}
