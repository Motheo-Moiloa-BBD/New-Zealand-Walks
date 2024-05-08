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
        public async Task<IEnumerable<Walk>> GetAllWalksAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, string? sortOrder = null, int? pageNumber = 1, int? pageSize = 5)
        {
            var walks = dbContext.Walks.Include(walk => walk.Difficulty).Include(walk => walk.Region).AsQueryable();

            //Filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(walk => walk.Name.Contains(filterQuery));
                }
                
                if(filterOn.Equals("Description", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(walk => walk.Description.Contains(filterQuery));
                }
                
                if (filterOn.Equals("Length"))
                {
                    walks = walks.Where(walk => walk.Length.Equals(int.Parse(filterQuery)));
                }
                
            }

            //Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false && string.IsNullOrWhiteSpace(sortOrder) == false) 
            { 
                if(sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    var isAsc = string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;
                    walks = isAsc ? walks.OrderBy(walk => walk.Name) : walks.OrderByDescending(walks => walks.Name);
                }

                if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    var isAsc = string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;
                    walks = isAsc ? walks.OrderBy(walk => walk.Length) : walks.OrderByDescending(walks => walks.Length);
                }
            }

            //Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await walks.Skip(skipResults ?? 0).Take(pageSize ?? 5).ToListAsync();
        }
    }
}
