using ECommerce.Shared.Payload.Request.ProductImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IImageService
    {
        public Task<string> UploadImage(UploadImgRequest uploadImgRequest);
    }
}
