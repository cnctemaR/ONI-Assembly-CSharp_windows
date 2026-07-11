using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(true)]
	[Serializable]
	public class WindowsPrincipal : IPrincipal
	{
		public WindowsPrincipal(WindowsIdentity ntIdentity)
		{
			if (ntIdentity == null)
			{
				throw new ArgumentNullException("ntIdentity");
			}
			this._identity = ntIdentity;
		}

		public virtual IIdentity Identity
		{
			get
			{
				return this._identity;
			}
		}

		public virtual bool IsInRole(int rid)
		{
			if (WindowsPrincipal.IsPosix)
			{
				return WindowsPrincipal.IsMemberOfGroupId(this.Token, (IntPtr)rid);
			}
			string text;
			switch (rid)
			{
			case 544:
				text = "BUILTIN\\Administrators";
				break;
			case 545:
				text = "BUILTIN\\Users";
				break;
			case 546:
				text = "BUILTIN\\Guests";
				break;
			case 547:
				text = "BUILTIN\\Power Users";
				break;
			case 548:
				text = "BUILTIN\\Account Operators";
				break;
			case 549:
				text = "BUILTIN\\System Operators";
				break;
			case 550:
				text = "BUILTIN\\Print Operators";
				break;
			case 551:
				text = "BUILTIN\\Backup Operators";
				break;
			case 552:
				text = "BUILTIN\\Replicator";
				break;
			default:
				return false;
			}
			return this.IsInRole(text);
		}

		public virtual bool IsInRole(string role)
		{
			if (role == null)
			{
				return false;
			}
			if (WindowsPrincipal.IsPosix)
			{
				return WindowsPrincipal.IsMemberOfGroupName(this.Token, role);
			}
			if (this.m_roles == null)
			{
				this.m_roles = WindowsIdentity._GetRoles(this.Token);
			}
			role = role.ToUpperInvariant();
			foreach (string text in this.m_roles)
			{
				if (text != null && role == text.ToUpperInvariant())
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool IsInRole(WindowsBuiltInRole role)
		{
			if (!WindowsPrincipal.IsPosix)
			{
				return this.IsInRole((int)role);
			}
			if (role != WindowsBuiltInRole.Administrator)
			{
				return false;
			}
			string text = "root";
			return this.IsInRole(text);
		}

		[ComVisible(false)]
		[MonoTODO("not implemented")]
		public virtual bool IsInRole(SecurityIdentifier sid)
		{
			throw new NotImplementedException();
		}

		private static bool IsPosix
		{
			get
			{
				int platform = (int)Environment.Platform;
				return platform == 128 || platform == 4 || platform == 6;
			}
		}

		private IntPtr Token
		{
			get
			{
				return this._identity.Token;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsMemberOfGroupId(IntPtr user, IntPtr group);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsMemberOfGroupName(IntPtr user, string group);

		private WindowsIdentity _identity;

		private string[] m_roles;
	}
}
