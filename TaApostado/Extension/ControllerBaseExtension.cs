using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using TaApostado.DTOs.OutputModels;

namespace TaApostado.Extension
{
    public static class ControllerBaseExtension
    {
        public static string ReadToken(this ControllerBase controller)
        {
            var id = controller.User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Sid).Value;
            return id;
        }

        public static ActionResult ServiceError(this ControllerBase controller, string error, int code)
        {
            return new ObjectResult(new OutputModelResponseError(code, $"{error}"));
        }

        public static ActionResult Error(this ControllerBase controller, ILogger logger, Exception e, object obj, string operation, int code)
        {
            logger.LogError(operation, e, obj);
            return new ObjectResult(new OutputModelResponseError(code,$"{operation} {e.Message}"));
        }
    }
}
