using Application.Dto;
using FluentValidation;


namespace BookCrossingBackEnd.Validators
{
    public class MessageValidator: AbstractValidator<MessageDto>
    {
        private int _minLength = 1;
        private int _maxLength = 580;
        public MessageValidator()
        {
            RuleFor(x => x.Message).Must(x => x.Length >=_minLength  && x.Length <= _maxLength);
        }
    }
}
