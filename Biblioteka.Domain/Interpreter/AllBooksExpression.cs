using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Interpreter
{
    public sealed class AllBooksExpression : IBookFilterExpression
    {
        public IEnumerable<Book> Interpret(IEnumerable<Book> input) => input;
    }
}
