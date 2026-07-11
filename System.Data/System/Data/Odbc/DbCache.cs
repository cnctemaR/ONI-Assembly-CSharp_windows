using System;

namespace System.Data.Odbc
{
	internal sealed class DbCache
	{
		internal DbCache(OdbcDataReader record, int count)
		{
			this._count = count;
			this._record = record;
			this._randomaccess = !record.IsBehavior(CommandBehavior.SequentialAccess);
			this._values = new object[count];
			this._isBadValue = new bool[count];
		}

		internal object this[int i]
		{
			get
			{
				if (this._isBadValue[i])
				{
					OverflowException ex = (OverflowException)this.Values[i];
					throw new OverflowException(ex.Message, ex);
				}
				return this.Values[i];
			}
			set
			{
				this.Values[i] = value;
				this._isBadValue[i] = false;
			}
		}

		internal int Count
		{
			get
			{
				return this._count;
			}
		}

		internal void InvalidateValue(int i)
		{
			this._isBadValue[i] = true;
		}

		internal object[] Values
		{
			get
			{
				return this._values;
			}
		}

		internal object AccessIndex(int i)
		{
			object[] values = this.Values;
			if (this._randomaccess)
			{
				for (int j = 0; j < i; j++)
				{
					if (values[j] == null)
					{
						values[j] = this._record.GetValue(j);
					}
				}
			}
			return values[i];
		}

		internal DbSchemaInfo GetSchema(int i)
		{
			if (this._schema == null)
			{
				this._schema = new DbSchemaInfo[this.Count];
			}
			if (this._schema[i] == null)
			{
				this._schema[i] = new DbSchemaInfo();
			}
			return this._schema[i];
		}

		internal void FlushValues()
		{
			int num = this._values.Length;
			for (int i = 0; i < num; i++)
			{
				this._values[i] = null;
			}
		}

		private bool[] _isBadValue;

		private DbSchemaInfo[] _schema;

		private object[] _values;

		private OdbcDataReader _record;

		internal int _count;

		internal bool _randomaccess = true;
	}
}
