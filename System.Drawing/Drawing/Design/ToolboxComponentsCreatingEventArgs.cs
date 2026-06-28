using System;
using System.ComponentModel.Design;
using System.Security.Permissions;

namespace System.Drawing.Design
{
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class ToolboxComponentsCreatingEventArgs : EventArgs
	{
		public ToolboxComponentsCreatingEventArgs(IDesignerHost host)
		{
			this.host = host;
		}

		public IDesignerHost DesignerHost
		{
			get
			{
				return this.host;
			}
		}

		private IDesignerHost host;
	}
}
