namespace BC.PAYMENT.CORE.Entities.General
{
    public class BcModels
    {
        public string Code { get; set; }
        public string RecType => Code switch
        {
            "ADJ-" => "M",
            "ADJ+" => "M",
            "OB" => "O",
            "TRAN" => "T",
            _ => ""
        };
        public string Description { get; set; }
        public string MovType => Code switch
        {
            "ADJ-" => "R",
            "ADJ+" => "I",
            "OB" => "O",
            "TRAN" => "T",
            _ => ""
        };
    }
}
