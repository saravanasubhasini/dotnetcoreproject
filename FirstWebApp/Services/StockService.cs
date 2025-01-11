using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ServiceContracts;
namespace Services
{
    public class StockService:IStockService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IConfiguration _configuration;
        public StockService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration; 
        }

        public async Task<Dictionary<string,object>?> StockServiceResquestAndResponse(string StockSymbol)
        {
            using (HttpClient httpClient = _httpClientFactory.CreateClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"https://finnhub.io/api/v1/quote?symbol={StockSymbol}&token={_configuration["StockToken"]}"),
                    Method = HttpMethod.Get,
                };

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                Stream stream = httpResponseMessage.Content.ReadAsStream();
                StreamReader streamReader = new StreamReader(stream);

                string resposne = streamReader.ReadToEnd();

                Dictionary<string,object>? dictionaryResposne =  JsonSerializer.Deserialize<Dictionary<string, object>>(resposne);

                if (dictionaryResposne == null)
                    throw new InvalidOperationException("Invalid response from finnhub");

                if (dictionaryResposne.ContainsKey("error"))
                    throw new InvalidOperationException((string)dictionaryResposne["error"]);

                return dictionaryResposne;
            }
        }
    }
}
