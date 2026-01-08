using FIAP.CloudGames.Payments.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Payments.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentStatusEnum Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected Payment() { } 

        public Payment(Guid orderId, Guid userId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            Id = Guid.NewGuid();
            OrderId = orderId;
            UserId = userId;
            Amount = amount;
            Status = PaymentStatusEnum.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void Confirm() => Status = PaymentStatusEnum.Confirmed;
        public void Fail() => Status = PaymentStatusEnum.Failed;
    }
}
