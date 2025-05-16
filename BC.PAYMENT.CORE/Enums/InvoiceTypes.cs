using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.Enums
{
    public enum InvoiceTypes
    {
        [Description("វិក្ក័យប័ត្រថ្មី")]
        NewInvoice,
        [Description("វិក្ក័យប័ត្រដូរ")]
        ChangeInvoice,
        [Description("វិក្ក័យប័ត្រចាស់")]
        OldInvoice
    }
}
