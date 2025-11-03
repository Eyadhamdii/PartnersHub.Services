using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.InnovationHub.Domain.Common;
using System.Net;
using System.Runtime.Intrinsics.X86;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace PartnersHub.InnovationHub.Apis.Controllers.Base
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ApiBaseController<C> : ControllerBase
    {
        private ILogger<C> logger;
        private IMediator mediator;

        private ILogger<C> _logger => logger ??= HttpContext.RequestServices.GetService<ILogger<C>>();
        private IMediator _mediator => mediator ??= HttpContext.RequestServices.GetService<IMediator>();


        protected async Task<ActionResult<ApiResponse>> Execute<T>(IRequest<T> request, CancellationToken token = default)
        {
            _logger.LogInformation("----- Sending Request: {@Request}", request);

            if (request == null)
                return StatusCode((int)HttpStatusCode.MethodNotAllowed,
                    new ApiResponse((int)HttpStatusCode.MethodNotAllowed, "Error", null, "Invalid request"));

            var result = await _mediator.Send(request, token);

            if (result is Result<Guid> r && r.IsFailure)
            {
                var httpStatusCode = (int)HttpStatusCode.BadRequest; 
                return StatusCode(httpStatusCode, new ApiResponse(httpStatusCode, "Error", null, r.Error));
            }
            if (result is Result<bool> c && c.IsFailure)
            {
                var httpStatusCode = (int)HttpStatusCode.BadRequest;
                return StatusCode(httpStatusCode, new ApiResponse(httpStatusCode, "Error", null, c.Error));
            }

            var response = new ApiResponse(
                (int)HttpStatusCode.OK,
                "Success",
                result,
                null);

            return Ok(response);
        }

        protected async Task<ActionResult<ApiResponse>> Execute(IRequest request, CancellationToken token = default)
        {
            _logger.LogInformation("----- Sending Request: {@Request}", request);

            if (request == null)
                return StatusCode((int)HttpStatusCode.MethodNotAllowed,
                    new ApiResponse((int)HttpStatusCode.MethodNotAllowed, "Error", null, "Invalid request"));

            await _mediator.Send(request, token);

            var response = new ApiResponse(
                (int)HttpStatusCode.OK,
                "Success",
                "Operation completed successfully",
                null);

            return Ok(response);
        }
    }

    public class ApiResponse
    {
        public int HttpCode { get; set; }
        public string Status { get; set; }
        public object Data { get; set; }
        public object Error { get; set; }

        public ApiResponse(int httpCode, string status, object data, object error)
        {
            HttpCode = httpCode;
            Status = status;
            Data = data;
            Error = error;
        }
    }
}