using AutoMapper;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Category;
using ECommerce.Shared.Payload.Response.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Implementations
{
    public class CategoryService : BaseService<CategoryService>, ICategoryService
    {
        public CategoryService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<CategoryService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<CategoryResponse> Create(CategoryRequest request)
        {
            // Kiểm tra xem có Parent Category hay không nếu có ParentId
            if (request.ParentId != null)
            {
                var parentCategory = await _unitOfWork.GetRepository<Category>().AnyAsync(predicate: p => p.Id == request.ParentId);
                if (!parentCategory)
                {
                    throw new EntryPointNotFoundException("Parent category not found.");
                }
            }

            var category = _mapper.Map<Category>(request);
            category.Id = Guid.NewGuid();
            category.Status = CategoryEnum.CategoryStatus.Active.ToString();
            category.CreatedDate = DateTime.UtcNow.AddHours(7);
            category.UpdatedDate = DateTime.UtcNow.AddHours(7);
            category.ParentId = request.ParentId;

            await _unitOfWork.GetRepository<Category>().InsertAsync(category);

            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new DataConflictException("Failed to create category");
            }

            var categoryResponse = _mapper.Map<CategoryResponse>(category);

            // Nếu có Parent Category, lấy thông tin và gán vào response
            if (category.ParentId.HasValue)
            {
                var parentCategory = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                    predicate: x => x.Id == category.ParentId);

                if (parentCategory != null)
                {
                    categoryResponse.ParentCategoryName = parentCategory.Name;
                }
            }

            return categoryResponse;
        }

        public async Task<bool> Delete(Guid id)
        {
            var category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: x => x.Id == id);

            if (category == null)
            {
                throw new EntryPointNotFoundException("Category not found");
            }

            // Kiểm tra xem category có sản phẩm hay không
            var hasProducts = await _unitOfWork.GetRepository<Product>().AnyAsync(
                predicate: x => x.CategoryId == id);

            if (hasProducts)
            {
                throw new BusinessRuleException("Cannot delete category that has products");
            }

            // Kiểm tra xem category có danh mục con hay không
            var hasChildren = await _unitOfWork.GetRepository<Category>().AnyAsync(
                predicate: x => x.ParentId == id);

            if (hasChildren)
            {
                throw new BusinessRuleException("Cannot delete category that has child categories");
            }

            // Thay vì xóa thật, chỉ cập nhật trạng thái
            category.Status = CategoryEnum.CategoryStatus.Inactive.ToString();
            category.UpdatedDate = DateTime.UtcNow.AddHours(7);

            _unitOfWork.GetRepository<Category>().UpdateAsync(category);

            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<IPaginate<CategoryResponse>> GetAll(string? search, string? orderBy, int page, int size)
        {
            Expression<Func<Category, bool>> predicate = x => true;

            if (!string.IsNullOrEmpty(search))
            {
                predicate = x => x.Name.Contains(search) || x.Description.Contains(search);
            }

            Func<IQueryable<Category>, IOrderedQueryable<Category>> orderByFunc = null;

            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "name":
                        orderByFunc = q => q.OrderBy(x => x.Name);
                        break;
                    case "name_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.Name);
                        break;
                    case "date":
                        orderByFunc = q => q.OrderBy(x => x.CreatedDate);
                        break;
                    case "date_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
                        break;
                    default:
                        orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
                        break;
                }
            }
            else
            {
                orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
            }

            var categories = await _unitOfWork.GetRepository<Category>().GetPagingListAsync(
                selector: x => _mapper.Map<CategoryResponse>(x),
                predicate: predicate,
                orderBy: orderByFunc,
                page: page,
                size: size
            );
            return categories;
        }

        public async Task<CategoryResponse> GetById(Guid id)
        {
            var category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: x => x.Id == id
            );

            if (category == null)
            {
                throw new EntryPointNotFoundException("Category not found");
            }

            var response = _mapper.Map<CategoryResponse>(category);

            if (category.ParentId != null)
            {
                var parentCategory = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                    predicate: x => x.Id == category.ParentId);
                response.ParentCategoryName = parentCategory.Name;
            }

            return response;
        }

        public async Task<IPaginate<CategoryResponse>> GetByParentId(Guid? parentId, string? search, string? orderBy, int page, int size)
        {
            Expression<Func<Category, bool>> predicate = x => x.ParentId == parentId;

            if (!string.IsNullOrEmpty(search))
            {
                predicate = x => x.ParentId == parentId && (x.Name.Contains(search) || x.Description.Contains(search));
            }

            Func<IQueryable<Category>, IOrderedQueryable<Category>> orderByFunc = null;

            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "name":
                        orderByFunc = q => q.OrderBy(x => x.Name);
                        break;
                    case "name_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.Name);
                        break;
                    case "date":
                        orderByFunc = q => q.OrderBy(x => x.CreatedDate);
                        break;
                    case "date_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
                        break;
                    default:
                        orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
                        break;
                }
            }
            else
            {
                orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
            }

            var categories = await _unitOfWork.GetRepository<Category>().GetPagingListAsync(
                selector: x => _mapper.Map<CategoryResponse>(x),
                predicate: predicate,
                orderBy: orderByFunc,
                page: page,
                size: size
            );

            return categories;
        }

        public async Task<CategoryResponse> Update(Guid id, CategoryRequest request)
        {
            var category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: x => x.Id == id);

            if (category == null)
            {
                throw new Exception("Category not found");
            }

            // Kiểm tra ParentId
            if (request.ParentId != null)
            {
                // Không cho phép đặt ParentId là chính nó
                if (request.ParentId == id)
                {
                    throw new Exception("A category cannot be its own parent");
                }

                // Kiểm tra xem Parent Category có tồn tại không
                var parentCategory = await _unitOfWork.GetRepository<Category>().AnyAsync(
                    predicate: p => p.Id == request.ParentId);

                if (!parentCategory)
                {
                    throw new Exception("Parent category not found");
                }

                // Kiểm tra để tránh tạo vòng lặp (cycle) trong cây phân cấp
                var tempParentId = request.ParentId;
                while (tempParentId != null)
                {
                    var parent = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                        predicate: p => p.Id == tempParentId);

                    if (parent == null)
                    {
                        break;
                    }

                    if (parent.ParentId == id)
                    {
                        throw new Exception("Circular reference detected in category hierarchy");
                    }

                    tempParentId = parent.ParentId;
                }
            }

            // Cập nhật thông tin category
            category.Name = request.Name;
            category.Description = request.Description;
            category.ParentId = request.ParentId;
            category.UpdatedDate = DateTime.UtcNow.AddHours(7);

            _unitOfWork.GetRepository<Category>().UpdateAsync(category);

            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new Exception("Failed to update category");
            }

            var categoryResponse = _mapper.Map<CategoryResponse>(category);

            // Nếu có Parent Category, lấy thông tin và gán vào response
            if (category.ParentId.HasValue)
            {
                var parentCategory = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                    predicate: x => x.Id == category.ParentId);

                if (parentCategory != null)
                {
                    categoryResponse.ParentCategoryName = parentCategory.Name;
                }
            }

            return categoryResponse;
        }
    }
}