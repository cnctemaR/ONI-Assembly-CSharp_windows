using System;
using System.Collections.Generic;
using System.Data.Common;

namespace System.Data.SqlClient
{
	internal sealed class SqlConnectionString : DbConnectionOptions
	{
		internal SqlConnectionString(string connectionString)
			: base(connectionString, SqlConnectionString.GetParseSynonyms())
		{
			this.ThrowUnsupportedIfKeywordSet("asynchronous processing");
			this.ThrowUnsupportedIfKeywordSet("connection reset");
			this.ThrowUnsupportedIfKeywordSet("context connection");
			if (base.ContainsKey("network library"))
			{
				throw SQL.NetworkLibraryKeywordNotSupported();
			}
			this._integratedSecurity = base.ConvertValueToIntegratedSecurity();
			this._encrypt = base.ConvertValueToBoolean("encrypt", false);
			this._enlist = base.ConvertValueToBoolean("enlist", true);
			this._mars = base.ConvertValueToBoolean("multipleactiveresultsets", false);
			this._persistSecurityInfo = base.ConvertValueToBoolean("persist security info", false);
			this._pooling = base.ConvertValueToBoolean("pooling", true);
			this._replication = base.ConvertValueToBoolean("replication", false);
			this._userInstance = base.ConvertValueToBoolean("user instance", false);
			this._multiSubnetFailover = base.ConvertValueToBoolean("multisubnetfailover", false);
			this._connectTimeout = base.ConvertValueToInt32("connect timeout", 15);
			this._loadBalanceTimeout = base.ConvertValueToInt32("load balance timeout", 0);
			this._maxPoolSize = base.ConvertValueToInt32("max pool size", 100);
			this._minPoolSize = base.ConvertValueToInt32("min pool size", 0);
			this._packetSize = base.ConvertValueToInt32("packet size", 8000);
			this._connectRetryCount = base.ConvertValueToInt32("connectretrycount", 1);
			this._connectRetryInterval = base.ConvertValueToInt32("connectretryinterval", 10);
			this._applicationIntent = this.ConvertValueToApplicationIntent();
			this._applicationName = base.ConvertValueToString("application name", "Core .Net SqlClient Data Provider");
			this._attachDBFileName = base.ConvertValueToString("attachdbfilename", "");
			this._currentLanguage = base.ConvertValueToString("current language", "");
			this._dataSource = base.ConvertValueToString("data source", "");
			this._localDBInstance = LocalDBAPI.GetLocalDbInstanceNameFromServerName(this._dataSource);
			this._failoverPartner = base.ConvertValueToString("failover partner", "");
			this._initialCatalog = base.ConvertValueToString("initial catalog", "");
			this._password = base.ConvertValueToString("password", "");
			this._trustServerCertificate = base.ConvertValueToBoolean("trustservercertificate", false);
			string text = base.ConvertValueToString("type system version", null);
			string text2 = base.ConvertValueToString("transaction binding", null);
			this._userID = base.ConvertValueToString("user id", "");
			this._workstationId = base.ConvertValueToString("workstation id", null);
			if (this._loadBalanceTimeout < 0)
			{
				throw ADP.InvalidConnectionOptionValue("load balance timeout");
			}
			if (this._connectTimeout < 0)
			{
				throw ADP.InvalidConnectionOptionValue("connect timeout");
			}
			if (this._maxPoolSize < 1)
			{
				throw ADP.InvalidConnectionOptionValue("max pool size");
			}
			if (this._minPoolSize < 0)
			{
				throw ADP.InvalidConnectionOptionValue("min pool size");
			}
			if (this._maxPoolSize < this._minPoolSize)
			{
				throw ADP.InvalidMinMaxPoolSizeValues();
			}
			if (this._packetSize < 512 || 32768 < this._packetSize)
			{
				throw SQL.InvalidPacketSizeValue();
			}
			this.ValidateValueLength(this._applicationName, 128, "application name");
			this.ValidateValueLength(this._currentLanguage, 128, "current language");
			this.ValidateValueLength(this._dataSource, 128, "data source");
			this.ValidateValueLength(this._failoverPartner, 128, "failover partner");
			this.ValidateValueLength(this._initialCatalog, 128, "initial catalog");
			this.ValidateValueLength(this._password, 128, "password");
			this.ValidateValueLength(this._userID, 128, "user id");
			if (this._workstationId != null)
			{
				this.ValidateValueLength(this._workstationId, 128, "workstation id");
			}
			if (!string.Equals("", this._failoverPartner, StringComparison.OrdinalIgnoreCase))
			{
				if (this._multiSubnetFailover)
				{
					throw SQL.MultiSubnetFailoverWithFailoverPartner(false, null);
				}
				if (string.Equals("", this._initialCatalog, StringComparison.OrdinalIgnoreCase))
				{
					throw ADP.MissingConnectionOptionValue("failover partner", "initial catalog");
				}
			}
			if (0 <= this._attachDBFileName.IndexOf('|'))
			{
				throw ADP.InvalidConnectionOptionValue("attachdbfilename");
			}
			this.ValidateValueLength(this._attachDBFileName, 260, "attachdbfilename");
			if (this._userInstance && !string.IsNullOrEmpty(this._failoverPartner))
			{
				throw SQL.UserInstanceFailoverNotCompatible();
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "Latest";
			}
			if (text.Equals("Latest", StringComparison.OrdinalIgnoreCase))
			{
				this._typeSystemVersion = SqlConnectionString.TypeSystem.Latest;
			}
			else if (text.Equals("SQL Server 2000", StringComparison.OrdinalIgnoreCase))
			{
				this._typeSystemVersion = SqlConnectionString.TypeSystem.SQLServer2000;
			}
			else if (text.Equals("SQL Server 2005", StringComparison.OrdinalIgnoreCase))
			{
				this._typeSystemVersion = SqlConnectionString.TypeSystem.SQLServer2005;
			}
			else if (text.Equals("SQL Server 2008", StringComparison.OrdinalIgnoreCase))
			{
				this._typeSystemVersion = SqlConnectionString.TypeSystem.Latest;
			}
			else
			{
				if (!text.Equals("SQL Server 2012", StringComparison.OrdinalIgnoreCase))
				{
					throw ADP.InvalidConnectionOptionValue("type system version");
				}
				this._typeSystemVersion = SqlConnectionString.TypeSystem.SQLServer2012;
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "Implicit Unbind";
			}
			if (text2.Equals("Implicit Unbind", StringComparison.OrdinalIgnoreCase))
			{
				this._transactionBinding = SqlConnectionString.TransactionBindingEnum.ImplicitUnbind;
			}
			else
			{
				if (!text2.Equals("Explicit Unbind", StringComparison.OrdinalIgnoreCase))
				{
					throw ADP.InvalidConnectionOptionValue("transaction binding");
				}
				this._transactionBinding = SqlConnectionString.TransactionBindingEnum.ExplicitUnbind;
			}
			if (this._applicationIntent == ApplicationIntent.ReadOnly && !string.IsNullOrEmpty(this._failoverPartner))
			{
				throw SQL.ROR_FailoverNotSupportedConnString();
			}
			if (this._connectRetryCount < 0 || this._connectRetryCount > 255)
			{
				throw ADP.InvalidConnectRetryCountValue();
			}
			if (this._connectRetryInterval < 1 || this._connectRetryInterval > 60)
			{
				throw ADP.InvalidConnectRetryIntervalValue();
			}
		}

