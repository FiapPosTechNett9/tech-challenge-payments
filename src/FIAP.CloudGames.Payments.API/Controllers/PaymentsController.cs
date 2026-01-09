using FIAP.CloudGames.Payments.Application.Dtos;
using FIAP.CloudGames.Payments.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FIAP.CloudGames.Payments.API.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentsController : ControllerBase
    {
        private static readonly ActivitySource ActivitySource =
            new("payments-service");

        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IPaymentService paymentService,
            ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Cria um novo pagamento e publica evento para processamento assíncrono.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            [FromBody] CreatePaymentRequestDto request)
        {
            using var activity = ActivitySource.StartActivity(
                "CreatePayment",
                ActivityKind.Internal);

            activity?.SetTag("payment.amount", request.Amount);

            _logger.LogInformation(
                "Creating payment for user {UserId} with amount {Amount}",
                request.UserId,
                request.Amount);

            var createdPayment = await _paymentService.CreatePaymentAsync(request);

            activity?.SetTag("payment.id", createdPayment);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdPayment },
                createdPayment);
        }

        /// <summary>
        /// Retorna os dados de um pagamento pelo ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            using var activity = ActivitySource.StartActivity(
                "GetPaymentById",
                ActivityKind.Internal);

            activity?.SetTag("payment.id", id);

            _logger.LogInformation(
                "Fetching payment with id {PaymentId}",
                id);

            var payment = await _paymentService.GetPaymentStatusAsync(id);

            if (payment is null)
            {
                activity?.SetTag("payment.found", false);
                return NotFound();
            }

            activity?.SetTag("payment.found", true);
            activity?.SetTag("payment.status", payment.Status);

            return Ok(payment);
        }
    }
}
