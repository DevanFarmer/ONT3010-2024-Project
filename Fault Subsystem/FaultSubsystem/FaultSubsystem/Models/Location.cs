namespace FaultSubsystem.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}
