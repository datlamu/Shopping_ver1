using System.Globalization;

namespace Shopping_ver1.Helpers
{
    public static class FormatCurrency
    {
        // Định dạng tiền việt
        public static string ToVnCurrency(decimal price)
        {
            var number = (int)price;
            var formatted = number.ToString("#,0", CultureInfo.InvariantCulture)
                                  .Replace(",", ".");

            return $"{formatted} đ";
        }
    }
}