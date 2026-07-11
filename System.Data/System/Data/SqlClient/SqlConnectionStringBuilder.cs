using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using Unity;

namespace System.Data.SqlClient
{
	public sealed class SqlConnectionStringBuilder : DbConnectionStringBuilder
	{
		private static string[] CreateValidKeywords()
		{
			string[] array = new string[29];
			array[25] = "ApplicationIntent";
			array[20] = "Application Name";
			array[2] = "AttachDbFilename";
			array[14] = "Connect Timeout";
			array[21] = "Current Language";
			array[0] = "Data Source";
			array[15] = "Encrypt";
			array[8] = "Enlist";
			array[1] = "Failover Partner";
			array[3] = "Initial Catalog";
			array[4] = "Integrated Security";
			array[17] = "Load Balance Timeout";
			array[11] = "Max Pool Size";
			array[10] = "Min Pool Size";
			array[12] = "MultipleActiveResultSets";
			array[26] = "MultiSubnetFailover";
			array[18] = "Packet Size";
			array[7] = "Password";
			array[5] = "Persist Security Info";
			array[9] = "Pooling";
			array[13] = "Replication";
			array[24] = "Transaction Binding";
			array[16] = "TrustServerCertificate";
			array[19] = "Type System Version";
			array[6] = "User ID";
			array[23] = "User Instance";
			array[22] = "Workstation ID";
			array[27] = "ConnectRetryCount";
			array[28] = "ConnectRetryInterval";
			return array;
		}

