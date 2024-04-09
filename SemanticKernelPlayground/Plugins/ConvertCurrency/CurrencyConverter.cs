using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelPlayground.Plugins.ConvertCurrency;

class CurrencyConverter
{
    [KernelFunction, Description("Convert an amount from one currency to another")]
    public static string ConvertAmount(
        [Description("The target currency code")] string targetCurrencyCode,
        [Description("The amount to convert")] string amount,
        [Description("The starting currency code")] string baseCurrencyCode)
    {
        var currencyDictionary = Currency.Currencies;

        Currency targetCurrency = currencyDictionary[targetCurrencyCode];
        Currency baseCurrency = currencyDictionary[baseCurrencyCode];

        double amountInUsd = Double.Parse(amount) * baseCurrency.USDPerUnit;
        double result = amountInUsd * targetCurrency.UnitsPerUSD;
        return @$"${amount} {baseCurrencyCode} is approximately 
            {result:F2} {targetCurrencyCode} ({targetCurrency.Name})";
    }
}