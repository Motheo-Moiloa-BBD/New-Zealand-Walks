using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.Core.Models.DTO;
using NZWalks.Services.Interfaces;

namespace NZWalks.API.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService imageService;

        public ImagesController(IImageService imageService)
        {
            this.imageService = imageService;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadDTO imageUploadDTO)
        {
            ValidateFileUpload(imageUploadDTO);

            if (ModelState.IsValid)
            {
                var image = await imageService.uploadImage(imageUploadDTO);
                return Ok(image);
            }

            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageUploadDTO request)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension.");
            }

            if(request.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size more than 10MB, please upload a smaller size file.");
            }
        }
    }
}
