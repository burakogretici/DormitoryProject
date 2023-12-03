using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KvsProject.Models;
using KvsProject.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using static System.Net.Mime.MediaTypeNames;
using KvsProject.Domain;
using System.Diagnostics.Metrics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        //var date = DateTime.Now.AddYears(i);
        ViewBag.Date = DateTime.Now;
        DateTime todayStart = DateTime.Today;
        DateTime todayEnd = todayStart.AddDays(1).AddTicks(-1);

        var getQuery = _centralService.NewQuery<Central>(x => x.CreateDate >= todayStart && x.CreateDate <= todayEnd && x.CheckInTime == null);
        var result = await _centralService.Query<Central>(getQuery, "Student");
        //var central = await _centralService.Query<Central>(x => x.CreateDate >= todayStart && x.CreateDate <= todayEnd && x.CheckInTime == null);

        return PartialView(result.Data);
    }


    [AllowAnonymous]
    public IActionResult Login(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
    //var userRenewals = await _KvsProjectService.Query<UserKvsProject>(x => x.Deleted != true && x.UserId == _userAccessor.User.Id && (x.KvsProject.EndDate < date || x.KvsProject.EndDate == null), "KvsProject");

    //var renewals = new List<MonthByKvsProjectModel>();
    //if (userRenewals.Data != null && userRenewals.Data.Count() > 1)
    //{
    //    renewals = userRenewals.Data.GroupBy(a => a.KvsProject.StartDate.Month).Select(a => new MonthByKvsProjectModel
    //    {
    //        Key= a.Key,
    //        Value = Month(a.Key),
    //        KvsProjects = a.Select(a => a.KvsProject).OrderBy(a=>a.StartDate).ToList(),
    //    }).OrderBy(x=>x.Key).ToList();
    //}
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

    //private string Month(int month)
    //{
    //    switch (month)
    //    {
    //        case 1:
    //            return "OCAK";
    //        case 2:
    //            return "ŞUBAT";
    //        case 3:
    //            return "MART";
    //        case 4:
    //            return "NİSAN";
    //        case 5:
    //            return "MAYIS";
    //        case 6:
    //            return "HAZİRAN";
    //        case 7:
    //            return "TEMMUZ";
    //        case 8:
    //            return "AĞUSTOS";
    //        case 9:
    //            return "EYLÜL";
    //        case 10:
    //            return "EKİM";
    //        case 11:
    //            return "KASIM";
    //        case 12:
    //            return "ARALIK";
    //        default:
    //            return "";

    //    }
}
//}

