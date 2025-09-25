using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Common
{
    public class Result
    {
        public required string Message { get; set; }
        public int StatusCode { get; set; }

        public static Result Success(string message = "Operation completed successfully", int statusCode = 200)
        {
            return new Result
            {
                Message = message,
                StatusCode = statusCode
            };
        }

        public static Result Failure(string message = "Operation failed", int statusCode = 500)
        {
            return new Result
            {
                Message = message,
                StatusCode = statusCode
            };
        }
    }
}
