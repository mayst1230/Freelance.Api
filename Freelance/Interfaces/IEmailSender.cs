using Freelance.Core.Models.Storage;

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

    /// <summary>
    /// Отправка сообщения об измении статуса заказа на почту.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <param name="order">Данные заказа.</param>
    /// <returns>Сообщение отправлено.</returns>
    bool SendEmailStatusOrder(string email, Order order);
}
