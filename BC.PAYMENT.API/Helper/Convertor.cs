namespace BC.PAYMENT.API.Helper;

public static class Convertor
{
    public static double ToDouble(this object value)
    {
        return Convert.ToDouble(value);
    }

    public static int ToInt(this object value)
    {
        return Convert.ToInt32(value);
    }
}