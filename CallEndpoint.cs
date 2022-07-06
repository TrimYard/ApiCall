using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiCall
{
    public sealed class CallEndpoint<T>
    {
        private readonly string _url;
        public HttpClient Client { get { if (_client == null) { _client = new HttpClient(); Client.BaseAddress = new Uri(_url); } return _client; } }

        private static HttpClient _client;
        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
                Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_token);
            }
        }

        private string _token;
        public T Object { get; private set; }

        public CallEndpoint(string url)
        {
            _url = url;
            Client.DefaultRequestHeaders.Accept.Clear();

        }

        public async Task<Uri> Post(T formParameters, string uri)
        {
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await Client.PostAsJsonAsync(uri, formParameters);

            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        public async Task<string> PostReturnString(T formParameters, string uri)
        {
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await Client.PostAsJsonAsync(uri, formParameters);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }


        
        public async Task<A> Post<A>(T formParameters, string uri)
        {
            try
            {
                Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await Client.PostAsJsonAsync(uri, formParameters);

                if (!response.IsSuccessStatusCode)
                {
                    
                    return default;
                }
                response.EnsureSuccessStatusCode();

                var obj = await response.Content.ReadAsAsync<A>();

                return obj;

            }
            catch (Exception error)
            {

                throw new Exception($"Something happened with the CallEndpoint Post. Check the error: {error}\n Also check the endpoint uri: {uri}");
            }        
        }



        public async Task<T> Put(T formParameters, string uri)
        {
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await Client.PutAsJsonAsync<T>(uri, formParameters);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<T>();
        }

        public async Task<HttpStatusCode> Delete(string id, string uri)
        {
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await Client.DeleteAsync($"{uri}/{id}");

            return response.StatusCode;
        }

        public async Task<string> Get(string uri)
        {
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await Client.GetAsync(uri);

            return await response.Content.ReadAsStringAsync();

        }
        public async Task<string> GetWithAuthHeaderOnly(string uri)
        {

            HttpResponseMessage response = await Client.GetAsync(uri);

            return await response.Content.ReadAsStringAsync();

        }
    }
}
