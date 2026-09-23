namespace BC.PAYMENT.CORE.Contracts.Criteria
{
    public class OldInvoiceCriteria
    {
        private string? _t0;
        private string? _t1;
        private string? _t2;
        private string? _t3;
        private string? _t4;
        private string? _t5;
        private string? _t6;
        private string? _t7;
        private string? _t8;
        private string? _t9;

        [JsonIgnore] public string? DbCode { get; set; }

        [JsonIgnore] public DateTime? Date { get; set; }

        public string? FromAccount { get; set; }

        public string? ToAccount { get; set; }

        public string? AccType { get; set; }

        public string? FromAnal { get; set; }

        public string? ToAnal { get; set; }

        public string T0
        {
            get
            {
                if (string.IsNullOrEmpty(_t0)) _t0 = "%";
                return _t0;
            }
            set => _t0 = value;
        }

        public string T1
        {
            get
            {
                if (string.IsNullOrEmpty(_t1)) _t1 = "%";
                return _t1;
            }
            set => _t1 = value;
        }

        public string T2
        {
            get
            {
                if (string.IsNullOrEmpty(_t2)) _t2 = "%";
                return _t2;
            }
            set => _t2 = value;
        }

        public string T3
        {
            get
            {
                if (string.IsNullOrEmpty(_t3)) _t3 = "%";
                return _t3;
            }
            set => _t3 = value;
        }

        public string T4
        {
            get
            {
                if (string.IsNullOrEmpty(_t4)) _t4 = "%";
                return _t4;
            }
            set => _t4 = value;
        }

        public string T5
        {
            get
            {
                if (string.IsNullOrEmpty(_t5)) _t5 = "%";
                return _t5;
            }
            set => _t5 = value;
        }

        public string T6
        {
            get
            {
                if (string.IsNullOrEmpty(_t6)) _t6 = "%";
                return _t6;
            }
            set => _t6 = value;
        }

        public string T7
        {
            get
            {
                if (string.IsNullOrEmpty(_t7)) _t7 = "%";
                return _t7;
            }
            set => _t7 = value;
        }

        public string T8
        {
            get
            {
                if (string.IsNullOrEmpty(_t8)) _t8 = "%";
                return _t8;
            }
            set => _t8 = value;
        }

        public string T9
        {
            get
            {
                if (string.IsNullOrEmpty(_t9)) _t9 = "%";
                return _t9;
            }
            set => _t9 = value;
        }


    }

    public class OldInvoiceCriteriaPost : OldInvoiceCriteria
    {
        public int AccountReceivableId { get; set; }
    }
}
