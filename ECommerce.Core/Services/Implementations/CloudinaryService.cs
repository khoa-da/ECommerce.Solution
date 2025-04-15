using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Payload.Request.ProductImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Implementations
{
    public class CloudinaryService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public async Task<string> UploadImage(UploadImgRequest uploadImgRequest)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(uploadImgRequest.Base64Image);

                using var stream = new MemoryStream(imageBytes);
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(uploadImgRequest.FileName, stream),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false
                };

                var result = await _cloudinary.UploadAsync(uploadParams);
                return result.SecureUrl?.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("Upload to Cloudinary failed: " + e.Message);
            }
        }
    }
}
