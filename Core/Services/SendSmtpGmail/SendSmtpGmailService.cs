using Core.Interfaces.SmtpGmail;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace Core.Services.SendSmtpGmail
{
	public class SendSmtpGmailService : IMailSmtpService
	{
		private IConfiguration _configuration;

		public SendSmtpGmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailGmailSmtpAsync(
			List<string> toEmails,
			string subject,
			string content,
			byte[] fileBytes,
			string fileName)
		{
			var GmailSmtpKey = _configuration["accesoGmail"];

			using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
			{
				smtp.Port = 587;
				smtp.Credentials = new System.Net.NetworkCredential(
					"ngsoftsoporte@gmail.com",
					GmailSmtpKey);
				smtp.EnableSsl = true;

				using (MailMessage mail = new MailMessage())
				{
					mail.From = new MailAddress(
						"ngsoftsoporte@gmail.com",
						"RefaNet App");

					foreach (var email in toEmails)
					{
						mail.To.Add(email);
					}

					mail.Subject = subject;
					mail.Body = content;
					mail.IsBodyHtml = true;

					if (fileBytes != null)
					{
						var stream = new MemoryStream(fileBytes);
						var attachment = new Attachment(stream, fileName, "application/pdf");
						mail.Attachments.Add(attachment);
					}

					await smtp.SendMailAsync(mail);
				}
			}
		}
	}
}
