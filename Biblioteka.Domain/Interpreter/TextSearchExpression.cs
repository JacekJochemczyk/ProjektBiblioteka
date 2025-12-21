using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Interpreter
{
    public sealed class TextSearchExpression : IBookFilterExpression
    {
        private readonly string _term;

        public TextSearchExpression(string term)
        {
            _term = term?.Trim() ?? "";
        }

        public IEnumerable<Book> Interpret(IEnumerable<Book> input)
        {
            if (string.IsNullOrWhiteSpace(_term))
                return input;

            return input.Where(b =>
                (b.Title?.Contains(_term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (b.Author?.Contains(_term, StringComparison.OrdinalIgnoreCase) ?? false));
        }
    }
}
