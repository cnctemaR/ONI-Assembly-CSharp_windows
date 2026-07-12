using System;
using System.Security;
using System.Security.Permissions;

namespace System.Drawing.Printing
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	[Serializable]
	public sealed class PrintingPermissionAttribute : CodeAccessSecurityAttribute
	{
		public PrintingPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public PrintingPermissionLevel Level { get; set; }

		public override IPermission CreatePermission()
		{
			return null;
		}
	}
}
