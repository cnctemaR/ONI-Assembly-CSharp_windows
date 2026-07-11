using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.SqlClient
{
	[DefaultProperty("DataSource")]
	[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlConnectionStringBuilderConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class SqlConnectionStringBuilder : DbConnectionStringBuilder
	{
		public SqlConnectionStringBuilder()
		{
		}

		public SqlConnectionStringBuilder(string connectionString)
		{
		}

		[DisplayName("Application Name")]
		[RefreshProperties(RefreshProperties.All)]
		public string ApplicationName
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Asynchronous Processing")]
		[RefreshProperties(RefreshProperties.All)]
		public bool AsynchronousProcessing
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("AttachDbFilename")]
		[Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[RefreshProperties(RefreshProperties.All)]
		public string AttachDBFilename
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Connection Reset")]
		[RefreshProperties(RefreshProperties.All)]
		public bool ConnectionReset
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Connect Timeout")]
		[RefreshProperties(RefreshProperties.All)]
		public int ConnectTimeout
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Context Connection")]
		[RefreshProperties(RefreshProperties.All)]
		public bool ContextConnection
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Current Language")]
		[RefreshProperties(RefreshProperties.All)]
		public string CurrentLanguage
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Data Source")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlDataSourceConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		public string DataSource
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Encrypt")]
		[RefreshProperties(RefreshProperties.All)]
		public bool Encrypt
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Enlist")]
		[RefreshProperties(RefreshProperties.All)]
		public bool Enlist
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Failover Partner")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlDataSourceConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		public string FailoverPartner
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Initial Catalog")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlInitialCatalogConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		public string InitialCatalog
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Integrated Security")]
		[RefreshProperties(RefreshProperties.All)]
		public bool IntegratedSecurity
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				throw null;
			}
		}

		public override object this[string keyword]
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override ICollection Keys
		{
			get
			{
				throw null;
			}
		}

		[DisplayName("Load Balance Timeout")]
		[RefreshProperties(RefreshProperties.All)]
		public int LoadBalanceTimeout
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Max Pool Size")]
		[RefreshProperties(RefreshProperties.All)]
		public int MaxPoolSize
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Min Pool Size")]
		[RefreshProperties(RefreshProperties.All)]
		public int MinPoolSize
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("MultipleActiveResultSets")]
		[RefreshProperties(RefreshProperties.All)]
		public bool MultipleActiveResultSets
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Network Library")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+NetworkLibraryConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		public string NetworkLibrary
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Packet Size")]
		[RefreshProperties(RefreshProperties.All)]
		public int PacketSize
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Password")]
		[PasswordPropertyText(true)]
		[RefreshProperties(RefreshProperties.All)]
		public string Password
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Persist Security Info")]
		[RefreshProperties(RefreshProperties.All)]
		public bool PersistSecurityInfo
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Pooling")]
		[RefreshProperties(RefreshProperties.All)]
		public bool Pooling
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Replication")]
		[RefreshProperties(RefreshProperties.All)]
		public bool Replication
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("TrustServerCertificate")]
		[RefreshProperties(RefreshProperties.All)]
		public bool TrustServerCertificate
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("Type System Version")]
		[RefreshProperties(RefreshProperties.All)]
		public string TypeSystemVersion
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("User ID")]
		[RefreshProperties(RefreshProperties.All)]
		public string UserID
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DisplayName("User Instance")]
		[RefreshProperties(RefreshProperties.All)]
		public bool UserInstance
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override ICollection Values
		{
			get
			{
				throw null;
			}
		}

		[DisplayName("Workstation ID")]
		[RefreshProperties(RefreshProperties.All)]
		public string WorkstationID
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override void Clear()
		{
		}

		public override bool ContainsKey(string keyword)
		{
			throw null;
		}

		public override bool Remove(string keyword)
		{
			throw null;
		}

		[MonoNotSupported("")]
		public override bool ShouldSerialize(string keyword)
		{
			throw null;
		}

		public override bool TryGetValue(string keyword, out object value)
		{
			value = null;
			throw null;
		}
	}
}
