using Common.DTO.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Query
{
    public class TransactionParameters : QueryStringParameters
    {
        public string? Status { get; set; }
    }
}
