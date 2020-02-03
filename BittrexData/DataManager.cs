using BittrexData.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BittrexData
{
	public class DataManager
	{

		public readonly ActorProvider actorProvider;
		public DataManager()
		{
			this.actorProvider = new ActorProvider();
		}

	}
}
