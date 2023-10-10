using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Client.Models.Dto;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;

namespace Client.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login (LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                // If there are validation errors, return to the view with validation messages
                return View("Index",loginDto);
            }
            var client = httpClientFactory.CreateClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7067/api/auth/login"),
                Content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (response != null)
            {
                HttpContext.Session.SetString("AccessToken", response.JwtToken);
                return RedirectToAction("Index", "Regions");
            }
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                // If there are validation errors, return to the view with validation messages
                return View(registerUserDto);
            }
            var client = httpClientFactory.CreateClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7067/api/auth/register"),
                Content = new StringContent(JsonSerializer.Serialize(registerUserDto), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Auth");
            }
            var errorResponse = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(responseContent);
            ViewBag.Errors = errorResponse["errors"];
            return View();
        }
    }
}
