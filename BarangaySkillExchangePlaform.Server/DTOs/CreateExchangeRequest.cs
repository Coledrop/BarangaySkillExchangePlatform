namespace BarangaySkillExchangePlaform.Server.DTOs
{
    public class CreateExchangeRequest
    {
        public int RequesterId { get; set; }
        public int ProviderId { get; set; }
        public int? SkillOfferId { get; set; }
        public int? ServiceRequestId { get; set; }

        public string Message { get; set; } = string.Empty;
        public string ProposedSchedule { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
