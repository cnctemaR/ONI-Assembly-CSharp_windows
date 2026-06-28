using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using Mono.Data.Tds;

namespace System.Data.SqlClient
{
	[Editor("Microsoft.VSDesigner.Data.Design.DBParametersEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ListBindable(false)]
	public sealed class SqlParameterCollection : DbParameterCollection, IList, IDataParameterCollection, IEnumerable, ICollection
	{
		internal SqlParameterCollection(SqlCommand command)
		{
			this.command = command;
			this.metaParameters = new TdsMetaParameterCollection();
		}

		public override int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				return this.list.IsFixedSize;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.list.IsReadOnly;
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		public override object SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SqlParameter this[int index]
		{
			get
			{
				if (index < 0 || index >= this.list.Count)
				{
					throw new IndexOutOfRangeException("The specified index is out of range.");
				}
				return (SqlParameter)this.list[index];
			}
			set
			{
				if (index < 0 || index >= this.list.Count)
				{
					throw new IndexOutOfRangeException("The specified index is out of range.");
				}
				this.list[index] = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SqlParameter this[string parameterName]
		{
			get
			{
				foreach (object obj in this.list)
				{
					SqlParameter sqlParameter = (SqlParameter)obj;
					if (sqlParameter.ParameterName.Equals(parameterName))
					{
						return sqlParameter;
					}
				}
				throw new IndexOutOfRangeException("The specified name does not exist: " + parameterName);
			}
			set
			{
				if (!this.Contains(parameterName))
				{
					throw new IndexOutOfRangeException("The specified name does not exist: " + parameterName);
				}
				this[this.IndexOf(parameterName)] = value;
			}
		}

		protected override DbParameter GetParameter(int index)
		{
			return this[index];
		}

		protected override DbParameter GetParameter(string parameterName)
		{
			return this[parameterName];
		}

		protected override void SetParameter(int index, DbParameter value)
		{
			this[index] = (SqlParameter)value;
		}

		protected override void SetParameter(string parameterName, DbParameter value)
		{
			this[parameterName] = (SqlParameter)value;
		}

		internal TdsMetaParameterCollection MetaParameters
		{
			get
			{
				return this.metaParameters;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int Add(object value)
		{
			if (!(value is SqlParameter))
			{
				throw new InvalidCastException("The parameter was not an SqlParameter.");
			}
			this.Add((SqlParameter)value);
			return this.IndexOf(value);
		}

		public SqlParameter Add(SqlParameter value)
		{
			if (value.Container != null)
			{
				throw new ArgumentException("The SqlParameter specified in the value parameter is already added to this or another SqlParameterCollection.");
			}
			value.Container = this;
			this.list.Add(value);
			this.metaParameters.Add(value.MetaParameter);
			return value;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Do not call this method.")]
		public SqlParameter Add(string parameterName, object value)
		{
			return this.Add(new SqlParameter(parameterName, value));
		}

		public SqlParameter AddWithValue(string parameterName, object value)
		{
			return this.Add(new SqlParameter(parameterName, value));
		}

		public SqlParameter Add(string parameterName, SqlDbType sqlDbType)
		{
			return this.Add(new SqlParameter(parameterName, sqlDbType));
		}

		public SqlParameter Add(string parameterName, SqlDbType sqlDbType, int size)
		{
			return this.Add(new SqlParameter(parameterName, sqlDbType, size));
		}

		public SqlParameter Add(string parameterName, SqlDbType sqlDbType, int size, string sourceColumn)
		{
			return this.Add(new SqlParameter(parameterName, sqlDbType, size, sourceColumn));
		}

		public override void Clear()
		{
			this.metaParameters.Clear();
			foreach (object obj in this.list)
			{
				SqlParameter sqlParameter = (SqlParameter)obj;
				sqlParameter.Container = null;
			}
			this.list.Clear();
		}

		public override bool Contains(object value)
		{
			if (!(value is SqlParameter))
			{
				throw new InvalidCastException("The parameter was not an SqlParameter.");
			}
			return this.Contains(((SqlParameter)value).ParameterName);
		}

		public override bool Contains(string value)
		{
			foreach (object obj in this.list)
			{
				SqlParameter sqlParameter = (SqlParameter)obj;
				if (sqlParameter.ParameterName.Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		public bool Contains(SqlParameter value)
		{
			return this.IndexOf(value) != -1;
		}

		public override void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public override int IndexOf(object value)
		{
			if (!(value is SqlParameter))
			{
				throw new InvalidCastException("The parameter was not an SqlParameter.");
			}
			return this.IndexOf(((SqlParameter)value).ParameterName);
		}

		public override int IndexOf(string parameterName)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].ParameterName.Equals(parameterName))
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf(SqlParameter value)
		{
			return this.list.IndexOf(value);
		}

		public override void Insert(int index, object value)
		{
			this.list.Insert(index, value);
		}

		public void Insert(int index, SqlParameter value)
		{
			this.list.Insert(index, value);
		}

		public override void Remove(object value)
		{
			((SqlParameter)value).Container = null;
			this.metaParameters.Remove(((SqlParameter)value).MetaParameter);
			this.list.Remove(value);
		}

		public void Remove(SqlParameter value)
		{
			value.Container = null;
			this.metaParameters.Remove(value.MetaParameter);
			this.list.Remove(value);
		}

		public override void RemoveAt(int index)
		{
			this[index].Container = null;
			this.metaParameters.RemoveAt(index);
			this.list.RemoveAt(index);
		}

		public override void RemoveAt(string parameterName)
		{
			this.RemoveAt(this.IndexOf(parameterName));
		}

		public override void AddRange(Array values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("The argument passed was null");
			}
			foreach (object obj in values)
			{
				if (!(obj is SqlParameter))
				{
					throw new InvalidCastException("Element in the array parameter was not an SqlParameter.");
				}
				SqlParameter sqlParameter = (SqlParameter)obj;
				if (sqlParameter.Container != null)
				{
					throw new ArgumentException("An SqlParameter specified in the array is already added to this or another SqlParameterCollection.");
				}
				sqlParameter.Container = this;
				this.list.Add(sqlParameter);
				this.metaParameters.Add(sqlParameter.MetaParameter);
			}
		}

		public void AddRange(SqlParameter[] values)
		{
			this.AddRange(values);
		}

		public void CopyTo(SqlParameter[] array, int index)
		{
			this.list.CopyTo(array, index);
		}

		private ArrayList list = new ArrayList();

		private TdsMetaParameterCollection metaParameters;

		private SqlCommand command;
	}
}
