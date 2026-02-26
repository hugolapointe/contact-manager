using ContactManager.WebSite.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace ContactManager.WebSite.Controllers;

[Authorize]
public class HomeController : Controller {

    [HttpGet]
    public IActionResult Index() => RedirectToAction("Index", "Contact");

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Privacy() => View();

    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}