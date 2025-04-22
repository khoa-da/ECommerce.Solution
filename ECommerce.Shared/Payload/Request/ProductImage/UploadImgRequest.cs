using System.ComponentModel.DataAnnotations;

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
