using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Core.Exceptions
{
    public class ExceptionResponse
    {
        public Guid Id { get; set; }
        public int statusCode { get; set; }
        public string? statusMessage { get; set; }
    }
}
