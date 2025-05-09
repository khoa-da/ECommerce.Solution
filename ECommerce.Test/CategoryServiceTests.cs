using AutoMapper;
using ECommerce.Core.Services.Implementations;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Category;
using ECommerce.Shared.Payload.Response.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Test
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork<EcommerceDbContext>> _unitOfWorkMock;
        private readonly Mock<ILogger<CategoryService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork<EcommerceDbContext>>();
            _loggerMock = new Mock<ILogger<CategoryService>>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _categoryService = new CategoryService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Fact]
        public async Task Create_ShouldReturnCategoryResponse_WhenValidRequest()
        {
            // Arrange
            var categoryRequest = new CategoryRequest
            {
                Name = "Test Category",
                Description = "Description for Test Category",
                ParentId = null
            };

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categoryRequest.Name,
                Description = categoryRequest.Description,
                ParentId = categoryRequest.ParentId,
                Status = CategoryEnum.CategoryStatus.Active.ToString(),
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            var categoryResponse = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Status = category.Status
            };

            _unitOfWorkMock
                .Setup(u => u.GetRepository<Category>().InsertAsync(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            _mapperMock
                .Setup(m => m.Map<Category>(categoryRequest))
                .Returns(category);

            _mapperMock
                .Setup(m => m.Map<CategoryResponse>(category))
                .Returns(categoryResponse);

            // Act
            var result = await _categoryService.Create(categoryRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryRequest.Name, result.Name);
            Assert.Equal(categoryRequest.Description, result.Description);
            Assert.Equal(CategoryEnum.CategoryStatus.Active.ToString(), result.Status);
            _unitOfWorkMock.Verify(u => u.GetRepository<Category>().InsertAsync(It.IsAny<Category>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        



        [Fact]
        public async Task Create_ShouldThrowException_WhenParentCategoryNotFound()
        {
            // Arrange
            var parentId = Guid.NewGuid();
            var categoryRequest = new CategoryRequest
            {
                Name = "Test Subcategory",
                Description = "Description for Test Subcategory",
                ParentId = parentId
            };

            _unitOfWorkMock
                .Setup(u => u.GetRepository<Category>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>>()))
                .ReturnsAsync((Category)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntryPointNotFoundException>(() => _categoryService.Create(categoryRequest));
        }

        [Fact]
        public async Task GetById_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category
            {
                Id = categoryId,
                Name = "Test Category",
                Description = "Description for Test Category",
                Status = CategoryEnum.CategoryStatus.Active.ToString()
            };

            var categoryResponse = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Status = category.Status
            };

            _unitOfWorkMock
                .Setup(u => u.GetRepository<Category>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>>()))
                .ReturnsAsync(category);

            _mapperMock
                .Setup(m => m.Map<CategoryResponse>(category))
                .Returns(categoryResponse);

            // Act
            var result = await _categoryService.GetById(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
            Assert.Equal(category.Name, result.Name);
            Assert.Equal(category.Description, result.Description);
        }

        [Fact]
        public async Task GetById_ShouldThrowException_WhenCategoryNotFound()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(u => u.GetRepository<Category>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>>()))
                .ReturnsAsync((Category)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntryPointNotFoundException>(() => _categoryService.GetById(categoryId));
        }


        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenCategoryExists()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category
            {
                Id = categoryId,
                Status = CategoryEnum.CategoryStatus.Active.ToString()
            };

            _unitOfWorkMock
     .Setup(u => u.GetRepository<Category>().SingleOrDefaultAsync(
         It.IsAny<Expression<Func<Category, bool>>>(),
         It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(),
         It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>>()))
     .ReturnsAsync(category);

            // Mock kiểm tra có sản phẩm liên quan (trả về false -> không có sản phẩm)
            _unitOfWorkMock
                .Setup(u => u.GetRepository<Product>().AnyAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(false);

            // Mock kiểm tra có danh mục con (trả về false -> không có danh mục con)
            _unitOfWorkMock
                .Setup(u => u.GetRepository<Category>().AnyAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>>()))
                .ReturnsAsync(false);


            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _categoryService.Delete(categoryId);

            // Assert
            Assert.True(result);
            _unitOfWorkMock.Verify(u => u.GetRepository<Category>().UpdateAsync(It.IsAny<Category>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenCategoryHasNoProducts()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category
            {
                Id = categoryId,
                Name = "Test Category",
                Status = CategoryEnum.CategoryStatus.Active.ToString()
            };

            _unitOfWorkMock
                .Setup(u => u.GetRepository<Category>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>>()))
                .ReturnsAsync(category);

            // Mock AnyAsync trả về false (không có sản phẩm)
            _unitOfWorkMock
                .Setup(u => u.GetRepository<Product>().AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _categoryService.Delete(categoryId);

            // Assert
            Assert.True(result);
            _unitOfWorkMock.Verify(u => u.GetRepository<Category>().UpdateAsync(It.IsAny<Category>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }


        [Fact]
        public async Task Delete_ShouldThrowException_WhenCategoryNotFound()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            _unitOfWorkMock
    .Setup(u => u.GetRepository<Category>().SingleOrDefaultAsync(
        It.IsAny<Expression<Func<Category, bool>>>(),
        It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(),
        It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>>()))
    .ReturnsAsync((Category)null);


            // Act & Assert
            await Assert.ThrowsAsync<EntryPointNotFoundException>(() => _categoryService.Delete(categoryId));
        }


    }
}
