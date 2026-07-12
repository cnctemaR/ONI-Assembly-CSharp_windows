using System;
using System.IO;
using System.Text;

namespace System.Data.SqlClient.SNI
{
	internal class DataSource
	{
		internal string ServerName { get; private set; }

		internal int Port { get; private set; } = -1;

		public string InstanceName { get; internal set; }

		public string PipeName { get; internal set; }

		public string PipeHostName { get; internal set; }

		internal bool IsBadDataSource { get; private set; }

		internal bool IsSsrpRequired { get; private set; }

		private DataSource(string dataSource)
		{
			this._workingDataSource = dataSource.Trim().ToLowerInvariant();
			int num = this._workingDataSource.IndexOf(':');
			this.PopulateProtocol();
			this._dataSourceAfterTrimmingProtocol = ((num > -1 && this.ConnectionProtocol != DataSource.Protocol.None) ? this._workingDataSource.Substring(num + 1).Trim() : this._workingDataSource);
			if (this._dataSourceAfterTrimmingProtocol.Contains("/"))
			{
				if (this.ConnectionProtocol == DataSource.Protocol.None)
				{
					this.ReportSNIError(SNIProviders.INVALID_PROV);
					return;
				}
				if (this.ConnectionProtocol == DataSource.Protocol.NP)
				{
					this.ReportSNIError(SNIProviders.NP_PROV);
					return;
				}
				if (this.ConnectionProtocol == DataSource.Protocol.TCP)
				{
					this.ReportSNIError(SNIProviders.TCP_PROV);
				}
			}
		}

		private void PopulateProtocol()
		{
			string[] array = this._workingDataSource.Split(':', StringSplitOptions.None);
			if (array.Length <= 1)
			{
				this.ConnectionProtocol = DataSource.Protocol.None;
				return;
			}
			string text = array[0].Trim();
			if (text == "tcp")
			{
				this.ConnectionProtocol = DataSource.Protocol.TCP;
				return;
			}
			if (text == "np")
			{
				this.ConnectionProtocol = DataSource.Protocol.NP;
				return;
			}
			if (!(text == "admin"))
			{
				this.ConnectionProtocol = DataSource.Protocol.None;
				return;
			}
			this.ConnectionProtocol = DataSource.Protocol.Admin;
		}

		public static string GetLocalDBInstance(string dataSource, out bool error)
		{
			string text = null;
			string[] array = dataSource.ToLowerInvariant().Split('\\', StringSplitOptions.None);
			error = false;
			if (array.Length == 2 && "(localdb)".Equals(array[0].TrimStart()))
			{
				if (string.IsNullOrWhiteSpace(array[1]))
				{
					SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 0U, 51U, string.Empty);
					error = true;
					return null;
				}
				text = array[1].Trim();
			}
			return text;
		}

		public static DataSource ParseServerName(string dataSource)
		{
			DataSource dataSource2 = new DataSource(dataSource);
			if (dataSource2.IsBadDataSource)
			{
				return null;
			}
			if (dataSource2.InferNamedPipesInformation())
			{
				return dataSource2;
			}
			if (dataSource2.IsBadDataSource)
			{
				return null;
			}
			if (dataSource2.InferConnectionDetails())
			{
				return dataSource2;
			}
			return null;
		}

		private void InferLocalServerName()
		{
			if (string.IsNullOrEmpty(this.ServerName) || DataSource.IsLocalHost(this.ServerName))
			{
				this.ServerName = ((this.ConnectionProtocol == DataSource.Protocol.Admin) ? Environment.MachineName : "localhost");
			}
		}

