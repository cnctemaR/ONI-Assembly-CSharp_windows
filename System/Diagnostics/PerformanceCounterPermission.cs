using System;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[Serializable]
	public sealed class PerformanceCounterPermission : global::System.Security.Permissions.ResourcePermissionBase
	{
		public PerformanceCounterPermission()
		{
			this.SetUp();
		}

		public PerformanceCounterPermission(PerformanceCounterPermissionEntry[] permissionAccessEntries)
		{
			if (permissionAccessEntries == null)
			{
				throw new ArgumentNullException("permissionAccessEntries");
			}
			this.SetUp();
			this.innerCollection = new PerformanceCounterPermissionEntryCollection(this);
			this.innerCollection.AddRange(permissionAccessEntries);
		}

		public PerformanceCounterPermission(PermissionState state)
			: base(state)
		{
			this.SetUp();
		}

		public PerformanceCounterPermission(PerformanceCounterPermissionAccess permissionAccess, string machineName, string categoryName)
		{
			this.SetUp();
			this.innerCollection = new PerformanceCounterPermissionEntryCollection(this);
			this.innerCollection.Add(new PerformanceCounterPermissionEntry(permissionAccess, machineName, categoryName));
		}

		public PerformanceCounterPermissionEntryCollection PermissionEntries
		{
			get
			{
				if (this.innerCollection == null)
				{
					this.innerCollection = new PerformanceCounterPermissionEntryCollection(this);
				}
				return this.innerCollection;
			}
		}

		private void SetUp()
		{
			base.TagNames = new string[] { "Machine", "Category" };
			base.PermissionAccessType = typeof(PerformanceCounterPermissionAccess);
		}

		internal global::System.Security.Permissions.ResourcePermissionBaseEntry[] GetEntries()
		{
			return base.GetPermissionEntries();
		}

		internal void ClearEntries()
		{
			base.Clear();
		}

		internal void Add(object obj)
		{
			PerformanceCounterPermissionEntry performanceCounterPermissionEntry = obj as PerformanceCounterPermissionEntry;
			base.AddPermissionAccess(performanceCounterPermissionEntry.CreateResourcePermissionBaseEntry());
		}

		internal void Remove(object obj)
		{
			PerformanceCounterPermissionEntry performanceCounterPermissionEntry = obj as PerformanceCounterPermissionEntry;
			base.RemovePermissionAccess(performanceCounterPermissionEntry.CreateResourcePermissionBaseEntry());
		}

		private PerformanceCounterPermissionEntryCollection innerCollection;
	}
}
