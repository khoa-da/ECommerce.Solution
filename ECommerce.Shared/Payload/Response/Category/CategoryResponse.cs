using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Response.Category
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public Guid? ParentId { get; set; }
        public string? ParentCategoryName { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string? Status { get; set; }
    }
}
