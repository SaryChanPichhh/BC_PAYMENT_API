namespace BC.PAYMENT.API.Helper
{
    public static class StringExtension
    {
        public static bool IsByte(this string str)
        {
            var tryParse = byte.TryParse(str, out _);
            return tryParse;
        }

        public static bool IsNumeric(this string str)
        {
            var tryParse = int.TryParse(str, out _);
            return tryParse;
        }

        public static bool IsDecimal(this string str)
        {
            var tryParse = decimal.TryParse(str, out _);
            return tryParse;
        }

        public static bool IsDateTime(this string str)
        {
            var tryParse = DateTime.TryParse(str, out _);
            return tryParse;
        }
    }
}
