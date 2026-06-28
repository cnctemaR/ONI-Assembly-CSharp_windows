using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.SqlClient
{
	[DefaultProperty("DataSource")]
	[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlConnectionStringBuilderConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class SqlConnectionStringBuilder : DbConnectionStringBuilder
	{
		public SqlConnectionStringBuilder()
			: this(string.Empty)
		{
		}

		public SqlConnectionStringBuilder(string connectionString)
		{
			this.Init();
			base.ConnectionString = connectionString;
		}

		static SqlConnectionStringBuilder()
		{
			SqlConnectionStringBuilder._keywords["APP"] = "Application Name";
			SqlConnectionStringBuilder._keywords["APPLICATION NAME"] = "Application Name";
			SqlConnectionStringBuilder._keywords["ATTACHDBFILENAME"] = "AttachDbFilename";
			SqlConnectionStringBuilder._keywords["EXTENDED PROPERTIES"] = "Extended Properties";
			SqlConnectionStringBuilder._keywords["INITIAL FILE NAME"] = "Initial File Name";
			SqlConnectionStringBuilder._keywords["TIMEOUT"] = "Connect Timeout";
			SqlConnectionStringBuilder._keywords["CONNECT TIMEOUT"] = "Connect Timeout";
			SqlConnectionStringBuilder._keywords["CONNECTION TIMEOUT"] = "Connect Timeout";
			SqlConnectionStringBuilder._keywords["CONNECTION RESET"] = "Connection Reset";
			SqlConnectionStringBuilder._keywords["LANGUAGE"] = "Current Language";
			SqlConnectionStringBuilder._keywords["CURRENT LANGUAGE"] = "Current Language";
			SqlConnectionStringBuilder._keywords["DATA SOURCE"] = "Data Source";
			SqlConnectionStringBuilder._keywords["SERVER"] = "Data Source";
			SqlConnectionStringBuilder._keywords["ADDRESS"] = "Data Source";
			SqlConnectionStringBuilder._keywords["ADDR"] = "Data Source";
			SqlConnectionStringBuilder._keywords["NETWORK ADDRESS"] = "Data Source";
			SqlConnectionStringBuilder._keywords["ENCRYPT"] = "Encrypt";
			SqlConnectionStringBuilder._keywords["ENLIST"] = "Enlist";
			SqlConnectionStringBuilder._keywords["INITIAL CATALOG"] = "Initial Catalog";
			SqlConnectionStringBuilder._keywords["DATABASE"] = "Initial Catalog";
			SqlConnectionStringBuilder._keywords["INTEGRATED SECURITY"] = "Integrated Security";
			SqlConnectionStringBuilder._keywords["TRUSTED_CONNECTION"] = "Integrated Security";
			SqlConnectionStringBuilder._keywords["MAX POOL SIZE"] = "Max Pool Size";
			SqlConnectionStringBuilder._keywords["MIN POOL SIZE"] = "Min Pool Size";
			SqlConnectionStringBuilder._keywords["MULTIPLEACTIVERESULTSETS"] = "MultipleActiveResultSets";
			SqlConnectionStringBuilder._keywords["ASYNCHRONOUS PROCESSING"] = "Asynchronous Processing";
			SqlConnectionStringBuilder._keywords["ASYNC"] = "Async";
			SqlConnectionStringBuilder._keywords["NET"] = "Network Library";
			SqlConnectionStringBuilder._keywords["NETWORK"] = "Network Library";
			SqlConnectionStringBuilder._keywords["NETWORK LIBRARY"] = "Network Library";
			SqlConnectionStringBuilder._keywords["PACKET SIZE"] = "Packet Size";
			SqlConnectionStringBuilder._keywords["PASSWORD"] = "Password";
			SqlConnectionStringBuilder._keywords["PWD"] = "Password";
			SqlConnectionStringBuilder._keywords["PERSISTSECURITYINFO"] = "Persist Security Info";
			SqlConnectionStringBuilder._keywords["PERSIST SECURITY INFO"] = "Persist Security Info";
			SqlConnectionStringBuilder._keywords["POOLING"] = "Pooling";
			SqlConnectionStringBuilder._keywords["UID"] = "User ID";
			SqlConnectionStringBuilder._keywords["USER"] = "User ID";
			SqlConnectionStringBuilder._keywords["USER ID"] = "User ID";
			SqlConnectionStringBuilder._keywords["WSID"] = "Workstation ID";
			SqlConnectionStringBuilder._keywords["WORKSTATION ID"] = "Workstation ID";
			SqlConnectionStringBuilder._keywords["USER INSTANCE"] = "User Instance";
			SqlConnectionStringBuilder._keywords["CONTEXT CONNECTION"] = "Context Connection";
			SqlConnectionStringBuilder._keywords["TRANSACTION BINDING"] = "Transaction Binding";
			SqlConnectionStringBuilder._keywords["FAILOVER PARTNER"] = "Failover Partner";
			SqlConnectionStringBuilder._keywords["REPLICATION"] = "Replication";
			SqlConnectionStringBuilder._keywords["TRUSTSERVERCERTIFICATE"] = "TrustServerCertificate";
			SqlConnectionStringBuilder._keywords["LOAD BALANCE TIMEOUT"] = "Load Balance Timeout";
			SqlConnectionStringBuilder._keywords["TYPE SYSTEM VERSION"] = "Type System Version";
			SqlConnectionStringBuilder._defaults = new Dictionary<string, object>();
			SqlConnectionStringBuilder._defaults.Add("Data Source", string.Empty);
			SqlConnectionStringBuilder._defaults.Add("Failover Partner", string.Empty);
			SqlConnectionStringBuilder._defaults.Add("AttachDbFilename", string.Empty);
			SqlConnectionStringBuilder._defaults.Add("Initial Catalog", string.Empty);
			SqlConnectionStringBuilder._defaults.Add("Integrated Security", false);
			SqlConnectionStringBuilder._defaults.Add("Persist Security Info", false);
			SqlConnectionStringBuilder._defaults.Add("User ID", string.Empty);
			SqlConnectionStringBuilder._defaults.Add("Password", string.Empty);
			SqlConnectionStringBuilder._defaults.Add("Enlist", false);
			SqlConnectionStringBuilder._defaults.Add("Pooling", true);
			SqlConnectionStringBuilder._defaults.Add("Min Pool Size", 0);
			SqlConnectionStringBuilder._defaults.Add("Max Pool Size", 100);
			SqlConnectionStringBuilder._defaults.Add("Asynchronous Processing", false);
			SqlConnectionStringBuilder._defaults.Add("Connection Reset", true);
			SqlConnectionStringBuilder._defaults.Add("MultipleActiveResultSets", false);
			SqlConnectionStringBuilder._defaults.Add("Replication", false);
			SqlConnectionStringBuilder._defaults.Add("Connect Timeout", 15);
			SqlConnectionStringBuilder._defaults.Add("Encrypt", false);
			SqlConnectionStringBuilder._defaults.Add("TrustServerCertificate", false);
			SqlConnectionStringBuilder._defaults.Add("Load Balance Timeout", 0);
			SqlConnectionStringBuilder._defaults.Add("Network Library", string.Empty);
			SqlConnectionStringBuilder._defaults.Add("Packet Size", 8000);
			SqlConnectionStringBuilder._defaults.Add("Type System Version", "Latest");
			SqlConnectionStringBuilder._defaults.Add("Application Name", ".NET SqlClient Data Provider");
			SqlConnectionStringBuilder._defaults.Add("Current Language", string.Empty);
			SqlConnectionStringBuilder._defaults.Add("Workstation ID", string.Empty);
			SqlConnectionStringBuilder._defaults.Add("User Instance", false);
			SqlConnectionStringBuilder._defaults.Add("Context Connection", false);
			SqlConnectionStringBuilder._defaults.Add("Transaction Binding", "Implicit Unbind");
		}

		[DisplayName("Application Name")]
		[RefreshProperties(RefreshProperties.All)]
		public string ApplicationName
		{
			get
			{
				return this._applicationName;
			}
			set
			{
				base["Application Name"] = value;
				this._applicationName = value;
			}
		}

		[DisplayName("Asynchronous Processing")]
		[RefreshProperties(RefreshProperties.All)]
		public bool AsynchronousProcessing
		{
			get
			{
				return this._asynchronousProcessing;
			}
			set
			{
				base["Asynchronous Processing"] = value;
				this._asynchronousProcessing = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("AttachDbFilename")]
		[Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public string AttachDBFilename
		{
			get
			{
				return this._attachDBFilename;
			}
			set
			{
				base["AttachDbFilename"] = value;
				this._attachDBFilename = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Connection Reset")]
		public bool ConnectionReset
		{
			get
			{
				return this._connectionReset;
			}
			set
			{
				base["Connection Reset"] = value;
				this._connectionReset = value;
			}
		}

		[DisplayName("Connect Timeout")]
		[RefreshProperties(RefreshProperties.All)]
		public int ConnectTimeout
		{
			get
			{
				return this._connectTimeout;
			}
			set
			{
				base["Connect Timeout"] = value;
				this._connectTimeout = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Current Language")]
		public string CurrentLanguage
		{
			get
			{
				return this._currentLanguage;
			}
			set
			{
				base["Current Language"] = value;
				this._currentLanguage = value;
			}
		}

		[DisplayName("Data Source")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlDataSourceConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		public string DataSource
		{
			get
			{
				return this._dataSource;
			}
			set
			{
				base["Data Source"] = value;
				this._dataSource = value;
			}
		}

		[DisplayName("Encrypt")]
		[RefreshProperties(RefreshProperties.All)]
		public bool Encrypt
		{
			get
			{
				return this._encrypt;
			}
			set
			{
				base["Encrypt"] = value;
				this._encrypt = value;
			}
		}

		[DisplayName("Enlist")]
		[RefreshProperties(RefreshProperties.All)]
		public bool Enlist
		{
			get
			{
				return this._enlist;
			}
			set
			{
				base["Enlist"] = value;
				this._enlist = value;
			}
		}

		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlDataSourceConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		[DisplayName("Failover Partner")]
		[RefreshProperties(RefreshProperties.All)]
		public string FailoverPartner
		{
			get
			{
				return this._failoverPartner;
			}
			set
			{
				base["Failover Partner"] = value;
				this._failoverPartner = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlInitialCatalogConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		[DisplayName("Initial Catalog")]
		public string InitialCatalog
		{
			get
			{
				return this._initialCatalog;
			}
			set
			{
				base["Initial Catalog"] = value;
				this._initialCatalog = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Integrated Security")]
		public bool IntegratedSecurity
		{
			get
			{
				return this._integratedSecurity;
			}
			set
			{
				base["Integrated Security"] = value;
				this._integratedSecurity = value;
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public override object this[string keyword]
		{
			get
			{
				string text = this.MapKeyword(keyword);
				if (base.ContainsKey(text))
				{
					return base[text];
				}
				return SqlConnectionStringBuilder._defaults[text];
			}
			set
			{
				this.SetValue(keyword, value);
			}
		}

		public override ICollection Keys
		{
			get
			{
				return new ReadOnlyCollection<string>(new List<string>
				{
					"Data Source", "Failover Partner", "AttachDbFilename", "Initial Catalog", "Integrated Security", "Persist Security Info", "User ID", "Password", "Enlist", "Pooling",
					"Min Pool Size", "Max Pool Size", "Asynchronous Processing", "Connection Reset", "MultipleActiveResultSets", "Replication", "Connect Timeout", "Encrypt", "TrustServerCertificate", "Load Balance Timeout",
					"Network Library", "Packet Size", "Type System Version", "Application Name", "Current Language", "Workstation ID", "User Instance", "Context Connection", "Transaction Binding"
				});
			}
		}

		[DisplayName("Load Balance Timeout")]
		[RefreshProperties(RefreshProperties.All)]
		public int LoadBalanceTimeout
		{
			get
			{
				return this._loadBalanceTimeout;
			}
			set
			{
				base["Load Balance Timeout"] = value;
				this._loadBalanceTimeout = value;
			}
		}

		[DisplayName("Max Pool Size")]
		[RefreshProperties(RefreshProperties.All)]
		public int MaxPoolSize
		{
			get
			{
				return this._maxPoolSize;
			}
			set
			{
				base["Max Pool Size"] = value;
				this._maxPoolSize = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Min Pool Size")]
		public int MinPoolSize
		{
			get
			{
				return this._minPoolSize;
			}
			set
			{
				base["Min Pool Size"] = value;
				this._minPoolSize = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("MultipleActiveResultSets")]
		public bool MultipleActiveResultSets
		{
			get
			{
				return this._multipleActiveResultSets;
			}
			set
			{
				base["Multiple Active Resultsets"] = value;
				this._multipleActiveResultSets = value;
			}
		}

		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+NetworkLibraryConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		[DisplayName("Network Library")]
		[RefreshProperties(RefreshProperties.All)]
		public string NetworkLibrary
		{
			get
			{
				return this._networkLibrary;
			}
			set
			{
				base["Network Library"] = value;
				this._networkLibrary = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Packet Size")]
		public int PacketSize
		{
			get
			{
				return this._packetSize;
			}
			set
			{
				base["Packet Size"] = value;
				this._packetSize = value;
			}
		}

		[DisplayName("Password")]
		[PasswordPropertyText(true)]
		[RefreshProperties(RefreshProperties.All)]
		public string Password
		{
			get
			{
				return this._password;
			}
			set
			{
				base["Password"] = value;
				this._password = value;
			}
		}

		[DisplayName("Persist Security Info")]
		[RefreshProperties(RefreshProperties.All)]
		public bool PersistSecurityInfo
		{
			get
			{
				return this._persistSecurityInfo;
			}
			set
			{
				base["Persist Security Info"] = value;
				this._persistSecurityInfo = value;
			}
		}

		[DisplayName("Pooling")]
		[RefreshProperties(RefreshProperties.All)]
		public bool Pooling
		{
			get
			{
				return this._pooling;
			}
			set
			{
				base["Pooling"] = value;
				this._pooling = value;
			}
		}

		[DisplayName("Replication")]
		[RefreshProperties(RefreshProperties.All)]
		public bool Replication
		{
			get
			{
				return this._replication;
			}
			set
			{
				base["Replication"] = value;
				this._replication = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("User ID")]
		public string UserID
		{
			get
			{
				return this._userID;
			}
			set
			{
				base["User Id"] = value;
				this._userID = value;
			}
		}

		public override ICollection Values
		{
			get
			{
				return new ReadOnlyCollection<object>(new List<object>
				{
					this._dataSource, this._failoverPartner, this._attachDBFilename, this._initialCatalog, this._integratedSecurity, this._persistSecurityInfo, this._userID, this._password, this._enlist, this._pooling,
					this._minPoolSize, this._maxPoolSize, this._asynchronousProcessing, this._connectionReset, this._multipleActiveResultSets, this._replication, this._connectTimeout, this._encrypt, this._trustServerCertificate, this._loadBalanceTimeout,
					this._networkLibrary, this._packetSize, this._typeSystemVersion, this._applicationName, this._currentLanguage, this._workstationID, this._userInstance, this._contextConnection, this._transactionBinding
				});
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Workstation ID")]
		public string WorkstationID
		{
			get
			{
				return this._workstationID;
			}
			set
			{
				base["Workstation Id"] = value;
				this._workstationID = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("TrustServerCertificate")]
		public bool TrustServerCertificate
		{
			get
			{
				return this._trustServerCertificate;
			}
			set
			{
				base["Trust Server Certificate"] = value;
				this._trustServerCertificate = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Type System Version")]
		public string TypeSystemVersion
		{
			get
			{
				return this._typeSystemVersion;
			}
			set
			{
				base["Type System Version"] = value;
				this._typeSystemVersion = value;
			}
		}

		[DisplayName("User Instance")]
		[RefreshProperties(RefreshProperties.All)]
		public bool UserInstance
		{
			get
			{
				return this._userInstance;
			}
			set
			{
				base["User Instance"] = value;
				this._userInstance = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Context Connection")]
		public bool ContextConnection
		{
			get
			{
				return this._contextConnection;
			}
			set
			{
				base["Context Connection"] = value;
				this._contextConnection = value;
			}
		}

		private void Init()
		{
			this._applicationName = ".NET SqlClient Data Provider";
			this._asynchronousProcessing = false;
			this._attachDBFilename = string.Empty;
			this._connectionReset = true;
			this._connectTimeout = 15;
			this._currentLanguage = string.Empty;
			this._dataSource = string.Empty;
			this._encrypt = false;
			this._enlist = false;
			this._failoverPartner = string.Empty;
			this._initialCatalog = string.Empty;
			this._integratedSecurity = false;
			this._loadBalanceTimeout = 0;
			this._maxPoolSize = 100;
			this._minPoolSize = 0;
			this._multipleActiveResultSets = false;
			this._networkLibrary = string.Empty;
			this._packetSize = 8000;
			this._password = string.Empty;
			this._persistSecurityInfo = false;
			this._pooling = true;
			this._replication = false;
			this._userID = string.Empty;
			this._workstationID = string.Empty;
			this._trustServerCertificate = false;
			this._typeSystemVersion = "Latest";
			this._userInstance = false;
			this._contextConnection = false;
			this._transactionBinding = "Implicit Unbind";
		}

		public override void Clear()
		{
			base.Clear();
			this.Init();
		}

		public override bool ContainsKey(string keyword)
		{
			keyword = keyword.ToUpper().Trim();
			return SqlConnectionStringBuilder._keywords.ContainsKey(keyword) && base.ContainsKey(SqlConnectionStringBuilder._keywords[keyword]);
		}

		public override bool Remove(string keyword)
		{
			if (!this.ContainsKey(keyword))
			{
				return false;
			}
			this[keyword] = null;
			return true;
		}

		[MonoNotSupported("")]
		public override bool ShouldSerialize(string keyword)
		{
			if (!this.ContainsKey(keyword))
			{
				return false;
			}
			keyword = keyword.ToUpper().Trim();
			return !(SqlConnectionStringBuilder._keywords[keyword] == "Password") && base.ShouldSerialize(SqlConnectionStringBuilder._keywords[keyword]);
		}

		public override bool TryGetValue(string keyword, out object value)
		{
			if (!this.ContainsKey(keyword))
			{
				value = string.Empty;
				return false;
			}
			return base.TryGetValue(SqlConnectionStringBuilder._keywords[keyword.ToUpper().Trim()], out value);
		}

		private string MapKeyword(string keyword)
		{
			keyword = keyword.ToUpper().Trim();
			if (!SqlConnectionStringBuilder._keywords.ContainsKey(keyword))
			{
				throw new ArgumentException("Keyword not supported :" + keyword);
			}
			return SqlConnectionStringBuilder._keywords[keyword];
		}

		private void SetValue(string key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key cannot be null!");
			}
			string text = this.MapKeyword(key);
			string text2 = text.ToUpper().Trim();
			if (text2 != null)
			{
				if (SqlConnectionStringBuilder.<>f__switch$map8 == null)
				{
					SqlConnectionStringBuilder.<>f__switch$map8 = new Dictionary<string, int>(26)
					{
						{ "APPLICATION NAME", 0 },
						{ "ATTACHDBFILENAME", 1 },
						{ "CONNECT TIMEOUT", 2 },
						{ "CONNECTION LIFETIME", 3 },
						{ "CONNECTION RESET", 4 },
						{ "CURRENT LANGUAGE", 5 },
						{ "CONTEXT CONNECTION", 6 },
						{ "DATA SOURCE", 7 },
						{ "ENCRYPT", 8 },
						{ "ENLIST", 9 },
						{ "INITIAL CATALOG", 10 },
						{ "INTEGRATED SECURITY", 11 },
						{ "MAX POOL SIZE", 12 },
						{ "MIN POOL SIZE", 13 },
						{ "MULTIPLEACTIVERESULTSETS", 14 },
						{ "ASYNCHRONOUS PROCESSING", 15 },
						{ "NETWORK LIBRARY", 16 },
						{ "LOAD BALANCE TIMEOUT", 17 },
						{ "PACKET SIZE", 18 },
						{ "PASSWORD", 19 },
						{ "PERSIST SECURITY INFO", 20 },
						{ "POOLING", 21 },
						{ "USER ID", 22 },
						{ "USER INSTANCE", 23 },
						{ "WORKSTATION ID", 24 },
						{ "TRANSACTION BINDING", 25 }
					};
				}
				int num;
				if (SqlConnectionStringBuilder.<>f__switch$map8.TryGetValue(text2, out num))
				{
					switch (num)
					{
					case 0:
						if (value == null)
						{
							this._applicationName = ".NET SqlClient Data Provider";
							base.Remove(text);
						}
						else
						{
							this.ApplicationName = value.ToString();
						}
						break;
					case 1:
						throw new NotImplementedException("Attachable database support is not implemented.");
					case 2:
						if (value == null)
						{
							this._connectTimeout = 15;
							base.Remove(text);
						}
						else
						{
							this.ConnectTimeout = DbConnectionStringBuilderHelper.ConvertToInt32(value);
						}
						break;
					case 3:
						break;
					case 4:
						if (value == null)
						{
							this._connectionReset = true;
							base.Remove(text);
						}
						else
						{
							this.ConnectionReset = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
						}
						break;
					case 5:
						if (value == null)
						{
							this._currentLanguage = string.Empty;
							base.Remove(text);
						}
						else
						{
							this.CurrentLanguage = value.ToString();
						}
						break;
					case 6:
						if (value == null)
						{
							this._contextConnection = false;
							base.Remove(text);
						}
						else
						{
							this.ContextConnection = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
						}
						break;
					case 7:
						if (value == null)
						{
							this._dataSource = string.Empty;
							base.Remove(text);
						}
						else
						{
							this.DataSource = value.ToString();
						}
						break;
					case 8:
						if (value == null)
						{
							this._encrypt = false;
							base.Remove(text);
						}
						else if (DbConnectionStringBuilderHelper.ConvertToBoolean(value))
						{
							throw new NotImplementedException("SSL encryption for data sent between client and server is not implemented.");
						}
						break;
					case 9:
						if (value == null)
						{
							this._enlist = false;
							base.Remove(text);
						}
						else if (!DbConnectionStringBuilderHelper.ConvertToBoolean(value))
						{
							throw new NotImplementedException("Disabling the automatic enlistment of connections in the thread's current transaction context is not implemented.");
						}
						break;
					case 10:
						if (value == null)
						{
							this._initialCatalog = string.Empty;
							base.Remove(text);
						}
						else
						{
							this.InitialCatalog = value.ToString();
						}
						break;
					case 11:
						if (value == null)
						{
							this._integratedSecurity = false;
							base.Remove(text);
						}
						else
						{
							this.IntegratedSecurity = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
						}
						break;
					case 12:
						if (value == null)
						{
							this._maxPoolSize = 100;
							base.Remove(text);
						}
						else
						{
							this.MaxPoolSize = DbConnectionStringBuilderHelper.ConvertToInt32(value);
						}
						break;
					case 13:
						if (value == null)
						{
							this._minPoolSize = 0;
							base.Remove(text);
						}
						else
						{
							this.MinPoolSize = DbConnectionStringBuilderHelper.ConvertToInt32(value);
						}
						break;
					case 14:
						if (value == null)
						{
							this._multipleActiveResultSets = false;
							base.Remove(text);
						}
						else if (DbConnectionStringBuilderHelper.ConvertToBoolean(value))
						{
							throw new NotImplementedException("MARS is not yet implemented!");
						}
						break;
					case 15:
						if (value == null)
						{
							this._asynchronousProcessing = false;
							base.Remove(text);
						}
						else
						{
							this.AsynchronousProcessing = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
						}
						break;
					case 16:
						if (value == null)
						{
							this._networkLibrary = string.Empty;
							base.Remove(text);
						}
						else
						{
							if (!value.ToString().ToUpper().Equals("DBMSSOCN"))
							{
								throw new ArgumentException("Unsupported network library.");
							}
							this.NetworkLibrary = value.ToString().ToLower();
						}
						break;
					case 17:
						break;
					case 18:
						if (value == null)
						{
							this._packetSize = 8000;
							base.Remove(text);
						}
						else
						{
							this.PacketSize = DbConnectionStringBuilderHelper.ConvertToInt32(value);
						}
						break;
					case 19:
						if (value == null)
						{
							this._password = string.Empty;
							base.Remove(text);
						}
						else
						{
							this.Password = value.ToString();
						}
						break;
					case 20:
						if (value == null)
						{
							this._persistSecurityInfo = false;
							base.Remove(text);
						}
						else if (DbConnectionStringBuilderHelper.ConvertToBoolean(value))
						{
							throw new NotImplementedException("Persisting security info is not yet implemented");
						}
						break;
					case 21:
						if (value == null)
						{
							this._pooling = true;
							base.Remove(text);
						}
						else
						{
							this.Pooling = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
						}
						break;
					case 22:
						if (value == null)
						{
							this._userID = string.Empty;
							base.Remove(text);
						}
						else
						{
							this.UserID = value.ToString();
						}
						break;
					case 23:
						if (value == null)
						{
							this._userInstance = false;
							base.Remove(text);
						}
						else
						{
							this.UserInstance = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
						}
						break;
					case 24:
						if (value == null)
						{
							this._workstationID = string.Empty;
							base.Remove(text);
						}
						else
						{
							this.WorkstationID = value.ToString();
						}
						break;
					case 25:
						break;
					default:
						goto IL_0655;
					}
					return;
				}
			}
			IL_0655:
			throw new ArgumentException("Keyword not supported :" + key);
		}

		private const string DEF_APPLICATIONNAME = ".NET SqlClient Data Provider";

		private const bool DEF_ASYNCHRONOUSPROCESSING = false;

		private const string DEF_ATTACHDBFILENAME = "";

		private const bool DEF_CONNECTIONRESET = true;

		private const int DEF_CONNECTTIMEOUT = 15;

		private const string DEF_CURRENTLANGUAGE = "";

		private const string DEF_DATASOURCE = "";

		private const bool DEF_ENCRYPT = false;

		private const bool DEF_ENLIST = false;

		private const string DEF_FAILOVERPARTNER = "";

		private const string DEF_INITIALCATALOG = "";

		private const bool DEF_INTEGRATEDSECURITY = false;

		private const int DEF_LOADBALANCETIMEOUT = 0;

		private const int DEF_MAXPOOLSIZE = 100;

		private const int DEF_MINPOOLSIZE = 0;

		private const bool DEF_MULTIPLEACTIVERESULTSETS = false;

		private const string DEF_NETWORKLIBRARY = "";

		private const int DEF_PACKETSIZE = 8000;

		private const string DEF_PASSWORD = "";

		private const bool DEF_PERSISTSECURITYINFO = false;

		private const bool DEF_POOLING = true;

		private const bool DEF_REPLICATION = false;

		private const string DEF_USERID = "";

		private const string DEF_WORKSTATIONID = "";

		private const string DEF_TYPESYSTEMVERSION = "Latest";

		private const bool DEF_TRUSTSERVERCERTIFICATE = false;

		private const bool DEF_USERINSTANCE = false;

		private const bool DEF_CONTEXTCONNECTION = false;

		private const string DEF_TRANSACTIONBINDING = "Implicit Unbind";

		private string _applicationName;

		private bool _asynchronousProcessing;

		private string _attachDBFilename;

		private bool _connectionReset;

		private int _connectTimeout;

		private string _currentLanguage;

		private string _dataSource;

		private bool _encrypt;

		private bool _enlist;

		private string _failoverPartner;

		private string _initialCatalog;

		private bool _integratedSecurity;

		private int _loadBalanceTimeout;

		private int _maxPoolSize;

		private int _minPoolSize;

		private bool _multipleActiveResultSets;

		private string _networkLibrary;

		private int _packetSize;

		private string _password;

		private bool _persistSecurityInfo;

		private bool _pooling;

		private bool _replication;

		private string _userID;

		private string _workstationID;

		private bool _trustServerCertificate;

		private string _typeSystemVersion;

		private bool _userInstance;

		private bool _contextConnection;

		private string _transactionBinding;

		private static Dictionary<string, string> _keywords = new Dictionary<string, string>();

		private static Dictionary<string, object> _defaults;
	}
}
