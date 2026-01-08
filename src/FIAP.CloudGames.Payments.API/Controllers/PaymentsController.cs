using FIAP.CloudGames.Payments.Application.Dtos;
using FIAP.CloudGames.Payments.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.CloudGames.Payments.API.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Cria um novo pagamento e publica evento para processamento assíncrono.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto request)
        {
            var createdPayment = await _paymentService.CreatePaymentAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = createdPayment }, createdPayment);
        }

        /// <summary>
        /// Retorna os dados de um pagamento pelo ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var payment = await _paymentService.GetPaymentStatusAsync(id);

            if (payment is null)
                return NotFound();

            return Ok(payment);
        }
    }
}
