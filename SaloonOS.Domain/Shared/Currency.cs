// Path: SaloonOS.Domain/Shared/Currency.cs
using SaloonOS.Domain.Common;

namespace SaloonOS.Domain.Shared;

/// <summary>
/// A Value Object representing a currency. It ensures that only valid, supported currencies can exist in our domain.
/// </summary>
public class Currency : ValueObject
{
    public string Code { get; private set; } // e.g., "USD", "IRR", "RUB"
    public string Symbol { get; private set; } // e.g., "$", "﷼", "₽"

    private Currency(string code, string symbol)
    {
        Code = code;
        Symbol = symbol;
    }

    // Static instances for our supported currencies
    public static readonly Currency USD = new("USD", "$");
    public static readonly Currency IRR = new("IRR", "﷼");
    public static readonly Currency RUB = new("RUB", "₽");

    private static readonly IReadOnlyCollection<Currency> SupportedCurrencies = new List<Currency> { USD, IRR, RUB };

    public static Currency FromCode(string code)
    {
        return SupportedCurrencies.FirstOrDefault(c => c.Code == code)
            ?? throw new NotSupportedException($"Currency code '{code}' is not supported.");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}

// You would also need to create the Base Class `ValueObject` in `SaloonOS.Domain/Common/`
// to handle the equality logic.