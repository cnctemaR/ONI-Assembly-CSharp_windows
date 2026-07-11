using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[ComVisible(true)]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true)]
	public class ComponentCollection : ReadOnlyCollectionBase
	{
		public ComponentCollection(IComponent[] components)
		{
			base.InnerList.AddRange(components);
		}

		public virtual IComponent this[string name]
		{
			get
			{
				if (name != null)
				{
					foreach (object obj in ((IEnumerable)base.InnerList))
					{
						IComponent component = (IComponent)obj;
						if (component != null && component.Site != null && component.Site.Name != null && string.Equals(component.Site.Name, name, StringComparison.OrdinalIgnoreCase))
						{
							return component;
						}
					}
				}
				return null;
			}
		}

		public virtual IComponent this[int index]
		{
			get
			{
				return (IComponent)base.InnerList[index];
			}
		}

		public void CopyTo(IComponent[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}
	}
}
