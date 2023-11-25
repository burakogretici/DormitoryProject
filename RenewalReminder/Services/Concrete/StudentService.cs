using RenewalReminder.Data;
using RenewalReminder.Domain;
using RenewalReminder.Services.Abstract;
using RenewalReminderr.Domain.Exceptions;

namespace RenewalReminder.Services.Concrete
{
    public class StudentService : ServiceBase, IStudentService
    {
        private readonly IRepository<Student> _repositoryStudent;
        public StudentService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _repositoryStudent = _serviceProvider.GetService<IRepository<Student>>();
        }

        public async Task<Result> DeleteStudent(int id)
        {
            var isTransactional = _unitOfWork.IsTransactional();
            try
            {

                if (!isTransactional)
                {
                    await _unitOfWork.BeginTransaction();
                }

                var entity = await _repositoryStudent.Get(id);
                if (entity == null)
                {
                    throw new BusException("Silinecek kayıt bulunamadı.");
                }

                await _repositoryStudent.Delete(entity);

                if (!isTransactional)
                {
                    await _unitOfWork.CommitTransaction();
                }

                return new Result();
            }
            catch (Exception ex)
            {
                if (!isTransactional)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new Result(ex.Message);
                }
                throw;
            }
        }

        public async Task<Result<Student>> SaveStudent(Student entity)
        {
            var isTransactional = _unitOfWork.IsTransactional();
            try
            {
                var validationResult = _validation.Validate(entity);
                if (validationResult.HasError)
                {
                    return new Result<Student>(validationResult);
                }

                var oldEntity = default(Student);
                if (entity.Id > 0)
                {
                    oldEntity = await _repositoryStudent.Get(a => a.Id == entity.Id);
                    if (oldEntity == null)
                    {
                        throw new BusException("Güncellenecek kayıt bulunamadı.");
                    }
                    entity.CreateDate = oldEntity.CreateDate;
                }

                if (entity.Id > 0)
                {
                    await _repositoryStudent.Update(entity);
                }
                else
                {
                    entity.FullName = entity.Name + " " + entity.Surname;
                    await _repositoryStudent.Add(entity);
                }

                return new Result<Student>() { Data = entity };
            }
            catch (Exception ex)
            {
                if (!isTransactional)
                {
                    return new Result<Student>(ex.Message);
                }
                throw;
            }
        }

    }
}
