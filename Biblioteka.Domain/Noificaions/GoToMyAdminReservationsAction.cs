using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Noificaions
{
    public sealed class GoToAdminReservationsAction : NotificationActionBase
    {
        public GoToAdminReservationsAction() : base("/admin/reservations") { }
    }
}
