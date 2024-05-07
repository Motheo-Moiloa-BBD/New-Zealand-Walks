using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Core.Models.DTO
{
    public class AddWalkDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Length { get; set; }
        public string? WalkImageUrl { get; set; }
        public Guid DifficultyId { get; set; }
        public Guid RegionId { get; set; }
    }
}
