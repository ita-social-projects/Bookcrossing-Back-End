﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.Password;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
       
        Task<List<UserDto>> GetAllUsers();
        Task UpdateUser(UserUpdateDto userDto);

        Task RemoveUser(int userId);

        /// <summary>
        /// Sending an email with unique confirmation code for password reset
        /// </summary>
        /// <param name="email">The email the letter will be sent to</param>
        /// <returns></returns>
        Task SendPasswordResetConfirmation(string email);

        /// <summary>
        /// Setting a new password
        /// </summary>
        /// <param name="newPassword">New password</param>
        /// <returns></returns>
        Task ResetPassword(ResetPasswordDto newPassword);

    }
}