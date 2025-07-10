using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Card.Service.Models
{
    public class ErrorResponse
    {
        public string Message {get;set;} = string.Empty;
        public int? ErrorCode {get;set;}
    }
}