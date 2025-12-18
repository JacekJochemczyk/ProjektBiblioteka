using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Noificaions
{
    public sealed class GoToMyReservationsAction : NotificationActionBase
    {
        public GoToMyReservationsAction() : base("/my-reservations") { }
    }
}
