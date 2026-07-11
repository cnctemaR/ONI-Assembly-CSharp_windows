using System;
using System.Security;
using System.Security.Permissions;

namespace System.Net
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class SocketPermissionAttribute : CodeAccessSecurityAttribute
	{
		public SocketPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public string Access
		{
			get
			{
				return this.m_access;
			}
			set
			{
				if (this.m_access != null)
				{
					this.AlreadySet("Access");
				}
				this.m_access = value;
			}
		}

		public string Host
		{
			get
			{
				return this.m_host;
			}
			set
			{
				if (this.m_host != null)
				{
					this.AlreadySet("Host");
				}
				this.m_host = value;
			}
		}

		public string Port
		{
			get
			{
				return this.m_port;
			}
			set
			{
				if (this.m_port != null)
				{
					this.AlreadySet("Port");
				}
				this.m_port = value;
			}
		}

		public string Transport
		{
			get
			{
				return this.m_transport;
			}
			set
			{
				if (this.m_transport != null)
				{
					this.AlreadySet("Transport");
				}
				this.m_transport = value;
			}
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new SocketPermission(PermissionState.Unrestricted);
			}
			string text = string.Empty;
			if (this.m_access == null)
			{
				text += "Access, ";
			}
			if (this.m_host == null)
			{
				text += "Host, ";
			}
			if (this.m_port == null)
			{
				text += "Port, ";
			}
			if (this.m_transport == null)
			{
				text += "Transport, ";
			}
			if (text.Length > 0)
			{
				string text2 = global::Locale.GetText("The value(s) for {0} must be specified.");
				text = text.Substring(0, text.Length - 2);
				throw new ArgumentException(string.Format(text2, text));
			}
			int num = -1;
			NetworkAccess networkAccess;
			if (string.Compare(this.m_access, "Connect", true) == 0)
			{
				networkAccess = NetworkAccess.Connect;
			}
			else
			{
				if (string.Compare(this.m_access, "Accept", true) != 0)
				{
					throw new ArgumentException(string.Format(global::Locale.GetText("The parameter value for 'Access', '{1}, is invalid."), this.m_access));
				}
				networkAccess = NetworkAccess.Accept;
			}
			if (string.Compare(this.m_port, "All", true) != 0)
			{
				try
				{
					num = int.Parse(this.m_port);
				}
				catch
				{
					throw new ArgumentException(string.Format(global::Locale.GetText("The parameter value for 'Port', '{1}, is invalid."), this.m_port));
				}
				new IPEndPoint(1L, num);
			}
			TransportType transportType;
			try
			{
				transportType = (TransportType)Enum.Parse(typeof(TransportType), this.m_transport, true);
			}
			catch
			{
				throw new ArgumentException(string.Format(global::Locale.GetText("The parameter value for 'Transport', '{1}, is invalid."), this.m_transport));
			}
			SocketPermission socketPermission = new SocketPermission(PermissionState.None);
			socketPermission.AddPermission(networkAccess, transportType, this.m_host, num);
			return socketPermission;
		}

		internal void AlreadySet(string property)
		{
			throw new ArgumentException(string.Format(global::Locale.GetText("The parameter '{0}' can be set only once."), property), property);
		}

		private string m_access;

		private string m_host;

		private string m_port;

		private string m_transport;
	}
}
