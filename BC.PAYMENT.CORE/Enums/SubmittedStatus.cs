namespace BC.PAYMENT.CORE.Enums;

public enum SubmittedStatus
{
    [Description("កំពុងរង់ចាំ")] Pending,
    [Description("បានបញ្ចប់")] Completed,
    [Description("ច្រានចោល")] Rejected,
    [Description("បានបោះបង់")] Cancel,
    [Description("បានអនុម័ត")] Approved,
    [Description("បានដាក់ស្នើររួចរាល់")] Submitted
}