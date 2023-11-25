//using Microsoft.AspNetCore.Mvc;
//using KvsProject.Domain;
//using KvsProject.Models;
//using KvsProject.Services.Abstract;
//using RenewalRemindr.Models;
//using System.Security.Cryptography;

//namespace KvsProject.Controllers
//{
//    public class KvsProjectController : Controller
//    {
//        private readonly IKvsProjectService _KvsProjectService;
//        private readonly IUserService _userService;

//        private readonly IUserAccessor _userAccessor;

//        public KvsProjectController(IKvsProjectService KvsProjectService, IUserAccessor userAccessor, IUserService userService)
//        {
//            _KvsProjectService = KvsProjectService;
//            _userAccessor = userAccessor;
//            _userService = userService;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }
//        public async Task<IActionResult> KvsProject_Read(GridRequest request)
//        {
//            this.StoreRequest(request);
//            var query = request.ToPagedQuery<UserKvsProject>();
//            query.Filters.Add(x => x.UserId == _userAccessor.User.Id);
//            return (await _KvsProjectService.Query(query)).ToGridResult(request);
//        }
//        public async Task<IActionResult> KvsProject_Edit(int id)
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
//            var model = new Domain.KvsProject() { UserKvsProjects = new List<UserKvsProject>() };

//            if (id > 0)
//            {
//                var getQuery = _KvsProjectService.NewQuery<Domain.KvsProject>(a => a.Id == id);
//                var result = await _KvsProjectService.Get<Domain.KvsProject>(getQuery, "UserKvsProjects");
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


//            model.UserKvsProjects = model.UserKvsProjects.Where(a => !a.Deleted && users.Data.Any(b => b.Id == a.UserId)).ToList();
//            foreach (var user in users.Data)
//            {
//                var userReminder = model.UserKvsProjects.FirstOrDefault(a => a.UserId == user.Id);
//                if (userReminder != null)
//                {
//                    userReminder.User = user;
//                }
//                else
//                {
//                    model.UserKvsProjects.Add(new UserKvsProject()
//                    {
//                        UserId = user.Id,
//                        KvsProjectId = model.Id,
//                        User = user
//                    });

//                }
//            }

//            return View(model);
//        }
//        public async Task<IActionResult> KvsProject_Save(Domain.KvsProject entity, List<UserKvsProject> users)
//        {
//            if (!ModelState.IsValid)
//            {
//                return this.ErrorJson(ModelState);
//            }
//            if (users != null)
//            {
//                users.ForEach(item => item.Deleted = !item.Deleted);
//            }

//            var result = await _KvsProjectService.SaveKvsProject(entity, users);
//            if (result.HasError)
//            {
//                return result.ToJson();
//            }
//            return this.SuccesJson(new { result.Data.Id });
//        }
//        public async Task<IActionResult> KvsProject_Delete(int id)
//        {
//            return (await _KvsProjectService.DeleteKvsProject(id)).ToJson();
//        }
//    }
//}
