﻿namespace Application.Dto.Password
{
    public class ResetPasswordDto
    {
        public string ConfirmationNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
