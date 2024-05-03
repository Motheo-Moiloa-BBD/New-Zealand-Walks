using Microsoft.EntityFrameworkCore;
using NZWalks.Core.Models.Domain;

namespace NZWalks.Infrastructure.Data
{
    public class NzWalksDbContext : DbContext 
    {
        public NzWalksDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Walk> Walks { get; set; }
    }
}
