using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException() : base("Violation of professional rules") { }
        public BusinessRuleException(string message) : base(message) { }
        public BusinessRuleException(string message, Exception innerException) : base(message, innerException) { }
    }
}
