using Biblioteka.Domain.Notificaions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{
    public interface INotificationService
    {
        
        // Dodaje pojedyncze powiadomienie dla konkretnego użytkownika.
        
        Task AddAsync(string userId, string message, CancellationToken ct = default);

        // Dodaje powiadomienia dla wszystkich użytkowników z rolą "Employee".

        Task AddForEmployeesAsync(string message, CancellationToken ct = default);


        // Zwraca nieprzeczytane powiadomienia danego użytkownika (np. do dzwonka).

        Task<IReadOnlyList<Notification>> GetUnreadAsync(string userId, CancellationToken ct = default);


        // Oznacza wszystkie powiadomienia użytkownika jako przeczytane.

        Task MarkAllAsReadAsync(string userId, CancellationToken ct = default);

        // Zwraca ostatnie powiadomienia (przeczytane i nieprzeczytane) dla użytkownika.
        Task<IReadOnlyList<Notification>> GetLatestAsync(string userId, int take = 20, CancellationToken ct = default);

        Task<IReadOnlyList<Notification>> GetAllAsync(
            string userId,
            CancellationToken ct = default);

        Task AddAsync(
            string userId,
            string message,
            NotificationType type,
            NotificationTarget target = NotificationTarget.None,
            int? relatedId = null,
            string? targetUrl = null,
            CancellationToken ct = default);

        Task AddForEmployeesAsync(
            string message,
            NotificationType type = NotificationType.General,
            NotificationTarget target = NotificationTarget.None,
            int? relatedId = null,
            string? targetUrl = null,
            CancellationToken ct = default);

    }

}
