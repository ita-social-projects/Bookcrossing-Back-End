﻿using Application.Dto;
using Application.Dto.QueryParams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ILocationHomeService
    {
        /// <summary>
        /// Create and add to db
        /// </summary>
        /// <param name="locationHomeDto">LocationHomeDto instance</param>
        /// <returns>resurn id of inserted location</returns>
        Task<int> Add(LocationHomeDto locationHomeDto);
        /// <summary>
        /// Retrive all locations in db
        /// </summary>
        /// <returns>Colletion of home locations DTOs from db</returns>
        Task<List<LocationHomeDto>> GetAll();
        /// <summary>
        /// Retrieve Pagination for Genre
        /// </summary>
        /// <param name="fullPaginationQuery">QueryParameters containing page index, pageSize, searchQuery and if it's a first Request</param>
        /// <returns>Returns Pagination with Page result and Total amount of items</returns>
        Task<PaginationDto<LocationHomeDto>> GetAll(FullPaginationQueryParams parameters);
        /// <summary>
        /// Retrieve location by id
        /// </summary>
        /// <param name="locationId">integer of location id</param>
        /// <returns>Location DTO from db</returns>
        Task<LocationHomeDto> GetById(int locationId);
        /// <summary>
        /// Delete from db the location by its id
        /// </summary>
        /// <param name="locationId">integer of location id</param>
        /// <returns>Location DTO from db</returns>
        Task<LocationHomeDto> Remove(int locationId);
        /// <summary>
        /// Updates specified location
        /// </summary>
        /// <param name="locationHomeDto">instance of location</param>
        /// <returns></returns>
        Task Update(LocationHomeDto locationHomeDto);
    }
}