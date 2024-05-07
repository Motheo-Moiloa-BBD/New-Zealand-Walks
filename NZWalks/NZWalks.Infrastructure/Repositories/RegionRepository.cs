using NZWalks.Core.Models.Domain;
using NZWalks.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NZWalks.Infrastructure.Data;

namespace NZWalks.Infrastructure.Repositories
{
    public class RegionRepository: GenericRepository<Region>, IRegionRepository
    {
        //Add methods that are specific to the Region Entity
        public RegionRepository(NzWalksDbContext dbContext) : base(dbContext)
        {
        }
    }
}
