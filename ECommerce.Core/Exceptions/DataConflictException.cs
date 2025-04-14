using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Exceptions
{
    public class DataConflictException : Exception
    {
        public DataConflictException() : base("Data conflicts") { }
        public DataConflictException(string message) : base(message) { }
        public DataConflictException(string message, Exception innerException) : base(message, innerException) { }
    }
}
