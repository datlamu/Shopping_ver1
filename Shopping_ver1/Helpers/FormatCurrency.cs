namespace Shopping_ver1.Helpers
{
    public static class FormatCurrency
    {
        public static string ToVnCurrency(decimal price)
        {
            return string.Format("{0:#,0}", (int)price).Replace(",", ".") + " VNĐ";
        }
    }
}
