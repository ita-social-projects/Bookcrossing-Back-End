namespace Domain.RDBMS.Entities
{
    public class LocationHome: IEntityBase
    {
        public int Id { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public bool IsActive { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public virtual User User { get; set; }
    }
}
