using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public sealed class HomeController : Controller
{
    public IActionResult Index() => View();
}
