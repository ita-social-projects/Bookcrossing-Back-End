﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto;

namespace Application.Services.Interfaces
{
    public interface IAphorismService
    {
        /// <summary>
        /// Change current aphorism
        /// </summary>
        /// <returns>Return Task</returns>
        Task MoveToNextAsync();

        /// <summary>
        /// Get current aphorism
        /// </summary>
        /// <returns>Returns Aphorism DTO</returns>
        Task<AphorismDto> GetCurrentAphorismAsync(bool current);

        /// <summary>
        /// Retrieve aphorism by ID
        /// </summary>
        /// <param name="aphorismId">Aphorism ID</param>
        /// <returns>returns Aphorism DTO</returns>
        Task<AphorismDto> GetAphorismByIdAsync(int aphorismId);

        /// <summary>
        /// Retrieve all aphorisms
        /// </summary>
        /// <returns>returns list of Aphorism DTOs</returns>
        Task<List<AphorismDto>> GetAllAphorismsAsync();

        /// <summary>
        /// Update specified aphorism
        /// </summary>
        /// <param name="aphorismDto">Aphorism DTO instance</param>
        /// <returns></returns>
        Task<bool> UpdateAphorismAsync(AphorismDto aphorismDto);

        /// <summary>
        /// Remove aphorism from database
        /// </summary>
        /// <param name="aphorismId">Aphorism ID</param>
        /// <returns>Returns removed aphorism DTO</returns>
        Task<bool> RemoveAphorismAsync(int aphorismId);

        /// <summary>
        /// Create new aphorism and add it into Database
        /// </summary>
        /// <param name="aphorismDto">NewAphorism DTO instance</param>
        /// <returns>Returns created Aphorism DTO </returns>
        Task<AphorismDto> AddAphorismAsync(AphorismDto aphorismDto);
    }
}
