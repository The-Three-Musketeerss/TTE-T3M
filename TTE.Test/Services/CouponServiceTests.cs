using AutoMapper;
using Moq;
using System.Linq.Expressions;
using TTE.Application.DTOs;
using TTE.Application.Services;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Tests.Services
{
    public class CouponServiceTests
    {
        private readonly Mock<IGenericRepository<Coupon>> _mockCouponRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CouponService _service;

        public CouponServiceTests()
        {
            _mockCouponRepo = new Mock<IGenericRepository<Coupon>>();
            _mockMapper = new Mock<IMapper>();
            _service = new CouponService(_mockCouponRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateCoupon_ShouldReturnSuccess_WhenCouponIsNew()
        {
            var request = new CouponRequestDto { Code = "SAVE10", Discount = 10 };
            _mockCouponRepo.Setup(r => r.GetByCondition(
                It.IsAny<Expression<Func<Coupon, bool>>>())).ReturnsAsync((Coupon)null);

            var coupon = new Coupon { Code = request.Code, Discount = request.Discount };
            _mockMapper.Setup(m => m.Map<Coupon>(request)).Returns(coupon);
            _mockCouponRepo.Setup(r => r.Add(coupon)).ReturnsAsync(1);

            // Act
            var result = await _service.CreateCoupon(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Coupon created successfully.", result.Message);
        }

        [Fact]
        public async Task CreateCoupon_ShouldReturnFail_WhenCodeAlreadyExists()
        {
            var request = new CouponRequestDto { Code = "SAVE10", Discount = 10 };
            _mockCouponRepo.Setup(r => r.GetByCondition(
                It.IsAny<Expression<Func<Coupon, bool>>>())).ReturnsAsync(new Coupon());

            // Act
            var result = await _service.CreateCoupon(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Coupon code already exists.", result.Message);
        }

        [Fact]
        public async Task UpdateCoupon_ShouldReturnSuccess_WhenCouponExists()
        {
            int id = 1;
            var request = new CouponRequestDto { Code = "NEWCODE", Discount = 20 };
            var existing = new Coupon { Id = id, Code = "OLD", Discount = 5 };

            _mockCouponRepo.Setup(r => r.GetByCondition(
                It.IsAny<Expression<Func<Coupon, bool>>>())).ReturnsAsync(existing);

            // Act
            var result = await _service.UpdateCoupon(id, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Coupon updated successfully.", result.Message);
            Assert.Equal("NEWCODE", existing.Code);
            Assert.Equal(20, existing.Discount);
        }

        [Fact]
        public async Task UpdateCoupon_ShouldReturnFail_WhenCouponNotFound()
        {
            _mockCouponRepo.Setup(r => r.GetByCondition(
                It.IsAny<Expression<Func<Coupon, bool>>>())).ReturnsAsync((Coupon)null);

            // Act
            var result = await _service.UpdateCoupon(99, new CouponRequestDto());

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Coupon not found.", result.Message);
        }

        [Fact]
        public async Task DeleteCoupon_ShouldReturnSuccess_WhenCouponExists()
        {
            var coupon = new Coupon { Id = 1 };
            _mockCouponRepo.Setup(r => r.GetByCondition(
                It.IsAny<Expression<Func<Coupon, bool>>>())).ReturnsAsync(coupon);

            // Act
            var result = await _service.DeleteCoupon(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Coupon deleted successfully.", result.Message);
        }

        [Fact]
        public async Task DeleteCoupon_ShouldReturnFail_WhenCouponNotFound()
        {
            _mockCouponRepo.Setup(r => r.GetByCondition(
                It.IsAny<Expression<Func<Coupon, bool>>>())).ReturnsAsync((Coupon)null);

            // Act
            var result = await _service.DeleteCoupon(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Coupon not found.", result.Message);
        }
    }
}
