namespace Application.Dto
{
    public class LocationHomeDto
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public bool IsActive { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int UserId  { get; set; }
    }
}
