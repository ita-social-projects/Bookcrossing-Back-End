using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Save image in file system
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>returns image name</returns>
        Task<string> UploadImage(IFormFile image);

        /// <summary>
        /// Delete image
        /// </summary>
        /// <param name="imagePath">Image path</param>
        void DeleteImage(string imagePath);
    }
}