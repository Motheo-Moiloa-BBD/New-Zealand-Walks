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
    public class ImageRepository : GenericRepository<Image>, IImageRepository
    {
        //Add methods that are specific to the Image Entity
        public ImageRepository(NzWalksDbContext dbContext) : base(dbContext)
        {
        }
    }
}
