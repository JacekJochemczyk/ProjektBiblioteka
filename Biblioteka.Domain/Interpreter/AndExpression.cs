using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Interpreter
{
    public sealed class AndExpression : IBookFilterExpression
    {
        private readonly IBookFilterExpression _left;
        private readonly IBookFilterExpression _right;

        public AndExpression(IBookFilterExpression left, IBookFilterExpression right)
        {
            _left = left;
            _right = right;
        }

        public IEnumerable<Book> Interpret(IEnumerable<Book> input)
        {
            // najpierw lewy filtr, potem prawy
            return _right.Interpret(_left.Interpret(input));
        }
    }
}
