using IdentityUnderTheHood.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
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
            WeatherForecastItens = await client.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast");
        }
    }
}
