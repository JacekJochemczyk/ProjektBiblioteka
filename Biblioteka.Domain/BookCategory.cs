using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{
    public sealed class BookCategory : BaseEntity
    {
                  
        public string Name { get; set; } = null!; // Nazwa kategorii

        // Nawigacja w drugą stronę: jedna kategoria ma wiele książek
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
