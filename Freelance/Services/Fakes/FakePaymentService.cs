using Freelance.Api.Exceptions;
using Freelance.Api.Interfaces;

namespace Freelance.Api.Services.Fakes;

/// <summary>
/// Фейковая служба для работы с платежной системой.
/// </summary>
public class FakePaymentService : IPaymentService
{
    public FakePaymentService()
    {

    }

    public bool Replenishment(string cardNumber, string cvc, decimal amountReplishment)
    {
        if (cardNumber.Length < 16 && cardNumber.Length > 16)
            throw new ApiException("Неверная длина номера карты.");

        if (cvc.Length < 3 && cvc.Length > 3)
            throw new ApiException("Неверная длина CVC-кода.");

        if (amountReplishment < 1.0m)
            throw new ApiException("Минимальная сумма пополнения 1руб.");

        return true;
    }

    public bool Withdrawal(string cardNumber, decimal amountWithdrawal)
    {
        if (cardNumber.Length < 16 && cardNumber.Length > 16)
            throw new ApiException("Неверная длина номера карты.");

        if (amountWithdrawal < 1.0m)
            throw new ApiException("Минимальная сумма списания 1руб.");

        return true;
    }
}
