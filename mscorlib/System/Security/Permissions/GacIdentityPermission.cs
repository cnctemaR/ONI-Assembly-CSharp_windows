using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public sealed class GacIdentityPermission : CodeAccessPermission, IBuiltInPermission
	{
		public GacIdentityPermission()
		{
		}

		public GacIdentityPermission(PermissionState state)
		{
			CodeAccessPermission.CheckPermissionState(state, false);
		}

		public override IPermission Copy()
		{
			return new GacIdentityPermission();
		}

		public override IPermission Intersect(IPermission target)
		{
			if (this.Cast(target) == null)
			{
				return null;
			}
			return this.Copy();
		}

		public override bool IsSubsetOf(IPermission target)
		{
			return this.Cast(target) != null;
		}

		public override IPermission Union(IPermission target)
		{
			this.Cast(target);
			return this.Copy();
		}

		public override void FromXml(SecurityElement securityElement)
		{
			CodeAccessPermission.CheckSecurityElement(securityElement, "securityElement", 1, 1);
		}

		public override SecurityElement ToXml()
		{
			return base.Element(1);
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 15;
		}

		private GacIdentityPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			GacIdentityPermission gacIdentityPermission = target as GacIdentityPermission;
			if (gacIdentityPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(GacIdentityPermission));
			}
			return gacIdentityPermission;
		}

		private const int version = 1;
	}
}
