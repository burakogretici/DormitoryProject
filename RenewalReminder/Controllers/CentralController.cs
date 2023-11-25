using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using KvsProject.Domain;
using KvsProject.Services.Abstract;
using RenewalRemindr.Models;

namespace KvsProject.Controllers
{
    public class CentralController : Controller
    {
        private readonly ICentralService _centralService;
        private readonly IStudentService _studentService;


        public CentralController(ICentralService centralService, IStudentService studentService)
        {
            _centralService = centralService;
            _studentService = studentService;
        }

        public IActionResult Index()
        {
            var studentQuery = _studentService.NewQuery<Student>(a => !a.Deleted);
            var students = _studentService.Query(studentQuery, a => new Student()
            {
                Id = a.Id,
                Name = a.Name,
                Surname = a.Surname,
                FullName = a.FullName,
            });

            ViewBag.Students = new SelectList(students.Result.Data, "Id", "FullName");
            return View();
        }
        public async Task<IActionResult> Central_Read(GridRequest request)
        {
            this.StoreRequest(request);

            DateTime todayStart = DateTime.Today;
            DateTime todayEnd = todayStart.AddDays(1).AddTicks(-1);

            var query = request.ToPagedQuery<Central>();
            if (request != null && request.Filters != null && request.Filters.Any(a => a.Field == "CreateDate"))
            {
                var filterVal = request.Filters.First(a => a.Field == "CreateDate").Value;


                if (DateTime.TryParse(filterVal, out DateTime parsedDate))
                {
                    var end = parsedDate.AddDays(1).AddTicks(-1);
                    query.Filters.Add(x => x.CreateDate >= parsedDate && x.CreateDate <= end);
                }

            }
            else
            {
                query.Filters.Add(x => x.CreateDate >= todayStart && x.CreateDate <= todayEnd);

            }
            var result = await _centralService.Query(query);


            return (await _centralService.Query(query)).ToGridResult(request);
        }

        public IActionResult MarketPermit()
        {
            var studentQuery = _studentService.NewQuery<Student>(a => !a.Deleted);
            var students =  _studentService.Query(studentQuery, a => new Student()
            {
                Id = a.Id,
                Name = a.Name,
                Surname = a.Surname,
                FullName = a.FullName,
            });

            ViewBag.Students = new SelectList(students.Result.Data, "Id", "FullName");
            return View();
        }
        public async Task<IActionResult> MarketPermit_Read(GridRequest request)
        {
            this.StoreRequest(request);

            var studentQuery = _studentService.NewQuery<Student>(a => !a.Deleted);
            var students = await _studentService.Query(studentQuery, a => new Student()
            {
                Id = a.Id,
                Name = a.Name,
                Surname = a.Surname,
                FullName = a.FullName,
                Number  = a.Number,
            });

            ViewBag.Students = new SelectList(students.Data, "Id", "FullName");

            DateTime todayStart = DateTime.Today;
            DateTime todayEnd = todayStart.AddDays(1).AddTicks(-1);

            var query = request.ToPagedQuery<MarketPermit>();

            if (request != null && request.Filters != null && request.Filters.Any(a => a.Field == "CreateDate"))
            {
                var filterVal = request.Filters.First(a => a.Field == "CreateDate").Value;


                if (DateTime.TryParse(filterVal, out DateTime parsedDate))
                {
                    var end = parsedDate.AddDays(1).AddTicks(-1);
                    query.Filters.Add(x => x.CreateDate >= parsedDate && x.CreateDate <= end);
                }

            }
            else
            {
                query.Filters.Add(x => x.CreateDate >= todayStart && x.CreateDate <= todayEnd);

            }
            var result = await _centralService.Query(query);


            return (await _centralService.Query(query)).ToGridResult(request);
        }

        public async Task<IActionResult> Central_Edit(int id)
        {
            var studentQuery = _studentService.NewQuery<Student>(a => !a.Deleted);
            var students = await _studentService.Query(studentQuery, a => new Student()
            {
                Id = a.Id,
                Name = a.Name,
                Surname = a.Surname,
                FullName = a.FullName,
            });

            ViewBag.Students = new SelectList(students.Data, "Id", "FullName");
            if (students.HasError)
            {
                return students.ToView(this);
            }


            var model = new Central();
            if (id > 0)
            {
                var getQuery = _centralService.NewQuery<Central>(a => a.Id == id);
                var result = await _centralService.Get<Central>(getQuery);
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

        public async Task<IActionResult> MarketPermit_Edit(int id)
        {
            var studentQuery = _studentService.NewQuery<Student>(a => !a.Deleted);
            var students = await _studentService.Query(studentQuery, a => new Student()
            {
                Id = a.Id,
                Name = a.Name,
                Surname = a.Surname,
                FullName = a.FullName,
            });

            ViewBag.Students = new SelectList(students.Data, "Id", "FullName");
            if (students.HasError)
            {
                return students.ToView(this);
            }


            var model = new MarketPermit();
            if (id > 0)
            {
                var getQuery = _centralService.NewQuery<MarketPermit>(a => a.Id == id);
                var result = await _centralService.Get<MarketPermit>(getQuery);
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
        public async Task<IActionResult> Central_Save(Central central)
        {
            if (central.Id == 0)
            {
                if (!ModelState.IsValid)
                {
                    return this.ErrorJson(ModelState);
                }
            }


            var result = await _centralService.SaveCentral(central);
            if (result.HasError)
            {
                return result.ToJson();
            }
            return this.SuccesJson(new { result.Data.Id });
        }

        public async Task<IActionResult> MarketPermit_Save(MarketPermit marketPermit)
        {
            //if (marketPermit.Id == 0)
            //{
            //    if (!ModelState.IsValid)
            //    {
            //        return this.ErrorJson(ModelState);
            //    }
            //}


            var result = await _centralService.SaveMarketPermit(marketPermit);
            if (result.HasError)
            {
                return result.ToJson();
            }
            return this.SuccesJson(new { result.Data.Id });
        }

    }
}

