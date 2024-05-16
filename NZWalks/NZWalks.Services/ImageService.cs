using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NZWalks.Core.Exceptions;
using NZWalks.Core.Interfaces;
using NZWalks.Core.Models.Domain;
using NZWalks.Core.Models.DTO;
using NZWalks.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Services
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public ImageService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }
        public async Task<ImageDTO> uploadImage(ImageUploadDTO imageUploadDTO)
        {
            var image = new Image
            {
                File = imageUploadDTO.File,
                FileExtension = Path.GetExtension(imageUploadDTO.File.FileName),
                FileSizeInBytes = imageUploadDTO.File.Length,
                FileName = imageUploadDTO.FileName,
                FileDescription = imageUploadDTO.FileDescription
            };

            var localPath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{image.FileName}{image.FileExtension}");

            //Upload Image to Local Path
            using var stream = new FileStream(localPath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

            image.FilePath = urlFilePath;

            //Add image to Images Table
            await unitOfWork.Images.Add(image);

            var result = await unitOfWork.Save();

            if (result > 0)
            {
                var imageDto = mapper.Map<Image, ImageDTO>(image);

                return imageDto;
            }
            else
            {
                throw new BadRequestException("There was a problem when saving the image.");
            }

        }

        
    }
}
