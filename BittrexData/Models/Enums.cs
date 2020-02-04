using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BittrexData
{
	public enum OperationType { Sell, Buy }
	public enum TransactionResult { Success, Failed, Error }

	public enum ActorType { HalfDaily, Daily, Weekly }
}
