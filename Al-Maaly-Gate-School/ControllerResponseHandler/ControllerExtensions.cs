using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.ControllerResponseHandler
{
    public static class ControllerExtensions
    {
        public static async Task<IActionResult> HandleAsync<T>(this ControllerBase controller, Func<Task<ServiceResult<T>>> action) 
        {
            var result = await action();
            if (!result.Success) 
            {
                var errorResponse = ApiResponse<T>.Fail(result.Message!);
                return controller.BadRequest(errorResponse);
            }
            return controller.Ok(ApiResponse<T>.Ok(result.Data!, result.Message));
        }
    }
}
