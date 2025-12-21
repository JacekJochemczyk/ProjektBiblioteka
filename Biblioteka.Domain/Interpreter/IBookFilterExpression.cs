using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Interpreter
{
    public interface IBookFilterExpression
    {
        IEnumerable<Book> Interpret(IEnumerable<Book> input);
    }
}
