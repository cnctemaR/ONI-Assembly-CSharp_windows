using System;
using System.Collections;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[Serializable]
	public class PerformanceCounterPermissionEntryCollection : CollectionBase
	{
		internal PerformanceCounterPermissionEntryCollection(PerformanceCounterPermission owner)
		{
			this.owner = owner;
			ResourcePermissionBaseEntry[] entries = owner.GetEntries();
			if (entries.Length != 0)
			{
				foreach (ResourcePermissionBaseEntry resourcePermissionBaseEntry in entries)
				{
					PerformanceCounterPermissionAccess permissionAccess = (PerformanceCounterPermissionAccess)resourcePermissionBaseEntry.PermissionAccess;
					string text = resourcePermissionBaseEntry.PermissionAccessPath[0];
					string text2 = resourcePermissionBaseEntry.PermissionAccessPath[1];
					PerformanceCounterPermissionEntry performanceCounterPermissionEntry = new PerformanceCounterPermissionEntry(permissionAccess, text, text2);
					base.InnerList.Add(performanceCounterPermissionEntry);
				}
			}
		}

		internal PerformanceCounterPermissionEntryCollection(ResourcePermissionBaseEntry[] entries)
		{
			foreach (ResourcePermissionBaseEntry resourcePermissionBaseEntry in entries)
			{
				base.List.Add(new PerformanceCounterPermissionEntry((PerformanceCounterPermissionAccess)resourcePermissionBaseEntry.PermissionAccess, resourcePermissionBaseEntry.PermissionAccessPath[0], resourcePermissionBaseEntry.PermissionAccessPath[1]));
			}
		}

		public PerformanceCounterPermissionEntry this[int index]
		{
			get
			{
				return (PerformanceCounterPermissionEntry)base.InnerList[index];
			}
			set
			{
				base.InnerList[index] = value;
			}
		}

		public int Add(PerformanceCounterPermissionEntry value)
		{
			return base.List.Add(value);
		}

		public void AddRange(PerformanceCounterPermissionEntry[] value)
		{
			foreach (PerformanceCounterPermissionEntry performanceCounterPermissionEntry in value)
			{
				base.List.Add(performanceCounterPermissionEntry);
			}
		}

		public void AddRange(PerformanceCounterPermissionEntryCollection value)
		{
			foreach (object obj in value)
			{
				PerformanceCounterPermissionEntry performanceCounterPermissionEntry = (PerformanceCounterPermissionEntry)obj;
				base.List.Add(performanceCounterPermissionEntry);
			}
		}

		public bool Contains(PerformanceCounterPermissionEntry value)
		{
			return base.List.Contains(value);
		}

		public void CopyTo(PerformanceCounterPermissionEntry[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public int IndexOf(PerformanceCounterPermissionEntry value)
		{
			return base.List.IndexOf(value);
		}

		public void Insert(int index, PerformanceCounterPermissionEntry value)
		{
			base.List.Insert(index, value);
		}

		protected override void OnClear()
		{
			this.owner.ClearEntries();
		}

		protected override void OnInsert(int index, object value)
		{
			this.owner.Add(value);
		}

		protected override void OnRemove(int index, object value)
		{
			this.owner.Remove(value);
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			this.owner.Remove(oldValue);
			this.owner.Add(newValue);
		}

		public void Remove(PerformanceCounterPermissionEntry value)
		{
			base.List.Remove(value);
		}

		private PerformanceCounterPermission owner;
	}
}
