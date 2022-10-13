using ApplicationCore.Entities.Base;
using ApplicationCore.Repositories.Base;
using Infrastructure.Data.Telegram;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Base
{
    public abstract class Repository<T> : IRepository<T> where T : BaseEntity
    {
        internal TelegramContext _context;
        internal ILogger<T> _logger;

        public Repository(TelegramContext context, ILogger<T> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<T> AddAsync(T entity)
        {
            var result = _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Создан объект {entity.GetType()}, с Id: {result.Entity.Id}");
            return entity;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Удален объект {entity.GetType()}, с Id: {entity.Id}");
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Обновлен объект {entity.GetType()}, с Id: {entity.Id}");
        }
    }
}
