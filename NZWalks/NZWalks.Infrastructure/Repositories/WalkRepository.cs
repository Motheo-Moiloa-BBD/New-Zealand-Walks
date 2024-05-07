using Microsoft.EntityFrameworkCore;
using NZWalks.Core.Interfaces;
using NZWalks.Core.Models.Domain;
using NZWalks.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Infrastructure.Repositories
{
    public class WalkRepository : GenericRepository<Walk>, IWalkRepository
    {
        //Add methods that are specific to the Walk Entity
        public WalkRepository(NzWalksDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Walk> GetWalkByIdAsync(Guid id)
        {
            return await dbContext.Walks.Include(walk => walk.Difficulty).Include(walk => walk.Region).FirstOrDefaultAsync(w => w.Id == id);
        }
        public async Task<IEnumerable<Walk>> GetAllWalksAsync()
        {
            return await dbContext.Walks.Include(walk => walk.Difficulty).Include(walk => walk.Region).ToListAsync();
        }
    }
}
