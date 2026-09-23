using System.Reflection;
using BC.PAYMENT.CORE.Entities;
using DevExpress.DataAccess.Web;

namespace BC.PAYMENT.REPORTS.Services;

public class CustomObjectDataSourceConstructorFilterService : IObjectDataSourceConstructorFilterService
{
    public IEnumerable<ConstructorInfo> Filter(Type dataSourceType, IEnumerable<ConstructorInfo> constructors)
    {
        if (dataSourceType == typeof(Aging))
            return constructors;
        else
            return constructors.Where(x => x.GetParameters().Length > 0);
    }
}