using BookIt.Api.Helpers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
            Type? validatorType = null;
            IValidator? validator = null;

            _logger.LogInformation("Validating request for {Controller}.{Action}", controllerName, actionName);

            foreach (var argument in context.ActionArguments)
            {
                var argumentType = argument.Value?.GetType();

                if (argumentType == null || IsSimpleType(argumentType))
                    continue;

                //in case we have list of objects to validate, we need to validate each item in the list
                if (argumentType.IsGenericType && argumentType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var itemType = argumentType.GetGenericArguments()[0];
                    validatorType = typeof(IValidator<>).MakeGenericType(itemType);
                    validator = _serviceProvider.GetService(validatorType) as IValidator;

                    if (validator != null)
                    {
                        var items = ((System.Collections.IEnumerable)argument.Value).Cast<object>();

                        foreach (var item in items)
                        {
                            var validationContext = new ValidationContext<object>(item);
                            var validationResult = await validator.ValidateAsync(validationContext);

                            if (validationResult.IsValid == false)
                            {
                                ValidationFailed(validationResult.Errors, context, argumentType.Name, controllerName, actionName);
                                continue;
                            }

                            _logger.LogInformation("Validation passed for {Type} in {Controller}.{Action}", argumentType.Name, controllerName, actionName);
                        }

                        if (!context.ModelState.IsValid)
                        {
                            context.Result = ResponseHelper.GenerateErrorResponse(context.ModelState);
                            return;
                        }
                    }

                    continue;
                }

                validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(argument.Value);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (validationResult.IsValid == false)
                    {
                        ValidationFailed(validationResult.Errors, context, argumentType.Name, controllerName, actionName);
                        context.Result = ResponseHelper.GenerateErrorResponse(context.ModelState);
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

        private void ValidationFailed(List<ValidationFailure> validationErrors, ActionExecutingContext context, 
            string argumentTypeName, string controllerName, string actionName )
        {
                _logger.LogWarning("Validation failed for {Type} in {Controller}.{Action}: {Errors}", argumentTypeName, controllerName,
                    actionName, string.Join(", ", validationErrors.Select(e => e.ErrorMessage)));

                foreach (var error in validationErrors)
                {
                    context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

        }
    }
}
