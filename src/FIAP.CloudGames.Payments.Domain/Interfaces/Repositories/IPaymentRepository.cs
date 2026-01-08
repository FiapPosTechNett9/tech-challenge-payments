using FIAP.CloudGames.Payments.Domain.Entities;

namespace FIAP.CloudGames.Payments.Domain.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<Payment?> GetByIdAsync(Guid id);
    }
}
