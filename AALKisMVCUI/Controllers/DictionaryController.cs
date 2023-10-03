using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using AALKisMVCUI.Models;

namespace AALKisMVCUI.Controllers
{
    public class DictionaryController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly Uri baseAddress = new Uri("https://localhost:7014");
        private readonly HttpClient _client;

        public DictionaryController(ILogger<DictionaryController> logger)
        {
            _logger = logger;
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        public async Task<IActionResult> Index()
        {
            List<DictionaryEntry> entries = new List<DictionaryEntry>();

            HttpResponseMessage response = await _client.GetAsync("api/DictionaryEntries");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                entries = JsonSerializer.Deserialize<List<DictionaryEntry>>(data);
            }

            return View(entries);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DictionaryEntry dictionaryEntry = null;
            HttpResponseMessage response = await _client.GetAsync($"api/DictionaryEntries/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                dictionaryEntry = JsonSerializer.Deserialize<DictionaryEntry>(data);
            }

            if (dictionaryEntry == null)
            {
                return NotFound();
            }

            return View(dictionaryEntry);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DictionaryEntry dictionaryEntry)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(dictionaryEntry), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("api/DictionaryEntries", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // Handle successful creation, e.g., return to the index page.
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle error response, e.g., show an error message to the user.
                    ModelState.AddModelError(string.Empty, "Failed to create the dictionary entry.");
                }
            }

            return View(dictionaryEntry);
        }

        // Implement Edit, Delete, and other actions as needed similarly.

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
