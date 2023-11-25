using RenewalReminder.Domain;
using RenewalReminder.Service;

namespace RenewalReminder.Services.Abstract
{
    public interface IStudentService : IServiceBase
    {
        Task<Result<Student>> SaveStudent(Student entity);
        Task<Result> DeleteStudent(int id);
    }
}
