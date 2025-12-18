using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Noificaions
{
    // baza (wymaganie: klasy bazowe) – wspólna logika, np. przechowywanie URL
    public abstract class NotificationActionBase : INotificationAction
    {
        protected NotificationActionBase(string targetUrl)
        {
            TargetUrl = targetUrl;
        }

        public string TargetUrl { get; }
    }
}
