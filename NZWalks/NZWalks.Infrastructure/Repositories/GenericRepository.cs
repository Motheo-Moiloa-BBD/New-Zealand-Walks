using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NZWalks.Core.Interfaces;
using NZWalks.Infrastructure.Data;

namespace NZWalks.Infrastructure.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly NzWalksDbContext dbContext;
        protected GenericRepository(NzWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbContext.Set<T>().ToListAsync();
        }
        public async Task<T> GetById(Guid id)
        {
            return await dbContext.Set<T>().FindAsync(id); 
        }
        public async Task Add(T entity)
        {
           await dbContext.Set<T>().AddAsync(entity);
        }
        public void Delete(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }
        public void Update(T entity)
        {
            dbContext.Set<T>().Update(entity);
        }
    }
}
