using System.Linq.Expressions;
using studentsapi.DTO;

namespace studentsapi.Data.Repository
{
    public interface ICollegeRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        Task<T> CreateAsync(T record);
        Task<T> UpdateAsync(T record);
        Task<bool> DeleteAsync(int id);

        Task<bool> Exists(Expression<Func<T,bool>>filter);
    }
}
