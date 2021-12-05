using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Etherscan
{
    public class EtherscanApiWrapper
    {
        private static readonly HttpClient Client = new HttpClient(new HttpClientHandler() { UseProxy = false })
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        public static string AuthorizeEndpointUrl = "https://api.etherscan.io/api?module=account&action=txlistinternal&address=0x7Be8076f4EA4A4AD08075C2508e481d6C946D12b&page=1&offset=10000&sort=asc&apikey=9QTG5UR41I61VS7TVWSUF3RP2HAXAJQ1KX";

        private static CancellationTokenSource cancellationTokenSource;

        public async Task<GetTransactionsResponse> GetTransactions(int startBlock, int endBlock)
        {
            var reqUrl = $"{AuthorizeEndpointUrl}&startblock={startBlock}&endblock={endBlock}";

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(HttpMethod.Get, reqUrl);
            cancellationTokenSource = new CancellationTokenSource();
            var response = await Client.SendAsync(request, cancellationTokenSource.Token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // successful
                var responseContent = await response.Content.ReadAsStringAsync();

                try
                {
                    return JsonConvert.DeserializeObject<GetTransactionsResponse>(responseContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            else
            {
                // unsuccessful
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Raw response content: " + responseContent);

                throw new Exception();
            }
        }
    }
}
