using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public sealed class ReflectionPermission : CodeAccessPermission, IUnrestrictedPermission, IBuiltInPermission
	{
		public ReflectionPermission(PermissionState state)
		{
			if (CodeAccessPermission.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				this.flags = ReflectionPermissionFlag.AllFlags;
				return;
			}
			this.flags = ReflectionPermissionFlag.NoFlags;
		}

		public ReflectionPermission(ReflectionPermissionFlag flag)
		{
			this.Flags = flag;
		}

		public ReflectionPermissionFlag Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				if ((value & (ReflectionPermissionFlag.TypeInformation | ReflectionPermissionFlag.MemberAccess | ReflectionPermissionFlag.ReflectionEmit | ReflectionPermissionFlag.RestrictedMemberAccess)) != value)
				{
					throw new ArgumentException(string.Format(Locale.GetText("Invalid flags {0}"), value), "ReflectionPermissionFlag");
				}
				this.flags = value;
			}
		}

		public override IPermission Copy()
		{
			return new ReflectionPermission(this.flags);
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			if (CodeAccessPermission.IsUnrestricted(esd))
			{
				this.flags = ReflectionPermissionFlag.AllFlags;
				return;
			}
			this.flags = ReflectionPermissionFlag.NoFlags;
			string text = esd.Attributes["Flags"] as string;
			if (text.IndexOf("MemberAccess") >= 0)
			{
				this.flags |= ReflectionPermissionFlag.MemberAccess;
			}
			if (text.IndexOf("ReflectionEmit") >= 0)
			{
				this.flags |= ReflectionPermissionFlag.ReflectionEmit;
			}
			if (text.IndexOf("TypeInformation") >= 0)
			{
				this.flags |= ReflectionPermissionFlag.TypeInformation;
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			ReflectionPermission reflectionPermission = this.Cast(target);
			if (reflectionPermission == null)
			{
				return null;
			}
			if (this.IsUnrestricted())
			{
				if (reflectionPermission.Flags == ReflectionPermissionFlag.NoFlags)
				{
					return null;
				}
				return reflectionPermission.Copy();
			}
			else if (reflectionPermission.IsUnrestricted())
			{
				if (this.flags == ReflectionPermissionFlag.NoFlags)
				{
					return null;
				}
				return this.Copy();
			}
			else
			{
				ReflectionPermission reflectionPermission2 = (ReflectionPermission)reflectionPermission.Copy();
				reflectionPermission2.Flags &= this.flags;
				if (reflectionPermission2.Flags != ReflectionPermissionFlag.NoFlags)
				{
					return reflectionPermission2;
				}
				return null;
			}
		}

		public override bool IsSubsetOf(IPermission target)
		{
			ReflectionPermission reflectionPermission = this.Cast(target);
			if (reflectionPermission == null)
			{
				return this.flags == ReflectionPermissionFlag.NoFlags;
			}
			if (this.IsUnrestricted())
			{
				return reflectionPermission.IsUnrestricted();
			}
			return reflectionPermission.IsUnrestricted() || (this.flags & reflectionPermission.Flags) == this.flags;
		}

		public bool IsUnrestricted()
		{
			return this.flags == ReflectionPermissionFlag.AllFlags;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = base.Element(1);
			if (this.IsUnrestricted())
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else if (this.flags == ReflectionPermissionFlag.NoFlags)
			{
				securityElement.AddAttribute("Flags", "NoFlags");
			}
			else if ((this.flags & ReflectionPermissionFlag.AllFlags) == ReflectionPermissionFlag.AllFlags)
			{
				securityElement.AddAttribute("Flags", "AllFlags");
			}
			else
			{
				string text = "";
				if ((this.flags & ReflectionPermissionFlag.MemberAccess) == ReflectionPermissionFlag.MemberAccess)
				{
					text = "MemberAccess";
				}
				if ((this.flags & ReflectionPermissionFlag.ReflectionEmit) == ReflectionPermissionFlag.ReflectionEmit)
				{
					if (text.Length > 0)
					{
						text += ", ";
					}
					text += "ReflectionEmit";
				}
				if ((this.flags & ReflectionPermissionFlag.TypeInformation) == ReflectionPermissionFlag.TypeInformation)
				{
					if (text.Length > 0)
					{
						text += ", ";
					}
					text += "TypeInformation";
				}
				securityElement.AddAttribute("Flags", text);
			}
			return securityElement;
		}

		public override IPermission Union(IPermission other)
		{
			ReflectionPermission reflectionPermission = this.Cast(other);
			if (other == null)
			{
				return this.Copy();
			}
			if (this.IsUnrestricted() || reflectionPermission.IsUnrestricted())
			{
				return new ReflectionPermission(PermissionState.Unrestricted);
			}
			ReflectionPermission reflectionPermission2 = (ReflectionPermission)reflectionPermission.Copy();
			reflectionPermission2.Flags |= this.flags;
			return reflectionPermission2;
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 4;
		}

		private ReflectionPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			ReflectionPermission reflectionPermission = target as ReflectionPermission;
			if (reflectionPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(ReflectionPermission));
			}
			return reflectionPermission;
		}

		private const int version = 1;

		private ReflectionPermissionFlag flags;
	}
}