		internal SqlConnectionString(SqlConnectionString connectionOptions, string dataSource, bool userInstance, bool? setEnlistValue)
			: base(connectionOptions)
		{
			this._integratedSecurity = connectionOptions._integratedSecurity;
			this._encrypt = connectionOptions._encrypt;
			if (setEnlistValue != null)
			{
				this._enlist = setEnlistValue.Value;
			}
			else
			{
				this._enlist = connectionOptions._enlist;
			}
			this._mars = connectionOptions._mars;
			this._persistSecurityInfo = connectionOptions._persistSecurityInfo;
			this._pooling = connectionOptions._pooling;
			this._replication = connectionOptions._replication;
			this._userInstance = userInstance;
			this._connectTimeout = connectionOptions._connectTimeout;
			this._loadBalanceTimeout = connectionOptions._loadBalanceTimeout;
			this._maxPoolSize = connectionOptions._maxPoolSize;
			this._minPoolSize = connectionOptions._minPoolSize;
			this._multiSubnetFailover = connectionOptions._multiSubnetFailover;
			this._packetSize = connectionOptions._packetSize;
			this._applicationName = connectionOptions._applicationName;
			this._attachDBFileName = connectionOptions._attachDBFileName;
			this._currentLanguage = connectionOptions._currentLanguage;
			this._dataSource = dataSource;
			this._localDBInstance = LocalDBAPI.GetLocalDbInstanceNameFromServerName(this._dataSource);
			this._failoverPartner = connectionOptions._failoverPartner;
			this._initialCatalog = connectionOptions._initialCatalog;
			this._password = connectionOptions._password;
			this._userID = connectionOptions._userID;
			this._workstationId = connectionOptions._workstationId;
			this._typeSystemVersion = connectionOptions._typeSystemVersion;
			this._transactionBinding = connectionOptions._transactionBinding;
			this._applicationIntent = connectionOptions._applicationIntent;
			this._connectRetryCount = connectionOptions._connectRetryCount;
			this._connectRetryInterval = connectionOptions._connectRetryInterval;
			this.ValidateValueLength(this._dataSource, 128, "data source");
		}

