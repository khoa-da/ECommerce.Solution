using ECommerce.Shared.Payload.Request.ProductImage;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IImageService
    {
        public Task<string> UploadImage(UploadImgRequest uploadImgRequest);
    }
}
