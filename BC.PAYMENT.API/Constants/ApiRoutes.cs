using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.API.Constants;

public static class ApiRoutes
{
    private const string Root =  "api";
    private const string Version = "v2";
    public const string Base = $"{Root}/{Version}";
    
    public abstract class Market :BasedRoute
    {
    }
}

public abstract class BasedRoute
{
    public const string Create ="";
    public const string Get ="";
    public const string Delete ="";
    public const string Update ="";
}