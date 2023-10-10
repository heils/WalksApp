using Client.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace Client.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public RegionsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<RegionDto> response = new List<RegionDto>();
            var client = httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("AccessToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var httpResponseMessage = await client.GetAsync("https://localhost:7067/api/regions");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>();
                response.AddRange(responseContent);
                return View(response);
            }
            //var errorResponse = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(responseContent);
            //ViewBag.Errors = errorResponse["errors"];
            return View();
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(CreateRegionDto regionDto)
        {
            if (!ModelState.IsValid)
            {
                // If there are validation errors, return to the view with validation messages
                return View(regionDto);
            }
            var client = httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("AccessToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7067/api/regions"),
                Content = new StringContent(JsonSerializer.Serialize(regionDto), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            //httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<CreateRegionDto>();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("AccessToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var httpResponseMessage = await client.GetAsync($"https://localhost:7067/api/regions/{id.ToString()}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();
                return View(responseContent);
            }    
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto regionDto)
        {
            var client = httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("AccessToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var httpRequestMessage = new HttpRequestMessage()
            {

                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7067/api/regions/{regionDto.Id.ToString()}"),
                Content = new StringContent(JsonSerializer.Serialize(regionDto), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();
            if(response is not null)
            {
                return RedirectToAction("Index", "Regions");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(RegionDto regionDto)
        {
            var client = httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("AccessToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var httResponseMessage = await client.DeleteAsync($"https://localhost:7067/api/regions/{regionDto.Id.ToString()}");
            httResponseMessage.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Regions");
        }
    }
}
