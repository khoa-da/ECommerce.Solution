﻿using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Category;
using ECommerce.Shared.Payload.Response.Category;

namespace ECommerce.Core.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponse> Create(CategoryRequest request);
        Task<CategoryResponse> GetById(Guid id);
        Task<CategoryResponse> Update(Guid id, CategoryRequest request);
        Task<bool> Delete(Guid id);
        Task<IPaginate<CategoryResponse>> GetAll(string? search, string? orderBy, int page, int size);
        Task<IPaginate<CategoryResponse>> GetByParentId(Guid? parentId, string? search, string? orderBy, int page, int size);
        Task<IPaginate<CategoryResponse>> GetAllParentCategory(string? status, int page, int size);
        Task<IPaginate<CategoryResponse>> GetAllChildrenCategory(string? status, int page, int size);
        Task<CategoryResponse> DeleteCategory(Guid id);
    }
}
