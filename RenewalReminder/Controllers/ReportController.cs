using Microsoft.AspNetCore.Mvc;
using KvsProject.Domain;
using KvsProject.Models;
using KvsProject.Services.Abstract;
using RenewalRemindr.Models;

namespace KvsProject.Controllers
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

            var centrals = await _centralService.Query<Central>(x => x.Deleted != true, "Student");
            RemoveCentralsFromStudent(centrals);

            var groupedStudent = GroupCentralsByStudent(centrals);

            var central = result.Data;

            var data = new Result<PagedList<StudentPermits>>
            {
                Data = new PagedList<StudentPermits>(groupedStudent, groupedStudent.Count, central.Page, central.PageSize),
                Errors = result.Errors,
                Extra = result.Extra
            };

            return data.ToGridResult(request);
        }

        private void RemoveCentralsFromStudent(Result<IEnumerable<Central>> centrals)
        {
            centrals.Data.ToList().ForEach(x => x.Student.Centrals = null);
        }

        private List<StudentPermits> GroupCentralsByStudent(Result<IEnumerable<Central>> centrals)
        {
            return centrals.Data
                .GroupBy(x => x.Student.Id)
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
                        IsExcused = central.IsExcused
                    };
                }).ToList();
        }

    }
}
