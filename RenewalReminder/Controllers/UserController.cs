using Microsoft.AspNetCore.Mvc;
using RenewalReminder.Domain;
using RenewalReminder.Services.Abstract;
using RenewalRemindr.Models;

namespace RenewalReminder.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> User_Read(GridRequest request)
        {
            this.StoreRequest(request);
            var query = request.ToPagedQuery<User>();
            return (await _userService.Query(query)).ToGridResult(request);
        }
        public async Task<IActionResult> User_Edit(int id)
        {
            var model = new User();
            if (id > 0)
            {
                var getQuery = _userService.NewQuery<User>(a => a.Id == id);
                var result = await _userService.Get<User>(getQuery);
                if (result.HasError)
                {
                    return result.ToView(this);
                }
                if (result.Data == null)
                {
                    ModelState.AddModelError("", "Kayıt bulunamadı.");
                    return View();
                }
                model = result.Data;
            }

            return View(model);
        }
        public async Task<IActionResult> User_Save(User entity)
        {
            if (!ModelState.IsValid)
            {
                return this.ErrorJson(ModelState);
            }
         
            var result = await _userService.SaveUSer(entity);
            if (result.HasError)
            {
                return result.ToJson();
            }
            return this.SuccesJson(new { result.Data.Id });
        }
        public async Task<IActionResult> User_Delete(int id)
        {
            return (await _userService.DeleteUser(id)).ToJson();
        }
    }
}
