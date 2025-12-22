using Biblioteka.Domain;
using Biblioteka.Domain.Notificaions;
using Biblioteka.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Infrastructure.Services
{
    public sealed class NotificationService : INotificationService
    {
        private readonly LibraryDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public NotificationService(LibraryDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task AddAsync(string userId, string message, CancellationToken ct = default)
        {
            var n = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _db.Notifications.Add(n);
            await _db.SaveChangesAsync(ct);
        }

        public async Task AddForEmployeesAsync(string message, CancellationToken ct = default)
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");

            foreach (var emp in employees)
            {
                var n = new Notification
                {
                    UserId = emp.Id,
                    Message = message,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
                _db.Notifications.Add(n);
            }

            await _db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<Notification>> GetUnreadAsync(string userId, CancellationToken ct = default)
        {
            return await _db.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Notification>> GetAllAsync(
            string userId,
            CancellationToken ct = default)
        {
            return await _db.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(ct);
        }


        public async Task MarkAllAsReadAsync(string userId, CancellationToken ct = default)
        {
            var notifs = await _db.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync(ct);

            foreach (var n in notifs)
            {
                n.IsRead = true;
            }

            await _db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<Notification>> GetLatestAsync(string userId, int take = 20, CancellationToken ct = default)
        {
            return await _db.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToListAsync(ct);
        }

        public async Task AddAsync(
            string userId,
            string message,
            NotificationType type,
            NotificationTarget target = NotificationTarget.None,
            int? relatedId = null,
            string? targetUrl = null,
            CancellationToken ct = default)
        {
            var n = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Type = type,
                RelatedId = relatedId,
                TargetUrl = targetUrl,
            };

            _db.Notifications.Add(n);
            await _db.SaveChangesAsync(ct);
        }

        public async Task AddForEmployeesAsync(
            string message,
                NotificationType type = NotificationType.General,
            NotificationTarget target = NotificationTarget.None,
            int? relatedId = null,
            string? targetUrl = null,
            CancellationToken ct = default)
        {
            // pobierz userów z roli Employee
            var employees = await _userManager.GetUsersInRoleAsync("Employee");

            if (employees is null || employees.Count == 0)
                return;

            var now = DateTime.UtcNow;

            foreach (var u in employees)
            {
                var n = new Notification
                {
                    UserId = u.Id,
                    Message = message,
                    CreatedAt = now,
                    IsRead = false,
                    Type = type,
                    RelatedId = relatedId,
                    TargetUrl = targetUrl
                };

                _db.Notifications.Add(n);
            }

            await _db.SaveChangesAsync(ct);
        }

    }
}
