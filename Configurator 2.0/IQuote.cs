using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Configurator_2._0
{
    public class IQuote
    {
        readonly Uri baseUrl = new Uri("https://sys01.iquotexpress.com/iqxAPI_rest/api/quote/get");
        public async Task GetData(string quoteID)
        {
            RestClient client = new RestClient(baseUrl);
            RestRequest request = new RestRequest();
            request.AddHeader("Authorization", "Bearer 2bcd01a6-11b0-4f9c-9864-95a21c7bf538-1514|SSID|1514");
            request.AddParameter("id", quoteID);
            request.AddParameter("include.items", "true");
            RestResponse response = await client.GetAsync(request, CancellationToken.None);
            QuoteData quote = JsonSerializer.Deserialize<QuoteData>(response.Content);
            
            ConfirmQuoteForm newForm = new ConfirmQuoteForm();
            newForm.label1.Text = quote.data.items[0].itemName;
            newForm.Show();
        }
    }
}