using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbParameterCollection : MarshalByRefObject, IList, IDataParameterCollection, IEnumerable, ICollection
	{
		object IDataParameterCollection.this[string parameterName]
		{
			get
			{
				return this[parameterName];
			}
			set
			{
				this[parameterName] = (DbParameter)value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (DbParameter)value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract int Count { get; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract bool IsFixedSize { get; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public abstract bool IsReadOnly { get; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public abstract bool IsSynchronized { get; }

		public DbParameter this[string parameterName]
		{
			get
			{
				int num = this.IndexOf(parameterName);
				return this[num];
			}
			set
			{
				int num = this.IndexOf(parameterName);
				this[num] = value;
			}
		}

		public DbParameter this[int index]
		{
			get
			{
				return this.GetParameter(index);
			}
			set
			{
				this.SetParameter(index, value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public abstract object SyncRoot { get; }

		public abstract int Add(object value);

		public abstract void AddRange(Array values);

		protected abstract DbParameter GetParameter(string parameterName);

		protected abstract void SetParameter(string parameterName, DbParameter value);

		public abstract void Clear();

		public abstract bool Contains(object value);

		public abstract bool Contains(string value);

		public abstract void CopyTo(Array array, int index);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract IEnumerator GetEnumerator();

		protected abstract DbParameter GetParameter(int index);

		public abstract int IndexOf(object value);

		public abstract int IndexOf(string parameterName);

		public abstract void Insert(int index, object value);

		public abstract void Remove(object value);

		public abstract void RemoveAt(int index);

		public abstract void RemoveAt(string parameterName);

		protected abstract void SetParameter(int index, DbParameter value);
	}
}
