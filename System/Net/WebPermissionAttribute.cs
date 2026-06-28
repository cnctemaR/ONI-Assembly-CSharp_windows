using System;
using System.Security;
using System.Security.Permissions;

namespace System.Net
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class WebPermissionAttribute : CodeAccessSecurityAttribute
	{
		public WebPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public string Accept
		{
			get
			{
				if (this.m_accept == null)
				{
					return null;
				}
				return (this.m_accept as WebPermissionInfo).Info;
			}
			set
			{
				if (this.m_accept != null)
				{
					this.AlreadySet("Accept", "Accept");
				}
				this.m_accept = new WebPermissionInfo(WebPermissionInfoType.InfoString, value);
			}
		}

		public string AcceptPattern
		{
			get
			{
				if (this.m_accept == null)
				{
					return null;
				}
				return (this.m_accept as WebPermissionInfo).Info;
			}
			set
			{
				if (this.m_accept != null)
				{
					this.AlreadySet("Accept", "AcceptPattern");
				}
				if (value == null)
				{
					throw new ArgumentNullException("AcceptPattern");
				}
				this.m_accept = new WebPermissionInfo(WebPermissionInfoType.InfoUnexecutedRegex, value);
			}
		}

		public string Connect
		{
			get
			{
				if (this.m_connect == null)
				{
					return null;
				}
				return (this.m_connect as WebPermissionInfo).Info;
			}
			set
			{
				if (this.m_connect != null)
				{
					this.AlreadySet("Connect", "Connect");
				}
				this.m_connect = new WebPermissionInfo(WebPermissionInfoType.InfoString, value);
			}
		}

		public string ConnectPattern
		{
			get
			{
				if (this.m_connect == null)
				{
					return null;
				}
				return (this.m_connect as WebPermissionInfo).Info;
			}
			set
			{
				if (this.m_connect != null)
				{
					this.AlreadySet("Connect", "ConnectConnectPattern");
				}
				if (value == null)
				{
					throw new ArgumentNullException("ConnectPattern");
				}
				this.m_connect = new WebPermissionInfo(WebPermissionInfoType.InfoUnexecutedRegex, value);
			}
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new WebPermission(PermissionState.Unrestricted);
			}
			WebPermission webPermission = new WebPermission();
			if (this.m_accept != null)
			{
				webPermission.AddPermission(NetworkAccess.Accept, (WebPermissionInfo)this.m_accept);
			}
			if (this.m_connect != null)
			{
				webPermission.AddPermission(NetworkAccess.Connect, (WebPermissionInfo)this.m_connect);
			}
			return webPermission;
		}

		internal void AlreadySet(string parameter, string property)
		{
			string text = global::Locale.GetText("The parameter '{0}' can be set only once.");
			throw new ArgumentException(string.Format(text, parameter), property);
		}

		private object m_accept;

		private object m_connect;
	}
}
