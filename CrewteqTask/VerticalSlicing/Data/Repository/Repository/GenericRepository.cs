using CrewteqTask.VerticalSlicing.Data.Context;
using CrewteqTask.VerticalSlicing.Data.Entities;
using CrewteqTask.VerticalSlicing.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;


namespace CrewteqTask.VerticalSlicing.Data.Repository.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, BaseEntity
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.Where(x => !x.IsDeleted);
        }

        public IQueryable<T> GetById(int id)
        {
            return _dbSet.Where(x => x.Id == id && !x.IsDeleted);
        }

        public IQueryable<T> GetByIdWithoutTrackingAsync(int id)
        {
            return _dbSet.AsNoTracking().Where(e => e.Id == id && !e.IsDeleted);
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            Update(entity);
        }

        public void HardDelete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                Update(entity);
            }
        }
        public void DeleteBulk(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void UpdateBulk(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

    }
}
