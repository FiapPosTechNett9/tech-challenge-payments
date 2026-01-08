using Amazon.SQS;
using Amazon.SQS.Model;
using FIAP.CloudGames.Payments.Domain.Entities;
using FIAP.CloudGames.Payments.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FIAP.CloudGames.Payments.Infrastructure.Messaging
{
    public class PaymentEventPublisher : IPaymentEventPublisher
    {
        private readonly IAmazonSQS _sqs;
        private readonly string _queueUrl;

        public PaymentEventPublisher(
            IAmazonSQS sqs,
            IConfiguration configuration)
        {
            _sqs = sqs;
            _queueUrl = configuration["Sqs:QueueUrl"]!;
        }

        public async Task PublishPaymentCreatedAsync(Payment payment)
        {
            var message = new
            {
                paymentId = payment.Id,
                orderId = payment.OrderId,
                userId = payment.UserId,
                amount = payment.Amount,
                createdAt = payment.CreatedAt
            };

            var request = new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = JsonSerializer.Serialize(message)
            };

            await _sqs.SendMessageAsync(request);
        }
    }
}
