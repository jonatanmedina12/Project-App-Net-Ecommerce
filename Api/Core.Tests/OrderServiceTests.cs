using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Services;
using Moq;
using Xunit;

namespace Core.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockRepo;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _mockRepo = new Mock<IOrderRepository>();
            _service = new OrderService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var expectedOrders = new List<Order>
            {
                new Order { Id = 1, CustomerEmail = "customer1@example.com", TotalAmount = 100.0m },
                new Order { Id = 2, CustomerEmail = "customer2@example.com", TotalAmount = 200.0m }
            };
            _mockRepo.Setup(repo => repo.GetAllOrdersAsync()).ReturnsAsync(expectedOrders);

            // Act
            var result = await _service.GetAllOrdersAsync();

            // Assert
            Assert.Equal(expectedOrders.Count, result.Count());
            Assert.Equal(expectedOrders, result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var expectedOrder = new Order { Id = 1, CustomerEmail = "customer@example.com", TotalAmount = 100.0m };
            _mockRepo.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync(expectedOrder);

            // Act
            var result = await _service.GetOrderByIdAsync(1);

            // Assert
            Assert.Equal(expectedOrder, result);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrder()
        {
            // Arrange
            var orderToCreate = new Order { CustomerEmail = "newcustomer@example.com", TotalAmount = 150.0m };
            var createdOrder = new Order { Id = 1, CustomerEmail = "newcustomer@example.com", TotalAmount = 150.0m };
            _mockRepo.Setup(repo => repo.CreateOrderAsync(orderToCreate)).ReturnsAsync(createdOrder);

            // Act
            var result = await _service.CreateOrderAsync(orderToCreate);

            // Assert
            Assert.Equal(createdOrder.Id, result.Id);
            Assert.Equal(orderToCreate.CustomerEmail, result.CustomerEmail);
            Assert.Equal(orderToCreate.TotalAmount, result.TotalAmount);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldUpdateOrder()
        {
            // Arrange
            var orderToUpdate = new Order { Id = 1, CustomerEmail = "updated@example.com", TotalAmount = 250.0m };
            _mockRepo.Setup(repo => repo.UpdateOrderAsync(orderToUpdate)).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateOrderAsync(orderToUpdate);

            // Assert
            _mockRepo.Verify(repo => repo.UpdateOrderAsync(orderToUpdate), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldDeleteOrder()
        {
            // Arrange
            int orderIdToDelete = 1;
            _mockRepo.Setup(repo => repo.DeleteOrderAsync(orderIdToDelete)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteOrderAsync(orderIdToDelete);

            // Assert
            _mockRepo.Verify(repo => repo.DeleteOrderAsync(orderIdToDelete), Times.Once);
        }
    }
}