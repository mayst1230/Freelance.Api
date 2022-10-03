using Freelance.Api.Exceptions;
using Freelance.Api.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Freelance.Api.Services;

/// <summary>
/// Служба для рассылки писем.
/// </summary>
public class EmailSenderService : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSenderService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Отправка сообщения на почту для восстановления пароля.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <param name="code">Код для восстановления.</param>
    /// <returns>Сообщение отправлено.</returns>
    public bool SendEmailRestorePasswordCode(string email, string code)
    {
        try
        {
            var message = new MailMessage(_configuration["Email:Sender:From"], email)
            {
                Subject = "Восстановление пароля для аккаунта на Freelance",
                Body = $"<h2>Код для восстановления пароля: {code}</h2>",
                IsBodyHtml = true,
            };

            var isParse = int.TryParse(_configuration["Email:Smtp:Port"], out var result);
            if (!isParse)
                throw new ApiException("Ошибка при получении SMTP-порта");

            var smtpClient = new SmtpClient(_configuration["Email:Smtp:Address"], result)
            {
                Credentials = new NetworkCredential(_configuration["Email:Credentials:Email"], _configuration["Email:Credentials:Password"]),
                EnableSsl = true,
            };

            smtpClient.Send(message);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
