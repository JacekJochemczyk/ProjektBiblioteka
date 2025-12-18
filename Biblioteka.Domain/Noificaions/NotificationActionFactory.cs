using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Noificaions
{
    public static class NotificationActionFactory
    {
        // Factory Method: zwraca obiekt akcji w zależności od danych z powiadomienia
        public static INotificationAction Create(NotificationType type, NotificationTarget target)
        {
            // priorytet: jeśli target jest ustawiony, to on rządzi
            return target switch
            {
                NotificationTarget.MyReservations => new GoToMyReservationsAction(),
                NotificationTarget.AdminReservations => new GoToAdminReservationsAction(),
                NotificationTarget.Books => new GoToBooksAction(),

                // jeśli target nie ustawiony / nieznany - decyduj po typie
                _ => type switch
                {
                    NotificationType.ReservationCreated => new GoToAdminReservationsAction(),
                    NotificationType.ReservationPrepared => new GoToMyReservationsAction(),
                    NotificationType.ReservationCancelled => new GoToMyReservationsAction(),
                    NotificationType.ReservationPickedUp => new GoToMyReservationsAction(),
                    NotificationType.ReservationReturned => new GoToMyReservationsAction(),

                    // bezpieczny fallback
                    _ => new GoToBooksAction()
                }
            };
        }
    }
}
