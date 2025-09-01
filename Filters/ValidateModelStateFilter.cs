using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AGAMinigameApi.Dtos;
using AGAMinigameApi.Dtos.Common; // Make sure this namespace matches your ApiResponse DTO
namespace AGAMinigameApi.Filters // You can adjust this namespace
{
    public class ValidateModelStateFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                // Extract validation error messages
                var errors = context.ModelState.Where(m => m.Value != null && m.Value.Errors.Any())
                                     .SelectMany(m => m.Value!.Errors.Select(e => e.ErrorMessage))
                                     .ToList();

                // Join multiple errors into a single message or format as needed
                string errorMessage = string.Join(" ", errors);

                // Create your custom ApiResponse for the error
                var apiResponse = new ApiResponse<object>(
                    success: false,
                    message: errorMessage,
                    data: null, // No data on validation failure
                    statusCode: StatusCodes.Status400BadRequest
                );

                // Set the result to return your custom ApiResponse
                context.Result = new BadRequestObjectResult(apiResponse);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after the method execution for this filter
        }
    }
}
