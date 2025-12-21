using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Interpreter
{
    public sealed class StatusExpression : IBookFilterExpression
    {
        private readonly string _status; // "all" | "available" | "reserved"

        public StatusExpression(string status)
        {
            _status = status ?? "all";
        }

        public IEnumerable<Book> Interpret(IEnumerable<Book> input)
        {
            return _status switch
            {
                "available" => input.Where(b => b.IsAvailable),
                "reserved" => input.Where(b => !b.IsAvailable),
                "archived" => input.Where(b => b.IsArchived),
                "all" or "" or null => input,
                _ => input
            };
        }
    }
}
