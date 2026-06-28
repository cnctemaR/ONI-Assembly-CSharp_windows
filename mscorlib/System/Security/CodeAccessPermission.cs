using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security
{
	[MonoTODO("CAS support is experimental (and unsupported).")]
	[ComVisible(true)]
	[PermissionSet(SecurityAction.InheritanceDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence, ControlPolicy\"/>\n</PermissionSet>\n")]
	[Serializable]
	public abstract class CodeAccessPermission : IPermission, ISecurityEncodable, IStackWalk
	{
		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public void Assert()
		{
		}

		internal bool CheckAssert(CodeAccessPermission asserted)
		{
			return asserted != null && asserted.GetType() == base.GetType() && this.IsSubsetOf(asserted);
		}

		internal bool CheckDemand(CodeAccessPermission target)
		{
			return target != null && target.GetType() == base.GetType() && this.IsSubsetOf(target);
		}

		internal bool CheckDeny(CodeAccessPermission denied)
		{
			if (denied == null)
			{
				return true;
			}
			Type type = denied.GetType();
			return type != base.GetType() || this.Intersect(denied) == null || denied.IsSubsetOf(PermissionBuilder.Create(type));
		}

		internal bool CheckPermitOnly(CodeAccessPermission target)
		{
			return target != null && target.GetType() == base.GetType() && this.IsSubsetOf(target);
		}

		public abstract IPermission Copy();

		public void Demand()
		{
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public void Deny()
		{
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
			SecurityElement securityElement = this.ToXml();
			return securityElement.ToString();
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

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public void PermitOnly()
		{
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertAll()
		{
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertAssert()
		{
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertDeny()
		{
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertPermitOnly()
		{
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
			if (state != PermissionState.None)
			{
				if (state != PermissionState.Unrestricted)
				{
					string text = string.Format(Locale.GetText("Invalid enum {0}"), state);
					throw new ArgumentException(text, "state");
				}
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
				string text = string.Format(Locale.GetText("Invalid tag {0}"), se.Tag);
				throw new ArgumentException(text, parameterName);
			}
			int num = minimumVersion;
			string text2 = se.Attribute("version");
			if (text2 != null)
			{
				try
				{
					num = int.Parse(text2);
				}
				catch (Exception ex)
				{
					string text3 = Locale.GetText("Couldn't parse version from '{0}'.");
					text3 = string.Format(text3, text2);
					throw new ArgumentException(text3, parameterName, ex);
				}
			}
			if (num < minimumVersion || num > maximumVersion)
			{
				string text4 = Locale.GetText("Unknown version '{0}', expected versions between ['{1}','{2}'].");
				text4 = string.Format(text4, num, minimumVersion, maximumVersion);
				throw new ArgumentException(text4, parameterName);
			}
			return num;
		}

		internal static bool IsUnrestricted(SecurityElement se)
		{
			string text = se.Attribute("Unrestricted");
			return text != null && string.Compare(text, bool.TrueString, true, CultureInfo.InvariantCulture) == 0;
		}

		internal bool ProcessFrame(SecurityFrame frame)
		{
			if (frame.PermitOnly != null)
			{
				bool flag = frame.PermitOnly.IsUnrestricted();
				if (!flag)
				{
					foreach (object obj in frame.PermitOnly)
					{
						IPermission permission = (IPermission)obj;
						if (this.CheckPermitOnly(permission as CodeAccessPermission))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					CodeAccessPermission.ThrowSecurityException(this, "PermitOnly", frame, SecurityAction.Demand, null);
				}
			}
			if (frame.Deny != null)
			{
				if (frame.Deny.IsUnrestricted())
				{
					CodeAccessPermission.ThrowSecurityException(this, "Deny", frame, SecurityAction.Demand, null);
				}
				foreach (object obj2 in frame.Deny)
				{
					IPermission permission2 = (IPermission)obj2;
					if (!this.CheckDeny(permission2 as CodeAccessPermission))
					{
						CodeAccessPermission.ThrowSecurityException(this, "Deny", frame, SecurityAction.Demand, permission2);
					}
				}
			}
			if (frame.Assert != null)
			{
				if (frame.Assert.IsUnrestricted())
				{
					return true;
				}
				foreach (object obj3 in frame.Assert)
				{
					IPermission permission3 = (IPermission)obj3;
					if (this.CheckAssert(permission3 as CodeAccessPermission))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal static void ThrowInvalidPermission(IPermission target, Type expected)
		{
			string text = Locale.GetText("Invalid permission type '{0}', expected type '{1}'.");
			text = string.Format(text, target.GetType(), expected);
			throw new ArgumentException(text, "target");
		}

		internal static void ThrowExecutionEngineException(SecurityAction stackmod)
		{
			string text = Locale.GetText("No {0} modifier is present on the current stack frame.");
			text = text + Environment.NewLine + "Currently only declarative stack modifiers are supported.";
			throw new ExecutionEngineException(string.Format(text, stackmod));
		}

		internal static void ThrowSecurityException(object demanded, string message, SecurityFrame frame, SecurityAction action, IPermission failed)
		{
			Assembly assembly = frame.Assembly;
			throw new SecurityException(Locale.GetText(message), assembly.UnprotectedGetName(), assembly.GrantedPermissionSet, assembly.DeniedPermissionSet, frame.Method, action, demanded, failed, assembly.UnprotectedGetEvidence());
		}
	}
}
