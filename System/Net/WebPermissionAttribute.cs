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

		public string Connect
		{
			get
			{
				return this.m_connect as string;
			}
			set
			{
				if (this.m_connect != null)
				{
					throw new ArgumentException(global::SR.GetString("The permission '{0}={1}' cannot be added. Add a separate Attribute statement.", new object[] { "Connect", value }), "value");
				}
				this.m_connect = value;
			}
		}

		public string Accept
		{
			get
			{
				return this.m_accept as string;
			}
			set
			{
				if (this.m_accept != null)
				{
					throw new ArgumentException(global::SR.GetString("The permission '{0}={1}' cannot be added. Add a separate Attribute statement.", new object[] { "Accept", value }), "value");
				}
				this.m_accept = value;
			}
		}

		public string ConnectPattern
		{
			get
			{
				if (this.m_connect is DelayedRegex)
				{
					return this.m_connect.ToString();
				}
				if (!(this.m_connect is bool) || !(bool)this.m_connect)
				{
					return null;
				}
				return ".*";
			}
			set
			{
				if (this.m_connect != null)
				{
					throw new ArgumentException(global::SR.GetString("The permission '{0}={1}' cannot be added. Add a separate Attribute statement.", new object[] { "ConnectPatern", value }), "value");
				}
				if (value == ".*")
				{
					this.m_connect = true;
					return;
				}
				this.m_connect = new DelayedRegex(value);
			}
		}

		public string AcceptPattern
		{
			get
			{
				if (this.m_accept is DelayedRegex)
				{
					return this.m_accept.ToString();
				}
				if (!(this.m_accept is bool) || !(bool)this.m_accept)
				{
					return null;
				}
				return ".*";
			}
			set
			{
				if (this.m_accept != null)
				{
					throw new ArgumentException(global::SR.GetString("The permission '{0}={1}' cannot be added. Add a separate Attribute statement.", new object[] { "AcceptPattern", value }), "value");
				}
				if (value == ".*")
				{
					this.m_accept = true;
					return;
				}
				this.m_accept = new DelayedRegex(value);
			}
		}

		public override IPermission CreatePermission()
		{
			WebPermission webPermission;
			if (base.Unrestricted)
			{
				webPermission = new WebPermission(PermissionState.Unrestricted);
			}
			else
			{
				NetworkAccess networkAccess = (NetworkAccess)0;
				if (this.m_connect is bool)
				{
					if ((bool)this.m_connect)
					{
						networkAccess |= NetworkAccess.Connect;
					}
					this.m_connect = null;
				}
				if (this.m_accept is bool)
				{
					if ((bool)this.m_accept)
					{
						networkAccess |= NetworkAccess.Accept;
					}
					this.m_accept = null;
				}
				webPermission = new WebPermission(networkAccess);
				if (this.m_accept != null)
				{
					if (this.m_accept is DelayedRegex)
					{
						webPermission.AddAsPattern(NetworkAccess.Accept, (DelayedRegex)this.m_accept);
					}
					else
					{
						webPermission.AddPermission(NetworkAccess.Accept, (string)this.m_accept);
					}
				}
				if (this.m_connect != null)
				{
					if (this.m_connect is DelayedRegex)
					{
						webPermission.AddAsPattern(NetworkAccess.Connect, (DelayedRegex)this.m_connect);
					}
					else
					{
						webPermission.AddPermission(NetworkAccess.Connect, (string)this.m_connect);
					}
				}
			}
			return webPermission;
		}

		private object m_accept;

		private object m_connect;
	}
}
