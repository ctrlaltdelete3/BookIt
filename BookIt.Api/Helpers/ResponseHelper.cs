using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BookIt.Api.Helpers
{
    public static class ResponseHelper
    {
        internal static BadRequestObjectResult GenerateErrorResponse(ModelStateDictionary modelState)
        {
            var response = new BadRequestObjectResult(new
            {
                Message = "Validation failed",
                Errors = modelState
                                    .Where(x => x.Value.Errors.Count > 0)
                                    .ToDictionary(
                                        x => x.Key,
                                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                    )
            });
            return response;
        }
    }
}
