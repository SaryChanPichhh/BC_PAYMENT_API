namespace BC.PAYMENT.REPORTS.Services
{
    public class TelegramOtpSender
    {
        private static readonly HttpClient httpClient = new HttpClient();

        // Replace with your bot token and chat ID
        //private const string BotToken = "7623645730:AAH3sToJ_OZoELFyeu0EPO9iQD0EJfgH7Ok";
        //private const string ChatId = "-4973764644"; // e.g., 123456789
        private const string BotToken = "8137207711:AAFE5g4LImc7BnceACgcHIO_2j5D9ih0pP0";
        private const string ChatId = "1290911572"; // e.g., 123456789

        public static async Task SendOtpToTelegram(string otp,string username)
        {
            string message =
                $"🔐 OTP Request\n👤 User: {username}\n🔢 OTP: {otp}\n🕐 Valid for 1 minute";



            string url = $"https://api.telegram.org/bot{BotToken}/sendMessage?chat_id={ChatId}&text={Uri.EscapeDataString(message)}";

            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("OTP sent to Telegram successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to send OTP. Status: {response.StatusCode}");
            }
        }
    }
}
