using System;
using System.Collections;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbErrorCollection : ICollection, IEnumerable
	{
		internal OleDbErrorCollection()
		{
		}

		public int Count
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public void CopyTo(Array array, int index)
		{
			throw ADP.OleDb();
		}

		public void CopyTo(OleDbError[] array, int index)
		{
			throw ADP.OleDb();
		}

		public IEnumerator GetEnumerator()
		{
			throw ADP.OleDb();
		}

		public OleDbError this[int index]
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw ADP.OleDb();
			}
		}
	}
}
