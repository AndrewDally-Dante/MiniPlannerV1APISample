using System;
namespace DanteAPI.Entities {
    public class Resource {
        public int ID { get; set; }
        public bool Active { get; set; }
        public string Reference { get; set; }
        public string ImportID { get; set; }
        public string Name { get; set; }
        public short TypeFlags { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string AddressCountryID { get; set; }
        public References.Country AddressCountry { get; set; }
        public References.GenericItem Site { get; set; }
        public string Postcode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public short? Capacity { get; set; }
        public int? BackgroundColour { get; set; }
        public int? ForegroundColour { get; set; }
        public string Notes { get; set; }
        public decimal? Cost { get; set; }
        public byte CostPer { get; set; }
        public string OnlineDisplayName { get; set; }
        public string CustomField { get; set; }
    }
}