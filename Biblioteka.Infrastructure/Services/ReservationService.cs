using Biblioteka.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Infrastructure.Services
{
    /// <summary>
    /// Implementacja serwisu rezerwacji oparta o EF Core i LibraryDbContext.
    /// </summary>
    public sealed class ReservationService : IReservationService
    {
        private readonly LibraryDbContext _db;

        public ReservationService(LibraryDbContext db)
        {
            _db = db;
        }

        public async Task<Reservation?> CreateReservationAsync(
            int bookId,
            string userId,
            TimeSpan duration,
            CancellationToken ct = default)
        {
            // pobierz książkę razem z istniejącymi rezerwacjami
            var book = await _db.Books
                .Include(b => b.Reservations)
                .FirstOrDefaultAsync(b => b.Id == bookId, ct);

            if (book is null)
            {
                // brak książki
                return null;
            }

            // jeśli jest już aktywna rezerwacja -> nie pozwalamy na kolejną
            if (book.Reservations.Any(r => r.IsActive))
            {
                return null;
            }

            // Simple Factory – tworzy poprawnie wypełnioną rezerwację
            var reservation = ReservationFactory.Create(bookId, userId, duration);

            // oznaczamy książkę jako niedostępną + ustawiamy datę rezerwacji
            book.IsAvailable = false;
            book.ReservedUntil = reservation.ReservedUntil;

            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync(ct);

            return reservation;
        }

        public async Task<List<Reservation>> GetUserReservationsAsync(
            string userId,
            CancellationToken ct = default)
        {
            return await _db.Reservations
                .Include(r => r.Book)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct);
        }


        public async Task<List<Reservation>> GetAllReservationsAsync(
            CancellationToken ct = default)
        {
            return await _db.Reservations
                .Include(r => r.Book)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<bool> ChangeStatusAsync(
            int reservationId,
            ReservationStatus newStatus,
            string? cancellationReason = null,
            CancellationToken ct = default)
        {
            var reservation = await _db.Reservations
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == reservationId, ct);

            if (reservation is null)
                return false;

            reservation.Status = newStatus;

            // Zawsze czyścimy powód, chyba że Cancelled
            reservation.CancellationReason = null;

            var book = reservation.Book;

            if (newStatus == ReservationStatus.Cancelled)
            {
                reservation.CancellationReason = cancellationReason;

                if (book is not null)
                {
                    book.IsAvailable = true;
                    book.ReservedUntil = null;
                }
            }
            else if (newStatus == ReservationStatus.Returned)
            {
                if (book is not null)
                {
                    book.IsAvailable = true;
                    book.ReservedUntil = null;
                }
            }
            else if (newStatus == ReservationStatus.PickedUp)
            {
                // wypożyczona -> książka nadal niedostępna
                if (book is not null)
                {
                    book.IsAvailable = false;
                    book.ReservedUntil = null; // już nie "rezerwacja do", tylko wypożyczenie
                }
            }
            else if (newStatus == ReservationStatus.Created || newStatus == ReservationStatus.Prepared)
            {
                // AKTYWNA REZERWACJA -> książka ma być niedostępna i mieć ReservedUntil
                if (book is not null)
                {
                    book.IsAvailable = false;
                    book.ReservedUntil = reservation.ReservedUntil;
                }
            }

            await _db.SaveChangesAsync(ct);
            return true;
        }


        public async Task<bool> DeleteReservationAsync(
        int reservationId,
        CancellationToken ct = default)
        {
            var reservation = await _db.Reservations
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == reservationId, ct);

            if (reservation is null)
                return false;

            // Jeśli usuwamy aktywną rezerwację (Created/Prepared),
            // to upewniamy się, że książka wróci do dostępnych.
            if (reservation.IsActive && reservation.Book is not null)
            {
                reservation.Book.IsAvailable = true;
                reservation.Book.ReservedUntil = null;
            }

            _db.Reservations.Remove(reservation);
            await _db.SaveChangesAsync(ct);

            return true;
        }


    }
}
