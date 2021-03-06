﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.Password;
using Application.Dto.QueryParams;
using Domain.RDBMS.Entities;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {

        Task<List<UserDto>> GetAllUsers();

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        /// 
        Task<PaginationDto<UserDto>> GetAllUsers(FullPaginationQueryParams parameters);

        Task<UserDto> GetById(Expression<Func<User, bool>> predicate);

        Task UpdateUser(UserUpdateDto userDto);

        Task<RegisterDto> AddUser(RegisterDto userRegisterDto);

        Task RemoveUser(int userId);

        Task RecoverDeletedUser(int userId);

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

        Task<bool> ForbidEmailNotification(ForbidEmailDto email);

        public void SendMail(User user, string message);
        public void SendNotificationToUser(int userIdAdmin, string message, string messageUk);
    }
}
