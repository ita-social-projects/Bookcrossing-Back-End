using Application.Dto.Password;
using FluentValidation;

namespace BookCrossingBackEnd.Validators
{
    public class PasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        public PasswordValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).Length(5, 25).Equal(customer => customer.PasswordConfirmation);
            RuleFor(x => x.PasswordConfirmation).Length(5, 25).Equal(customer => customer.Password);
        }
    }
}
