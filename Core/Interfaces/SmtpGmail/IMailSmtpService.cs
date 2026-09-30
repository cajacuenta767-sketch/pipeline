namespace Core.Interfaces.SmtpGmail
{
	public interface IMailSmtpService
	{
		Task SendEmailGmailSmtpAsync(
	   List<string> toEmails,
	   string subject,
	   string content,
	   byte[] fileBytes,
	   string fileName);
	}
}
