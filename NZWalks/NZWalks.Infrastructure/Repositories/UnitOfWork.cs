using NZWalks.Core.Interfaces;
using NZWalks.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Infrastructure.Repositories
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly NzWalksDbContext dbContext;
        public IRegionRepository Regions { get; private set; }

        public UnitOfWork(NzWalksDbContext dbContext, IRegionRepository regionRepository)
        {
            this.dbContext = dbContext;
            Regions = regionRepository;
        }

        public async Task<int> Save()
        {
            return await dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                dbContext.Dispose();
            }
        }
    }
}
