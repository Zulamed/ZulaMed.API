using FluentValidation;

namespace ZulaMed.VideoConversion.Endpoints;

public class BodyValidationEndpointFilter<T> : IEndpointFilter
{
    private readonly IValidator<T> _validator;

    public BodyValidationEndpointFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = (T?)context.Arguments.SingleOrDefault(x => x is T);
        if (request is null)
            return Results.BadRequest(new
            {
                Error = "Request body is missing"
            });
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.BadRequest(new
            {
                Error = validationResult.Errors.Select(x => x.ErrorMessage)
            });
        return await next(context);
    }
}