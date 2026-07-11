using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data
{
	[Editor("Microsoft.VSDesigner.Data.Design.DataViewSettingsCollectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class DataViewSettingCollection : ICollection, IEnumerable
	{
		internal DataViewSettingCollection()
		{
		}

		[Browsable(false)]
		public virtual int Count
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		public bool IsSynchronized
		{
			get
			{
				throw null;
			}
		}

		public virtual DataViewSetting this[DataTable table]
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public virtual DataViewSetting this[int index]
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public virtual DataViewSetting this[string tableName]
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		public object SyncRoot
		{
			get
			{
				throw null;
			}
		}

		public void CopyTo(Array ar, int index)
		{
		}

		public void CopyTo(DataViewSetting[] ar, int index)
		{
		}

		public IEnumerator GetEnumerator()
		{
			throw null;
		}
	}
}
