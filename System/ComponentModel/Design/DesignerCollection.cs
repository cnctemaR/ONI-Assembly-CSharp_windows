using System;
using System.Collections;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public class DesignerCollection : ICollection, IEnumerable
	{
		public DesignerCollection(IDesignerHost[] designers)
		{
			if (designers != null)
			{
				this.designers = new ArrayList(designers);
				return;
			}
			this.designers = new ArrayList();
		}

		public DesignerCollection(IList designers)
		{
			this.designers = designers;
		}

		public int Count
		{
			get
			{
				return this.designers.Count;
			}
		}

		public virtual IDesignerHost this[int index]
		{
			get
			{
				return (IDesignerHost)this.designers[index];
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.designers.GetEnumerator();
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.designers.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private IList designers;
	}
}