		private static Dictionary<string, SqlConnectionStringBuilder.Keywords> CreateKeywordsDictionary()
		{
			return new Dictionary<string, SqlConnectionStringBuilder.Keywords>(47, StringComparer.OrdinalIgnoreCase)
			{
				{
					"ApplicationIntent",
					SqlConnectionStringBuilder.Keywords.ApplicationIntent
				},
				{
					"Application Name",
					SqlConnectionStringBuilder.Keywords.ApplicationName
				},
				{
					"AttachDbFilename",
					SqlConnectionStringBuilder.Keywords.AttachDBFilename
				},
				{
					"Connect Timeout",
					SqlConnectionStringBuilder.Keywords.ConnectTimeout
				},
				{
					"Current Language",
					SqlConnectionStringBuilder.Keywords.CurrentLanguage
				},
				{
					"Data Source",
					SqlConnectionStringBuilder.Keywords.DataSource
				},
				{
					"Encrypt",
					SqlConnectionStringBuilder.Keywords.Encrypt
				},
				{
					"Enlist",
					SqlConnectionStringBuilder.Keywords.Enlist
				},
				{
					"Failover Partner",
					SqlConnectionStringBuilder.Keywords.FailoverPartner
				},
				{
					"Initial Catalog",
					SqlConnectionStringBuilder.Keywords.InitialCatalog
				},
				{
					"Integrated Security",
					SqlConnectionStringBuilder.Keywords.IntegratedSecurity
				},
				{
					"Load Balance Timeout",
					SqlConnectionStringBuilder.Keywords.LoadBalanceTimeout
				},
				{
					"MultipleActiveResultSets",
					SqlConnectionStringBuilder.Keywords.MultipleActiveResultSets
				},
				{
					"Max Pool Size",
					SqlConnectionStringBuilder.Keywords.MaxPoolSize
				},
				{
					"Min Pool Size",
					SqlConnectionStringBuilder.Keywords.MinPoolSize
				},
				{
					"MultiSubnetFailover",
					SqlConnectionStringBuilder.Keywords.MultiSubnetFailover
				},
				{
					"Packet Size",
					SqlConnectionStringBuilder.Keywords.PacketSize
				},
				{
					"Password",
					SqlConnectionStringBuilder.Keywords.Password
				},
				{
					"Persist Security Info",
					SqlConnectionStringBuilder.Keywords.PersistSecurityInfo
				},
				{
					"Pooling",
					SqlConnectionStringBuilder.Keywords.Pooling
				},
				{
					"Replication",
					SqlConnectionStringBuilder.Keywords.Replication
				},
				{
					"Transaction Binding",
					SqlConnectionStringBuilder.Keywords.TransactionBinding
				},
				{
					"TrustServerCertificate",
					SqlConnectionStringBuilder.Keywords.TrustServerCertificate
				},
				{
					"Type System Version",
					SqlConnectionStringBuilder.Keywords.TypeSystemVersion
				},
				{
					"User ID",
					SqlConnectionStringBuilder.Keywords.UserID
				},
				{
					"User Instance",
					SqlConnectionStringBuilder.Keywords.UserInstance
				},
				{
					"Workstation ID",
					SqlConnectionStringBuilder.Keywords.WorkstationID
				},
				{
					"ConnectRetryCount",
					SqlConnectionStringBuilder.Keywords.ConnectRetryCount
				},
				{
					"ConnectRetryInterval",
					SqlConnectionStringBuilder.Keywords.ConnectRetryInterval
				},
				{
					"app",
					SqlConnectionStringBuilder.Keywords.ApplicationName
				},
				{
					"extended properties",
					SqlConnectionStringBuilder.Keywords.AttachDBFilename
				},
				{
					"initial file name",
					SqlConnectionStringBuilder.Keywords.AttachDBFilename
				},
				{
					"connection timeout",
					SqlConnectionStringBuilder.Keywords.ConnectTimeout
				},
				{
					"timeout",
					SqlConnectionStringBuilder.Keywords.ConnectTimeout
				},
				{
					"language",
					SqlConnectionStringBuilder.Keywords.CurrentLanguage
				},
				{
					"addr",
					SqlConnectionStringBuilder.Keywords.DataSource
				},
				{
					"address",
					SqlConnectionStringBuilder.Keywords.DataSource
				},
				{
					"network address",
					SqlConnectionStringBuilder.Keywords.DataSource
				},
				{
					"server",
					SqlConnectionStringBuilder.Keywords.DataSource
				},
				{
					"database",
					SqlConnectionStringBuilder.Keywords.InitialCatalog
				},
				{
					"trusted_connection",
					SqlConnectionStringBuilder.Keywords.IntegratedSecurity
				},
				{
					"connection lifetime",
					SqlConnectionStringBuilder.Keywords.LoadBalanceTimeout
				},
				{
					"pwd",
					SqlConnectionStringBuilder.Keywords.Password
				},
				{
					"persistsecurityinfo",
					SqlConnectionStringBuilder.Keywords.PersistSecurityInfo
				},
				{
					"uid",
					SqlConnectionStringBuilder.Keywords.UserID
				},
				{
					"user",
					SqlConnectionStringBuilder.Keywords.UserID
				},
				{
					"wsid",
					SqlConnectionStringBuilder.Keywords.WorkstationID
				}
			};
		}

		public SqlConnectionStringBuilder()
			: this(null)
		{
		}

		public SqlConnectionStringBuilder(string connectionString)
		{
			if (!string.IsNullOrEmpty(connectionString))
			{
				base.ConnectionString = connectionString;
			}
		}

