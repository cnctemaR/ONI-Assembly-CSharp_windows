using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data
{
	[Editor("Microsoft.VSDesigner.Data.Design.DataViewSettingsCollectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class DataViewSettingCollection : IEnumerable, ICollection
	{
		internal DataViewSettingCollection(DataViewManager manager)
		{
			this.settingList = new ArrayList();
			if (manager.DataSet != null)
			{
				foreach (object obj in manager.DataSet.Tables)
				{
					DataTable dataTable = (DataTable)obj;
					this.settingList.Add(new DataViewSetting(manager, dataTable));
				}
			}
		}

		[Browsable(false)]
		public virtual int Count
		{
			get
			{
				return this.settingList.Count;
			}
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				return this.settingList.IsReadOnly;
			}
		}

		[Browsable(false)]
		public bool IsSynchronized
		{
			get
			{
				return this.settingList.IsSynchronized;
			}
		}

		public virtual DataViewSetting this[DataTable table]
		{
			get
			{
				for (int i = 0; i < this.settingList.Count; i++)
				{
					DataViewSetting dataViewSetting = (DataViewSetting)this.settingList[i];
					if (dataViewSetting.Table == table)
					{
						return dataViewSetting;
					}
				}
				return null;
			}
			set
			{
				this[table] = value;
			}
		}

		public virtual DataViewSetting this[string tableName]
		{
			get
			{
				for (int i = 0; i < this.settingList.Count; i++)
				{
					DataViewSetting dataViewSetting = (DataViewSetting)this.settingList[i];
					if (dataViewSetting.Table.TableName == tableName)
					{
						return dataViewSetting;
					}
				}
				return null;
			}
		}

		public virtual DataViewSetting this[int index]
		{
			get
			{
				return (DataViewSetting)this.settingList[index];
			}
			set
			{
				this.settingList[index] = value;
			}
		}

		[Browsable(false)]
		public object SyncRoot
		{
			get
			{
				return this.settingList.SyncRoot;
			}
		}

		public void CopyTo(Array ar, int index)
		{
			this.settingList.CopyTo(ar, index);
		}

		public void CopyTo(DataViewSetting[] ar, int index)
		{
			this.settingList.CopyTo(ar, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.settingList.GetEnumerator();
		}

		private readonly ArrayList settingList;
	}
}
