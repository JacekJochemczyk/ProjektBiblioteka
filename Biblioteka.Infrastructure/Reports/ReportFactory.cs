using Biblioteka.Domain.Reports.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Infrastructure.Reports
{
    public sealed class ReportFactory : IReportFactory
    {
        private readonly IEnumerable<IReportGenerator> _generators;

        public ReportFactory(IEnumerable<IReportGenerator> generators)
        {
            _generators = generators;
        }

        public IReportGenerator Create(ReportFormat format)
        {
            var gen = _generators.FirstOrDefault(g => g.Format == format);
            if (gen is null)
                throw new InvalidOperationException($"Brak generatora dla formatu: {format}");

            return gen;
        }
    }
}
