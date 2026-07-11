using System;
using System.Collections;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[Serializable]
	public class EventLogPermissionEntryCollection : CollectionBase
	{
		internal EventLogPermissionEntryCollection(EventLogPermission owner)
		{
			this.owner = owner;
			ResourcePermissionBaseEntry[] entries = owner.GetEntries();
			if (entries.Length != 0)
			{
				foreach (ResourcePermissionBaseEntry resourcePermissionBaseEntry in entries)
				{
					EventLogPermissionEntry eventLogPermissionEntry = new EventLogPermissionEntry((EventLogPermissionAccess)resourcePermissionBaseEntry.PermissionAccess, resourcePermissionBaseEntry.PermissionAccessPath[0]);
					base.InnerList.Add(eventLogPermissionEntry);
				}
			}
		}

		public EventLogPermissionEntry this[int index]
		{
			get
			{
				return (EventLogPermissionEntry)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public int Add(EventLogPermissionEntry value)
		{
			return base.List.Add(value);
		}

		public void AddRange(EventLogPermissionEntry[] value)
		{
			foreach (EventLogPermissionEntry eventLogPermissionEntry in value)
			{
				base.List.Add(eventLogPermissionEntry);
			}
		}

		public void AddRange(EventLogPermissionEntryCollection value)
		{
			foreach (object obj in value)
			{
				EventLogPermissionEntry eventLogPermissionEntry = (EventLogPermissionEntry)obj;
				base.List.Add(eventLogPermissionEntry);
			}
		}

		public bool Contains(EventLogPermissionEntry value)
		{
			return base.List.Contains(value);
		}

		public void CopyTo(EventLogPermissionEntry[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public int IndexOf(EventLogPermissionEntry value)
		{
			return base.List.IndexOf(value);
		}

		public void Insert(int index, EventLogPermissionEntry value)
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

		public void Remove(EventLogPermissionEntry value)
		{
			base.List.Remove(value);
		}

		private EventLogPermission owner;
	}
}
