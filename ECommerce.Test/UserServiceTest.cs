using AutoMapper;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Implementations;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.User;
using ECommerce.Shared.Payload.Response.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Test
{
    public class UserServiceTest
    {
        private readonly Mock<IUnitOfWork<EcommerceDbContext>> _unitOfWorkMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork<EcommerceDbContext>>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _userService = new UserService(_unitOfWorkMock.Object, _loggerMock.Object, _mapperMock.Object, _httpContextAccessorMock.Object);
        }
        [Fact]
        public async Task CreateUser_ShouldCreateUser_WhenValidRequest()
        {
            // Arrange
            var createUserRequest = new UserRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "password123",
                PhoneNumber = "1234567890"
            };

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = createUserRequest.Username,
                Email = createUserRequest.Email,
                PasswordHash = "hashedPassword",
                PhoneNumber = createUserRequest.PhoneNumber,
                Role = RoleEnum.Customer.ToString(),
                RegisteredDate = DateTime.UtcNow.AddHours(7),
                Status = UserEnum.Status.Active.ToString()
            };

            var userResponse = new UserResponse
            {
                Username = newUser.Username,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                Status = newUser.Status
            };

            
            // Mock lần 1: Kiểm tra Username hoặc Email đã tồn tại
            _unitOfWorkMock
                .Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()))
                .ReturnsAsync((User)null);

            // Mock lần 2: Kiểm tra PhoneNumber đã tồn tại
            _unitOfWorkMock
                .Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()))
                .ReturnsAsync((User)null);

            // Mock việc map từ UserRequest sang User
            _mapperMock
                .Setup(m => m.Map<User>(createUserRequest))
                .Returns(newUser);

            // Mock việc map từ User sang UserResponse
            _mapperMock
                .Setup(m => m.Map<UserResponse>(newUser))
                .Returns(userResponse);

            // Mock hàm Insert và Commit
            _unitOfWorkMock
                .Setup(u => u.GetRepository<User>().InsertAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _userService.CreateUser(createUserRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createUserRequest.Username, result.Username);
            Assert.Equal(createUserRequest.Email, result.Email);
            Assert.Equal(createUserRequest.PhoneNumber, result.PhoneNumber);

            // Verify rằng hàm Insert được gọi 1 lần
            _unitOfWorkMock.Verify(u => u.GetRepository<User>().InsertAsync(It.IsAny<User>()), Times.Once);

            // Verify rằng hàm Commit được gọi 1 lần
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task HardDeleteUser_ShouldDeleteUser_WhenValidId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _unitOfWorkMock.Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.HardDeleteUser(userId);

            // Assert
            Assert.True(result);
            _unitOfWorkMock.Verify(u => u.GetRepository<User>().DeleteAsync(user), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }
        [Fact]
        public async Task DeleteUser_ShouldSoftDeleteUser_WhenValidId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Status = UserEnum.Status.Active.ToString() };

            _unitOfWorkMock.Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(),
                     It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                     It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()))
                 .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.DeleteUser(userId);

            // Assert
            Assert.True(result);
            Assert.Equal(UserEnum.Status.Inactive.ToString(), user.Status);
        }

        [Fact]
        public async Task UpdateUserStatus_ShouldUpdateStatus_WhenValidId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Status = UserEnum.Status.Active.ToString() };

            _unitOfWorkMock.Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(),
         It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
         It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()))
     .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.UpdateUserStatus(userId, UserEnum.Status.Inactive.ToString());

            // Assert
            Assert.True(result);
            Assert.Equal(UserEnum.Status.Inactive.ToString(), user.Status);
        }
        [Fact]
        public async Task GetById_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(),
         It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
         It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.GetById(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetByField_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                PasswordHash = "hashedPassword",
                Role = RoleEnum.Customer.ToString(),
                RegisteredDate = DateTime.UtcNow.AddHours(7),
                Status = UserEnum.Status.Active.ToString()
            };

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                RegisteredDate = user.RegisteredDate,
                Status = user.Status
            };

            // Mock hàm SingleOrDefaultAsync để trả về user giả lập
            _unitOfWorkMock.Setup(u => u.GetRepository<User>().SingleOrDefaultAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()))
                .ReturnsAsync(user);

            // Mock việc map từ User sang UserResponse
            _mapperMock.Setup(m => m.Map<UserResponse>(user)).Returns(userResponse);

            // Act
            var result = await _userService.GetByField("user_name", "testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.PhoneNumber, result.PhoneNumber);
            Assert.Equal(user.Role, result.Role);
            Assert.Equal(user.RegisteredDate, result.RegisteredDate);
            Assert.Equal(user.Status, result.Status);

            // Kiểm tra hàm SingleOrDefaultAsync được gọi đúng 1 lần
            _unitOfWorkMock.Verify(u => u.GetRepository<User>().SingleOrDefaultAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>()), Times.Once);
        }


        [Fact]
        public async Task GetAll_ShouldReturnPaginatedUsers_WhenCalled()
        {
            // Arrange
            var paginatedUsers = new Mock<IPaginate<UserResponse>>();

            _unitOfWorkMock
                .Setup(u => u.GetRepository<User>().GetPagingListAsync<UserResponse>(
                    It.IsAny<Expression<Func<User, UserResponse>>>(),
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                    It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(paginatedUsers.Object);

            // Act
            var result = await _userService.GetAll(null, null, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginatedUsers.Object, result);
        }



    }
}
