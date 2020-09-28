using System.Collections.Generic;
using Domain.RDBMS.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class BookPostDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string Publisher { get; set; }
        public BookState State { get; set; }
        public string Notice { get; set; }
        public IFormFile Image { get; set; }
        public int LanguageId { get; set; }
        public string ISBN { get; set; }
        public List<AuthorDto> Authors { get; set; }
        public List<GenreDto> Genres { get; set; }
    }
}
