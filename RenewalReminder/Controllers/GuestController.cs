using Microsoft.AspNetCore.Mvc;
using KvsProject.Domain;
using KvsProject.Services.Abstract;
using RenewalRemindr.Models;
using KvsProject.Services.Concrete;

namespace KvsProject.Controllers
{
    public class GuestController : Controller
    {
        private readonly ICentralService _centralService;

        public GuestController(ICentralService centralService)
        {
            _centralService = centralService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Guest_Read(GridRequest request)
        {
            this.StoreRequest(request);

            DateTime todayStart = DateTime.Today;
            DateTime todayEnd = todayStart.AddDays(1).AddTicks(-1);

            var query = request.ToPagedQuery<Guest>();

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

            return (await _centralService.Query(query)).ToGridResult(request);
        }
        public async Task<IActionResult> Guest_Edit(int id)
        {
            var model = new Guest();
            if (id > 0)
            {
                var getQuery = _centralService.NewQuery<Guest>(a => a.Id == id);
                var result = await _centralService.Get<Guest>(getQuery);
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
        public async Task<IActionResult> Guest_Save(Guest entity)
        {
            if (!ModelState.IsValid)
            {
                return this.ErrorJson(ModelState);
            }

            var result = await _centralService.SaveGuest(entity);
            if (result.HasError)
            {
                return result.ToJson();
            }
            return this.SuccesJson(new { result.Data.Id });
        }

        public async Task<IActionResult> Guest_Delete(int id)
        {
            return (await _centralService.DeleteGuest(id)).ToJson();
        }


    }
}
