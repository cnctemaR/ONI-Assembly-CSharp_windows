using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class ReflectionPermissionAttribute : CodeAccessSecurityAttribute
	{
		public ReflectionPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public ReflectionPermissionFlag Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
				this.memberAccess = (this.flags & ReflectionPermissionFlag.MemberAccess) == ReflectionPermissionFlag.MemberAccess;
				this.reflectionEmit = (this.flags & ReflectionPermissionFlag.ReflectionEmit) == ReflectionPermissionFlag.ReflectionEmit;
				this.typeInfo = (this.flags & ReflectionPermissionFlag.TypeInformation) == ReflectionPermissionFlag.TypeInformation;
			}
		}

		public bool MemberAccess
		{
			get
			{
				return this.memberAccess;
			}
			set
			{
				if (value)
				{
					this.flags |= ReflectionPermissionFlag.MemberAccess;
				}
				else
				{
					this.flags -= 2;
				}
				this.memberAccess = value;
			}
		}

		[Obsolete]
		public bool ReflectionEmit
		{
			get
			{
				return this.reflectionEmit;
			}
			set
			{
				if (value)
				{
					this.flags |= ReflectionPermissionFlag.ReflectionEmit;
				}
				else
				{
					this.flags -= 4;
				}
				this.reflectionEmit = value;
			}
		}

		public bool RestrictedMemberAccess
		{
			get
			{
				return (this.flags & ReflectionPermissionFlag.RestrictedMemberAccess) == ReflectionPermissionFlag.RestrictedMemberAccess;
			}
			set
			{
				if (value)
				{
					this.flags |= ReflectionPermissionFlag.RestrictedMemberAccess;
					return;
				}
				this.flags -= 8;
			}
		}

		[Obsolete("not enforced in 2.0+")]
		public bool TypeInformation
		{
			get
			{
				return this.typeInfo;
			}
			set
			{
				if (value)
				{
					this.flags |= ReflectionPermissionFlag.TypeInformation;
				}
				else
				{
					this.flags--;
				}
				this.typeInfo = value;
			}
		}

		public override IPermission CreatePermission()
		{
			ReflectionPermission reflectionPermission;
			if (base.Unrestricted)
			{
				reflectionPermission = new ReflectionPermission(PermissionState.Unrestricted);
			}
			else
			{
				reflectionPermission = new ReflectionPermission(this.flags);
			}
			return reflectionPermission;
		}

		private ReflectionPermissionFlag flags;

		private bool memberAccess;

		private bool reflectionEmit;

		private bool typeInfo;
	}
}
