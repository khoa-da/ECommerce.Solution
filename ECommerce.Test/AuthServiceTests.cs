using AutoMapper;
using Castle.Core.Logging;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Implementations;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Infrastructure.Utils;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Auth;
using ECommerce.Shared.Payload.Response.Auth;
using Microsoft.AspNetCore.Http;
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
    public class AuthServiceTests
    {
        public readonly Mock<IUnitOfWork<EcommerceDbContext>> _unitOfWorkMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork<EcommerceDbContext>>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _cartServiceMock = new Mock<ICartService>();

            _authService = new AuthService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _httpContextAccessorMock.Object,
                _cartServiceMock.Object
            );
        }

        
        // Test 1: Đăng nhập thành công
        [Fact]
        public async Task Login_ShouldReturnLoginResponse_WhenValidCredentials()
        {
            // Arrange
            var loginRequest = new LoginRequest { UsernameOrEmail = "testuser", Password = "khoa" };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "123456789",
                Status = "Active",
                Role = "Customer",
                PasswordHash = PasswordUtil.HashPassword("khoa")
            };
            var loginResponse = new LoginResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            _unitOfWorkMock
                .Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<User, bool>>>(), null, null))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.Login(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);
        }


        // Test 2: Đăng nhập thất bại do không tìm thấy User
        [Fact]
        public async Task Login_ShouldThrowEntityNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var loginRequest = new LoginRequest { UsernameOrEmail = "unknownuser", Password = "password123" };

            _unitOfWorkMock
                .Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<User, bool>>>(), null, null))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _authService.Login(loginRequest));
        }

        [Fact]
        public async Task Login_ShouldThrowBusinessRuleException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginRequest = new LoginRequest { UsernameOrEmail = "khoav1", Password = "wrongpassword" };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "khoav1",
                Email = "khoav1@gmail.com",
                FirstName = "Testv1",
                LastName = "Userv1",
                PhoneNumber = "123456789",
                Status = "Active",
                Role = "Customer",
                PasswordHash = "v6plobem2ptzJLRd532mc835oAiq5JhrqBgHaCbjR+Y=" // Đúng mật khẩu hash
            };

            _unitOfWorkMock
                .Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<User, bool>>>(), null, null))
                .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() => _authService.Login(loginRequest));
            Assert.Equal("Invalid Email or Username or Password", exception.Message);
        }


        // Test 4: Refresh Token thành công
        //[Fact]
        //public async Task RefreshToken_ShouldReturnNewToken_WhenUserExists()
        //{
        //    // Arrange
        //    var userId = Guid.NewGuid();
        //    var refreshTokenRequest = new RefreshTokenRequest
        //    {
        //        UserId = userId,
        //        RefreshToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI3Njk5MGRlNS1jMDQ5LTRjZjEtYjc5NC1jNTk5NWNmNjZmZWIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJDdXN0b21lciIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiYzZhYWRmM2MtNTk0Yi00NzhlLTkyOTQtNTRlOWM3YWExZmMyIiwiVXNlcklkIjoiYzZhYWRmM2MtNTk0Yi00NzhlLTkyOTQtNTRlOWM3YWExZmMyIiwibmJmIjoxNzQ2NjAyODc1LCJleHAiOjE3NDcyMDc2NzUsImlzcyI6Iklzc3VlciJ9.cwK6xI0o1NwLWtBcYeIy-2_nBxPQc-gfH9yNiJ7atT4"
        //    };

        //    var user = new User
        //    {
        //        Id = userId,
        //        Username = "khoav1",
        //        Email = "khoav1@gmail.com",
        //        FirstName = "Testv1",
        //        LastName = "Userv1",
        //        PhoneNumber = "123456789",
        //        Status = "Active",
        //        Role = "Customer",
        //        PasswordHash = "v6plobem2ptzJLRd532mc835oAiq5JhrqBgHaCbjR+Y="
        //    };

        //    _unitOfWorkMock
        //        .Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(
        //            It.IsAny<Expression<Func<User, bool>>>(), null, null))
        //        .ReturnsAsync(user);

        //    // Act
        //    var result = await _authService.RefreshToken(refreshTokenRequest);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(user.Id, result.UserId);
        //    Assert.Equal(user.Username, result.Username);
        //    Assert.Equal(user.Email, result.Email);
        //    Assert.False(string.IsNullOrEmpty(result.AccessToken), "AccessToken should not be empty");
        //    Assert.False(string.IsNullOrEmpty(result.RefreshToken), "RefreshToken should not be empty");
        //    Assert.True(result.AccessTokenExpires > DateTime.UtcNow, "AccessTokenExpires should be a future date");
        //    Assert.True(result.RefreshTokenExpires > DateTime.UtcNow, "RefreshTokenExpires should be a future date");
        //}


        // Test 5: Refresh Token thất bại do User không tồn tại
        [Fact]
        public async Task RefreshToken_ShouldThrowEntityNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var refreshTokenRequest = new RefreshTokenRequest { UserId = Guid.NewGuid() };

            _unitOfWorkMock
                .Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<User, bool>>>(), null, null))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _authService.RefreshToken(refreshTokenRequest));
        }
    }
}
