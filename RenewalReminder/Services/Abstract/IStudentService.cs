using KvsProject.Domain;
using KvsProject.Service;

namespace KvsProject.Services.Abstract
{
    public interface IStudentService : IServiceBase
    {
        Task<Result<Student>> SaveStudent(Student entity);
        Task<Result> DeleteStudent(int id);
    }
}
