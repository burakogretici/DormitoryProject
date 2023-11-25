using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RenewalReminder.Models;
using RenewalReminder.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using static System.Net.Mime.MediaTypeNames;
using RenewalReminder.Domain;
using System.Diagnostics.Metrics;

namespace RenewalReminder.Controllers;

public class HomeController : Controller
{
    private readonly IAuthService _authService;
    //private readonly IRenewalReminderService _renewalReminderService;

    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IUserAccessor _userAccessor;

    public HomeController(IAuthService authService, IWebHostEnvironment webHostEnvironment, IUserAccessor userAccessor = null)
    {
        _authService = authService;
        //_renewalReminderService = renewalReminderService;
        _webHostEnvironment = webHostEnvironment;
        _userAccessor = userAccessor;
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> List(int i = 1)
    {
        //var date = DateTime.Now.AddYears(i);
        //ViewBag.Date = date;

        //var userRenewals = await _renewalReminderService.Query<UserRenewalReminder>(x => x.Deleted != true && x.UserId == _userAccessor.User.Id && (x.RenewalReminder.EndDate < date || x.RenewalReminder.EndDate == null), "RenewalReminder");

        //var renewals = new List<MonthByRenewalReminderModel>();
        //if (userRenewals.Data != null && userRenewals.Data.Count() > 1)
        //{
        //    renewals = userRenewals.Data.GroupBy(a => a.RenewalReminder.StartDate.Month).Select(a => new MonthByRenewalReminderModel
        //    {
        //        Key= a.Key,
        //        Value = Month(a.Key),
        //        RenewalReminders = a.Select(a => a.RenewalReminder).OrderBy(a=>a.StartDate).ToList(),
        //    }).OrderBy(x=>x.Key).ToList();
        //}

        return PartialView();
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

