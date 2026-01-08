using FIAP.CloudGames.Payments.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Payments.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<Guid> CreatePaymentAsync(CreatePaymentRequestDto dto);
        Task<PaymentStatusDto> GetPaymentStatusAsync(Guid paymentId);
    }
}
