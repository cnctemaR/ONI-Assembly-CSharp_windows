using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public sealed class PrincipalPermission : IPermission, ISecurityEncodable, IUnrestrictedPermission, IBuiltInPermission
	{
		public PrincipalPermission(PermissionState state)
		{
			this.principals = new ArrayList();
			if (CodeAccessPermission.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				PrincipalPermission.PrincipalInfo principalInfo = new PrincipalPermission.PrincipalInfo(null, null, true);
				this.principals.Add(principalInfo);
			}
		}

		public PrincipalPermission(string name, string role)
			: this(name, role, true)
		{
		}

		public PrincipalPermission(string name, string role, bool isAuthenticated)
		{
			this.principals = new ArrayList();
			PrincipalPermission.PrincipalInfo principalInfo = new PrincipalPermission.PrincipalInfo(name, role, isAuthenticated);
			this.principals.Add(principalInfo);
		}

		internal PrincipalPermission(ArrayList principals)
		{
			this.principals = (ArrayList)principals.Clone();
		}

		public IPermission Copy()
		{
			return new PrincipalPermission(this.principals);
		}

		[SecuritySafeCritical]
		public void Demand()
		{
			IPrincipal currentPrincipal = Thread.CurrentPrincipal;
			if (currentPrincipal == null)
			{
				throw new SecurityException("no Principal");
			}
			if (this.principals.Count > 0)
			{
				bool flag = false;
				foreach (object obj in this.principals)
				{
					PrincipalPermission.PrincipalInfo principalInfo = (PrincipalPermission.PrincipalInfo)obj;
					if ((principalInfo.Name == null || principalInfo.Name == currentPrincipal.Identity.Name) && (principalInfo.Role == null || currentPrincipal.IsInRole(principalInfo.Role)) && ((principalInfo.IsAuthenticated && currentPrincipal.Identity.IsAuthenticated) || !principalInfo.IsAuthenticated))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new SecurityException("Demand for principal refused.");
				}
			}
		}

		public void FromXml(SecurityElement elem)
		{
			this.CheckSecurityElement(elem, "elem", 1, 1);
			this.principals.Clear();
			if (elem.Children != null)
			{
				foreach (object obj in elem.Children)
				{
					SecurityElement securityElement = (SecurityElement)obj;
					if (securityElement.Tag != "Identity")
					{
						throw new ArgumentException("not IPermission/Identity");
					}
					string text = securityElement.Attribute("ID");
					string text2 = securityElement.Attribute("Role");
					string text3 = securityElement.Attribute("Authenticated");
					bool flag = false;
					if (text3 != null)
					{
						try
						{
							flag = bool.Parse(text3);
						}
						catch
						{
						}
					}
					PrincipalPermission.PrincipalInfo principalInfo = new PrincipalPermission.PrincipalInfo(text, text2, flag);
					this.principals.Add(principalInfo);
				}
			}
		}

		public IPermission Intersect(IPermission target)
		{
			PrincipalPermission principalPermission = this.Cast(target);
			if (principalPermission == null)
			{
				return null;
			}
			if (this.IsUnrestricted())
			{
				return principalPermission.Copy();
			}
			if (principalPermission.IsUnrestricted())
			{
				return this.Copy();
			}
			PrincipalPermission principalPermission2 = new PrincipalPermission(PermissionState.None);
			foreach (object obj in this.principals)
			{
				PrincipalPermission.PrincipalInfo principalInfo = (PrincipalPermission.PrincipalInfo)obj;
				foreach (object obj2 in principalPermission.principals)
				{
					PrincipalPermission.PrincipalInfo principalInfo2 = (PrincipalPermission.PrincipalInfo)obj2;
					if (principalInfo.IsAuthenticated == principalInfo2.IsAuthenticated)
					{
						string text = null;
						if (principalInfo.Name == principalInfo2.Name || principalInfo2.Name == null)
						{
							text = principalInfo.Name;
						}
						else if (principalInfo.Name == null)
						{
							text = principalInfo2.Name;
						}
						string text2 = null;
						if (principalInfo.Role == principalInfo2.Role || principalInfo2.Role == null)
						{
							text2 = principalInfo.Role;
						}
						else if (principalInfo.Role == null)
						{
							text2 = principalInfo2.Role;
						}
						if (text != null || text2 != null)
						{
							PrincipalPermission.PrincipalInfo principalInfo3 = new PrincipalPermission.PrincipalInfo(text, text2, principalInfo.IsAuthenticated);
							principalPermission2.principals.Add(principalInfo3);
						}
					}
				}
			}
			if (principalPermission2.principals.Count <= 0)
			{
				return null;
			}
			return principalPermission2;
		}

		public bool IsSubsetOf(IPermission target)
		{
			PrincipalPermission principalPermission = this.Cast(target);
			if (principalPermission == null)
			{
				return this.IsEmpty();
			}
			if (this.IsUnrestricted())
			{
				return principalPermission.IsUnrestricted();
			}
			if (principalPermission.IsUnrestricted())
			{
				return true;
			}
			foreach (object obj in this.principals)
			{
				PrincipalPermission.PrincipalInfo principalInfo = (PrincipalPermission.PrincipalInfo)obj;
				bool flag = false;
				foreach (object obj2 in principalPermission.principals)
				{
					PrincipalPermission.PrincipalInfo principalInfo2 = (PrincipalPermission.PrincipalInfo)obj2;
					if ((principalInfo.Name == principalInfo2.Name || principalInfo2.Name == null) && (principalInfo.Role == principalInfo2.Role || principalInfo2.Role == null) && principalInfo.IsAuthenticated == principalInfo2.IsAuthenticated)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsUnrestricted()
		{
			foreach (object obj in this.principals)
			{
				PrincipalPermission.PrincipalInfo principalInfo = (PrincipalPermission.PrincipalInfo)obj;
				if (principalInfo.Name == null && principalInfo.Role == null && principalInfo.IsAuthenticated)
				{
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			return this.ToXml().ToString();
		}

		public SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement("Permission");
			Type type = base.GetType();
			securityElement.AddAttribute("class", type.FullName + ", " + type.Assembly.ToString().Replace('"', '\''));
			securityElement.AddAttribute("version", 1.ToString());
			foreach (object obj in this.principals)
			{
				PrincipalPermission.PrincipalInfo principalInfo = (PrincipalPermission.PrincipalInfo)obj;
				SecurityElement securityElement2 = new SecurityElement("Identity");
				if (principalInfo.Name != null)
				{
					securityElement2.AddAttribute("ID", principalInfo.Name);
				}
				if (principalInfo.Role != null)
				{
					securityElement2.AddAttribute("Role", principalInfo.Role);
				}
				if (principalInfo.IsAuthenticated)
				{
					securityElement2.AddAttribute("Authenticated", "true");
				}
				securityElement.AddChild(securityElement2);
			}
			return securityElement;
		}

		public IPermission Union(IPermission other)
		{
			PrincipalPermission principalPermission = this.Cast(other);
			if (principalPermission == null)
			{
				return this.Copy();
			}
			if (this.IsUnrestricted() || principalPermission.IsUnrestricted())
			{
				return new PrincipalPermission(PermissionState.Unrestricted);
			}
			PrincipalPermission principalPermission2 = new PrincipalPermission(this.principals);
			foreach (object obj in principalPermission.principals)
			{
				PrincipalPermission.PrincipalInfo principalInfo = (PrincipalPermission.PrincipalInfo)obj;
				principalPermission2.principals.Add(principalInfo);
			}
			return principalPermission2;
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			PrincipalPermission principalPermission = obj as PrincipalPermission;
			if (principalPermission == null)
			{
				return false;
			}
			if (this.principals.Count != principalPermission.principals.Count)
			{
				return false;
			}
			foreach (object obj2 in this.principals)
			{
				PrincipalPermission.PrincipalInfo principalInfo = (PrincipalPermission.PrincipalInfo)obj2;
				bool flag = false;
				foreach (object obj3 in principalPermission.principals)
				{
					PrincipalPermission.PrincipalInfo principalInfo2 = (PrincipalPermission.PrincipalInfo)obj3;
					if ((principalInfo.Name == principalInfo2.Name || principalInfo2.Name == null) && (principalInfo.Role == principalInfo2.Role || principalInfo2.Role == null) && principalInfo.IsAuthenticated == principalInfo2.IsAuthenticated)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 8;
		}

		private PrincipalPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			PrincipalPermission principalPermission = target as PrincipalPermission;
			if (principalPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(PrincipalPermission));
			}
			return principalPermission;
		}

		private bool IsEmpty()
		{
			return this.principals.Count == 0;
		}

		internal int CheckSecurityElement(SecurityElement se, string parameterName, int minimumVersion, int maximumVersion)
		{
			if (se == null)
			{
				throw new ArgumentNullException(parameterName);
			}
			if (se.Tag != "Permission")
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

		private const int version = 1;

		private ArrayList principals;

		internal class PrincipalInfo
		{
			public PrincipalInfo(string name, string role, bool isAuthenticated)
			{
				this._name = name;
				this._role = role;
				this._isAuthenticated = isAuthenticated;
			}

			public string Name
			{
				get
				{
					return this._name;
				}
			}

			public string Role
			{
				get
				{
					return this._role;
				}
			}

			public bool IsAuthenticated
			{
				get
				{
					return this._isAuthenticated;
				}
			}

			private string _name;

			private string _role;

			private bool _isAuthenticated;
		}
	}
}
