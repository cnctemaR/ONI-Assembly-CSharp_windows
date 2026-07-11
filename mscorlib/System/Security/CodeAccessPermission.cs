using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security
{
	[MonoTODO("CAS support is experimental (and unsupported).")]
	[ComVisible(true)]
	[SecurityPermission(SecurityAction.InheritanceDemand, ControlEvidence = true, ControlPolicy = true)]
	[Serializable]
	public abstract class CodeAccessPermission : IPermission, ISecurityEncodable, IStackWalk
	{
		[SecuritySafeCritical]
		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public void Assert()
		{
			new PermissionSet(this).Assert();
		}

		public abstract IPermission Copy();

		[SecuritySafeCritical]
		public void Demand()
		{
			if (!SecurityManager.SecurityEnabled)
			{
				return;
			}
			new PermissionSet(this).CasOnlyDemand(3);
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		[SecuritySafeCritical]
		[Obsolete("Deny is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		public void Deny()
		{
			new PermissionSet(this).Deny();
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != base.GetType())
			{
				return false;
			}
			CodeAccessPermission codeAccessPermission = obj as CodeAccessPermission;
			return this.IsSubsetOf(codeAccessPermission) && codeAccessPermission.IsSubsetOf(this);
		}

		public abstract void FromXml(SecurityElement elem);

		[ComVisible(false)]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public abstract IPermission Intersect(IPermission target);

		public abstract bool IsSubsetOf(IPermission target);

		public override string ToString()
		{
			return this.ToXml().ToString();
		}

		public abstract SecurityElement ToXml();

		public virtual IPermission Union(IPermission other)
		{
			if (other != null)
			{
				throw new NotSupportedException();
			}
			return null;
		}

		[SecuritySafeCritical]
		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public void PermitOnly()
		{
			new PermissionSet(this).PermitOnly();
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertAll()
		{
			if (!SecurityManager.SecurityEnabled)
			{
				return;
			}
			throw new NotImplementedException();
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertAssert()
		{
			if (!SecurityManager.SecurityEnabled)
			{
				return;
			}
			throw new NotImplementedException();
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertDeny()
		{
			if (!SecurityManager.SecurityEnabled)
			{
				return;
			}
			throw new NotImplementedException();
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertPermitOnly()
		{
			if (!SecurityManager.SecurityEnabled)
			{
				return;
			}
			throw new NotImplementedException();
		}

		internal SecurityElement Element(int version)
		{
			SecurityElement securityElement = new SecurityElement("IPermission");
			Type type = base.GetType();
			securityElement.AddAttribute("class", type.FullName + ", " + type.Assembly.ToString().Replace('"', '\''));
			securityElement.AddAttribute("version", version.ToString());
			return securityElement;
		}

		internal static PermissionState CheckPermissionState(PermissionState state, bool allowUnrestricted)
		{
			if (state != PermissionState.None && state != PermissionState.Unrestricted)
			{
				throw new ArgumentException(string.Format(Locale.GetText("Invalid enum {0}"), state), "state");
			}
			return state;
		}

		internal static int CheckSecurityElement(SecurityElement se, string parameterName, int minimumVersion, int maximumVersion)
		{
			if (se == null)
			{
				throw new ArgumentNullException(parameterName);
			}
			if (se.Tag != "IPermission")
			{
				throw new ArgumentException(string.Format(Locale.GetText("Invalid tag {0}"), se.Tag), parameterName);
			}
			int num = minimumVersion;
			string text = se.Attribute("version");
			if (text != null)
			{
				try
				{
					num = int.Parse(text);
				}
				catch (Exception ex)
				{
					throw new ArgumentException(string.Format(Locale.GetText("Couldn't parse version from '{0}'."), text), parameterName, ex);
				}
			}
			if (num < minimumVersion || num > maximumVersion)
			{
				throw new ArgumentException(string.Format(Locale.GetText("Unknown version '{0}', expected versions between ['{1}','{2}']."), num, minimumVersion, maximumVersion), parameterName);
			}
			return num;
		}

		internal static bool IsUnrestricted(SecurityElement se)
		{
			string text = se.Attribute("Unrestricted");
			return text != null && string.Compare(text, bool.TrueString, true, CultureInfo.InvariantCulture) == 0;
		}

		internal static void ThrowInvalidPermission(IPermission target, Type expected)
		{
			throw new ArgumentException(string.Format(Locale.GetText("Invalid permission type '{0}', expected type '{1}'."), target.GetType(), expected), "target");
		}
	}
}
