

using CrewteqTask.VerticalSlicing.Data.Entities;

namespace CrewteqTask.VerticalSlicing.Data.Repository.Interface
{
    public interface IGenericRepository<T> where T : class, BaseEntity
    {
        IQueryable<T> GetAll();

        IQueryable<T> GetById(int id);

        IQueryable<T> GetByIdWithoutTrackingAsync(int id);

        Task AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);
        void UpdateBulk(IEnumerable<T> entities);
        void HardDelete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        void Delete(T entity);

        void DeleteBulk(IEnumerable<T> entities);

        Task<int> SaveChangesAsync();

        //Task<int> GetPageCountWithSpecAsync(ISpecification<T> spec, int pageSize);
        //Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecification<T> Spec);
    }
}