		internal bool IntegratedSecurity
		{
			get
			{
				return this._integratedSecurity;
			}
		}

		internal bool Asynchronous
		{
			get
			{
				return true;
			}
		}

		internal bool ConnectionReset
		{
			get
			{
				return true;
			}
		}

		internal bool Encrypt
		{
			get
			{
				return this._encrypt;
			}
		}

		internal bool TrustServerCertificate
		{
			get
			{
				return this._trustServerCertificate;
			}
		}

		internal bool Enlist
		{
			get
			{
				return this._enlist;
			}
		}

		internal bool MARS
		{
			get
			{
				return this._mars;
			}
		}

		internal bool MultiSubnetFailover
		{
			get
			{
				return this._multiSubnetFailover;
			}
		}

		internal bool PersistSecurityInfo
		{
			get
			{
				return this._persistSecurityInfo;
			}
		}

		internal bool Pooling
		{
			get
			{
				return this._pooling;
			}
		}

		internal bool Replication
		{
			get
			{
				return this._replication;
			}
		}

		internal bool UserInstance
		{
			get
			{
				return this._userInstance;
			}
		}

		internal int ConnectTimeout
		{
			get
			{
				return this._connectTimeout;
			}
		}

		internal int LoadBalanceTimeout
		{
			get
			{
				return this._loadBalanceTimeout;
			}
		}

		internal int MaxPoolSize
		{
			get
			{
				return this._maxPoolSize;
			}
		}

		internal int MinPoolSize
		{
			get
			{
				return this._minPoolSize;
			}
		}

		internal int PacketSize
		{
			get
			{
				return this._packetSize;
			}
		}

		internal int ConnectRetryCount
		{
			get
			{
				return this._connectRetryCount;
			}
		}

		internal int ConnectRetryInterval
		{
			get
			{
				return this._connectRetryInterval;
			}
		}

		internal ApplicationIntent ApplicationIntent
		{
			get
			{
				return this._applicationIntent;
			}
		}

		internal string ApplicationName
		{
			get
			{
				return this._applicationName;
			}
		}

		internal string AttachDBFilename
		{
			get
			{
				return this._attachDBFileName;
			}
		}

		internal string CurrentLanguage
		{
			get
			{
				return this._currentLanguage;
			}
		}

		internal string DataSource
		{
			get
			{
				return this._dataSource;
			}
		}

		internal string LocalDBInstance
		{
			get
			{
				return this._localDBInstance;
			}
		}

		internal string FailoverPartner
		{
			get
			{
				return this._failoverPartner;
			}
		}

		internal string InitialCatalog
		{
			get
			{
				return this._initialCatalog;
			}
		}

		internal string Password
		{
			get
			{
				return this._password;
			}
		}

		internal string UserID
		{
			get
			{
				return this._userID;
			}
		}

		internal string WorkstationId
		{
			get
			{
				return this._workstationId;
			}
		}

		internal SqlConnectionString.TypeSystem TypeSystemVersion
		{
			get
			{
				return this._typeSystemVersion;
			}
		}

		internal SqlConnectionString.TransactionBindingEnum TransactionBinding
		{
			get
			{
				return this._transactionBinding;
			}
		}

