using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Application.Exceptions
{
    public class DuplicateDataException : Exception
    {
        public DuplicateDataException(string message) : base(message)
        {
        }
    }
}
