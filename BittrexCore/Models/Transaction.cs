using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BittrexCore.Models
{
    public class Transaction
    {
        public Guid Guid;

        public OperationType Type;
        public DateTime Time;
        public TransactionResult TransactionResult;
    }
}
