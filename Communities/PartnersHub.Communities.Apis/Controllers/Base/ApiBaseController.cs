using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace PartnersHub.Communities.Apis.Controllers.Base
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ApiBaseController<C> : ControllerBase
    {
        private  ILogger<C> logger;
        private  IMediator mediator;

        private ILogger<C> _logger => logger ??= HttpContext.RequestServices.GetService<ILogger<C>>();
        private IMediator _mediator => mediator ??= HttpContext.RequestServices.GetService<IMediator>();


        protected async Task<ActionResult<ApiResponse>> Execute<T>(IRequest<T> request, CancellationToken token = default) 
        {
            _logger.LogInformation("----- Sending Request: {@Request}", request);

            if (request == null)
                return StatusCode((int)HttpStatusCode.MethodNotAllowed,
                    new ApiResponse((int)HttpStatusCode.MethodNotAllowed, "Error", null, "Invalid request"));

            var result = await _mediator.Send(request, token);

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
