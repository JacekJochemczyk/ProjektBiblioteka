using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Notificaions
{
    public sealed class GoToBooksAction : NotificationActionBase
    {
        public GoToBooksAction() : base("/books") { }
    }
}
