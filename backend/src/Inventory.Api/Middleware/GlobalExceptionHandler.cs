using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Middleware
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de la clase GlobalExceptionHandler.
        /// </summary>
        /// <param name="logger">Instancia de ILogger utilizada para el registro de excepciones.</param>
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Maneja excepciones no controladas escribiendo una respuesta JSON con detalles del problema y registra el
        /// error.
        /// </summary>
        /// <param name="httpContext">Contexto HTTP actual.</param>
        /// <param name="exception">Excepción no controlada que se produjo.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asincrónica.</param>
        /// <returns>true si la excepción fue manejada.</returns>
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Error no controlado. TraceId: {TraceId}", httpContext.TraceIdentifier);

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Error interno del servidor",
                Detail = "Ocurrió un error inesperado. Intenta nuevamente."
            };

            problem.Extensions["traceId"] = httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}
