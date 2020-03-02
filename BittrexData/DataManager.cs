using BittrexData.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BittrexData
{
	public class DataManager
	{

		public readonly ActorProvider ActorProvider;
		public readonly CurrencyProvider CurrencyProvider;

		public DataManager()
		{
			this.ActorProvider = new ActorProvider();
			this.CurrencyProvider = new CurrencyProvider();
		}

	}
}
