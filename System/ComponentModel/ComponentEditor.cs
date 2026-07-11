using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public abstract class ComponentEditor
	{
		public bool EditComponent(object component)
		{
			return this.EditComponent(null, component);
		}

		public abstract bool EditComponent(ITypeDescriptorContext context, object component);
	}
}
