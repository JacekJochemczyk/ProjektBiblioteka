using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Interpreter
{
    public sealed class CategoryExpression : IBookFilterExpression
    {
        private readonly int? _categoryId;

        public CategoryExpression(int? categoryId)
        {
            _categoryId = categoryId;
        }

        public IEnumerable<Book> Interpret(IEnumerable<Book> input)
        {
            if (!_categoryId.HasValue)
                return input;

            return input.Where(b => b.BookCategoryId == _categoryId.Value);
        }
    }
}
