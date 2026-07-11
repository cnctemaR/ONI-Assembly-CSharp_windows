using System;
using System.Data.ProviderBase;

namespace System.Data.Odbc
{
	internal sealed class OdbcReferenceCollection : DbReferenceCollection
	{
		public override void Add(object value, int tag)
		{
			base.AddItem(value, tag);
		}

		protected override void NotifyItem(int message, int tag, object value)
		{
			if (message != 0)
			{
				if (message == 1 && 1 == tag)
				{
					((OdbcCommand)value).RecoverFromConnection();
					return;
				}
			}
			else if (1 == tag)
			{
				((OdbcCommand)value).CloseFromConnection();
			}
		}

		public override void Remove(object value)
		{
			base.RemoveItem(value);
		}

		internal const int Closing = 0;

		internal const int Recover = 1;

		internal const int CommandTag = 1;
	}
}
