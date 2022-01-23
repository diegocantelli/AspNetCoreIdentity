using IdentityUnderTheHood.Authorization;
using IdentityUnderTheHood.DTO;
using IdentityUnderTheHood.Pages.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IdentityUnderTheHood.Pages
{
    [Authorize(Policy = "HrManagerOnly")]
    public class HRmanagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public List<WeatherForecastDTO> WeatherForecastItens { get; set; }
        public HRmanagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("OurWebApi");
            var res = await client.PostAsJsonAsync("auth", new Credential { UserName = "Admin", Password = "123" });
            var strJwt = await res.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<JwtToken>(strJwt);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            WeatherForecastItens = await client.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast");
        }
    }
}
