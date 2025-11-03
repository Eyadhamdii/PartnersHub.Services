using Microsoft.Extensions.Localization;
using PartnersHub.Communities.Apis.Controllers.Base;
using PartnersHub.Communities.Application.Common.Exceptions;
using PartnersHub.Communities.Domain.Resources;
using System.Net;
using System.Text.Json;

namespace PartnersHub.Communities.Apis.Common
{
    public class GlobalExceptionMiddleware(RequestDelegate _next, ILogger<GlobalExceptionMiddleware> _logger,IStringLocalizer<HandlerMessages> localizer )
    {

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

            var lang = context.Request.Headers["Accept-Language"].FirstOrDefault()
                ?? context.Request.Headers["X-Language"].FirstOrDefault()
                ?? "en";

            try
            {
                var culture = new System.Globalization.CultureInfo(lang);
                System.Globalization.CultureInfo.CurrentCulture = culture;
                System.Globalization.CultureInfo.CurrentUICulture = culture;
            }
            catch
            {
                // fallback to English if invalid culture
                System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en");
                System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("en");
            }
            ApiResponse response;
            context.Response.ContentType = "application/json";

            if (exception is UserFriendlyException userEx)
            {
                var message = localizer[userEx.Message];
                var apiError = new ApiResponseError(message, message, userEx.Message);

                response = new ApiResponse((int)userEx.HttpStatusCode, "Error", userEx.CustomData, apiError);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                var message = localizer["MsgUnexpectedError"];
                var apiError = new ApiResponseError(message, message, "MsgUnexpectedError");

                response = new ApiResponse((int)HttpStatusCode.InternalServerError, "Error", null, apiError);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

    }

    public class ApiResponseError
    {
        public string MessageEn { get; set; }
        public string MessageAr { get; set; }
        public string Key { get; set; }

        public ApiResponseError(string messageEn, string messageAr, string key)
        {
            MessageEn = messageEn;
            MessageAr = messageAr;
            Key = key;
        }
    }
}

