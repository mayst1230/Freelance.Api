namespace Freelance.Api.Interfaces;

/// <summary>
/// Платежная система.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Пополнение счета пользователя.
    /// </summary>
    /// <param name="cardNumber">Номер карты.</param>
    /// <param name="cvc">CVC-код.</param>
    /// <param name="amountReplishment">Сумма пополнения.</param>
    /// <returns>Операция прошла успешно.</returns>
    bool Replenishment(string cardNumber, string cvc, decimal amountReplishment);

    /// <summary>
    /// Вывод средств со счета пользователя.
    /// </summary>
    /// <param name="cardNumber">Номер карты.</param>
    /// <param name="amountWithdrawal">Сумма для вывода.</param>
    /// <returns>Операция прошла успешно.</returns>
    bool Withdrawal(string cardNumber, decimal amountWithdrawal);
}
