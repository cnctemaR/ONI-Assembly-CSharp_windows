using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public class DbEnumerator : IEnumerator
	{
		public DbEnumerator(IDataReader reader)
		{
		}

		public DbEnumerator(IDataReader reader, bool closeReader)
		{
		}

		public object Current
		{
			get
			{
				throw null;
			}
		}

		public bool MoveNext()
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset()
		{
		}
	}
}
