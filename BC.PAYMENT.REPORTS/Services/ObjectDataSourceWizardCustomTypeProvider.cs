using BC.PAYMENT.CORE.Entities;
using DevExpress.DataAccess.Web;

namespace BC.PAYMENT.REPORTS.Services;

public class ObjectDataSourceWizardCustomTypeProvider : IObjectDataSourceWizardTypeProvider
{
    public IEnumerable<Type> GetAvailableTypes(string context)
    {
        return new[] { typeof(Aging) };
    }
}