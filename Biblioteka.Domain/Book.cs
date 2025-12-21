namespace Biblioteka.Domain
{
    public sealed class Book : BaseEntity
    {
        
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public int YearPublished { get; set; }
        public int? BookCategoryId { get; set; }      
        public BookCategory? Category { get; set; }   // nawigacja do kategorii
        public bool IsAvailable { get; set; } = true;
        public bool IsArchived { get; set; } = false;
        public DateTime? ReservedUntil { get; set; }  // null = nie zarezerwowana
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
