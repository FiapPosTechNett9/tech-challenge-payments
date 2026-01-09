using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using System.Text.Json;
using Microsoft.Data.SqlClient;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PaymentsProcessor.Function;

public class Function
{

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {

    }


    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="evnt">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        foreach(var message in evnt.Records)
        {
            await ProcessMessageAsync(message, context);
        }
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        var paymentEvent = JsonSerializer.Deserialize<PaymentCreatedEvent>(message.Body, JsonOptions);

        context.Logger.LogInformation(
            $"[PAYMENT] Processing payment {paymentEvent!.PaymentId} - Amount: {paymentEvent.Amount}");

        // Simula processamento (gateway, antifraude, etc.)
        await Task.Delay(5000);

        var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (string.IsNullOrWhiteSpace(connStr))
            throw new Exception("ConnectionStrings__DefaultConnection não configurada");

        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
        UPDATE Payments
        SET Status = @status
        WHERE Id = @id";
        cmd.Parameters.AddWithValue("@status", 2);
        cmd.Parameters.AddWithValue("@id", paymentEvent.PaymentId);

        var rows = await cmd.ExecuteNonQueryAsync();
        context.Logger.LogInformation($"[PAYMENT] UPDATE rows affected: {rows}");

        if (rows == 0)
        {
            context.Logger.LogWarning($"[PAYMENT] Nenhum registro encontrado para Id={paymentEvent.PaymentId}. Verifique se o payment foi criado nesse mesmo DB.");
        }

        context.Logger.LogInformation(
            $"[PAYMENT] Payment {paymentEvent.PaymentId} processed successfully");
    }

}