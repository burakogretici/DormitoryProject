using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using KvsProject.Domain;
using KvsProject.Services.Abstract;
using RenewalRemindr.Models;
using KvsProject.Services.Concrete;
using KvsProject.Models;

namespace KvsProject.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Student_Read(GridRequest request)
        {
            this.StoreRequest(request);
            var query = request.ToPagedQuery<Student>();

            var result = await _studentService.Query(query);
            return (result.ToGridResult(request));
        }
        public async Task<IActionResult> Student_Edit(int id)
        {
            var model = new Student();
            if (id > 0)
            {
                var getQuery = _studentService.NewQuery<Student>(a => a.Id == id);
                var result = await _studentService.Get<Student>(getQuery);
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
        public async Task<IActionResult> Student_Save(Student entity)
        {
            if (!ModelState.IsValid)
            {
                return this.ErrorJson(ModelState);
            }

            var result = await _studentService.SaveStudent(entity);
            if (result.HasError)
            {
                return result.ToJson();
            }
            return result.ToJson();
        }
        public async Task<IActionResult> Student_Delete(int id)
        {
            return (await _studentService.DeleteStudent(id)).ToJson();
        }
    }
}