		public override object this[string keyword]
		{
			get
			{
				SqlConnectionStringBuilder.Keywords index = this.GetIndex(keyword);
				return this.GetAt(index);
			}
			set
			{
				if (value == null)
				{
					this.Remove(keyword);
					return;
				}
				switch (this.GetIndex(keyword))
				{
				case SqlConnectionStringBuilder.Keywords.DataSource:
					this.DataSource = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.FailoverPartner:
					this.FailoverPartner = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.AttachDBFilename:
					this.AttachDBFilename = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.InitialCatalog:
					this.InitialCatalog = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.IntegratedSecurity:
					this.IntegratedSecurity = SqlConnectionStringBuilder.ConvertToIntegratedSecurity(value);
					return;
				case SqlConnectionStringBuilder.Keywords.PersistSecurityInfo:
					this.PersistSecurityInfo = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.UserID:
					this.UserID = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.Password:
					this.Password = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.Enlist:
					this.Enlist = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.Pooling:
					this.Pooling = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.MinPoolSize:
					this.MinPoolSize = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.MaxPoolSize:
					this.MaxPoolSize = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.MultipleActiveResultSets:
					this.MultipleActiveResultSets = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.Replication:
					this.Replication = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.ConnectTimeout:
					this.ConnectTimeout = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.Encrypt:
					this.Encrypt = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.TrustServerCertificate:
					this.TrustServerCertificate = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.LoadBalanceTimeout:
					this.LoadBalanceTimeout = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.PacketSize:
					this.PacketSize = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.TypeSystemVersion:
					this.TypeSystemVersion = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.ApplicationName:
					this.ApplicationName = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.CurrentLanguage:
					this.CurrentLanguage = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.WorkstationID:
					this.WorkstationID = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.UserInstance:
					this.UserInstance = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.TransactionBinding:
					this.TransactionBinding = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.ApplicationIntent:
					this.ApplicationIntent = SqlConnectionStringBuilder.ConvertToApplicationIntent(keyword, value);
					return;
				case SqlConnectionStringBuilder.Keywords.MultiSubnetFailover:
					this.MultiSubnetFailover = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.ConnectRetryCount:
					this.ConnectRetryCount = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.ConnectRetryInterval:
					this.ConnectRetryInterval = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				default:
					throw this.UnsupportedKeyword(keyword);
				}
			}
		}

		public ApplicationIntent ApplicationIntent
		{
			get
			{
				return this._applicationIntent;
			}
			set
			{
				if (!DbConnectionStringBuilderUtil.IsValidApplicationIntentValue(value))
				{
					throw ADP.InvalidEnumerationValue(typeof(ApplicationIntent), (int)value);
				}
				this.SetApplicationIntentValue(value);
				this._applicationIntent = value;
			}
		}

		public string ApplicationName
		{
			get
			{
				return this._applicationName;
			}
			set
			{
				this.SetValue("Application Name", value);
				this._applicationName = value;
			}
		}

		public string AttachDBFilename
		{
			get
			{
				return this._attachDBFilename;
			}
			set
			{
				this.SetValue("AttachDbFilename", value);
				this._attachDBFilename = value;
			}
		}

		public int ConnectTimeout
		{
			get
			{
				return this._connectTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ADP.InvalidConnectionOptionValue("Connect Timeout");
				}
				this.SetValue("Connect Timeout", value);
				this._connectTimeout = value;
			}
		}

		public string CurrentLanguage
		{
			get
			{
				return this._currentLanguage;
			}
			set
			{
				this.SetValue("Current Language", value);
				this._currentLanguage = value;
			}
		}

		public string DataSource
		{
			get
			{
				return this._dataSource;
			}
			set
			{
				this.SetValue("Data Source", value);
				this._dataSource = value;
			}
		}

		public bool Encrypt
		{
			get
			{
				return this._encrypt;
			}
			set
			{
				this.SetValue("Encrypt", value);
				this._encrypt = value;
			}
		}

		public bool TrustServerCertificate
		{
			get
			{
				return this._trustServerCertificate;
			}
			set
			{
				this.SetValue("TrustServerCertificate", value);
				this._trustServerCertificate = value;
			}
		}

		public bool Enlist
		{
			get
			{
				return this._enlist;
			}
			set
			{
				this.SetValue("Enlist", value);
				this._enlist = value;
			}
		}

		public string FailoverPartner
		{
			get
			{
				return this._failoverPartner;
			}
			set
			{
				this.SetValue("Failover Partner", value);
				this._failoverPartner = value;
			}
		}

		[TypeConverter(typeof(SqlConnectionStringBuilder.SqlInitialCatalogConverter))]
		public string InitialCatalog
		{
			get
			{
				return this._initialCatalog;
			}
			set
			{
				this.SetValue("Initial Catalog", value);
				this._initialCatalog = value;
			}
		}

		public bool IntegratedSecurity
		{
			get
			{
				return this._integratedSecurity;
			}
			set
			{
				this.SetValue("Integrated Security", value);
				this._integratedSecurity = value;
			}
		}

