namespace BC.PAYMENT.API.Helper
{
    public static class TimeCalculatorExtension
    {
        public static string TimeAgo(this DateTime dateTime)
        {
            string result;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = $"{timeSpan.Seconds} វិនាទី";
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ? $"{timeSpan.Minutes} នាទី"
                    :
                    "មួយនាទី";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ? $"{timeSpan.Hours} ម៉ោង"
                    :
                    "មួយម៉ោង";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ? $"{timeSpan.Days} ថ្ងៃ"
                    :
                    "ម្សិលមិញ";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ? $"{timeSpan.Days / 30} ខែ"
                    :
                    "ខែមុន";
            }
            else
            {
                result = timeSpan.Days > 365 ? $"{timeSpan.Days / 365} ឆ្នាំ"
                    :
                    "មួយឆ្នាំ";
            }

            return result;
        }
    }
}
