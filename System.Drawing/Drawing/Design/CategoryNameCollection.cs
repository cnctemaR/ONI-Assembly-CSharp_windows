using System;
using System.Collections;
using System.Security.Permissions;

namespace System.Drawing.Design
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public sealed class CategoryNameCollection : ReadOnlyCollectionBase
	{
		public CategoryNameCollection(CategoryNameCollection value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			base.InnerList.AddRange(value);
		}

		public CategoryNameCollection(string[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			base.InnerList.AddRange(value);
		}

		public string this[int index]
		{
			get
			{
				return (string)base.InnerList[index];
			}
		}

		public bool Contains(string value)
		{
			return base.InnerList.Contains(value);
		}

		public void CopyTo(string[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}

		public int IndexOf(string value)
		{
			return base.InnerList.IndexOf(value);
		}
	}
}
