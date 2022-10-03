namespace Freelance.Api.Interfaces;

/// <summary>
/// Рассылка писем.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Отправка сообщения на почту для восстановления пароля.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <param name="code">Код для восстановления.</param>
    /// <returns>Сообщение отправлено.</returns>
    bool SendEmailRestorePasswordCode(string email, string code);
}
