using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace FIAP.CloudGames.Payments.Infrastructure.Logging;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        // Tenta obter do header; senão, cria um novo
        var correlationId = context.Request.Headers.ContainsKey(CorrelationIdHeader)
            ? context.Request.Headers[CorrelationIdHeader].ToString()
            : Guid.NewGuid().ToString();

        // Adiciona o header à resposta também (opcional, mas útil)
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        // Insere no contexto do Serilog
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}