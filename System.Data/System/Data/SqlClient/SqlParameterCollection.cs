using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.SqlClient
{
	[Editor("Microsoft.VSDesigner.Data.Design.DBParametersEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ListBindable(false)]
	public sealed class SqlParameterCollection : DbParameterCollection, ICollection, IEnumerable, IList, IDataParameterCollection
	{
		internal SqlParameterCollection()
		{
		}

		public override int Count
		{
			get
			{
				throw null;
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				throw null;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				throw null;
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SqlParameter this[int index]
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SqlParameter this[string parameterName]
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override object SyncRoot
		{
			get
			{
				throw null;
			}
		}

		public SqlParameter Add(SqlParameter value)
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int Add(object value)
		{
			throw null;
		}

		public SqlParameter Add(string parameterName, SqlDbType sqlDbType)
		{
			throw null;
		}

		public SqlParameter Add(string parameterName, SqlDbType sqlDbType, int size)
		{
			throw null;
		}

		public SqlParameter Add(string parameterName, SqlDbType sqlDbType, int size, string sourceColumn)
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Do not call this method.")]
		public SqlParameter Add(string parameterName, object value)
		{
			throw null;
		}

		public override void AddRange(Array values)
		{
		}

		public void AddRange(SqlParameter[] values)
		{
		}

		public SqlParameter AddWithValue(string parameterName, object value)
		{
			throw null;
		}

		public override void Clear()
		{
		}

		public bool Contains(SqlParameter value)
		{
			throw null;
		}

		public override bool Contains(object value)
		{
			throw null;
		}

		public override bool Contains(string value)
		{
			throw null;
		}

		public override void CopyTo(Array array, int index)
		{
		}

		public void CopyTo(SqlParameter[] array, int index)
		{
		}

		public override IEnumerator GetEnumerator()
		{
			throw null;
		}

		protected override DbParameter GetParameter(int index)
		{
			throw null;
		}

		protected override DbParameter GetParameter(string parameterName)
		{
			throw null;
		}

		public int IndexOf(SqlParameter value)
		{
			throw null;
		}

		public override int IndexOf(object value)
		{
			throw null;
		}

		public override int IndexOf(string parameterName)
		{
			throw null;
		}

		public void Insert(int index, SqlParameter value)
		{
		}

		public override void Insert(int index, object value)
		{
		}

		public void Remove(SqlParameter value)
		{
		}

		public override void Remove(object value)
		{
		}

		public override void RemoveAt(int index)
		{
		}

		public override void RemoveAt(string parameterName)
		{
		}

		protected override void SetParameter(int index, DbParameter value)
		{
		}

		protected override void SetParameter(string parameterName, DbParameter value)
		{
		}
	}
}
