using BC.PAYMENT.CORE.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface IAreaRepository
    {
        Task<List<Area>> GetArea(string dbCode);
    }
}
