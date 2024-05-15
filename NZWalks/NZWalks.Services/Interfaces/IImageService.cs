using NZWalks.Core.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Services.Interfaces
{
    public interface IImageService
    {
        Task<ImageDTO> uploadImage(ImageUploadDTO imageUploadDTO);
    }
}
