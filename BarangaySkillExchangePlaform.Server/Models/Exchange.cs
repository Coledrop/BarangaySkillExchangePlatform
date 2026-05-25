namespace BarangaySkillExchangePlaform.Server.Models
{
public class Exchange
{
    public int Id { get; set; }

    public int RequesterId { get; set; }
    public User? Requester { get; set; }

    public int ProviderId { get; set; }
    public User? Provider { get; set; }

    public int? SkillOfferId { get; set; }
    public SkillOffer? SkillOffer { get; set; }

    public int? ServiceRequestId { get; set; }
    public ServiceRequest? ServiceRequest { get; set; }

    public string Message { get; set; } = string.Empty;
    public string ProposedSchedule { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
}
