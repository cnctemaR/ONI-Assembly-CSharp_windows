using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc
{
	[Editor("Microsoft.VSDesigner.Data.Design.DBParametersEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ListBindable(false)]
	public sealed class OdbcParameterCollection : DbParameterCollection
	{
		internal OdbcParameterCollection()
		{
		}

		public override int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public OdbcParameter this[int index]
		{
			get
			{
				return (OdbcParameter)this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public OdbcParameter this[string parameterName]
		{
			get
			{
				foreach (object obj in this.list)
				{
					OdbcParameter odbcParameter = (OdbcParameter)obj;
					if (odbcParameter.ParameterName.Equals(parameterName))
					{
						return odbcParameter;
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
				return false;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int Add(object value)
		{
			if (!(value is OdbcParameter))
			{
				throw new InvalidCastException("The parameter was not an OdbcParameter.");
			}
			this.Add((OdbcParameter)value);
			return this.IndexOf(value);
		}

		public OdbcParameter Add(OdbcParameter value)
		{
			if (value.Container != null)
			{
				throw new ArgumentException("The OdbcParameter specified in the value parameter is already added to this or another OdbcParameterCollection.");
			}
			if (value.ParameterName == null || value.ParameterName.Length == 0)
			{
				value.ParameterName = "Parameter" + this.nullParamCount;
				this.nullParamCount++;
			}
			value.Container = this;
			this.list.Add(value);
			return value;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Add(String parameterName, Object value) has been deprecated.  Use AddWithValue(String parameterName, Object value).")]
		public OdbcParameter Add(string parameterName, object value)
		{
			return this.Add(new OdbcParameter(parameterName, value));
		}

		public OdbcParameter Add(string parameterName, OdbcType odbcType)
		{
			return this.Add(new OdbcParameter(parameterName, odbcType));
		}

		public OdbcParameter Add(string parameterName, OdbcType odbcType, int size)
		{
			return this.Add(new OdbcParameter(parameterName, odbcType, size));
		}

		public OdbcParameter Add(string parameterName, OdbcType odbcType, int size, string sourceColumn)
		{
			return this.Add(new OdbcParameter(parameterName, odbcType, size, sourceColumn));
		}

		public override void Clear()
		{
			foreach (object obj in this.list)
			{
				OdbcParameter odbcParameter = (OdbcParameter)obj;
				odbcParameter.Container = null;
			}
			this.list.Clear();
		}

		public override bool Contains(object value)
		{
			if (value == null)
			{
				return false;
			}
			if (!(value is OdbcParameter))
			{
				throw new InvalidCastException("The parameter was not an OdbcParameter.");
			}
			return this.Contains(((OdbcParameter)value).ParameterName);
		}

		public override bool Contains(string value)
		{
			if (value == null || value.Length == 0)
			{
				return false;
			}
			string text = value.ToUpper();
			foreach (object obj in this)
			{
				OdbcParameter odbcParameter = (OdbcParameter)obj;
				if (odbcParameter.ParameterName.ToUpper().Equals(text))
				{
					return true;
				}
			}
			return false;
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
			if (value == null)
			{
				return -1;
			}
			if (!(value is OdbcParameter))
			{
				throw new InvalidCastException("The parameter was not an OdbcParameter.");
			}
			return this.list.IndexOf(value);
		}

		public override int IndexOf(string parameterName)
		{
			if (parameterName == null || parameterName.Length == 0)
			{
				return -1;
			}
			string text = parameterName.ToUpper();
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].ParameterName.ToUpper().Equals(text))
				{
					return i;
				}
			}
			return -1;
		}

		public override void Insert(int index, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!(value is OdbcParameter))
			{
				throw new InvalidCastException("The parameter was not an OdbcParameter.");
			}
			this.Insert(index, (OdbcParameter)value);
		}

		public override void Remove(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!(value is OdbcParameter))
			{
				throw new InvalidCastException("The parameter was not an OdbcParameter.");
			}
			this.Remove((OdbcParameter)value);
		}

		public override void RemoveAt(int index)
		{
			if (index >= this.list.Count || index < 0)
			{
				throw new IndexOutOfRangeException(string.Format("Invalid index {0} for this OdbcParameterCollection with count = {1}", index, this.list.Count));
			}
			this[index].Container = null;
			this.list.RemoveAt(index);
		}

		public override void RemoveAt(string parameterName)
		{
			this.RemoveAt(this.IndexOf(parameterName));
		}

		protected override DbParameter GetParameter(string name)
		{
			return this[name];
		}

		protected override DbParameter GetParameter(int index)
		{
			return this[index];
		}

		protected override void SetParameter(string name, DbParameter value)
		{
			this[name] = (OdbcParameter)value;
		}

		protected override void SetParameter(int index, DbParameter value)
		{
			this[index] = (OdbcParameter)value;
		}

		public override void AddRange(Array values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			using (IEnumerator enumerator = values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if ((OdbcParameter)enumerator.Current == null)
					{
						throw new ArgumentNullException("values", "The OdbcParameterCollection only accepts non-null OdbcParameter type objects");
					}
				}
			}
			foreach (object obj in values)
			{
				OdbcParameter odbcParameter = (OdbcParameter)obj;
				this.Add(odbcParameter);
			}
		}

		public void AddRange(OdbcParameter[] values)
		{
			this.AddRange(values);
		}

		public void Insert(int index, OdbcParameter value)
		{
			if (index > this.list.Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "The index must be non-negative and less than or equal to size of the collection");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Container != null)
			{
				throw new ArgumentException("The OdbcParameter is already contained by another collection");
			}
			if (string.IsNullOrEmpty(value.ParameterName))
			{
				value.ParameterName = "Parameter" + this.nullParamCount;
				this.nullParamCount++;
			}
			value.Container = this;
			this.list.Insert(index, value);
		}

		public OdbcParameter AddWithValue(string parameterName, object value)
		{
			if (value == null)
			{
				return this.Add(new OdbcParameter(parameterName, OdbcType.NVarChar));
			}
			return this.Add(new OdbcParameter(parameterName, value));
		}

		public void Remove(OdbcParameter value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Container != this)
			{
				throw new ArgumentException("values", "Attempted to remove an OdbcParameter that is not contained in this OdbcParameterCollection");
			}
			value.Container = null;
			this.list.Remove(value);
		}

		public bool Contains(OdbcParameter value)
		{
			return value != null && value.Container == this && this.Contains(value.ParameterName);
		}

		public int IndexOf(OdbcParameter value)
		{
			if (value == null)
			{
				return -1;
			}
			return this.IndexOf(value);
		}

		public void CopyTo(OdbcParameter[] array, int index)
		{
			this.list.CopyTo(array, index);
		}

		private readonly ArrayList list = new ArrayList();

		private int nullParamCount = 1;
	}
}
