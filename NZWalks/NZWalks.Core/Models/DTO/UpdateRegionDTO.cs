using System.ComponentModel.DataAnnotations;

namespace NZWalks.Core.Models.DTO
{
    public class UpdateRegionDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "Code has to have a minimum of 3 characters")]
        [MaxLength(3, ErrorMessage = "Code has to have a maximum of 3 characters")]
        public string Code { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Name has to have a maximum of 100 characters")]
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; }
    }
}
