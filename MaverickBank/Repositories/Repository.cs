using MaverickBank.Contexts;
using MaverickBank.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaverickBank.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly MaverickBankContext _context;

        protected Repository(MaverickBankContext context)
        {
            _context = context;
        }

        public abstract Task<IEnumerable<T>> GetAll();
        public abstract Task<T> GetById(K id);

        public async Task<T> Add(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Update(K id, T entity)
        {
            var existingEntity = await GetById(id);
            if (existingEntity == null)
                throw new KeyNotFoundException("Entity not found");

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Delete(K id)
        {
            var entity = await GetById(id);
            if (entity == null)
                throw new KeyNotFoundException("Entity not found");

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
