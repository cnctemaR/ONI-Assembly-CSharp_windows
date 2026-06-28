using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public class ComponentCollection : ReadOnlyCollectionBase
	{
		public ComponentCollection(IComponent[] components)
		{
			base.InnerList.AddRange(components);
		}

		public virtual IComponent this[int index]
		{
			get
			{
				return (IComponent)base.InnerList[index];
			}
		}

		public virtual IComponent this[string name]
		{
			get
			{
				foreach (object obj in base.InnerList)
				{
					IComponent component = (IComponent)obj;
					if (component.Site != null && component.Site.Name == name)
					{
						return component;
					}
				}
				return null;
			}
		}

		public void CopyTo(IComponent[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}
	}
}
