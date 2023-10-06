using Client.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

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
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMessage = await client.GetAsync("https://localhost:7067/api/regions");
                httpResponseMessage.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());
            }
            catch (Exception)
            {

                throw;
            }

            return View(response);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(CreateRegionDto regionDto)
        {
            var client = httpClientFactory.CreateClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7067/api/regions"),
                Content = new StringContent(JsonSerializer.Serialize(regionDto), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<CreateRegionDto>();
            if (response is not null)
            {
                return RedirectToAction("Index", "Regions");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetFromJsonAsync<RegionDto>($"https://localhost:7067/api/regions/{id.ToString()}");
            if(response is not null)
            {
                return View(response);
            }
            return View(null);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto regionDto)
        {
            var client = httpClientFactory.CreateClient();
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
            var httResponseMessage = await client.DeleteAsync($"https://localhost:7067/api/regions/{regionDto.Id.ToString()}");
            httResponseMessage.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Regions");
        }
    }
}
