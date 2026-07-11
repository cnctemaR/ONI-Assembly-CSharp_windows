using System;
using System.Globalization;

namespace System.Data.SqlClient
{
	internal sealed class ServerInfo
	{
		internal string ExtendedServerName { get; private set; }

		internal string ResolvedServerName { get; private set; }

		internal string ResolvedDatabaseName { get; private set; }

		internal string UserProtocol { get; private set; }

		internal string UserServerName
		{
			get
			{
				return this._userServerName;
			}
			private set
			{
				this._userServerName = value;
			}
		}

		internal ServerInfo(SqlConnectionString userOptions)
			: this(userOptions, userOptions.DataSource)
		{
		}

		internal ServerInfo(SqlConnectionString userOptions, string serverName)
		{
			this.UserServerName = serverName ?? string.Empty;
			this.UserProtocol = string.Empty;
			this.ResolvedDatabaseName = userOptions.InitialCatalog;
			this.PreRoutingServerName = null;
		}

		internal ServerInfo(SqlConnectionString userOptions, RoutingInfo routing, string preRoutingServerName)
		{
			if (routing == null || routing.ServerName == null)
			{
				this.UserServerName = string.Empty;
			}
			else
			{
				this.UserServerName = string.Format(CultureInfo.InvariantCulture, "{0},{1}", routing.ServerName, routing.Port);
			}
			this.PreRoutingServerName = preRoutingServerName;
			this.UserProtocol = "tcp";
			this.SetDerivedNames(this.UserProtocol, this.UserServerName);
			this.ResolvedDatabaseName = userOptions.InitialCatalog;
		}

		internal void SetDerivedNames(string protocol, string serverName)
		{
			if (!string.IsNullOrEmpty(protocol))
			{
				this.ExtendedServerName = protocol + ":" + serverName;
			}
			else
			{
				this.ExtendedServerName = serverName;
			}
			this.ResolvedServerName = serverName;
		}

		private string _userServerName;

		internal readonly string PreRoutingServerName;
	}
}
