using FIAP.CloudGames.Payments.Domain.Entities;
using FIAP.CloudGames.Payments.Domain.Enums;

namespace FIAP.CloudGames.Payments.Domain.Tests.Entities
{
    public class PaymentTests
    {
        [Fact]
        public void Constructor_Should_Create_Payment_With_Valid_Data()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var amount = 100m;

            var before = DateTime.UtcNow;

            // Act
            var payment = new Payment(orderId, userId, amount);

            var after = DateTime.UtcNow;

            // Assert
            Assert.NotNull(payment);
            Assert.NotEqual(Guid.Empty, payment.Id);

            Assert.Equal(orderId, payment.OrderId);
            Assert.Equal(userId, payment.UserId);
            Assert.Equal(amount, payment.Amount);

            Assert.Equal(PaymentStatusEnum.Pending, payment.Status);

            // CreatedAt deve estar entre before e after
            Assert.True(payment.CreatedAt >= before && payment.CreatedAt <= after);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_Should_Throw_Exception_When_Amount_Is_Invalid(decimal invalidAmount)
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Payment(orderId, userId, invalidAmount)
            );

            Assert.Equal("Amount must be greater than zero", exception.Message);
        }

        [Fact]
        public void Constructor_Should_Set_Status_As_Pending_By_Default()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var amount = 50m;

            // Act
            var payment = new Payment(orderId, userId, amount);

            // Assert
            Assert.Equal(PaymentStatusEnum.Pending, payment.Status);
        }
    }
}
