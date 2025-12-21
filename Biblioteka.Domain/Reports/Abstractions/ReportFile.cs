using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Reports.Abstractions
{
    public sealed class ReportFile
    {
        public required string FileName { get; init; }
        public required string ContentType { get; init; }
        public required byte[] Content { get; init; }
    }
}
