using FIAP.CloudGames.Payments.Application.Dtos;
using FIAP.CloudGames.Payments.Application.Interfaces;
using FIAP.CloudGames.Payments.Domain.Entities;
using FIAP.CloudGames.Payments.Domain.Interfaces;
using FIAP.CloudGames.Payments.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Payments.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly IPaymentEventPublisher _eventPublisher;

        public PaymentService(
            IPaymentRepository repository,
            IPaymentEventPublisher eventPublisher)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Guid> CreatePaymentAsync(CreatePaymentRequestDto dto)
        {
            var payment = new Payment(
                dto.OrderId,
                dto.UserId,
                dto.Amount
            );

            await _repository.AddAsync(payment);

            await _eventPublisher.PublishPaymentCreatedAsync(payment);

            return payment.Id;
        }

        public async Task<PaymentStatusDto?> GetPaymentStatusAsync(Guid paymentId)
        {
            var payment = await _repository.GetByIdAsync(paymentId);
            if (payment is null) return null;

            return new PaymentStatusDto
            {
                PaymentId = payment.Id,
                Status = payment.Status.ToString()
            };
        }
    }
}