		internal static Dictionary<string, string> GetParseSynonyms()
		{
			Dictionary<string, string> dictionary = SqlConnectionString.s_sqlClientSynonyms;
			if (dictionary == null)
			{
				dictionary = new Dictionary<string, string>(54)
				{
					{ "applicationintent", "applicationintent" },
					{ "application name", "application name" },
					{ "asynchronous processing", "asynchronous processing" },
					{ "attachdbfilename", "attachdbfilename" },
					{ "connect timeout", "connect timeout" },
					{ "connection reset", "connection reset" },
					{ "context connection", "context connection" },
					{ "current language", "current language" },
					{ "data source", "data source" },
					{ "encrypt", "encrypt" },
					{ "enlist", "enlist" },
					{ "failover partner", "failover partner" },
					{ "initial catalog", "initial catalog" },
					{ "integrated security", "integrated security" },
					{ "load balance timeout", "load balance timeout" },
					{ "multipleactiveresultsets", "multipleactiveresultsets" },
					{ "max pool size", "max pool size" },
					{ "min pool size", "min pool size" },
					{ "multisubnetfailover", "multisubnetfailover" },
					{ "network library", "network library" },
					{ "packet size", "packet size" },
					{ "password", "password" },
					{ "persist security info", "persist security info" },
					{ "pooling", "pooling" },
					{ "replication", "replication" },
					{ "trustservercertificate", "trustservercertificate" },
					{ "transaction binding", "transaction binding" },
					{ "type system version", "type system version" },
					{ "user id", "user id" },
					{ "user instance", "user instance" },
					{ "workstation id", "workstation id" },
					{ "connectretrycount", "connectretrycount" },
					{ "connectretryinterval", "connectretryinterval" },
					{ "app", "application name" },
					{ "async", "asynchronous processing" },
					{ "extended properties", "attachdbfilename" },
					{ "initial file name", "attachdbfilename" },
					{ "connection timeout", "connect timeout" },
					{ "timeout", "connect timeout" },
					{ "language", "current language" },
					{ "addr", "data source" },
					{ "address", "data source" },
					{ "network address", "data source" },
					{ "server", "data source" },
					{ "database", "initial catalog" },
					{ "trusted_connection", "integrated security" },
					{ "connection lifetime", "load balance timeout" },
					{ "net", "network library" },
					{ "network", "network library" },
					{ "pwd", "password" },
					{ "persistsecurityinfo", "persist security info" },
					{ "uid", "user id" },
					{ "user", "user id" },
					{ "wsid", "workstation id" }
				};
				SqlConnectionString.s_sqlClientSynonyms = dictionary;
			}
			return dictionary;
		}

		internal string ObtainWorkstationId()
		{
			string text = this.WorkstationId;
			if (text == null)
			{
				text = ADP.MachineName();
				this.ValidateValueLength(text, 128, "workstation id");
			}
			return text;
		}

		private void ValidateValueLength(string value, int limit, string key)
		{
			if (limit < value.Length)
			{
				throw ADP.InvalidConnectionOptionValueLength(key, limit);
			}
		}

		internal ApplicationIntent ConvertValueToApplicationIntent()
		{
			string text;
			if (!base.TryGetParsetableValue("applicationintent", out text))
			{
				return ApplicationIntent.ReadWrite;
			}
			ApplicationIntent applicationIntent;
			try
			{
				applicationIntent = DbConnectionStringBuilderUtil.ConvertToApplicationIntent("applicationintent", text);
			}
			catch (FormatException ex)
			{
				throw ADP.InvalidConnectionOptionValue("applicationintent", ex);
			}
			catch (OverflowException ex2)
			{
				throw ADP.InvalidConnectionOptionValue("applicationintent", ex2);
			}
			return applicationIntent;
		}

		internal void ThrowUnsupportedIfKeywordSet(string keyword)
		{
			if (base.ContainsKey(keyword))
			{
				throw SQL.UnsupportedKeyword(keyword);
			}
		}

		internal const int SynonymCount = 18;

		internal const int DeprecatedSynonymCount = 3;

		private static Dictionary<string, string> s_sqlClientSynonyms;

		private readonly bool _integratedSecurity;

		private readonly bool _encrypt;

		private readonly bool _trustServerCertificate;

		private readonly bool _enlist;

		private readonly bool _mars;

		private readonly bool _persistSecurityInfo;

		private readonly bool _pooling;

		private readonly bool _replication;

		private readonly bool _userInstance;

		private readonly bool _multiSubnetFailover;

		private readonly int _connectTimeout;

		private readonly int _loadBalanceTimeout;

		private readonly int _maxPoolSize;

		private readonly int _minPoolSize;

		private readonly int _packetSize;

		private readonly int _connectRetryCount;

		private readonly int _connectRetryInterval;

		private readonly ApplicationIntent _applicationIntent;

		private readonly string _applicationName;

		private readonly string _attachDBFileName;

		private readonly string _currentLanguage;

		private readonly string _dataSource;

		private readonly string _localDBInstance;

		private readonly string _failoverPartner;

		private readonly string _initialCatalog;

		private readonly string _password;

		private readonly string _userID;

		private readonly string _workstationId;

		private readonly SqlConnectionString.TypeSystem _typeSystemVersion;

		private readonly SqlConnectionString.TransactionBindingEnum _transactionBinding;

		internal static class DEFAULT
		{
			internal const ApplicationIntent ApplicationIntent = ApplicationIntent.ReadWrite;

			internal const string Application_Name = "Core .Net SqlClient Data Provider";

			internal const string AttachDBFilename = "";

			internal const int Connect_Timeout = 15;

