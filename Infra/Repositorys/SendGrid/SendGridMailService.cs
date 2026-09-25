using Core.Interfaces.Fijas;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infra.Repositorys.SendGrid
{
	public class SendGridMailService : IMailService
	{
		private IConfiguration _configuration;

		public SendGridMailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string content)
		{
			var apiKey = _configuration["SendGridAPIKey"];
			var client = new SendGridClient(apiKey);
			var from = new EmailAddress("ngsoftsoporte@gmail.com", "Sistema de cobro de lotes");
			var to = new EmailAddress(toEmail);
			var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
			var response = await client.SendEmailAsync(msg);
		}
	}
}
