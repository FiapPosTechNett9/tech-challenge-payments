using FIAP.CloudGames.Payments.Domain.Entities;
using FIAP.CloudGames.Payments.Domain.Interfaces.Repositories;
using FIAP.CloudGames.Payments.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Payments.Infrastructure.Repositories
{
    public class PaymentRepository(PaymentsDbContext context) : IPaymentRepository
    {
        private readonly PaymentsDbContext _context = context;

        public async Task AddAsync(Payment payment)
        {
            await _context.Set<Payment>().AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Payment>().FindAsync(id);
        }
    }
}
