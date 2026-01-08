using FIAP.CloudGames.Payments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Payments.Domain.Interfaces
{
    public interface IPaymentEventPublisher
    {
        Task PublishPaymentCreatedAsync(Payment payment);
    }
}
