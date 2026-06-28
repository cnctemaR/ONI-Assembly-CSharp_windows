using System;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace System.Net
{
	[global::System.MonoTODO("Most private members that include functionallity are not implemented!")]
	[Serializable]
	public sealed class WebPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		public WebPermission()
		{
		}

		public WebPermission(PermissionState state)
		{
			this.m_noRestriction = state == PermissionState.Unrestricted;
		}

		public WebPermission(NetworkAccess access, string uriString)
		{
			this.AddPermission(access, uriString);
		}

		public WebPermission(NetworkAccess access, global::System.Text.RegularExpressions.Regex uriRegex)
		{
			this.AddPermission(access, uriRegex);
		}

		public IEnumerator AcceptList
		{
			get
			{
				return this.m_acceptList.GetEnumerator();
			}
		}

		public IEnumerator ConnectList
		{
			get
			{
				return this.m_connectList.GetEnumerator();
			}
		}

		public void AddPermission(NetworkAccess access, string uriString)
		{
			WebPermissionInfo webPermissionInfo = new WebPermissionInfo(WebPermissionInfoType.InfoString, uriString);
			this.AddPermission(access, webPermissionInfo);
		}

		public void AddPermission(NetworkAccess access, global::System.Text.RegularExpressions.Regex uriRegex)
		{
			WebPermissionInfo webPermissionInfo = new WebPermissionInfo(uriRegex);
			this.AddPermission(access, webPermissionInfo);
		}

		internal void AddPermission(NetworkAccess access, WebPermissionInfo info)
		{
			if (access != NetworkAccess.Connect)
			{
				if (access != NetworkAccess.Accept)
				{
					string text = global::Locale.GetText("Unknown NetworkAccess value {0}.");
					throw new ArgumentException(string.Format(text, access), "access");
				}
				this.m_acceptList.Add(info);
			}
			else
			{
				this.m_connectList.Add(info);
			}
		}

		public override IPermission Copy()
		{
			return new WebPermission((!this.m_noRestriction) ? PermissionState.None : PermissionState.Unrestricted)
			{
				m_connectList = (ArrayList)this.m_connectList.Clone(),
				m_acceptList = (ArrayList)this.m_acceptList.Clone()
			};
		}

		public override IPermission Intersect(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			WebPermission webPermission = target as WebPermission;
			if (webPermission == null)
			{
				throw new ArgumentException("Argument not of type WebPermission");
			}
			if (this.m_noRestriction)
			{
				IPermission permission2;
				if (this.IntersectEmpty(webPermission))
				{
					IPermission permission = null;
					permission2 = permission;
				}
				else
				{
					permission2 = webPermission.Copy();
				}
				return permission2;
			}
			if (webPermission.m_noRestriction)
			{
				IPermission permission3;
				if (this.IntersectEmpty(this))
				{
					IPermission permission = null;
					permission3 = permission;
				}
				else
				{
					permission3 = this.Copy();
				}
				return permission3;
			}
			WebPermission webPermission2 = new WebPermission(PermissionState.None);
			this.Intersect(this.m_connectList, webPermission.m_connectList, webPermission2.m_connectList);
			this.Intersect(this.m_acceptList, webPermission.m_acceptList, webPermission2.m_acceptList);
			return (!this.IntersectEmpty(webPermission2)) ? webPermission2 : null;
		}

		private bool IntersectEmpty(WebPermission permission)
		{
			return !permission.m_noRestriction && permission.m_connectList.Count == 0 && permission.m_acceptList.Count == 0;
		}

		[global::System.MonoTODO]
		private void Intersect(ArrayList list1, ArrayList list2, ArrayList result)
		{
			throw new NotImplementedException();
		}

		public override bool IsSubsetOf(IPermission target)
		{
			if (target == null)
			{
				return !this.m_noRestriction && this.m_connectList.Count == 0 && this.m_acceptList.Count == 0;
			}
			WebPermission webPermission = target as WebPermission;
			if (webPermission == null)
			{
				throw new ArgumentException("Parameter target must be of type WebPermission");
			}
			return webPermission.m_noRestriction || (!this.m_noRestriction && ((this.m_acceptList.Count == 0 && this.m_connectList.Count == 0) || ((webPermission.m_acceptList.Count != 0 || webPermission.m_connectList.Count != 0) && this.IsSubsetOf(this.m_connectList, webPermission.m_connectList) && this.IsSubsetOf(this.m_acceptList, webPermission.m_acceptList))));
		}

		[global::System.MonoTODO]
		private bool IsSubsetOf(ArrayList list1, ArrayList list2)
		{
			throw new NotImplementedException();
		}

		public bool IsUnrestricted()
		{
			return this.m_noRestriction;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement("IPermission");
			securityElement.AddAttribute("class", base.GetType().AssemblyQualifiedName);
			securityElement.AddAttribute("version", "1");
			if (this.m_noRestriction)
			{
				securityElement.AddAttribute("Unrestricted", "true");
				return securityElement;
			}
			if (this.m_connectList.Count > 0)
			{
				this.ToXml(securityElement, "ConnectAccess", this.m_connectList.GetEnumerator());
			}
			if (this.m_acceptList.Count > 0)
			{
				this.ToXml(securityElement, "AcceptAccess", this.m_acceptList.GetEnumerator());
			}
			return securityElement;
		}

		private void ToXml(SecurityElement root, string childName, IEnumerator enumerator)
		{
			SecurityElement securityElement = new SecurityElement(childName, null);
			root.AddChild(securityElement);
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				WebPermissionInfo webPermissionInfo = obj as WebPermissionInfo;
				if (webPermissionInfo != null)
				{
					SecurityElement securityElement2 = new SecurityElement("URI");
					securityElement2.AddAttribute("uri", webPermissionInfo.Info);
					securityElement.AddChild(securityElement2);
				}
			}
		}

		public override void FromXml(SecurityElement securityElement)
		{
			if (securityElement == null)
			{
				throw new ArgumentNullException("securityElement");
			}
			if (securityElement.Tag != "IPermission")
			{
				throw new ArgumentException("securityElement");
			}
			string text = securityElement.Attribute("Unrestricted");
			if (text != null)
			{
				this.m_noRestriction = string.Compare(text, "true", true) == 0;
				if (this.m_noRestriction)
				{
					return;
				}
			}
			this.m_noRestriction = false;
			this.m_connectList = new ArrayList();
			this.m_acceptList = new ArrayList();
			ArrayList children = securityElement.Children;
			foreach (object obj in children)
			{
				SecurityElement securityElement2 = (SecurityElement)obj;
				if (securityElement2.Tag == "ConnectAccess")
				{
					this.FromXml(securityElement2.Children, NetworkAccess.Connect);
				}
				else if (securityElement2.Tag == "AcceptAccess")
				{
					this.FromXml(securityElement2.Children, NetworkAccess.Accept);
				}
			}
		}

		private void FromXml(ArrayList endpoints, NetworkAccess access)
		{
			throw new NotImplementedException();
		}

		public override IPermission Union(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			WebPermission webPermission = target as WebPermission;
			if (webPermission == null)
			{
				throw new ArgumentException("Argument not of type WebPermission");
			}
			if (this.m_noRestriction || webPermission.m_noRestriction)
			{
				return new WebPermission(PermissionState.Unrestricted);
			}
			WebPermission webPermission2 = (WebPermission)webPermission.Copy();
			webPermission2.m_acceptList.InsertRange(webPermission2.m_acceptList.Count, this.m_acceptList);
			webPermission2.m_connectList.InsertRange(webPermission2.m_connectList.Count, this.m_connectList);
			return webPermission2;
		}

		private ArrayList m_acceptList = new ArrayList();

		private ArrayList m_connectList = new ArrayList();

		private bool m_noRestriction;
	}
}
