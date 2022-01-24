using IdentityUnderTheHood.Authorization;
using IdentityUnderTheHood.DTO;
using IdentityUnderTheHood.Pages.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
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
            WeatherForecastItens = await InvokeEndpoint<List<WeatherForecastDTO>>("OurWebApi", "WeatherForecast");
        }

        private async Task<JwtToken> Authenticate()
        {
            var client = _httpClientFactory.CreateClient("OurWebApi");
            var res = await client.PostAsJsonAsync("auth", new Credential { UserName = "Admin", Password = "123" });
            res.EnsureSuccessStatusCode();
            var strJwt = await res.Content.ReadAsStringAsync();

            HttpContext.Session.SetString("access_token", strJwt);
            return JsonConvert.DeserializeObject<JwtToken>(strJwt);
        }

        private async Task<T> InvokeEndpoint<T>(string clientName, string url)
        {
            JwtToken token = null;
            var strTokenObj = HttpContext.Session.GetString("access_token");

            if (string.IsNullOrEmpty(strTokenObj))
            {
                token = await Authenticate();
            }
            else
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj);

            if (token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
            {
                token = await Authenticate();
            }

            var client = _httpClientFactory.CreateClient(clientName);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            return await client.GetFromJsonAsync<T>(url);
        }
    }
}
