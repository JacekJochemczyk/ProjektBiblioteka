using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{
   
    // Powiadomienie dla konkretnego użytkownika.
    // Na razie trzymamy prosty tekst + informację, czy zostało odczytane.
    
    public sealed class Notification : BaseEntity
    {
        
        // Identyfikator użytkownika z AspNetUsers (AppUser.Id).
        
        public string UserId { get; set; } = null!;

        
        // Treść powiadomienia (np. "Książka 'Pan Tadeusz' jest gotowa do odbioru").
        
        public string Message { get; set; } = null!;

        
        // Czy powiadomienie zostało odczytane przez użytkownika.
        
        public bool IsRead { get; set; } = false;

        
        // Kiedy powiadomienie zostało utworzone (UTC).
        
        public DateTime CreatedAt { get; set; }
    }
}