		private bool InferConnectionDetails()
		{
			string[] array = this._dataSourceAfterTrimmingProtocol.Split(new char[] { '\\', ',' });
			this.ServerName = array[0].Trim();
			int num = this._dataSourceAfterTrimmingProtocol.IndexOf(',');
			int num2 = this._dataSourceAfterTrimmingProtocol.IndexOf('\\');
			if (num > -1)
			{
				string text = ((num2 > -1) ? ((num > num2) ? array[2].Trim() : array[1].Trim()) : array[1].Trim());
				if (string.IsNullOrEmpty(text))
				{
					this.ReportSNIError(SNIProviders.INVALID_PROV);
					return false;
				}
				if (this.ConnectionProtocol == DataSource.Protocol.None)
				{
					this.ConnectionProtocol = DataSource.Protocol.TCP;
				}
				else if (this.ConnectionProtocol != DataSource.Protocol.TCP)
				{
					this.ReportSNIError(SNIProviders.INVALID_PROV);
					return false;
				}
				int num3;
				if (!int.TryParse(text, out num3))
				{
					this.ReportSNIError(SNIProviders.TCP_PROV);
					return false;
				}
				if (num3 < 1)
				{
					this.ReportSNIError(SNIProviders.TCP_PROV);
					return false;
				}
				this.Port = num3;
			}
			else if (num2 > -1)
			{
				this.InstanceName = array[1].Trim();
				if (string.IsNullOrWhiteSpace(this.InstanceName))
				{
					this.ReportSNIError(SNIProviders.INVALID_PROV);
					return false;
				}
				if ("mssqlserver".Equals(this.InstanceName))
				{
					this.ReportSNIError(SNIProviders.INVALID_PROV);
					return false;
				}
				this.IsSsrpRequired = true;
			}
			this.InferLocalServerName();
			return true;
		}

		private void ReportSNIError(SNIProviders provider)
		{
			SNILoadHandle.SingletonInstance.LastError = new SNIError(provider, 0U, 25U, string.Empty);
			this.IsBadDataSource = true;
		}

		private bool InferNamedPipesInformation()
		{
			if (!this._dataSourceAfterTrimmingProtocol.StartsWith("\\\\") && this.ConnectionProtocol != DataSource.Protocol.NP)
			{
				return false;
			}
			if (!this._dataSourceAfterTrimmingProtocol.Contains('\\'))
			{
				this.PipeHostName = (this.ServerName = this._dataSourceAfterTrimmingProtocol);
				this.InferLocalServerName();
				this.PipeName = "sql\\query";
				return true;
			}
			try
			{
				string[] array = this._dataSourceAfterTrimmingProtocol.Split('\\', StringSplitOptions.None);
				if (array.Length < 6)
				{
					this.ReportSNIError(SNIProviders.NP_PROV);
					return false;
				}
				string text = array[2];
				if (string.IsNullOrEmpty(text))
				{
					this.ReportSNIError(SNIProviders.NP_PROV);
					return false;
				}
				if (!"pipe".Equals(array[3]))
				{
					this.ReportSNIError(SNIProviders.NP_PROV);
					return false;
				}
				if (array[4].StartsWith("mssql$"))
				{
					this.InstanceName = array[4].Substring("mssql$".Length);
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 4; i < array.Length - 1; i++)
				{
					stringBuilder.Append(array[i]);
					stringBuilder.Append(Path.DirectorySeparatorChar);
				}
				stringBuilder.Append(array[array.Length - 1]);
				this.PipeName = stringBuilder.ToString();
				if (string.IsNullOrWhiteSpace(this.InstanceName) && !"sql\\query".Equals(this.PipeName))
				{
					this.InstanceName = "pipe" + this.PipeName;
				}
				this.ServerName = (DataSource.IsLocalHost(text) ? Environment.MachineName : text);
				this.PipeHostName = text;
			}
			catch (UriFormatException)
			{
				this.ReportSNIError(SNIProviders.NP_PROV);
				return false;
			}
			if (this.ConnectionProtocol == DataSource.Protocol.None)
			{
				this.ConnectionProtocol = DataSource.Protocol.NP;
			}
			else if (this.ConnectionProtocol != DataSource.Protocol.NP)
			{
				this.ReportSNIError(SNIProviders.NP_PROV);
				return false;
			}
			return true;
		}

		private static bool IsLocalHost(string serverName)
		{
			return ".".Equals(serverName) || "(local)".Equals(serverName) || "localhost".Equals(serverName);
		}

		private const char CommaSeparator = ',';

		private const char BackSlashSeparator = '\\';

		private const string DefaultHostName = "localhost";

		private const string DefaultSqlServerInstanceName = "mssqlserver";

		private const string PipeBeginning = "\\\\";

		private const string PipeToken = "pipe";

		private const string LocalDbHost = "(localdb)";

		private const string NamedPipeInstanceNameHeader = "mssql$";

		private const string DefaultPipeName = "sql\\query";

		internal DataSource.Protocol ConnectionProtocol = DataSource.Protocol.None;

		private string _workingDataSource;

		private string _dataSourceAfterTrimmingProtocol;

		internal enum Protocol
		{
			TCP,
			NP,
			None,
			Admin
		}
	}
}
