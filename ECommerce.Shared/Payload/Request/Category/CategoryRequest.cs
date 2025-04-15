using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Request.Category
{
    public class CategoryRequest
    {
        public string Name { get; set; } = null!;

        public Guid? ParentId { get; set; }

        public string? Description { get; set; }
    }
}
