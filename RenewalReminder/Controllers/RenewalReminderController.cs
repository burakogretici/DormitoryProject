//using Microsoft.AspNetCore.Mvc;
//using RenewalReminder.Domain;
//using RenewalReminder.Models;
//using RenewalReminder.Services.Abstract;
//using RenewalRemindr.Models;
//using System.Security.Cryptography;

//namespace RenewalReminder.Controllers
//{
//    public class RenewalReminderController : Controller
//    {
//        private readonly IRenewalReminderService _renewalReminderService;
//        private readonly IUserService _userService;

//        private readonly IUserAccessor _userAccessor;

//        public RenewalReminderController(IRenewalReminderService renewalReminderService, IUserAccessor userAccessor, IUserService userService)
//        {
//            _renewalReminderService = renewalReminderService;
//            _userAccessor = userAccessor;
//            _userService = userService;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }
//        public async Task<IActionResult> RenewalReminder_Read(GridRequest request)
//        {
//            this.StoreRequest(request);
//            var query = request.ToPagedQuery<UserRenewalReminder>();
//            query.Filters.Add(x => x.UserId == _userAccessor.User.Id);
//            return (await _renewalReminderService.Query(query)).ToGridResult(request);
//        }
//        public async Task<IActionResult> RenewalReminder_Edit(int id)
//        {
//            var userQuery = _userService.NewQuery<User>(a => !a.Deleted);
//            var users = await _userService.Query(userQuery, a => new User()
//            {
//                Id = a.Id,
//                Name = a.Name,
//                Surname = a.Surname,
//                Email = a.Email
//            });
//            if (users.HasError)
//            {
//                return users.ToView(this);
//            }
//            var model = new Domain.RenewalReminder() { UserRenewalReminders = new List<UserRenewalReminder>() };

//            if (id > 0)
//            {
//                var getQuery = _renewalReminderService.NewQuery<Domain.RenewalReminder>(a => a.Id == id);
//                var result = await _renewalReminderService.Get<Domain.RenewalReminder>(getQuery, "UserRenewalReminders");
//                if (result.HasError)
//                {
//                    return result.ToView(this);
//                }
//                if (result.Data == null)
//                {
//                    ModelState.AddModelError("", "Kayıt bulunamadı.");
//                    return View();
//                }
//                model = result.Data;
//            }


//            model.UserRenewalReminders = model.UserRenewalReminders.Where(a => !a.Deleted && users.Data.Any(b => b.Id == a.UserId)).ToList();
//            foreach (var user in users.Data)
//            {
//                var userReminder = model.UserRenewalReminders.FirstOrDefault(a => a.UserId == user.Id);
//                if (userReminder != null)
//                {
//                    userReminder.User = user;
//                }
//                else
//                {
//                    model.UserRenewalReminders.Add(new UserRenewalReminder()
//                    {
//                        UserId = user.Id,
//                        RenewalReminderId = model.Id,
//                        User = user
//                    });

//                }
//            }

//            return View(model);
//        }
//        public async Task<IActionResult> RenewalReminder_Save(Domain.RenewalReminder entity, List<UserRenewalReminder> users)
//        {
//            if (!ModelState.IsValid)
//            {
//                return this.ErrorJson(ModelState);
//            }
//            if (users != null)
//            {
//                users.ForEach(item => item.Deleted = !item.Deleted);
//            }

//            var result = await _renewalReminderService.SaveRenewalReminder(entity, users);
//            if (result.HasError)
//            {
//                return result.ToJson();
//            }
//            return this.SuccesJson(new { result.Data.Id });
//        }
//        public async Task<IActionResult> RenewalReminder_Delete(int id)
//        {
//            return (await _renewalReminderService.DeleteRenewalReminder(id)).ToJson();
//        }
//    }
//}
