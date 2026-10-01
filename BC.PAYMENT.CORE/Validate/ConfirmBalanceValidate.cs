using BC.PAYMENT.CORE.Contracts.Request.ConfirmBalance;
using FluentValidation;

namespace BC.PAYMENT.CORE.Validate;

public class CreateConfirmBalanceValidate : AbstractValidator<CreateConfirmBalanceRequest>
{
    public CreateConfirmBalanceValidate()
    {
        RuleFor(x => x.ConfirmBalanceOwner)
            .NotEmpty().WithMessage("Confirm balance owner is required.");

        RuleFor(x => x.Participants)
            .NotEmpty().WithMessage("Participants is required.");
    }
}

public class UpdateConfirmBalanceValidate : AbstractValidator<UpdateConfirmBalanceRequest>
{
    public UpdateConfirmBalanceValidate()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");

        RuleFor(x => x.ConfirmBalanceOwner)
            .NotEmpty().WithMessage("Confirm balance owner is required.");

        RuleFor(x => x.Participants)
            .NotEmpty().WithMessage("Participants is required.");
    }
}

public class ConfirmBalanceValidate : CreateConfirmBalanceValidate
{
}