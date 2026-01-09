using FIAP.CloudGames.Payments.Application.Dtos;
using FIAP.CloudGames.Payments.Application.Services;
using FIAP.CloudGames.Payments.Domain.Entities;
using FIAP.CloudGames.Payments.Domain.Interfaces;
using FIAP.CloudGames.Payments.Domain.Interfaces.Repositories;
using Moq;

namespace FIAP.CloudGames.Payments.Tests.Application
{
    public class PaymentServiceTests
    {
        [Fact]
        public async Task CreatePaymentAsync_Should_Save_Payment_And_Publish_Event()
        {
            // Arrange
            var repositoryMock = new Mock<IPaymentRepository>();
            var publisherMock = new Mock<IPaymentEventPublisher>();

            repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Payment>()))
                .Returns(Task.CompletedTask);

            publisherMock
                .Setup(p => p.PublishPaymentCreatedAsync(It.IsAny<Payment>()))
                .Returns(Task.CompletedTask);

            var service = new PaymentService(
                repositoryMock.Object,
                publisherMock.Object
            );

            var dto = new CreatePaymentRequestDto
            {
                OrderId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Amount = 99.90m
            };

            // Act
            var paymentId = await service.CreatePaymentAsync(dto);

            // Assert
            repositoryMock.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Once);
            publisherMock.Verify(p => p.PublishPaymentCreatedAsync(It.IsAny<Payment>()), Times.Once);

            Assert.NotEqual(Guid.Empty, paymentId);
        }

        [Fact]
        public async Task GetPaymentStatusAsync_Should_Return_Status_When_Payment_Exists()
        {
            // Arrange
            var repositoryMock = new Mock<IPaymentRepository>();
            var publisherMock = new Mock<IPaymentEventPublisher>();

            var paymentId = Guid.NewGuid();

            var payment = new Payment(
                Guid.NewGuid(),
                Guid.NewGuid(),
                100m
            );

            repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(payment);

            var service = new PaymentService(
                repositoryMock.Object,
                publisherMock.Object
            );

            // Act
            var result = await service.GetPaymentStatusAsync(paymentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(payment.Id, result.PaymentId);
            Assert.Equal(payment.Status.ToString(), result.Status);

            repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task GetPaymentStatusAsync_Should_Return_Null_When_Payment_Not_Found()
        {
            // Arrange
            var repositoryMock = new Mock<IPaymentRepository>();
            var publisherMock = new Mock<IPaymentEventPublisher>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Payment?)null);

            var service = new PaymentService(
                repositoryMock.Object,
                publisherMock.Object
            );

            // Act
            var result = await service.GetPaymentStatusAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
            repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
