using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public class OleDbParameterCollection : DbParameterCollection
	{
		internal OleDbParameterCollection()
		{
		}

		public OleDbParameter Add(OleDbParameter value)
		{
			throw ADP.OleDb();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int Add(object value)
		{
			throw ADP.OleDb();
		}

		public OleDbParameter Add(string parameterName, OleDbType oleDbType)
		{
			throw ADP.OleDb();
		}

		public OleDbParameter Add(string parameterName, OleDbType oleDbType, int size)
		{
			throw ADP.OleDb();
		}

		public OleDbParameter Add(string parameterName, OleDbType oleDbType, int size, string sourceColumn)
		{
			throw ADP.OleDb();
		}

		public OleDbParameter Add(string parameterName, object value)
		{
			throw ADP.OleDb();
		}

		public override void AddRange(Array values)
		{
			throw ADP.OleDb();
		}

		public void AddRange(OleDbParameter[] values)
		{
			throw ADP.OleDb();
		}

		public OleDbParameter AddWithValue(string parameterName, object value)
		{
			throw ADP.OleDb();
		}

		public override void Clear()
		{
			throw ADP.OleDb();
		}

		public bool Contains(OleDbParameter value)
		{
			throw ADP.OleDb();
		}

		public override bool Contains(object value)
		{
			throw ADP.OleDb();
		}

		public override bool Contains(string value)
		{
			throw ADP.OleDb();
		}

		public override void CopyTo(Array array, int index)
		{
			throw ADP.OleDb();
		}

		public void CopyTo(OleDbParameter[] array, int index)
		{
			throw ADP.OleDb();
		}

		public override IEnumerator GetEnumerator()
		{
			throw ADP.OleDb();
		}

		protected override DbParameter GetParameter(int index)
		{
			throw ADP.OleDb();
		}

		protected override DbParameter GetParameter(string parameterName)
		{
			throw ADP.OleDb();
		}

		public int IndexOf(OleDbParameter value)
		{
			throw ADP.OleDb();
		}

		public override int IndexOf(object value)
		{
			throw ADP.OleDb();
		}

		public override int IndexOf(string parameterName)
		{
			throw ADP.OleDb();
		}

		public void Insert(int index, OleDbParameter value)
		{
			throw ADP.OleDb();
		}

		public override void Insert(int index, object value)
		{
			throw ADP.OleDb();
		}

		public void Remove(OleDbParameter value)
		{
			throw ADP.OleDb();
		}

		public override void Remove(object value)
		{
			throw ADP.OleDb();
		}

		public override void RemoveAt(int index)
		{
			throw ADP.OleDb();
		}

		public override void RemoveAt(string parameterName)
		{
			throw ADP.OleDb();
		}

		protected override void SetParameter(int index, DbParameter value)
		{
			throw ADP.OleDb();
		}

		protected override void SetParameter(string parameterName, DbParameter value)
		{
			throw ADP.OleDb();
		}

		public override int Count
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public OleDbParameter this[int index]
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public OleDbParameter this[string parameterName]
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override object SyncRoot
		{
			get
			{
				throw ADP.OleDb();
			}
		}
	}
}
