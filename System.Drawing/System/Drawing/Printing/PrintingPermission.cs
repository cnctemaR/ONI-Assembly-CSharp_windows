using System;
using System.Security;
using System.Security.Permissions;

namespace System.Drawing.Printing
{
	[Serializable]
	public sealed class PrintingPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		public PrintingPermission(PrintingPermissionLevel printingLevel)
		{
		}

		public PrintingPermission(PermissionState state)
		{
		}

		public PrintingPermissionLevel Level { get; set; }

		public override IPermission Copy()
		{
			return null;
		}

		public override void FromXml(SecurityElement element)
		{
		}

		public override IPermission Intersect(IPermission target)
		{
			return null;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			return false;
		}

		public bool IsUnrestricted()
		{
			return false;
		}

		public override SecurityElement ToXml()
		{
			return null;
		}

		public override IPermission Union(IPermission target)
		{
			return null;
		}
	}
}
