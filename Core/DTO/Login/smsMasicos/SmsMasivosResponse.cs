namespace Core.DTO.Login.smsMasicos
{
	public class SmsMasivosResponse
	{
		public bool Success { get; set; }
		public string? Message { get; set; }
		public int Status { get; set; }
		public string? Code { get; set; }
		public int TotalMessages { get; set; }
		public List<SmsReference> References { get; set; } = new();
		public decimal Credit { get; set; }
		public int? CampaignId { get; set; }
	}

	public class SmsReference
	{
		public string? Reference { get; set; }
		public string? Number { get; set; }
	}
}
