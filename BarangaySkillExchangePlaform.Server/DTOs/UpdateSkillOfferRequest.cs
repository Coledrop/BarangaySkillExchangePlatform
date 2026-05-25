namespace BarangaySkillExchangePlaform.Server.DTOs
{
    public class UpdateSkillOfferRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public string LocationPreference { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }
}
