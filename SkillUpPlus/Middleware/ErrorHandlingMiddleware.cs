using System.Net;
using System.Text.Json;
using Npgsql; 
using SkillUpPlus.Exceptions; 
using System; 
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Http; 
using Microsoft.Extensions.Logging; 

namespace SkillUpPlus.Middleware
{
    /// <summary>
    /// Middleware global
    /// para capturar TODAS as exceções não tratadas da aplicação.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Tenta executar o próximo middleware
                await _next(context);
            }
            catch (Exception ex)
            {
                // Se um erro estourar é capturado aqui
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Pega o Status Code e a Mensagem com base no TIPO da exceção
            var (statusCode, message) = exception switch
            {
                // Exceção de Banco de Dados (ex: falha de conexão)
                NpgsqlException => (HttpStatusCode.ServiceUnavailable, "Estamos com problemas em nosso banco de dados. Tente novamente mais tarde."),

                // Exceção customizada
                NotFoundException => (HttpStatusCode.NotFound, exception.Message),

                // Qualquer outra exceção não esperada
                _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno inesperado."),
            };

            // Loga o erro real (completo) no console do Docker
            _logger.LogError(exception, "Erro capturado pelo Middleware Global: {Message}", exception.Message);

            // Prepara a resposta JSON para o cliente
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new { message });
            await context.Response.WriteAsync(result);
        }
    }
}
