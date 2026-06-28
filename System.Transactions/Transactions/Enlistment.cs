using System;

namespace System.Transactions
{
	public class Enlistment
	{
		internal Enlistment()
		{
			this.done = false;
		}

		public void Done()
		{
			this.done = true;
		}

		internal bool done;
	}
}
