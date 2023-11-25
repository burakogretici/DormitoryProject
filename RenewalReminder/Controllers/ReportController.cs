using Microsoft.AspNetCore.Mvc;
using RenewalReminder.Domain;
using RenewalReminder.Models;
using RenewalReminder.Services.Abstract;
using RenewalRemindr.Models;

namespace RenewalReminder.Controllers
{
    public class ReportController : Controller
    {
        private readonly ICentralService _centralService;

        public ReportController(ICentralService centralService)
        {
            _centralService = centralService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ExcusedLeave()
        {
            return View();
        }

        public async Task<IActionResult> ExcusedLeave_Read(GridRequest request)
        {

            this.StoreRequest(request);
            var query = request.ToPagedQuery<Central>();
            query.Filters.Add(x => x.IsExcused == true);
            var result = await _centralService.Query(query);
            
            var groupedStudent = result.Data
    .GroupBy(x => x.StudentId)
    .Select(group =>
    {
        var central = group.First();
        var permitDetails = group.Select(x => new PermitDetail
        {
            CheckOutTime = x.CheckOutTime,
            CheckInTime = x.CheckInTime
        }).ToList();

        return new StudentPermits
        {
            StudentNumber = central.Student.Number,
            FullName = central.Student?.FullName ?? string.Empty,
            TotalLeave = group.Sum(x => x.ElapsedTime ?? 0),
            PermitDetails = permitDetails,
            Student = central.Student,
        };
    }).ToList();

            var central = result.Data;

            Result<PagedList<StudentPermits>> data = new()
            {
                Data = new PagedList<StudentPermits>(groupedStudent, groupedStudent.Count, central.Page, central.PageSize),
                Errors = result.Errors,
                Extra = result.Extra
            };

         

            return (data.ToGridResult(request));


        }

        public async Task<IActionResult> Report_Read(GridRequest request)
        {

            this.StoreRequest(request);
            var query = request.ToPagedQuery<Central>();

            var result = await _centralService.Query(query);
            var groupedStudent = result.Data
    .GroupBy(x => x.StudentId)
    .Select(group =>
    {
        var central = group.First();
        var permitDetails = group.Select(x => new PermitDetail
        {
            CheckOutTime = x.CheckOutTime,
            CheckInTime = x.CheckInTime
        }).ToList();

        return new StudentPermits
        {
            StudentNumber = central.Student.Number,
            FullName = central.Student?.FullName ?? string.Empty,
            TotalLeave = group.Sum(x => x.ElapsedTime ?? 0),
            PermitDetails = permitDetails,
            Student = central.Student,
        };
    }).ToList();

            var central = result.Data;

            Result<PagedList<StudentPermits>> data = new()
            {
                Data = new PagedList<StudentPermits>(groupedStudent, groupedStudent.Count, central.Page, central.PageSize),
                Errors = result.Errors,
                Extra = result.Extra
            };

            //var groupedStudent = result.Data
            //   .GroupBy(x => x.StudentId)
            //   .Select(group =>
            //   {
            //       var central = group.First();
            //       return new StudentPermits { StudentNumber = central.Student.Number, FullName = central.Student?.FullName ?? string.Empty, CheckOutTime = central.CheckOutTime, CheckInTime = central.CheckInTime, TotalLeave = group.Sum(x => x.ElapsedTime ?? 0), Student = central.Student, };
            //   }).ToList();

            //var central = result.Data;

            //Result<PagedList<StudentPermits>> data = new()
            //{
            //    Data = new PagedList<StudentPermits>(groupedStudent, groupedStudent.Count, central.Page, central.PageSize),
            //    Errors = result.Errors,
            //    Extra = result.Extra

            //};

            return (data.ToGridResult(request));


        }
    }
}
