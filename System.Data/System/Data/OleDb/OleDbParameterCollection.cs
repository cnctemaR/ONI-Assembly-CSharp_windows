using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb
{
	[ListBindable(false)]
	[Editor("Microsoft.VSDesigner.Data.Design.DBParametersEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public sealed class OleDbParameterCollection : DbParameterCollection, IList, IDataParameterCollection, IEnumerable, ICollection
	{
		internal OleDbParameterCollection()
		{
		}

		public override int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public OleDbParameter this[int index]
		{
			get
			{
				return (OleDbParameter)this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public OleDbParameter this[string parameterName]
		{
			get
			{
				foreach (object obj in this.list)
				{
					OleDbParameter oleDbParameter = (OleDbParameter)obj;
					if (oleDbParameter.ParameterName.Equals(parameterName))
					{
						return oleDbParameter;
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

		internal IntPtr GdaParameterList
		{
			[MonoTODO]
			get
			{
				return libgda.gda_parameter_list_new();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int Add(object value)
		{
			if (!(value is OleDbParameter))
			{
				throw new InvalidCastException("The parameter was not an OleDbParameter.");
			}
			this.Add((OleDbParameter)value);
			return this.IndexOf(value);
		}

		public OleDbParameter Add(OleDbParameter value)
		{
			if (value.Container != null)
			{
				throw new ArgumentException("The OleDbParameter specified in the value parameter is already added to this or another OleDbParameterCollection.");
			}
			value.Container = this;
			this.list.Add(value);
			return value;
		}

		[Obsolete("OleDbParameterCollection.Add(string, value) is now obsolete. Use OleDbParameterCollection.AddWithValue(string, object) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public OleDbParameter Add(string parameterName, object value)
		{
			return this.Add(new OleDbParameter(parameterName, value));
		}

		public OleDbParameter AddWithValue(string parameterName, object value)
		{
			return this.Add(new OleDbParameter(parameterName, value));
		}

		public OleDbParameter Add(string parameterName, OleDbType oleDbType)
		{
			return this.Add(new OleDbParameter(parameterName, oleDbType));
		}

		public OleDbParameter Add(string parameterName, OleDbType oleDbType, int size)
		{
			return this.Add(new OleDbParameter(parameterName, oleDbType, size));
		}

		public OleDbParameter Add(string parameterName, OleDbType oleDbType, int size, string sourceColumn)
		{
			return this.Add(new OleDbParameter(parameterName, oleDbType, size, sourceColumn));
		}

		public override void AddRange(Array values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			foreach (object obj in values)
			{
				this.Add(obj);
			}
		}

		public void AddRange(OleDbParameter[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			foreach (OleDbParameter oleDbParameter in values)
			{
				this.Add(oleDbParameter);
			}
		}

		public override void Clear()
		{
			foreach (object obj in this.list)
			{
				OleDbParameter oleDbParameter = (OleDbParameter)obj;
				oleDbParameter.Container = null;
			}
			this.list.Clear();
		}

		public override bool Contains(object value)
		{
			if (!(value is OleDbParameter))
			{
				throw new InvalidCastException("The parameter was not an OleDbParameter.");
			}
			return this.Contains(((OleDbParameter)value).ParameterName);
		}

		public override bool Contains(string value)
		{
			foreach (object obj in this.list)
			{
				OleDbParameter oleDbParameter = (OleDbParameter)obj;
				if (oleDbParameter.ParameterName.Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		public bool Contains(OleDbParameter value)
		{
			return this.IndexOf(value) != -1;
		}

		public override void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public void CopyTo(OleDbParameter[] array, int index)
		{
			this.CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		[MonoTODO]
		protected override DbParameter GetParameter(int index)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected override DbParameter GetParameter(string parameterName)
		{
			throw new NotImplementedException();
		}

		public override int IndexOf(object value)
		{
			if (!(value is OleDbParameter))
			{
				throw new InvalidCastException("The parameter was not an OleDbParameter.");
			}
			return this.IndexOf(((OleDbParameter)value).ParameterName);
		}

		public int IndexOf(OleDbParameter value)
		{
			return this.IndexOf(value);
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

		public override void Insert(int index, object value)
		{
			this.list.Insert(index, value);
		}

		public void Insert(int index, OleDbParameter value)
		{
			this.Insert(index, value);
		}

		public override void Remove(object value)
		{
			((OleDbParameter)value).Container = null;
			this.list.Remove(value);
		}

		public void Remove(OleDbParameter value)
		{
			this.Remove(value);
		}

		public override void RemoveAt(int index)
		{
			this[index].Container = null;
			this.list.RemoveAt(index);
		}

		public override void RemoveAt(string parameterName)
		{
			this.RemoveAt(this.IndexOf(parameterName));
		}

		[MonoTODO]
		protected override void SetParameter(int index, DbParameter value)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected override void SetParameter(string parameterName, DbParameter value)
		{
			throw new NotImplementedException();
		}

		private ArrayList list = new ArrayList();
	}
}
