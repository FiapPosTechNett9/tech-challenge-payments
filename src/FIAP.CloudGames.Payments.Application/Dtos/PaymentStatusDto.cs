using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Payments.Application.Dtos
{
    public class PaymentStatusDto
    {
        public Guid PaymentId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
