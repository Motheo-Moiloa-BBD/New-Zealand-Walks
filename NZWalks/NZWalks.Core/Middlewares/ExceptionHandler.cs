using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NZWalks.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnauthorizedAccessException = NZWalks.Core.Exceptions.UnauthorizedAccessException;

namespace NZWalks.Core.Middlewares
{
    public class ExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> logger;
        private readonly RequestDelegate next;

        public ExceptionHandler(ILogger<ExceptionHandler> logger, RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            ExceptionResponse exceptionResponse = new ExceptionResponse();
            var response = httpContext.Response;
            httpContext.Response.ContentType = "application/json";

            var errorId = Guid.NewGuid();

            exceptionResponse.Id = errorId;

            logger.LogError(ex, $"{errorId} : {ex.Message}");

            switch (ex)
            {
                case ApplicationException:
                    exceptionResponse.statusCode = (int)HttpStatusCode.BadRequest;
                    exceptionResponse.statusMessage = ex.Message;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case BadRequestException:
                    exceptionResponse.statusCode = (int)HttpStatusCode.BadRequest;
                    exceptionResponse.statusMessage = ex.Message;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case UnauthorizedAccessException:
                    exceptionResponse.statusCode = (int)HttpStatusCode.Unauthorized;
                    exceptionResponse.statusMessage = ex.Message;
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case NotFoundException:
                    exceptionResponse.statusCode = (int)HttpStatusCode.NotFound;
                    exceptionResponse.statusMessage = ex.Message;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    exceptionResponse.statusCode = (int)HttpStatusCode.InternalServerError;
                    exceptionResponse.statusMessage = "Internal Server Error, Please retry after a few minutes.";
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;

            }

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(exceptionResponse));
        }
    }
}
