using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Common
{
    public class Result<T> : Result
    {
        public required T Value { get; set; }
        public static Result<T> Success(T value, string message = "Operation completed successfully", int statusCode = 200)
        {
            return new Result<T>
            {
                Value = value,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static Result<T> Failure(string message = "Operation failed", int statusCode = 500)
        {
            return new Result<T>
            {
                Message = message,
                StatusCode = statusCode,
                Value = default!
            };
        }
    }
}
