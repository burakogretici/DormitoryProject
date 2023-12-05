using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KvsProject.Models;
using KvsProject.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using KvsProject.Domain;


namespace KvsProject.Controllers;

public class HomeController : Controller
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IUserAccessor _userAccessor;
    private readonly ICentralService _centralService;


    public HomeController(IAuthService authService, IWebHostEnvironment webHostEnvironment, IUserAccessor userAccessor = null, ICentralService centralService = null)
    {
        _authService = authService;
        _webHostEnvironment = webHostEnvironment;
        _userAccessor = userAccessor;
        _centralService = centralService;
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> List(int i = 1)
    {
        ViewBag.Date = DateTime.Now;
        DateTime todayStart = DateTime.Today;
        DateTime todayEnd = todayStart.AddDays(1).AddTicks(-1);

        var getQuery = _centralService.NewQuery<Central>(x =>x.CheckInTime == null);
        var result = await _centralService.Query<Central>(getQuery, "Student");

        return PartialView(result.Data);
    }


    [AllowAnonymous]
    public IActionResult Login(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserForLogin model, string? returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!string.IsNullOrEmpty(returnUrl))
        {
            if (returnUrl.StartsWith("http:"))
            {
                returnUrl = "";
            }
        }
        ViewBag.ReturnUrl = returnUrl;

        var result = await _authService.Login(model.Username, model.Password);
        if (result.HasError)
        {
            ModelState.AddModelError("", string.Join(",", result.Errors));
            return View(model);
        }

        if (string.IsNullOrEmpty(returnUrl))
        {
            return Redirect("/");
        }
        else
        {
            return Redirect(returnUrl);
        }
    }

    [AllowAnonymous]
    public IActionResult Logout()
    {
        _authService.Logout();
        return Redirect("/");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}

