using AutoMapper;
using ECommerce.Core.Services.Implementations;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Product;
using ECommerce.Shared.Payload.Response.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using Xunit;
using ECommerce.Shared.Payload.Request.ProductImage;
using ECommerce.Shared.Payload.Response.ProductImage;

namespace ECommerce.Test
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork<EcommerceDbContext>> _unitOfWorkMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork<EcommerceDbContext>>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _imageServiceMock = new Mock<IImageService>();
            _cartServiceMock = new Mock<ICartService>();

            _productService = new ProductService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _httpContextAccessorMock.Object,
                _imageServiceMock.Object,
                _cartServiceMock.Object
            );
        }

        [Fact]
        public async Task Create_ShouldReturnCreateProductResponse_WhenValidRequest()
        {
            // Arrange
            var request = new ProductRequest
            {
                CategoryId = Guid.NewGuid(),
                Name = "Test Product",
                ProductImageBase64 = new List<string> { "base64Image1", "base64Image2" }
            };

            var category = new Category { Id = request.CategoryId, Name = "Test Category" };
            var product = new Product { Id = Guid.NewGuid(), Name = request.Name };

            var savedImages = new List<ProductImage>
    {
        new ProductImage { Id = Guid.NewGuid(), ImageUrl = "imageUrl1", IsMain = true },
        new ProductImage { Id = Guid.NewGuid(), ImageUrl = "imageUrl2", IsMain = false }
    };

            var response = new CreateProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                CategoryName = category.Name,
                ProductImageResponses = new List<ProductImageResponse>
        {
            new ProductImageResponse { ImageUrl = "imageUrl1" },
            new ProductImageResponse { ImageUrl = "imageUrl2" }
        }
            };

            // Mock category validation
            _unitOfWorkMock
                .Setup(u => u.GetRepository<Category>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(),
                    It.IsAny<Func<IQueryable<Category>, IIncludableQueryable<Category, object>>>()))
                .ReturnsAsync(category);

            // Mock product creation
            _mapperMock.Setup(m => m.Map<Product>(request)).Returns(product);
            _unitOfWorkMock.Setup(u => u.GetRepository<Product>().InsertAsync(product)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Mock image upload service to always return valid URLs
            _imageServiceMock.Setup(i => i.UploadImage(It.Is<UploadImgRequest>(r => r.Base64Image == "base64Image1")))
                .ReturnsAsync("imageUrl1");
            _imageServiceMock.Setup(i => i.UploadImage(It.Is<UploadImgRequest>(r => r.Base64Image == "base64Image2")))
                .ReturnsAsync("imageUrl2");

            // Mock ProcessAndSaveProductImagesAsync to return saved images
            _mapperMock.Setup(m => m.Map<CreateProductResponse>(product)).Returns(response);

            // Act
            var result = await _productService.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Name, result.CategoryName);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(2, result.ProductImageResponses.Count);
            Assert.Equal("imageUrl1", result.ProductImageResponses[0].ImageUrl);
            Assert.Equal("imageUrl2", result.ProductImageResponses[1].ImageUrl);

            _unitOfWorkMock.Verify(u => u.GetRepository<Product>().InsertAsync(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _imageServiceMock.Verify(i => i.UploadImage(It.IsAny<UploadImgRequest>()), Times.Exactly(2));
        }






        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Status = ProductEnum.ProductStatus.Active.ToString() };

            _unitOfWorkMock.Setup(u => u.GetRepository<Product>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(product);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _productService.Delete(productId);

            // Assert
            Assert.True(result);
            Assert.Equal(ProductEnum.ProductStatus.Deleted.ToString(), product.Status);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnProductDetailResponse_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "ProductName",
                Category = new Category { Name = "CategoryName" }
            };

            _unitOfWorkMock.Setup(u => u.GetRepository<Product>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(product);

            _mapperMock.Setup(m => m.Map<ProductDetailResponse>(product))
                .Returns(new ProductDetailResponse { Id = productId, Name = "ProductName" });

            // Act
            var result = await _productService.GetById(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("ProductName", result.Name);
        }

        [Fact]
        public async Task Update_ShouldReturnUpdatedProductResponse_WhenValidRequest()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new UpdateProductRequest
            {
                Name = "Updated Product",
                ProductImageBase64 = new List<string> { "base64Image1" }
            };

            var product = new Product
            {
                Id = productId,
                Name = "Old Product",
                Status = ProductEnum.ProductStatus.Active.ToString()
            };

            var updatedProduct = new Product
            {
                Id = productId,
                Name = "Updated Product",
                Status = ProductEnum.ProductStatus.Active.ToString()
            };

            var updatedResponse = new ProductResponse
            {
                Id = productId,
                Name = "Updated Product",
                MainImage = "imageUrl1"
            };

            _unitOfWorkMock
                .Setup(u => u.GetRepository<Product>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(product);

            _mapperMock.Setup(m => m.Map<Product>(request)).Returns(updatedProduct);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            _mapperMock.Setup(m => m.Map<ProductResponse>(It.IsAny<Product>()))
                .Returns(updatedResponse);

            // Act
            var result = await _productService.Update(productId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Product", result.Name);
            Assert.Equal("imageUrl1", result.MainImage);
            _unitOfWorkMock.Verify(u => u.GetRepository<Product>().UpdateAsync(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }


    }
}
