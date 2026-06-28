using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	public sealed class SharedPropertyGroup
	{
		internal SharedPropertyGroup(ISharedPropertyGroup propertyGroup)
		{
			this.propertyGroup = propertyGroup;
		}

		public SharedProperty CreateProperty(string name, out bool fExists)
		{
			return new SharedProperty(this.propertyGroup.CreateProperty(name, out fExists));
		}

		public SharedProperty CreatePropertyByPosition(int position, out bool fExists)
		{
			return new SharedProperty(this.propertyGroup.CreatePropertyByPosition(position, out fExists));
		}

		public SharedProperty Property(string name)
		{
			return new SharedProperty(this.propertyGroup.Property(name));
		}

		public SharedProperty PropertyByPosition(int position)
		{
			return new SharedProperty(this.propertyGroup.PropertyByPosition(position));
		}

		private ISharedPropertyGroup propertyGroup;
	}
}
