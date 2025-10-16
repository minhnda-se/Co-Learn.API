using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Services.Exceptions
{
    public class BookingConflictException : Exception
    {
        public int StatusCode { get; }

        public BookingConflictException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
