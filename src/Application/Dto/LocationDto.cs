using System.Collections.Generic;

namespace Application.Dto
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string OfficeName { get; set; }
        public bool? IsActive { get; set; }
        public string HomeAdress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<string> Rooms { get; set; }
    }
}
