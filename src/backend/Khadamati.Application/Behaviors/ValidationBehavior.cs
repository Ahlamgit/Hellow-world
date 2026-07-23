using FluentValidation;
using Khadamati.Application.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Khadamati.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, IServiceProvider serviceProvider)
    {
        _validators = validators;
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var failures = new List<FluentValidation.Results.ValidationFailure>();

        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            failures.AddRange(results.SelectMany(r => r.Errors).Where(f => f != null)!);
        }

        foreach (var property in typeof(TRequest).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
        {
            if (property.GetIndexParameters().Length > 0) continue;
            if (!ShouldValidateNested(property.PropertyType)) continue;

            var value = property.GetValue(request);
            if (value is null) continue;

            failures.AddRange(await ValidateNestedAsync(value, cancellationToken));
        }

        if (failures.Count != 0)
            throw new Common.ValidationException(failures.Select(f => f.ErrorMessage));

        return await next();
    }

    private static bool ShouldValidateNested(Type type) =>
        type.IsClass && type != typeof(string);

    private async Task<IReadOnlyList<FluentValidation.Results.ValidationFailure>> ValidateNestedAsync(
        object instance, CancellationToken cancellationToken)
    {
        var instanceType = instance.GetType();
        var validatorType = typeof(IValidator<>).MakeGenericType(instanceType);
        if (_serviceProvider.GetService(validatorType) is not IValidator validator)
            return Array.Empty<FluentValidation.Results.ValidationFailure>();

        var contextType = typeof(ValidationContext<>).MakeGenericType(instanceType);
        var context = (IValidationContext)Activator.CreateInstance(contextType, instance)!;
        var result = await validator.ValidateAsync(context, cancellationToken).ConfigureAwait(false);
        return result.Errors;
    }
}
