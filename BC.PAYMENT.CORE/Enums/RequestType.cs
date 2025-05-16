
using System.ComponentModel;

namespace BC.PAYMENT.CORE.Enums
{
    public enum RequestType
    {
        [Description("ជួសជុល")] Repair,
        [Description("ផ្លាស់ប្តូរ")] Exchange,
        [Description("បោះពុម្ភវិក្ក័យបត្រ័")] Invoice
    }
}
