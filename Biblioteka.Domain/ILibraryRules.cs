using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{
    public interface ILibraryRules
    {
        DateTime CalculatePickupDeadline(DateTime reservationLocalTime);
        bool IsWorkingDay(DateOnly date);
    }
}
