using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookIt.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ValidationFilter> _logger;

        public ValidationFilter(IServiceProvider serviceProvider, ILogger<ValidationFilter> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var controllerName = context.Controller.GetType().Name;
            var actionName = context.ActionDescriptor.DisplayName;

            _logger.LogInformation("Validating request for {Controller}.{Action}", controllerName, actionName);

            foreach (var argument in context.ActionArguments)
            {
                var argumentType = argument.Value?.GetType();

                if (argumentType == null || IsSimpleType(argumentType))
                    continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(argument.Value);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        _logger.LogWarning("Validation failed for {Type} in {Controller}.{Action}: {Errors}", argumentType.Name, controllerName,
                            actionName, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                        foreach (var error in validationResult.Errors)
                        {
                            context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }

                        context.Result = new BadRequestObjectResult(new
                        {
                            Message = "Validation failed",
                            Errors = context.ModelState
                                .Where(x => x.Value.Errors.Count > 0)
                                .ToDictionary(
                                    x => x.Key,
                                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                )
                        });

                        return;
                    }

                    _logger.LogInformation("Validation passed for {Type} in {Controller}.{Action}", argumentType.Name, controllerName, actionName);
                }
            }

            await next();
        }

        private bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan)
                || type == typeof(Guid);
        }
    }
}
