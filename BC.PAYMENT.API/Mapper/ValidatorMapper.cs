using BC.PAYMENT.API.Models;
using FluentValidation.Results;

namespace BC.PAYMENT.API.MapperHelper;

public static class ValidatorMapper
{
    public static List<ValidatorResponse> toProperties(this List<ValidationFailure> validationFailure)
    {
        return validationFailure.Select(x=>new ValidatorResponse{errorMessage = x.ErrorMessage,PropertyName = x.PropertyName}).ToList();
    }
    
}