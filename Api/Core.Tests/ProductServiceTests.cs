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
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _service = new ProductService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10.0m },
                new Product { Id = 2, Name = "Product 2", Price = 20.0m }
            };
            _mockRepo.Setup(repo => repo.GetAllProductsAsync()).ReturnsAsync(expectedProducts);

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.Equal(expectedProducts.Count, result.Count());
            Assert.Equal(expectedProducts, result);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var expectedProduct = new Product { Id = 1, Name = "Product 1", Price = 10.0m };
            _mockRepo.Setup(repo => repo.GetProductByIdAsync(1)).ReturnsAsync(expectedProduct);

            // Act
            var result = await _service.GetProductByIdAsync(1);

            // Assert
            Assert.Equal(expectedProduct, result);
        }

        [Fact]
        public async Task AddProductAsync_ShouldAddProduct()
        {
            // Arrange
            var productToAdd = new Product { Name = "New Product", Price = 15.0m };
            var addedProduct = new Product { Id = 1, Name = "New Product", Price = 15.0m };
            _mockRepo.Setup(repo => repo.AddProductAsync(productToAdd)).ReturnsAsync(addedProduct);

            // Act
            var result = await _service.AddProductAsync(productToAdd);

            // Assert
            Assert.Equal(addedProduct.Id, result.Id);
            Assert.Equal(productToAdd.Name, result.Name);
            Assert.Equal(productToAdd.Price, result.Price);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateProduct()
        {
            // Arrange
            var productToUpdate = new Product { Id = 1, Name = "Updated Product", Price = 25.0m };
            _mockRepo.Setup(repo => repo.UpdateProductAsync(productToUpdate)).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateProductAsync(productToUpdate);

            // Assert
            _mockRepo.Verify(repo => repo.UpdateProductAsync(productToUpdate), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldDeleteProduct()
        {
            // Arrange
            int productIdToDelete = 1;
            _mockRepo.Setup(repo => repo.DeleteProductAsync(productIdToDelete)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteProductAsync(productIdToDelete);

            // Assert
            _mockRepo.Verify(repo => repo.DeleteProductAsync(productIdToDelete), Times.Once);
        }
    }
}