		public int LoadBalanceTimeout
		{
			get
			{
				return this._loadBalanceTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ADP.InvalidConnectionOptionValue("Load Balance Timeout");
				}
				this.SetValue("Load Balance Timeout", value);
				this._loadBalanceTimeout = value;
			}
		}

		public int MaxPoolSize
		{
			get
			{
				return this._maxPoolSize;
			}
			set
			{
				if (value < 1)
				{
					throw ADP.InvalidConnectionOptionValue("Max Pool Size");
				}
				this.SetValue("Max Pool Size", value);
				this._maxPoolSize = value;
			}
		}

		public int ConnectRetryCount
		{
			get
			{
				return this._connectRetryCount;
			}
			set
			{
				if (value < 0 || value > 255)
				{
					throw ADP.InvalidConnectionOptionValue("ConnectRetryCount");
				}
				this.SetValue("ConnectRetryCount", value);
				this._connectRetryCount = value;
			}
		}

		public int ConnectRetryInterval
		{
			get
			{
				return this._connectRetryInterval;
			}
			set
			{
				if (value < 1 || value > 60)
				{
					throw ADP.InvalidConnectionOptionValue("ConnectRetryInterval");
				}
				this.SetValue("ConnectRetryInterval", value);
				this._connectRetryInterval = value;
			}
		}

		public int MinPoolSize
		{
			get
			{
				return this._minPoolSize;
			}
			set
			{
				if (value < 0)
				{
					throw ADP.InvalidConnectionOptionValue("Min Pool Size");
				}
				this.SetValue("Min Pool Size", value);
				this._minPoolSize = value;
			}
		}

		public bool MultipleActiveResultSets
		{
			get
			{
				return this._multipleActiveResultSets;
			}
			set
			{
				this.SetValue("MultipleActiveResultSets", value);
				this._multipleActiveResultSets = value;
			}
		}

		public bool MultiSubnetFailover
		{
			get
			{
				return this._multiSubnetFailover;
			}
			set
			{
				this.SetValue("MultiSubnetFailover", value);
				this._multiSubnetFailover = value;
			}
		}

		public int PacketSize
		{
			get
			{
				return this._packetSize;
			}
			set
			{
				if (value < 512 || 32768 < value)
				{
					throw SQL.InvalidPacketSizeValue();
				}
				this.SetValue("Packet Size", value);
				this._packetSize = value;
			}
		}

		public string Password
		{
			get
			{
				return this._password;
			}
			set
			{
				this.SetValue("Password", value);
				this._password = value;
			}
		}

		public bool PersistSecurityInfo
		{
			get
			{
				return this._persistSecurityInfo;
			}
			set
			{
				this.SetValue("Persist Security Info", value);
				this._persistSecurityInfo = value;
			}
		}

		public bool Pooling
		{
			get
			{
				return this._pooling;
			}
			set
			{
				this.SetValue("Pooling", value);
				this._pooling = value;
			}
		}

		public bool Replication
		{
			get
			{
				return this._replication;
			}
			set
			{
				this.SetValue("Replication", value);
				this._replication = value;
			}
		}

		public string TransactionBinding
		{
			get
			{
				return this._transactionBinding;
			}
			set
			{
				this.SetValue("Transaction Binding", value);
				this._transactionBinding = value;
			}
		}

		public string TypeSystemVersion
		{
			get
			{
				return this._typeSystemVersion;
			}
			set
			{
				this.SetValue("Type System Version", value);
				this._typeSystemVersion = value;
			}
		}

		public string UserID
		{
			get
			{
				return this._userID;
			}
			set
			{
				this.SetValue("User ID", value);
				this._userID = value;
			}
		}

		public bool UserInstance
		{
			get
			{
				return this._userInstance;
			}
			set
			{
				this.SetValue("User Instance", value);
				this._userInstance = value;
			}
		}

		public string WorkstationID
		{
			get
			{
				return this._workstationID;
			}
			set
			{
				this.SetValue("Workstation ID", value);
				this._workstationID = value;
			}
		}

		public override ICollection Keys
		{
			get
			{
				return new ReadOnlyCollection<string>(SqlConnectionStringBuilder.s_validKeywords);
			}
		}

		public override ICollection Values
		{
			get
			{
				object[] array = new object[SqlConnectionStringBuilder.s_validKeywords.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.GetAt((SqlConnectionStringBuilder.Keywords)i);
				}
				return new ReadOnlyCollection<object>(array);
			}
		}

		public override void Clear()
		{
			base.Clear();
			for (int i = 0; i < SqlConnectionStringBuilder.s_validKeywords.Length; i++)
			{
				this.Reset((SqlConnectionStringBuilder.Keywords)i);
			}
		}

		public override bool ContainsKey(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			return SqlConnectionStringBuilder.s_keywords.ContainsKey(keyword);
		}

		private static bool ConvertToBoolean(object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToBoolean(value);
		}

		private static int ConvertToInt32(object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToInt32(value);
		}

		private static bool ConvertToIntegratedSecurity(object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToIntegratedSecurity(value);
		}

		private static string ConvertToString(object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToString(value);
		}

		private static ApplicationIntent ConvertToApplicationIntent(string keyword, object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToApplicationIntent(keyword, value);
		}

		private object GetAt(SqlConnectionStringBuilder.Keywords index)
		{
			switch (index)
			{
			case SqlConnectionStringBuilder.Keywords.DataSource:
				return this.DataSource;
			case SqlConnectionStringBuilder.Keywords.FailoverPartner:
				return this.FailoverPartner;
			case SqlConnectionStringBuilder.Keywords.AttachDBFilename:
				return this.AttachDBFilename;
			case SqlConnectionStringBuilder.Keywords.InitialCatalog:
				return this.InitialCatalog;
			case SqlConnectionStringBuilder.Keywords.IntegratedSecurity:
				return this.IntegratedSecurity;
			case SqlConnectionStringBuilder.Keywords.PersistSecurityInfo:
				return this.PersistSecurityInfo;
			case SqlConnectionStringBuilder.Keywords.UserID:
				return this.UserID;
			case SqlConnectionStringBuilder.Keywords.Password:
				return this.Password;
			case SqlConnectionStringBuilder.Keywords.Enlist:
				return this.Enlist;
			case SqlConnectionStringBuilder.Keywords.Pooling:
				return this.Pooling;
			case SqlConnectionStringBuilder.Keywords.MinPoolSize:
				return this.MinPoolSize;
			case SqlConnectionStringBuilder.Keywords.MaxPoolSize:
				return this.MaxPoolSize;
			case SqlConnectionStringBuilder.Keywords.MultipleActiveResultSets:
				return this.MultipleActiveResultSets;
			case SqlConnectionStringBuilder.Keywords.Replication:
				return this.Replication;
			case SqlConnectionStringBuilder.Keywords.ConnectTimeout:
				return this.ConnectTimeout;
			case SqlConnectionStringBuilder.Keywords.Encrypt:
				return this.Encrypt;
			case SqlConnectionStringBuilder.Keywords.TrustServerCertificate:
				return this.TrustServerCertificate;
			case SqlConnectionStringBuilder.Keywords.LoadBalanceTimeout:
				return this.LoadBalanceTimeout;
			case SqlConnectionStringBuilder.Keywords.PacketSize:
				return this.PacketSize;
			case SqlConnectionStringBuilder.Keywords.TypeSystemVersion:
				return this.TypeSystemVersion;
			case SqlConnectionStringBuilder.Keywords.ApplicationName:
				return this.ApplicationName;
			case SqlConnectionStringBuilder.Keywords.CurrentLanguage:
				return this.CurrentLanguage;
			case SqlConnectionStringBuilder.Keywords.WorkstationID:
				return this.WorkstationID;
			case SqlConnectionStringBuilder.Keywords.UserInstance:
				return this.UserInstance;
			case SqlConnectionStringBuilder.Keywords.TransactionBinding:
				return this.TransactionBinding;
			case SqlConnectionStringBuilder.Keywords.ApplicationIntent:
				return this.ApplicationIntent;
			case SqlConnectionStringBuilder.Keywords.MultiSubnetFailover:
				return this.MultiSubnetFailover;
			case SqlConnectionStringBuilder.Keywords.ConnectRetryCount:
				return this.ConnectRetryCount;
			case SqlConnectionStringBuilder.Keywords.ConnectRetryInterval:
				return this.ConnectRetryInterval;
			default:
				throw this.UnsupportedKeyword(SqlConnectionStringBuilder.s_validKeywords[(int)index]);
			}
		}

		private SqlConnectionStringBuilder.Keywords GetIndex(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			SqlConnectionStringBuilder.Keywords keywords;
			if (SqlConnectionStringBuilder.s_keywords.TryGetValue(keyword, out keywords))
			{
				return keywords;
			}
			throw this.UnsupportedKeyword(keyword);
		}

		public override bool Remove(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			SqlConnectionStringBuilder.Keywords keywords;
			if (SqlConnectionStringBuilder.s_keywords.TryGetValue(keyword, out keywords) && base.Remove(SqlConnectionStringBuilder.s_validKeywords[(int)keywords]))
			{
				this.Reset(keywords);
				return true;
			}
			return false;
		}

		private void Reset(SqlConnectionStringBuilder.Keywords index)
		{
			switch (index)
			{
			case SqlConnectionStringBuilder.Keywords.DataSource:
				this._dataSource = "";
				return;
			case SqlConnectionStringBuilder.Keywords.FailoverPartner:
				this._failoverPartner = "";
				return;
			case SqlConnectionStringBuilder.Keywords.AttachDBFilename:
				this._attachDBFilename = "";
				return;
			case SqlConnectionStringBuilder.Keywords.InitialCatalog:
				this._initialCatalog = "";
				return;
			case SqlConnectionStringBuilder.Keywords.IntegratedSecurity:
				this._integratedSecurity = false;
				return;
			case SqlConnectionStringBuilder.Keywords.PersistSecurityInfo:
				this._persistSecurityInfo = false;
				return;
			case SqlConnectionStringBuilder.Keywords.UserID:
				this._userID = "";
				return;
			case SqlConnectionStringBuilder.Keywords.Password:
				this._password = "";
				return;
			case SqlConnectionStringBuilder.Keywords.Enlist:
				this._enlist = true;
				return;
			case SqlConnectionStringBuilder.Keywords.Pooling:
				this._pooling = true;
				return;
			case SqlConnectionStringBuilder.Keywords.MinPoolSize:
				this._minPoolSize = 0;
				return;
			case SqlConnectionStringBuilder.Keywords.MaxPoolSize:
				this._maxPoolSize = 100;
				return;
			case SqlConnectionStringBuilder.Keywords.MultipleActiveResultSets:
				this._multipleActiveResultSets = false;
				return;
			case SqlConnectionStringBuilder.Keywords.Replication:
				this._replication = false;
				return;
			case SqlConnectionStringBuilder.Keywords.ConnectTimeout:
				this._connectTimeout = 15;
				return;
			case SqlConnectionStringBuilder.Keywords.Encrypt:
				this._encrypt = false;
				return;
			case SqlConnectionStringBuilder.Keywords.TrustServerCertificate:
				this._trustServerCertificate = false;
				return;
			case SqlConnectionStringBuilder.Keywords.LoadBalanceTimeout:
				this._loadBalanceTimeout = 0;
				return;
			case SqlConnectionStringBuilder.Keywords.PacketSize:
				this._packetSize = 8000;
				return;
			case SqlConnectionStringBuilder.Keywords.TypeSystemVersion:
				this._typeSystemVersion = "Latest";
				return;
			case SqlConnectionStringBuilder.Keywords.ApplicationName:
				this._applicationName = "Core .Net SqlClient Data Provider";
				return;
			case SqlConnectionStringBuilder.Keywords.CurrentLanguage:
				this._currentLanguage = "";
				return;
			case SqlConnectionStringBuilder.Keywords.WorkstationID:
				this._workstationID = "";
				return;
			case SqlConnectionStringBuilder.Keywords.UserInstance:
				this._userInstance = false;
				return;
			case SqlConnectionStringBuilder.Keywords.TransactionBinding:
				this._transactionBinding = "Implicit Unbind";
				return;
			case SqlConnectionStringBuilder.Keywords.ApplicationIntent:
				this._applicationIntent = ApplicationIntent.ReadWrite;
				return;
			case SqlConnectionStringBuilder.Keywords.MultiSubnetFailover:
				this._multiSubnetFailover = false;
				return;
			case SqlConnectionStringBuilder.Keywords.ConnectRetryCount:
				this._connectRetryCount = 1;
				return;
			case SqlConnectionStringBuilder.Keywords.ConnectRetryInterval:
				this._connectRetryInterval = 10;
				return;
			default:
				throw this.UnsupportedKeyword(SqlConnectionStringBuilder.s_validKeywords[(int)index]);
			}
		}

		private void SetValue(string keyword, bool value)
		{
			base[keyword] = value.ToString();
		}

		private void SetValue(string keyword, int value)
		{
			base[keyword] = value.ToString(null);
		}

		private void SetValue(string keyword, string value)
		{
			ADP.CheckArgumentNull(value, keyword);
			base[keyword] = value;
		}

		private void SetApplicationIntentValue(ApplicationIntent value)
		{
			base["ApplicationIntent"] = DbConnectionStringBuilderUtil.ApplicationIntentToString(value);
		}

		public override bool ShouldSerialize(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			SqlConnectionStringBuilder.Keywords keywords;
			return SqlConnectionStringBuilder.s_keywords.TryGetValue(keyword, out keywords) && base.ShouldSerialize(SqlConnectionStringBuilder.s_validKeywords[(int)keywords]);
		}

		public override bool TryGetValue(string keyword, out object value)
		{
			SqlConnectionStringBuilder.Keywords keywords;
			if (SqlConnectionStringBuilder.s_keywords.TryGetValue(keyword, out keywords))
			{
				value = this.GetAt(keywords);
				return true;
			}
			value = null;
			return false;
		}

		private Exception UnsupportedKeyword(string keyword)
		{
			if (SqlConnectionStringBuilder.s_notSupportedKeywords.Contains(keyword, StringComparer.OrdinalIgnoreCase))
			{
				return SQL.UnsupportedKeyword(keyword);
			}
			if (SqlConnectionStringBuilder.s_notSupportedNetworkLibraryKeywords.Contains(keyword, StringComparer.OrdinalIgnoreCase))
			{
				return SQL.NetworkLibraryKeywordNotSupported();
			}
			return ADP.KeywordNotSupported(keyword);
		}

		[Obsolete("This property is ignored beginning in .NET Framework 4.5.For more information about SqlClient support for asynchronous programming, seehttps://docs.microsoft.com/en-us/dotnet/framework/data/adonet/asynchronous-programming")]
		public bool AsynchronousProcessing { get; set; }

		[Obsolete("ConnectionReset has been deprecated.  SqlConnection will ignore the 'connection reset'keyword and always reset the connection")]
		public bool ConnectionReset { get; set; }

		[MonoTODO("Not implemented in corefx: https://github.com/dotnet/corefx/issues/22474")]
		public bool ContextConnection
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO("Not implemented in corefx: https://github.com/dotnet/corefx/issues/22474")]
		public string NetworkLibrary
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO("Not implemented in corefx: https://github.com/dotnet/corefx/issues/22474")]
		public PoolBlockingPeriod PoolBlockingPeriod
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO("Not implemented in corefx: https://github.com/dotnet/corefx/issues/22474")]
		public bool TransparentNetworkIPResolution
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO("Not implemented in corefx: https://github.com/dotnet/corefx/issues/22474")]
		public SqlConnectionColumnEncryptionSetting ColumnEncryptionSetting
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public SqlAuthenticationMethod Authentication
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return SqlAuthenticationMethod.NotSpecified;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		internal const int KeywordsCount = 29;

		internal const int DeprecatedKeywordsCount = 4;

		private static readonly string[] s_validKeywords = SqlConnectionStringBuilder.CreateValidKeywords();

		private static readonly Dictionary<string, SqlConnectionStringBuilder.Keywords> s_keywords = SqlConnectionStringBuilder.CreateKeywordsDictionary();

		private ApplicationIntent _applicationIntent;

		private string _applicationName = "Core .Net SqlClient Data Provider";

		private string _attachDBFilename = "";

		private string _currentLanguage = "";

		private string _dataSource = "";

		private string _failoverPartner = "";

		private string _initialCatalog = "";

		private string _password = "";

		private string _transactionBinding = "Implicit Unbind";

		private string _typeSystemVersion = "Latest";

		private string _userID = "";

		private string _workstationID = "";

		private int _connectTimeout = 15;

		private int _loadBalanceTimeout;

		private int _maxPoolSize = 100;

		private int _minPoolSize;

		private int _packetSize = 8000;

		private int _connectRetryCount = 1;

		private int _connectRetryInterval = 10;

		private bool _encrypt;

		private bool _trustServerCertificate;

		private bool _enlist = true;

		private bool _integratedSecurity;

		private bool _multipleActiveResultSets;

		private bool _multiSubnetFailover;

		private bool _persistSecurityInfo;

		private bool _pooling = true;

		private bool _replication;

		private bool _userInstance;

		private static readonly string[] s_notSupportedKeywords = new string[] { "Asynchronous Processing", "Connection Reset", "Context Connection", "Transaction Binding", "async" };

		private static readonly string[] s_notSupportedNetworkLibraryKeywords = new string[] { "Network Library", "net", "network" };

		private enum Keywords
		{
			DataSource,
			FailoverPartner,
			AttachDBFilename,
			InitialCatalog,
			IntegratedSecurity,
			PersistSecurityInfo,
			UserID,
			Password,
			Enlist,
			Pooling,
			MinPoolSize,
			MaxPoolSize,
			MultipleActiveResultSets,
			Replication,
			ConnectTimeout,
			Encrypt,
			TrustServerCertificate,
			LoadBalanceTimeout,
			PacketSize,
			TypeSystemVersion,
			ApplicationName,
			CurrentLanguage,
			WorkstationID,
			UserInstance,
			TransactionBinding,
			ApplicationIntent,
			MultiSubnetFailover,
			ConnectRetryCount,
			ConnectRetryInterval,
			KeywordsCount
		}

		private sealed class SqlInitialCatalogConverter : StringConverter
		{
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				return this.GetStandardValuesSupportedInternal(context);
			}

			private bool GetStandardValuesSupportedInternal(ITypeDescriptorContext context)
			{
				bool flag = false;
				if (context != null)
				{
					SqlConnectionStringBuilder sqlConnectionStringBuilder = context.Instance as SqlConnectionStringBuilder;
					if (sqlConnectionStringBuilder != null && 0 < sqlConnectionStringBuilder.DataSource.Length && (sqlConnectionStringBuilder.IntegratedSecurity || 0 < sqlConnectionStringBuilder.UserID.Length))
					{
						flag = true;
					}
				}
				return flag;
			}

			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			{
				return false;
			}

			public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				if (this.GetStandardValuesSupportedInternal(context))
				{
					List<string> list = new List<string>();
					try
					{
						SqlConnectionStringBuilder sqlConnectionStringBuilder = (SqlConnectionStringBuilder)context.Instance;
						using (SqlConnection sqlConnection = new SqlConnection())
						{
							sqlConnection.ConnectionString = sqlConnectionStringBuilder.ConnectionString;
							sqlConnection.Open();
							foreach (object obj in sqlConnection.GetSchema("DATABASES").Rows)
							{
								string text = (string)((DataRow)obj)["database_name"];
								list.Add(text);
							}
						}
					}
					catch (SqlException ex)
					{
						ADP.TraceExceptionWithoutRethrow(ex);
					}
					return new TypeConverter.StandardValuesCollection(list);
				}
				return null;
			}
		}
	}
}
