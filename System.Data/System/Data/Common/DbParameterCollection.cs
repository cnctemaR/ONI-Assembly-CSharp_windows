using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbParameterCollection : MarshalByRefObject, ICollection, IEnumerable, IList, IDataParameterCollection
	{
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract int Count { get; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract bool IsFixedSize { get; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract bool IsReadOnly { get; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract bool IsSynchronized { get; }

		public DbParameter this[int index]
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public DbParameter this[string parameterName]
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
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract object SyncRoot { get; }

		object IList.this[int index]
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		object IDataParameterCollection.this[string parameterName]
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public abstract int Add(object value);

		public abstract void AddRange(Array values);

		public abstract void Clear();

		public abstract bool Contains(object value);

		public abstract bool Contains(string value);

		public abstract void CopyTo(Array array, int index);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract IEnumerator GetEnumerator();

		protected abstract DbParameter GetParameter(int index);

		protected abstract DbParameter GetParameter(string parameterName);

		public abstract int IndexOf(object value);

		public abstract int IndexOf(string parameterName);

		public abstract void Insert(int index, object value);

		public abstract void Remove(object value);

		public abstract void RemoveAt(int index);

		public abstract void RemoveAt(string parameterName);

		protected abstract void SetParameter(int index, DbParameter value);

		protected abstract void SetParameter(string parameterName, DbParameter value);
	}
}
