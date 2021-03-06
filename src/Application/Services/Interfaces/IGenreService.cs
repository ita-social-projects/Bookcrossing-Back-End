﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;

namespace Application.Services.Interfaces
{
    public interface IGenreService
    {
        /// <summary>
        /// Retrieve Genre by ID
        /// </summary>
        /// <param name="genreId">Genre's ID</param>
        /// <returns>returns Genre DTO</returns>
        Task<GenreDto> GetById(int genreId);

        /// <summary>
        /// Retrieve all Genres
        /// </summary>
        /// <returns>returns list of Genres DTOs</returns>
        Task<List<GenreDto>> GetAll();

        /// <summary>
        /// Retrieve Pagination for Genre
        /// </summary>
        /// <param name="fullPaginationQuery">QueryParameters containing page index, pageSize, searchQuery and if it's a first Request</param>
        /// <returns>Returns Pagination with Page result and Total amount of items</returns>
        Task<PaginationDto<GenreDto>> GetAll(FullPaginationQueryParams fullPaginationQuery);

        /// <summary>
        /// Update specified Genre
        /// </summary>
        /// <param name="genreDto">Genre's DTO instance</param>
        /// <returns></returns>
        Task<bool> Update(GenreDto genreDto);

        /// <summary>
        /// Remove genre from database
        /// </summary>
        /// <param name="genreId">Genre's ID</param>
        /// <returns>Returns removed genre's DTO</returns>
        Task<bool> Remove(int genreId);

        /// <summary>
        /// Create new genre and add it into Database
        /// </summary>
        /// <param name="genreDto">NewGenre DTO instance</param>
        /// <returns>Returns created Genre's DTO </returns>
        Task<GenreDto> Add(GenreDto genreDto);
    }
}