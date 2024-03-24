using System.Net;
using System.Net.Http;
using System.Text;

using AALKisMVCUI.Utility;
using AALKisShared.Records;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;



namespace AALKisMVCUI.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<EditorController> _logger;
        private readonly APIClient _client;
        private readonly IHttpContextAccessor _contextAccessor;

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public UserController(ILogger<EditorController> logger, APIClient client, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _client = client;
            _contextAccessor = contextAccessor;
        }

        [Route("signin")]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync(Users user)
        {
            if (user == null)
            {
                return View();
            }
            else
            {
                string json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                string targetUri = $"/User/signin";
                try
                {
                    var response = await _client
                        .Fetch(targetUri, HttpMethod.Post, content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Your registration has been successful!";
                        return RedirectToAction("User", "LogIn");
                    }
                }
                catch (BadHttpRequestException ex)
                {
                    if ((HttpStatusCode)ex.StatusCode == HttpStatusCode.Forbidden)
                    {
                        TempData["ErrorMessage"] = "This name is already taken";
                    }
                    else if ((HttpStatusCode)ex.StatusCode == HttpStatusCode.BadRequest)
                    {
                        TempData["ErrorMessage"] = "The passwords you entered do not match!";
                    }
                    else if ((HttpStatusCode)ex.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["ErrorMessage"] = "Invalid email";
                    }
                    else if ((HttpStatusCode)ex.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        TempData["ErrorMessage"] = "Password and name must be entered and can not have more then 80 symbols";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while processing your request";
                    }
                    return View();
                }
                return View();
            }
        }


        [Route("login")]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogInAsync(Users user)
        {
            if (user == null)
            {
                return View();
            }
            else
            {
                string json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                string targetUri = $"/User/login";
                try
                {
                    var response = await _client
                        .Fetch(targetUri, HttpMethod.Post, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var loggedUser = JsonConvert.DeserializeObject<Users>(responseBody);
                        Console.WriteLine("\n\nlogin " + loggedUser.Id + "\n\n");

                        _contextAccessor.HttpContext.Session.SetString("User", loggedUser.Name);
                        _contextAccessor.HttpContext.Session.SetInt32("Id", loggedUser.Id);
                        return RedirectToAction("MyNotes", "Index");
                    }
                }
                catch (BadHttpRequestException ex)
                {
                    if ((HttpStatusCode)ex.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        TempData["ErrorMessage"] = "Wrong log in information";
                    }
                    else
                    {
                        Console.WriteLine("\n\n" + ex.Message + "\n\n" + ex.StatusCode + "\n\n");
                        Console.WriteLine(user.Name + "  " + user.Password);
                        TempData["ErrorMessage"] = "An error occurred while processing your request";
                    }
                    return View();
                }
                return View();
            }
        }

        public  IActionResult LogOut()
        {
            _contextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("User", "LogIn");
        }
    }
}
