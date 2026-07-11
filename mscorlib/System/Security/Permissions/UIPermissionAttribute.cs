using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class UIPermissionAttribute : CodeAccessSecurityAttribute
	{
		public UIPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public UIPermissionClipboard Clipboard
		{
			get
			{
				return this.clipboard;
			}
			set
			{
				this.clipboard = value;
			}
		}

		public UIPermissionWindow Window
		{
			get
			{
				return this.window;
			}
			set
			{
				this.window = value;
			}
		}

		public override IPermission CreatePermission()
		{
			UIPermission uipermission;
			if (base.Unrestricted)
			{
				uipermission = new UIPermission(PermissionState.Unrestricted);
			}
			else
			{
				uipermission = new UIPermission(this.window, this.clipboard);
			}
			return uipermission;
		}

		private UIPermissionClipboard clipboard;

		private UIPermissionWindow window;
	}
}
