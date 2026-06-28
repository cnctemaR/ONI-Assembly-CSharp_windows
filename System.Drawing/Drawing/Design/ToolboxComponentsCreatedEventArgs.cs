using System;
using System.ComponentModel;
using System.Security.Permissions;

namespace System.Drawing.Design
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class ToolboxComponentsCreatedEventArgs : EventArgs
	{
		public ToolboxComponentsCreatedEventArgs(IComponent[] components)
		{
			this.components = components;
		}

		public IComponent[] Components
		{
			get
			{
				return this.components;
			}
		}

		private IComponent[] components;
	}
}
