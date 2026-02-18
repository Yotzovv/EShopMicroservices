namespace Ordering.Application.Dtos
{
    public record PaymentDto(
        string CardName,
        string CardNumber,
        DateTime ExpirationDate,
        string Cvv,
        int PaymentMethod
    );
}
