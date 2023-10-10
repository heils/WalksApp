using Client.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Client.Controllers
{
    public class WalksController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public WalksController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string searchQuery)
        {
            List<WalkDto> response = new List<WalkDto>();
            var client = httpClientFactory.CreateClient();
            /*var token = HttpContext.Session.GetString("AccessToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);*/
            var httpResponseMessage = await client.GetAsync($"https://localhost:7067/api/walks?filterOn=Name&filterQuery={searchQuery}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<WalkDto>>();
                response.AddRange(responseContent);
                return View(response);
            }
            //var errorResponse = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(responseContent);
            //ViewBag.Errors = errorResponse["errors"];
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var response = await GetFilters();
            List<RegionDto> regions = response.Regions;
            List<DifficultyDto> difficulties = response.Difficulties;
            ViewBag.Regions = regions;
            ViewBag.Difficulties = difficulties;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateWalkDto walkDto)
        {
            var filters = await GetFilters();
            List<RegionDto> regions = filters.Regions;
            List<DifficultyDto> difficulties = filters.Difficulties;
            ViewBag.Regions = regions;
            ViewBag.Difficulties = difficulties;
            if (!ModelState.IsValid)
            {
                // If there are validation errors, return to the view with validation messages
                return View(walkDto);
            }
            var client = httpClientFactory.CreateClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7067/api/walks"),
                Content = new StringContent(JsonSerializer.Serialize(walkDto), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<CreateWalkDto>();
            if (response is not null)
            {
                return RedirectToAction("Index", "Walks");
            }
            return View();
        }
        public async Task<OptionsListDto> GetFilters()
        {
            var client = httpClientFactory.CreateClient();
            var httpResponseMessage = await client.GetAsync("https://localhost:7067/api/walks/getoptionslist");
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<OptionsListDto>();
            return response;
        }
    }
}
