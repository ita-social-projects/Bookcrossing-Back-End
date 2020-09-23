using Application.Dto;
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
        Task<int> Add(LocationHomePostDto locationHomeDto);
        /// <summary>
        /// Retrive all locations in db
        /// </summary>
        /// <returns>Colletion of home locations DTOs from db</returns>
        Task<IEnumerable<LocationHomeDto>> GetAll();
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