			internal const string Current_Language = "";

			internal const string Data_Source = "";

			internal const bool Encrypt = false;

			internal const bool Enlist = true;

			internal const string FailoverPartner = "";

			internal const string Initial_Catalog = "";

			internal const bool Integrated_Security = false;

			internal const int Load_Balance_Timeout = 0;

			internal const bool MARS = false;

			internal const int Max_Pool_Size = 100;

			internal const int Min_Pool_Size = 0;

			internal const bool MultiSubnetFailover = false;

			internal const int Packet_Size = 8000;

			internal const string Password = "";

			internal const bool Persist_Security_Info = false;

			internal const bool Pooling = true;

			internal const bool TrustServerCertificate = false;

			internal const string Type_System_Version = "";

			internal const string User_ID = "";

			internal const bool User_Instance = false;

			internal const bool Replication = false;

			internal const int Connect_Retry_Count = 1;

			internal const int Connect_Retry_Interval = 10;
		}

		internal static class KEY
		{
			internal const string ApplicationIntent = "applicationintent";

			internal const string Application_Name = "application name";

			internal const string AsynchronousProcessing = "asynchronous processing";

			internal const string AttachDBFilename = "attachdbfilename";

			internal const string Connect_Timeout = "connect timeout";

			internal const string Connection_Reset = "connection reset";

			internal const string Context_Connection = "context connection";

			internal const string Current_Language = "current language";

			internal const string Data_Source = "data source";

			internal const string Encrypt = "encrypt";

			internal const string Enlist = "enlist";

			internal const string FailoverPartner = "failover partner";

			internal const string Initial_Catalog = "initial catalog";

			internal const string Integrated_Security = "integrated security";

			internal const string Load_Balance_Timeout = "load balance timeout";

			internal const string MARS = "multipleactiveresultsets";

			internal const string Max_Pool_Size = "max pool size";

			internal const string Min_Pool_Size = "min pool size";

			internal const string MultiSubnetFailover = "multisubnetfailover";

			internal const string Network_Library = "network library";

			internal const string Packet_Size = "packet size";

			internal const string Password = "password";

			internal const string Persist_Security_Info = "persist security info";

			internal const string Pooling = "pooling";

			internal const string TransactionBinding = "transaction binding";

			internal const string TrustServerCertificate = "trustservercertificate";

			internal const string Type_System_Version = "type system version";

			internal const string User_ID = "user id";

			internal const string User_Instance = "user instance";

			internal const string Workstation_Id = "workstation id";

			internal const string Replication = "replication";

			internal const string Connect_Retry_Count = "connectretrycount";

			internal const string Connect_Retry_Interval = "connectretryinterval";
		}

		private static class SYNONYM
		{
			internal const string APP = "app";

			internal const string Async = "async";

			internal const string EXTENDED_PROPERTIES = "extended properties";

			internal const string INITIAL_FILE_NAME = "initial file name";

			internal const string CONNECTION_TIMEOUT = "connection timeout";

			internal const string TIMEOUT = "timeout";

			internal const string LANGUAGE = "language";

			internal const string ADDR = "addr";

			internal const string ADDRESS = "address";

			internal const string SERVER = "server";

			internal const string NETWORK_ADDRESS = "network address";

			internal const string DATABASE = "database";

			internal const string TRUSTED_CONNECTION = "trusted_connection";

			internal const string Connection_Lifetime = "connection lifetime";

			internal const string NET = "net";

			internal const string NETWORK = "network";

			internal const string Pwd = "pwd";

			internal const string PERSISTSECURITYINFO = "persistsecurityinfo";

			internal const string UID = "uid";

			internal const string User = "user";

			internal const string WSID = "wsid";
		}

		internal enum TypeSystem
		{
			Latest = 2008,
			SQLServer2000 = 2000,
			SQLServer2005 = 2005,
			SQLServer2008 = 2008,
			SQLServer2012 = 2012
		}

		internal static class TYPESYSTEMVERSION
		{
			internal const string Latest = "Latest";

			internal const string SQL_Server_2000 = "SQL Server 2000";

			internal const string SQL_Server_2005 = "SQL Server 2005";

			internal const string SQL_Server_2008 = "SQL Server 2008";

			internal const string SQL_Server_2012 = "SQL Server 2012";
		}

		internal enum TransactionBindingEnum
		{
			ImplicitUnbind,
			ExplicitUnbind
		}

		internal static class TRANSACTIONBINDING
		{
			internal const string ImplicitUnbind = "Implicit Unbind";

			internal const string ExplicitUnbind = "Explicit Unbind";
		}
	}
}
