using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Reports.Abstractions
{
    public interface IReportFactory
    {
        IReportGenerator Create(ReportFormat format);
    }
}
