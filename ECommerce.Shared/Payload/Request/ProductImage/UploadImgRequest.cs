using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Request.ProductImage
{
    public class UploadImgRequest
    {
        [Required]
        public string? Base64Image { get; set; }
        [Required]
        public string? FileName { get; set; }
    }
}
