namespace BC.PAYMENT.CORE.Enums;

public enum InventoryTrackingTypes
{
    /// <summary>
    /// Symbol pattern (-)
    /// </summary>
    [Description("ស្តុកបូកបញ្ចូល")] Subtract,

    /// <summary>
    /// Symbol pattern (+)
    /// </summary>
    [Description("ស្តុកដកចេញ")] Plus,

    /// <summary>
    /// Symbol pattern (=>)
    /// </summary>
    [Description("ស្តុកផ្ទេរ")] Transfer
}