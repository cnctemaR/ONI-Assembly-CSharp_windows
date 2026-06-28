using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.EnterpriseServices;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	[DefaultEvent("InfoMessage")]
	public sealed class SqlConnection : DbConnection, IDisposable, IDbConnection, ICloneable
	{
		public SqlConnection()
			: this(null)
		{
		}

		public SqlConnection(string connectionString)
		{
			this.ConnectionString = connectionString;
		}

		public event SqlInfoMessageEventHandler InfoMessage;

		object ICloneable.Clone()
		{
			return new SqlConnection(this.ConnectionString);
		}

		[Editor("Microsoft.VSDesigner.Data.SQL.Design.SqlConnectionStringEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		[RecommendedAsConfigurable(true)]
		public override string ConnectionString
		{
			get
			{
				if (this.connectionString == null)
				{
					return string.Empty;
				}
				return this.connectionString;
			}
			[MonoTODO("persist security info, encrypt, enlist keyword not implemented")]
			set
			{
				if (this.state == ConnectionState.Open)
				{
					throw new InvalidOperationException("Not Allowed to change ConnectionString property while Connection state is OPEN");
				}
				this.SetConnectionString(value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ConnectionTimeout
		{
			get
			{
				return this.connectionTimeout;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Database
		{
			get
			{
				if (this.State == ConnectionState.Open)
				{
					return this.tds.Database;
				}
				return this.parms.Database;
			}
		}

		internal SqlDataReader DataReader
		{
			get
			{
				return this.dataReader;
			}
			set
			{
				this.dataReader = value;
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string DataSource
		{
			get
			{
				return this.dataSource;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PacketSize
		{
			get
			{
				if (this.State == ConnectionState.Open)
				{
					return this.tds.PacketSize;
				}
				return this.packetSize;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string ServerVersion
		{
			get
			{
				if (this.state == ConnectionState.Closed)
				{
					throw ExceptionHelper.ConnectionClosed();
				}
				return this.tds.ServerVersion;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ConnectionState State
		{
			get
			{
				return this.state;
			}
		}

		internal Tds Tds
		{
			get
			{
				return this.tds;
			}
		}

		internal SqlTransaction Transaction
		{
			get
			{
				return this.transaction;
			}
			set
			{
				this.transaction = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string WorkstationId
		{
			get
			{
				return this.parms.Hostname;
			}
		}

		internal XmlReader XmlReader
		{
			get
			{
				return this.xmlReader;
			}
			set
			{
				this.xmlReader = value;
			}
		}

		public bool FireInfoMessageEventOnUserErrors
		{
			get
			{
				return this.fireInfoMessageEventOnUserErrors;
			}
			set
			{
				this.fireInfoMessageEventOnUserErrors = value;
			}
		}

		[DefaultValue(false)]
		public bool StatisticsEnabled
		{
			get
			{
				return this.statisticsEnabled;
			}
			set
			{
				this.statisticsEnabled = value;
			}
		}

		private void ErrorHandler(object sender, TdsInternalErrorMessageEventArgs e)
		{
			try
			{
				if (!this.tds.IsConnected)
				{
					this.Close();
				}
			}
			catch
			{
				try
				{
					this.Close();
				}
				catch
				{
				}
			}
			throw new SqlException(e.Class, e.LineNumber, e.Message, e.Number, e.Procedure, e.Server, "Mono SqlClient Data Provider", e.State);
		}

		private void MessageHandler(object sender, TdsInternalInfoMessageEventArgs e)
		{
			this.OnSqlInfoMessage(this.CreateSqlInfoMessageEvent(e.Errors));
		}

		public new SqlTransaction BeginTransaction()
		{
			return this.BeginTransaction(IsolationLevel.ReadCommitted, string.Empty);
		}

		public new SqlTransaction BeginTransaction(IsolationLevel iso)
		{
			return this.BeginTransaction(iso, string.Empty);
		}

		public SqlTransaction BeginTransaction(string transactionName)
		{
			return this.BeginTransaction(IsolationLevel.ReadCommitted, transactionName);
		}

		public SqlTransaction BeginTransaction(IsolationLevel iso, string transactionName)
		{
			if (this.state == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			if (this.transaction != null)
			{
				throw new InvalidOperationException("SqlConnection does not support parallel transactions.");
			}
			string text = string.Empty;
			IsolationLevel isolationLevel = iso;
			if (isolationLevel != IsolationLevel.Unspecified)
			{
				if (isolationLevel == IsolationLevel.Chaos)
				{
					throw new ArgumentOutOfRangeException("IsolationLevel", string.Format(CultureInfo.CurrentCulture, "The IsolationLevel enumeration value, {0}, is not supported by the .Net Framework SqlClient Data Provider.", new object[] { (int)iso }));
				}
				if (isolationLevel != IsolationLevel.ReadUncommitted)
				{
					if (isolationLevel != IsolationLevel.ReadCommitted)
					{
						if (isolationLevel != IsolationLevel.RepeatableRead)
						{
							if (isolationLevel != IsolationLevel.Serializable)
							{
								if (isolationLevel != IsolationLevel.Snapshot)
								{
									throw new ArgumentOutOfRangeException("IsolationLevel", string.Format(CultureInfo.CurrentCulture, "The IsolationLevel enumeration value, {0}, is invalid.", new object[] { (int)iso }));
								}
								text = "SNAPSHOT";
							}
							else
							{
								text = "SERIALIZABLE";
							}
						}
						else
						{
							text = "REPEATABLE READ";
						}
					}
					else
					{
						text = "READ COMMITTED";
					}
				}
				else
				{
					text = "READ UNCOMMITTED";
				}
			}
			else
			{
				iso = IsolationLevel.ReadCommitted;
				text = "READ COMMITTED";
			}
			this.tds.Execute(string.Format("SET TRANSACTION ISOLATION LEVEL {0};BEGIN TRANSACTION {1}", text, transactionName));
			this.transaction = new SqlTransaction(this, iso);
			return this.transaction;
		}

		public override void ChangeDatabase(string database)
		{
			if (!SqlConnection.IsValidDatabaseName(database))
			{
				throw new ArgumentException(string.Format("The database name {0} is not valid.", database));
			}
			if (this.state != ConnectionState.Open)
			{
				throw new InvalidOperationException("The connection is not open.");
			}
			this.tds.Execute(string.Format("use [{0}]", database));
		}

		private void ChangeState(ConnectionState currentState)
		{
			if (currentState == this.state)
			{
				return;
			}
			ConnectionState connectionState = this.state;
			this.state = currentState;
			this.OnStateChange(this.CreateStateChangeEvent(connectionState, currentState));
		}

		public override void Close()
		{
			if (this.transaction != null && this.transaction.IsOpen)
			{
				this.transaction.Rollback();
			}
			if (this.dataReader != null || this.xmlReader != null)
			{
				if (this.tds != null)
				{
					this.tds.SkipToEnd();
				}
				this.dataReader = null;
				this.xmlReader = null;
			}
			if (this.tds != null && this.tds.IsConnected)
			{
				if (this.pooling && this.tds.Pooling)
				{
					if (this.pool != null)
					{
						this.pool.ReleaseConnection(this.tds);
						this.pool = null;
					}
				}
				else
				{
					this.tds.Disconnect();
				}
			}
			if (this.tds != null)
			{
				this.tds.TdsErrorMessage -= this.ErrorHandler;
				this.tds.TdsInfoMessage -= this.MessageHandler;
			}
			this.ChangeState(ConnectionState.Closed);
		}

		public new SqlCommand CreateCommand()
		{
			return new SqlCommand
			{
				Connection = this
			};
		}

		private SqlInfoMessageEventArgs CreateSqlInfoMessageEvent(TdsInternalErrorCollection errors)
		{
			return new SqlInfoMessageEventArgs(errors);
		}

		private StateChangeEventArgs CreateStateChangeEvent(ConnectionState originalState, ConnectionState currentState)
		{
			return new StateChangeEventArgs(originalState, currentState);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && !this.disposed)
				{
					if (this.State == ConnectionState.Open)
					{
						this.Close();
					}
					this.ConnectionString = null;
				}
			}
			finally
			{
				this.disposed = true;
				base.Dispose(disposing);
			}
		}

		[MonoTODO("Not sure what this means at present.")]
		public void EnlistDistributedTransaction(ITransaction transaction)
		{
			throw new NotImplementedException();
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return this.BeginTransaction(isolationLevel);
		}

		protected override DbCommand CreateDbCommand()
		{
			return this.CreateCommand();
		}

		public override void Open()
		{
			string empty = string.Empty;
			if (this.state == ConnectionState.Open)
			{
				throw new InvalidOperationException("The Connection is already Open (State=Open)");
			}
			if (this.connectionString == null || this.connectionString.Trim().Length == 0)
			{
				throw new InvalidOperationException("Connection string has not been initialized.");
			}
			try
			{
				if (!this.pooling)
				{
					if (!this.ParseDataSource(this.dataSource, out this.port, out empty))
					{
						throw new SqlException(20, 0, "SQL Server does not exist or access denied.", 17, "ConnectionOpen (Connect()).", this.dataSource, this.parms.ApplicationName, 0);
					}
					this.tds = new Tds80(empty, this.port, this.PacketSize, this.ConnectionTimeout);
					this.tds.Pooling = false;
				}
				else
				{
					if (!this.ParseDataSource(this.dataSource, out this.port, out empty))
					{
						throw new SqlException(20, 0, "SQL Server does not exist or access denied.", 17, "ConnectionOpen (Connect()).", this.dataSource, this.parms.ApplicationName, 0);
					}
					TdsConnectionInfo tdsConnectionInfo = new TdsConnectionInfo(empty, this.port, this.packetSize, this.ConnectionTimeout, this.minPoolSize, this.maxPoolSize);
					this.pool = SqlConnection.sqlConnectionPools.GetConnectionPool(this.connectionString, tdsConnectionInfo);
					this.tds = this.pool.GetConnection();
				}
			}
			catch (TdsTimeoutException ex)
			{
				throw SqlException.FromTdsInternalException(ex);
			}
			catch (TdsInternalException ex2)
			{
				throw SqlException.FromTdsInternalException(ex2);
			}
			this.tds.TdsErrorMessage += this.ErrorHandler;
			this.tds.TdsInfoMessage += this.MessageHandler;
			if (!this.tds.IsConnected)
			{
				try
				{
					this.tds.Connect(this.parms);
				}
				catch
				{
					if (this.pooling)
					{
						this.pool.ReleaseConnection(this.tds);
					}
					throw;
				}
			}
			this.disposed = false;
			this.ChangeState(ConnectionState.Open);
		}

		private bool ParseDataSource(string theDataSource, out int thePort, out string theServerName)
		{
			theServerName = string.Empty;
			string text = string.Empty;
			if (theDataSource == null)
			{
				throw new ArgumentException("Format of initialization string does not conform to specifications");
			}
			thePort = 1433;
			bool flag = true;
			int num;
			if ((num = theDataSource.IndexOf(',')) > -1)
			{
				theServerName = theDataSource.Substring(0, num);
				string text2 = theDataSource.Substring(num + 1);
				thePort = int.Parse(text2);
			}
			else if ((num = theDataSource.IndexOf('\\')) > -1)
			{
				theServerName = theDataSource.Substring(0, num);
				text = theDataSource.Substring(num + 1);
				this.port = this.DiscoverTcpPortViaSqlMonitor(theServerName, text);
				if (this.port == -1)
				{
					flag = false;
				}
			}
			else
			{
				theServerName = theDataSource;
			}
			if (theServerName.Length == 0 || theServerName == "(local)" || theServerName == ".")
			{
				theServerName = "localhost";
			}
			if ((num = theServerName.IndexOf("tcp:")) > -1)
			{
				theServerName = theServerName.Substring(num + 4);
			}
			return flag;
		}

		private bool ConvertIntegratedSecurity(string value)
		{
			return value.ToUpper() == "SSPI" || this.ConvertToBoolean("integrated security", value, false);
		}

		private bool ConvertToBoolean(string key, string value, bool defaultValue)
		{
			if (value.Length == 0)
			{
				return defaultValue;
			}
			string text = value.ToUpper();
			if (text == "TRUE" || text == "YES")
			{
				return true;
			}
			if (text == "FALSE" || text == "NO")
			{
				return false;
			}
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value \"{0}\" for key '{1}'.", new object[] { value, key }));
		}

		private int ConvertToInt32(string key, string value, int defaultValue)
		{
			if (value.Length == 0)
			{
				return defaultValue;
			}
			int num;
			try
			{
				num = int.Parse(value);
			}
			catch (Exception ex)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value \"{0}\" for key '{1}'.", new object[] { value, key }), ex);
			}
			return num;
		}

		private int DiscoverTcpPortViaSqlMonitor(string ServerName, string InstanceName)
		{
			SqlConnection.SqlMonitorSocket sqlMonitorSocket = new SqlConnection.SqlMonitorSocket(ServerName, InstanceName);
			return sqlMonitorSocket.DiscoverTcpPort(this.ConnectionTimeout);
		}

		private void SetConnectionString(string connectionString)
		{
			this.SetDefaultConnectionParameters();
			if (connectionString == null || connectionString.Trim().Length == 0)
			{
				this.connectionString = connectionString;
				return;
			}
			connectionString += ";";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			string text = string.Empty;
			string text2 = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < connectionString.Length; i++)
			{
				char c = connectionString[i];
				char c2;
				if (i == connectionString.Length - 1)
				{
					c2 = '\0';
				}
				else
				{
					c2 = connectionString[i + 1];
				}
				char c3 = c;
				switch (c3)
				{
				case ' ':
					if (flag || flag2)
					{
						stringBuilder.Append(c);
					}
					else if (stringBuilder.Length > 0 && !c2.Equals(';'))
					{
						stringBuilder.Append(c);
					}
					break;
				default:
					switch (c3)
					{
					case ';':
						if (flag2 || flag)
						{
							stringBuilder.Append(c);
						}
						else
						{
							if (text != string.Empty && text != null)
							{
								text2 = stringBuilder.ToString();
								this.SetProperties(text.ToLower().Trim(), text2);
							}
							else if (stringBuilder.Length != 0)
							{
								throw new ArgumentException("Format of initialization string does not conform to specifications");
							}
							flag3 = true;
							text = string.Empty;
							text2 = string.Empty;
							stringBuilder = new StringBuilder();
						}
						break;
					default:
						if (c3 != '\'')
						{
							stringBuilder.Append(c);
						}
						else if (flag2)
						{
							stringBuilder.Append(c);
						}
						else if (c2.Equals(c))
						{
							stringBuilder.Append(c);
							i++;
						}
						else
						{
							flag = !flag;
						}
						break;
					case '=':
						if (flag2 || flag || !flag3)
						{
							stringBuilder.Append(c);
						}
						else if (c2.Equals(c))
						{
							stringBuilder.Append(c);
							i++;
						}
						else
						{
							text = stringBuilder.ToString();
							stringBuilder = new StringBuilder();
							flag3 = false;
						}
						break;
					}
					break;
				case '"':
					if (flag)
					{
						stringBuilder.Append(c);
					}
					else if (c2.Equals(c))
					{
						stringBuilder.Append(c);
						i++;
					}
					else
					{
						flag2 = !flag2;
					}
					break;
				}
			}
			if (this.minPoolSize > this.maxPoolSize)
			{
				throw new ArgumentException("Invalid value for 'min pool size' or 'max pool size'; 'min pool size' must not be greater than 'max pool size'.");
			}
			connectionString = connectionString.Substring(0, connectionString.Length - 1);
			this.connectionString = connectionString;
		}

		private void SetDefaultConnectionParameters()
		{
			if (this.parms == null)
			{
				this.parms = new TdsConnectionParameters();
			}
			else
			{
				this.parms.Reset();
			}
			this.dataSource = string.Empty;
			this.connectionTimeout = 15;
			this.connectionReset = true;
			this.pooling = true;
			this.maxPoolSize = 100;
			this.minPoolSize = 0;
			this.packetSize = 8000;
			this.port = 1433;
			this.async = false;
		}

		private void SetProperties(string name, string value)
		{
			if (name != null)
			{
				if (SqlConnection.<>f__switch$map6 == null)
				{
					SqlConnection.<>f__switch$map6 = new Dictionary<string, int>(43)
					{
						{ "app", 0 },
						{ "application name", 0 },
						{ "attachdbfilename", 1 },
						{ "extended properties", 1 },
						{ "initial file name", 1 },
						{ "timeout", 2 },
						{ "connect timeout", 2 },
						{ "connection timeout", 2 },
						{ "connection lifetime", 3 },
						{ "connection reset", 4 },
						{ "language", 5 },
						{ "current language", 5 },
						{ "data source", 6 },
						{ "server", 6 },
						{ "address", 6 },
						{ "addr", 6 },
						{ "network address", 6 },
						{ "encrypt", 7 },
						{ "enlist", 8 },
						{ "initial catalog", 9 },
						{ "database", 9 },
						{ "integrated security", 10 },
						{ "trusted_connection", 10 },
						{ "max pool size", 11 },
						{ "min pool size", 12 },
						{ "multipleactiveresultsets", 13 },
						{ "asynchronous processing", 14 },
						{ "async", 14 },
						{ "net", 15 },
						{ "network", 15 },
						{ "network library", 15 },
						{ "packet size", 16 },
						{ "password", 17 },
						{ "pwd", 17 },
						{ "persistsecurityinfo", 18 },
						{ "persist security info", 18 },
						{ "pooling", 19 },
						{ "uid", 20 },
						{ "user", 20 },
						{ "user id", 20 },
						{ "wsid", 21 },
						{ "workstation id", 21 },
						{ "user instance", 22 }
					};
				}
				int num;
				if (SqlConnection.<>f__switch$map6.TryGetValue(name, out num))
				{
					switch (num)
					{
					case 0:
						this.parms.ApplicationName = value;
						break;
					case 1:
						this.parms.AttachDBFileName = value;
						break;
					case 2:
					{
						int num2 = this.ConvertToInt32("connect timeout", value, 15);
						if (num2 < 0)
						{
							throw new ArgumentException("Invalid 'connect timeout'. Must be an integer >=0 ");
						}
						this.connectionTimeout = num2;
						break;
					}
					case 3:
						break;
					case 4:
						this.connectionReset = this.ConvertToBoolean("connection reset", value, true);
						break;
					case 5:
						this.parms.Language = value;
						break;
					case 6:
						this.dataSource = value;
						break;
					case 7:
						if (this.ConvertToBoolean(name, value, false))
						{
							throw new NotImplementedException("SSL encryption for data sent between client and server is not implemented.");
						}
						break;
					case 8:
						if (!this.ConvertToBoolean(name, value, true))
						{
							throw new NotImplementedException("Disabling the automatic enlistment of connections in the thread's current transaction context is not implemented.");
						}
						break;
					case 9:
						this.parms.Database = value;
						break;
					case 10:
						this.parms.DomainLogin = this.ConvertIntegratedSecurity(value);
						break;
					case 11:
					{
						int num3 = this.ConvertToInt32(name, value, 100);
						if (num3 < 1)
						{
							throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid '{0}'. The value must be greater than {1}.", new object[] { name, 1 }));
						}
						this.maxPoolSize = num3;
						break;
					}
					case 12:
					{
						int num4 = this.ConvertToInt32(name, value, 0);
						if (num4 < 0)
						{
							throw new ArgumentException("Invalid 'min pool size'. Must be a integer >= 0");
						}
						this.minPoolSize = num4;
						break;
					}
					case 13:
						this.ConvertToBoolean(name, value, false);
						break;
					case 14:
						this.async = this.ConvertToBoolean(name, value, false);
						break;
					case 15:
						if (!value.ToUpper().Equals("DBMSSOCN"))
						{
							throw new ArgumentException("Unsupported network library.");
						}
						break;
					case 16:
					{
						int num5 = this.ConvertToInt32(name, value, 8000);
						if (num5 < 512 || num5 > 32768)
						{
							throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid 'Packet Size'. The value must be between {0} and {1}.", new object[] { 512, 32768 }));
						}
						this.packetSize = num5;
						break;
					}
					case 17:
						this.parms.Password = value;
						break;
					case 18:
						break;
					case 19:
						this.pooling = this.ConvertToBoolean(name, value, true);
						break;
					case 20:
						this.parms.User = value;
						break;
					case 21:
						this.parms.Hostname = value;
						break;
					case 22:
						this.userInstance = this.ConvertToBoolean(name, value, false);
						break;
					default:
						goto IL_054E;
					}
					return;
				}
			}
			IL_054E:
			throw new ArgumentException("Keyword not supported : '" + name + "'.");
		}

		private static bool IsValidDatabaseName(string database)
		{
			if (database == null || database.Trim().Length == 0 || database.Length > 128)
			{
				return false;
			}
			if (database[0] == '"' && database[database.Length] == '"')
			{
				database = database.Substring(1, database.Length - 2);
			}
			else if (char.IsDigit(database[0]))
			{
				return false;
			}
			if (database[0] == '_')
			{
				return false;
			}
			foreach (char c in database.Substring(1, database.Length - 1))
			{
				if (!char.IsLetterOrDigit(c) && c != '_' && c != '-')
				{
					return false;
				}
			}
			return true;
		}

		private void OnSqlInfoMessage(SqlInfoMessageEventArgs value)
		{
			if (this.InfoMessage != null)
			{
				this.InfoMessage(this, value);
			}
		}

		public override DataTable GetSchema()
		{
			if (this.state == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			return SqlConnection.MetaDataCollections.Instance;
		}

		public override DataTable GetSchema(string collectionName)
		{
			return this.GetSchema(collectionName, null);
		}

		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			if (this.state == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			string text = null;
			DataTable instance = SqlConnection.MetaDataCollections.Instance;
			int num = ((restrictionValues != null) ? restrictionValues.Length : 0);
			foreach (object obj in instance.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (string.Compare((string)dataRow["CollectionName"], collectionName, true) == 0)
				{
					if (num > (int)dataRow["NumberOfRestrictions"])
					{
						throw new ArgumentException("More restrictions were provided than the requested schema ('" + dataRow["CollectionName"].ToString() + "') supports");
					}
					text = dataRow["CollectionName"].ToString();
				}
			}
			if (text == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The requested collection ({0}) is not defined.", new object[] { collectionName }));
			}
			SqlCommand sqlCommand = null;
			DataTable dataTable = new DataTable();
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
			string text2 = text;
			switch (text2)
			{
			case "Databases":
				sqlCommand = new SqlCommand("select name as database_name, dbid, crdate as create_date from master.sys.sysdatabases where (name = @Name or (@Name is null))", this);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				break;
			case "ForeignKeys":
				sqlCommand = new SqlCommand("select CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CONSTRAINT_TYPE, IS_DEFERRABLE, INITIALLY_DEFERRED from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where (CONSTRAINT_CATALOG = @Catalog or (@Catalog is null)) and (CONSTRAINT_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @Table or (@Table is null)) and (CONSTRAINT_NAME = @Name or (@Name is null)) and CONSTRAINT_TYPE = 'FOREIGN KEY' order by CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				break;
			case "Indexes":
				sqlCommand = new SqlCommand("select distinct db_name() as constraint_catalog, constraint_schema = user_name (o.uid), constraint_name = x.name, table_catalog = db_name (), table_schema = user_name (o.uid), table_name = o.name, index_name  = x.name from sysobjects o, sysindexes x, sysindexkeys xk where o.type in ('U') and x.id = o.id and o.id = xk.id and x.indid = xk.indid and xk.keyno = x.keycnt and (db_name() = @Catalog or (@Catalog is null)) and (user_name() = @Owner or (@Owner is null)) and (o.name = @Table or (@Table is null)) and (x.name = @Name or (@Name is null))order by table_name, index_name", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				break;
			case "IndexColumns":
				sqlCommand = new SqlCommand("select distinct db_name() as constraint_catalog, constraint_schema = user_name (o.uid), constraint_name = x.name, table_catalog = db_name (), table_schema = user_name (o.uid), table_name = o.name, column_name = c.name, ordinal_position = convert (int, xk.keyno), keyType = c.xtype, index_name = x.name from sysobjects o, sysindexes x, syscolumns c, sysindexkeys xk where o.type in ('U') and x.id = o.id and o.id = c.id and o.id = xk.id and x.indid = xk.indid and c.colid = xk.colid and xk.keyno <= x.keycnt and permissions (o.id, c.name) <> 0 and (db_name() = @Catalog or (@Catalog is null)) and (user_name() = @Owner or (@Owner is null)) and (o.name = @Table or (@Table is null)) and (x.name = @ConstraintName or (@ConstraintName is null)) and (c.name = @Column or (@Column is null)) order by table_name, index_name", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 8);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 13);
				sqlCommand.Parameters.Add("@ConstraintName", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Column", SqlDbType.NVarChar, 4000);
				break;
			case "Procedures":
				sqlCommand = new SqlCommand("select SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ROUTINE_CATALOG, ROUTINE_SCHEMA, ROUTINE_NAME, ROUTINE_TYPE, CREATED, LAST_ALTERED from INFORMATION_SCHEMA.ROUTINES where (SPECIFIC_CATALOG = @Catalog or (@Catalog is null)) and (SPECIFIC_SCHEMA = @Owner or (@Owner is null)) and (SPECIFIC_NAME = @Name or (@Name is null)) and (ROUTINE_TYPE = @Type or (@Type is null)) order by SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Type", SqlDbType.NVarChar, 4000);
				break;
			case "ProcedureParameters":
				sqlCommand = new SqlCommand("select SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ORDINAL_POSITION, PARAMETER_MODE, IS_RESULT, AS_LOCATOR, PARAMETER_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, COLLATION_CATALOG, COLLATION_SCHEMA, COLLATION_NAME, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE, DATETIME_PRECISION, INTERVAL_TYPE, INTERVAL_PRECISION from INFORMATION_SCHEMA.PARAMETERS where (SPECIFIC_CATALOG = @Catalog or (@Catalog is null)) and (SPECIFIC_SCHEMA = @Owner or (@Owner is null)) and (SPECIFIC_NAME = @Name or (@Name is null)) and (PARAMETER_NAME = @Parameter or (@Parameter is null)) order by SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, PARAMETER_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Parameter", SqlDbType.NVarChar, 4000);
				break;
			case "Tables":
				sqlCommand = new SqlCommand("select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE from INFORMATION_SCHEMA.TABLES where (TABLE_CATALOG = @catalog or (@catalog is null)) and (TABLE_SCHEMA = @owner or (@owner is null))and (TABLE_NAME = @name or (@name is null)) and (TABLE_TYPE = @table_type or (@table_type is null))", this);
				sqlCommand.Parameters.Add("@catalog", SqlDbType.NVarChar, 8);
				sqlCommand.Parameters.Add("@owner", SqlDbType.NVarChar, 3);
				sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar, 11);
				sqlCommand.Parameters.Add("@table_type", SqlDbType.NVarChar, 10);
				break;
			case "Columns":
				sqlCommand = new SqlCommand("select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE, DATETIME_PRECISION, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, COLLATION_CATALOG from INFORMATION_SCHEMA.COLUMNS where (TABLE_CATALOG = @Catalog or (@Catalog is null)) and (TABLE_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @table or (@Table is null)) and (COLUMN_NAME = @column or (@Column is null)) order by TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Column", SqlDbType.NVarChar, 4000);
				break;
			case "Users":
				sqlCommand = new SqlCommand("select uid, name as user_name, createdate, updatedate from sysusers where (name = @Name or (@Name is null))", this);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				break;
			case "StructuredTypeMembers":
				throw new NotImplementedException();
			case "Views":
				sqlCommand = new SqlCommand("select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CHECK_OPTION, IS_UPDATABLE from INFORMATION_SCHEMA.VIEWS where (TABLE_CATALOG = @Catalog or (@Catalog is null)) TABLE_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @table or (@Table is null)) order by TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 4000);
				break;
			case "ViewColumns":
				sqlCommand = new SqlCommand("select VIEW_CATALOG, VIEW_SCHEMA, VIEW_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME from INFORMATION_SCHEMA.VIEW_COLUMN_USAGE where (VIEW_CATALOG = @Catalog (@Catalog is null)) and (VIEW_SCHEMA = @Owner (@Owner is null)) and (VIEW_NAME = @Table or (@Table is null)) and (COLUMN_NAME = @Column or (@Column is null)) order by VIEW_CATALOG, VIEW_SCHEMA, VIEW_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Column", SqlDbType.NVarChar, 4000);
				break;
			case "UserDefinedTypes":
				sqlCommand = new SqlCommand("select assemblies.name as assembly_name, types.assembly_class as udt_name, ASSEMBLYPROPERTY(assemblies.name, 'VersionMajor') as version_major, ASSEMBLYPROPERTY(assemblies.name, 'VersionMinor') as version_minor, ASSEMBLYPROPERTY(assemblies.name, 'VersionBuild') as version_build, ASSEMBLYPROPERTY(assemblies.name, 'VersionRevision') as version_revision, ASSEMBLYPROPERTY(assemblies.name, 'CultureInfo') as culture_info, ASSEMBLYPROPERTY(assemblies.name, 'PublicKey') as public_key, is_fixed_length, max_length, Create_Date, Permission_set_desc from sys.assemblies as assemblies join sys.assembly_types as types on assemblies.assembly_id = types.assembly_id where (assportemblies.name = @AssemblyName or (@AssemblyName is null)) and (types.assembly_class = @UDTName or (@UDTName is null))", this);
				sqlCommand.Parameters.Add("@AssemblyName", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@UDTName", SqlDbType.NVarChar, 4000);
				break;
			case "MetaDataCollections":
				return SqlConnection.MetaDataCollections.Instance;
			case "DataSourceInformation":
				return SqlConnection.DataSourceInformation.GetInstance(this);
			case "DataTypes":
				return SqlConnection.DataTypes.Instance;
			case "ReservedWords":
				return SqlConnection.ReservedWords.Instance;
			case "Restrictions":
				return SqlConnection.Restrictions.Instance;
			}
			for (int i = 0; i < num; i++)
			{
				sqlCommand.Parameters[i].Value = restrictionValues[i];
			}
			sqlDataAdapter.SelectCommand = sqlCommand;
			sqlDataAdapter.Fill(dataTable);
			return dataTable;
		}

		public static void ChangePassword(string connectionString, string newPassword)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("The 'connectionString' cannot be null or empty.");
			}
			if (string.IsNullOrEmpty(newPassword))
			{
				throw new ArgumentNullException("The 'newPassword' cannot be null or empty.");
			}
			if (newPassword.Length > 128)
			{
				throw new ArgumentException("The length of 'newPassword' cannot exceed 128 characters.");
			}
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				sqlConnection.tds.Execute(string.Format("sp_password '{0}', '{1}', '{2}'", sqlConnection.parms.Password, newPassword, sqlConnection.parms.User));
			}
		}

		public static void ClearAllPools()
		{
			IDictionary connectionPool = SqlConnection.sqlConnectionPools.GetConnectionPool();
			foreach (object obj in connectionPool.Values)
			{
				TdsConnectionPool tdsConnectionPool = (TdsConnectionPool)obj;
				if (tdsConnectionPool != null)
				{
					tdsConnectionPool.ResetConnectionPool();
				}
			}
			connectionPool.Clear();
		}

		public static void ClearPool(SqlConnection connection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (connection.pooling)
			{
				TdsConnectionPool connectionPool = SqlConnection.sqlConnectionPools.GetConnectionPool(connection.ConnectionString);
				if (connectionPool != null)
				{
					connectionPool.ResetConnectionPool();
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool AsyncProcessing
		{
			get
			{
				return this.async;
			}
		}

		private const int DEFAULT_PACKETSIZE = 8000;

		private const int MAX_PACKETSIZE = 32768;

		private const int MIN_PACKETSIZE = 512;

		private const int DEFAULT_CONNECTIONTIMEOUT = 15;

		private const int DEFAULT_MAXPOOLSIZE = 100;

		private const int MIN_MAXPOOLSIZE = 1;

		private const int DEFAULT_MINPOOLSIZE = 0;

		private const int DEFAULT_PORT = 1433;

		private bool disposed;

		private static TdsConnectionPoolManager sqlConnectionPools = new TdsConnectionPoolManager(TdsVersion.tds80);

		private TdsConnectionPool pool;

		private string connectionString;

		private SqlTransaction transaction;

		private TdsConnectionParameters parms;

		private bool connectionReset;

		private bool pooling;

		private string dataSource;

		private int connectionTimeout;

		private int minPoolSize;

		private int maxPoolSize;

		private int packetSize;

		private int port;

		private bool fireInfoMessageEventOnUserErrors;

		private bool statisticsEnabled;

		private ConnectionState state;

		private SqlDataReader dataReader;

		private XmlReader xmlReader;

		private Tds tds;

		private bool async;

		private bool userInstance;

		private sealed class SqlMonitorSocket : UdpClient
		{
			internal SqlMonitorSocket(string ServerName, string InstanceName)
				: base(ServerName, SqlConnection.SqlMonitorSocket.SqlMonitorUdpPort)
			{
				this.server = ServerName;
				this.instance = InstanceName;
			}

			internal int DiscoverTcpPort(int timeoutSeconds)
			{
				base.Client.Blocking = false;
				ASCIIEncoding asciiencoding = new ASCIIEncoding();
				byte[] array = new byte[this.instance.Length + 1];
				array[0] = 4;
				asciiencoding.GetBytes(this.instance, 0, this.instance.Length, array, 1);
				base.Send(array, array.Length);
				if (!base.Active)
				{
					return -1;
				}
				long num = (long)(timeoutSeconds * 1000000);
				if (!base.Client.Poll((int)num, SelectMode.SelectRead))
				{
					return -1;
				}
				if (base.Client.Available <= 0)
				{
					return -1;
				}
				IPEndPoint ipendPoint = new IPEndPoint(Dns.GetHostEntry("localhost").AddressList[0], 0);
				byte[] array2 = base.Receive(ref ipendPoint);
				string @string = Encoding.ASCII.GetString(array2);
				string[] array3 = @string.Split(new char[] { ';' });
				Hashtable hashtable = new Hashtable();
				int num2 = 0;
				while (num2 < array3.Length / 2 && num2 < 256)
				{
					hashtable[array3[num2 * 2]] = array3[num2 * 2 + 1];
					num2++;
				}
				if (!hashtable.ContainsKey("tcp"))
				{
					string text = "Mono does not support names pipes or shared memory for connecting to SQL Server. Please enable the TCP/IP protocol.";
					throw new NotImplementedException(text);
				}
				int num3 = int.Parse((string)hashtable["tcp"]);
				base.Close();
				return num3;
			}

			private static readonly int SqlMonitorUdpPort = 1434;

			private string server;

			private string instance;
		}

		private struct ColumnInfo
		{
			public ColumnInfo(string name, Type type)
			{
				this.name = name;
				this.type = type;
			}

			public string name;

			public Type type;
		}

		private static class ReservedWords
		{
			public static DataTable Instance
			{
				get
				{
					if (SqlConnection.ReservedWords.instance == null)
					{
						SqlConnection.ReservedWords.instance = new DataTable("ReservedWords");
						SqlConnection.ReservedWords.instance.Columns.Add("ReservedWord", typeof(string));
						foreach (string text in SqlConnection.ReservedWords.reservedWords)
						{
							DataRow dataRow = SqlConnection.ReservedWords.instance.NewRow();
							dataRow["ReservedWord"] = text;
							SqlConnection.ReservedWords.instance.Rows.Add(dataRow);
						}
					}
					return SqlConnection.ReservedWords.instance;
				}
			}

			private static readonly string[] reservedWords = new string[]
			{
				"ADD", "EXCEPT", "PERCENT", "ALL", "EXEC", "PLAN", "ALTER", "EXECUTE", "PRECISION", "AND",
				"EXISTS", "PRIMARY", "ANY", "EXIT", "PRINT", "AS", "FETCH", "PROC", "ASC", "FILE",
				"PROCEDURE", "AUTHORIZATION", "FILLFACTOR", "PUBLIC", "BACKUP", "FOR", "RAISERROR", "BEGIN", "FOREIGN", "READ",
				"BETWEEN", "FREETEXT", "READTEXT", "BREAK", "FREETEXTTABLE", "RECONFIGURE", "BROWSE", "FROM", "REFERENCES", "BULK",
				"FULL", "REPLICATION", "BY", "FUNCTION", "RESTORE", "CASCADE", "GOTO", "RESTRICT", "CASE", "GRANT",
				"RETURN", "CHECK", "GROUP", "REVOKE", "CHECKPOINT", "HAVING", "RIGHT", "CLOSE", "HOLDLOCK", "ROLLBACK",
				"CLUSTERED", "IDENTITY", "ROWCOUNT", "COALESCE", "IDENTITY_INSERT", "ROWGUIDCOL", "COLLATE", "IDENTITYCOL", "RULE", "COLUMN",
				"IF", "SAVE", "COMMIT", "IN", "SCHEMA", "COMPUTE", "INDEX", "SELECT", "CONSTRAINT", "INNER",
				"SESSION_USER", "CONTAINS", "INSERT", "SET", "CONTAINSTABLE", "INTERSECT", "SETUSER", "CONTINUE", "INTO", "SHUTDOWN",
				"CONVERT", "IS", "SOME", "CREATE", "JOIN", "STATISTICS", "CROSS", "KEY", "SYSTEM_USER", "CURRENT",
				"KILL", "TABLE", "CURRENT_DATE", "LEFT", "TEXTSIZE", "CURRENT_TIME", "LIKE", "THEN", "CURRENT_TIMESTAMP", "LINENO",
				"TO", "CURRENT_USER", "LOAD", "TOP", "CURSOR", "NATIONAL", "TRAN", "DATABASE", "NOCHECK", "TRANSACTION",
				"DBCC", "NONCLUSTERED", "TRIGGER", "DEALLOCATE", "NOT", "TRUNCATE", "DECLARE", "NULL", "TSEQUAL", "DEFAULT",
				"NULLIF", "UNION", "DELETE", "OF", "UNIQUE", "DENY", "OFF", "UPDATE", "DESC", "OFFSETS",
				"UPDATETEXT", "DISK", "ON", "USE", "DISTINCT", "OPEN", "USER", "DISTRIBUTED", "OPENDATASOURCE", "VALUES",
				"DOUBLE", "OPENQUERY", "VARYING", "DROP", "OPENROWSET", "VIEW", "DUMMY", "OPENXML", "WAITFOR", "DUMP",
				"OPTION", "WHEN", "ELSE", "OR", "WHERE", "END", "ORDER", "WHILE", "ERRLVL", "OUTER",
				"WITH", "ESCAPE", "OVER", "WRITETEXT", "ABSOLUTE", "FOUND", "PRESERVE", "ACTION", "FREE", "PRIOR",
				"ADMIN", "GENERAL", "PRIVILEGES", "AFTER", "GET", "READS", "AGGREGATE", "GLOBAL", "REAL", "ALIAS",
				"GO", "RECURSIVE", "ALLOCATE", "GROUPING", "REF", "ARE", "HOST", "REFERENCING", "ARRAY", "HOUR",
				"RELATIVE", "ASSERTION", "IGNORE", "RESULT", "AT", "IMMEDIATE", "RETURNS", "BEFORE", "INDICATOR", "ROLE",
				"BINARY", "INITIALIZE", "ROLLUP", "BIT", "INITIALLY", "ROUTINE", "BLOB", "INOUT", "ROW", "BOOLEAN",
				"INPUT", "ROWS", "BOTH", "INT", "SAVEPOINT", "BREADTH", "INTEGER", "SCROLL", "CALL", "INTERVAL",
				"SCOPE", "CASCADED", "ISOLATION", "SEARCH", "CAST", "ITERATE", "SECOND", "CATALOG", "LANGUAGE", "SECTION",
				"CHAR", "LARGE", "SEQUENCE", "CHARACTER", "LAST", "SESSION", "CLASS", "LATERAL", "SETS", "CLOB",
				"LEADING", "SIZE", "COLLATION", "LESS", "SMALLINT", "COMPLETION", "LEVEL", "SPACE", "CONNECT", "LIMIT",
				"SPECIFIC", "CONNECTION", "LOCAL", "SPECIFICTYPE", "CONSTRAINTS", "LOCALTIME", "SQL", "CONSTRUCTOR", "LOCALTIMESTAMP", "SQLEXCEPTION",
				"CORRESPONDING", "LOCATOR", "SQLSTATE", "CUBE", "MAP", "SQLWARNING", "CURRENT_PATH", "MATCH", "START", "CURRENT_ROLE",
				"MINUTE", "STATE", "CYCLE", "MODIFIES", "STATEMENT", "DATA", "MODIFY", "STATIC", "DATE", "MODULE",
				"STRUCTURE", "DAY", "MONTH", "TEMPORARY", "DEC", "NAMES", "TERMINATE", "DECIMAL", "NATURAL", "THAN",
				"DEFERRABLE", "NCHAR", "TIME", "DEFERRED", "NCLOB", "TIMESTAMP", "DEPTH", "NEW", "TIMEZONE_HOUR", "DEREF",
				"NEXT", "TIMEZONE_MINUTE", "DESCRIBE", "NO", "TRAILING", "DESCRIPTOR", "NONE", "TRANSLATION", "DESTROY", "NUMERIC",
				"TREAT", "DESTRUCTOR", "OBJECT", "TRUE", "DETERMINISTIC", "OLD", "UNDER", "DICTIONARY", "ONLY", "UNKNOWN",
				"DIAGNOSTICS", "OPERATION", "UNNEST", "DISCONNECT", "ORDINALITY", "USAGE", "DOMAIN", "OUT", "USING", "DYNAMIC",
				"OUTPUT", "VALUE", "EACH", "PAD", "VARCHAR", "END-EXEC", "PARAMETER", "VARIABLE", "EQUALS", "PARAMETERS",
				"WHENEVER", "EVERY", "PARTIAL", "WITHOUT", "EXCEPTION", "PATH", "WORK", "EXTERNAL", "POSTFIX", "WRITE",
				"FALSE", "PREFIX", "YEAR", "FIRST", "PREORDER", "ZONE", "FLOAT", "PREPARE", "ADA", "AVG",
				"BIT_LENGTH", "CHAR_LENGTH", "CHARACTER_LENGTH", "COUNT", "EXTRACT", "FORTRAN", "INCLUDE", "INSENSITIVE", "LOWER", "MAX",
				"MIN", "OCTET_LENGTH", "OVERLAPS", "PASCAL", "POSITION", "SQLCA", "SQLCODE", "SQLERROR", "SUBSTRING", "SUM",
				"TRANSLATE", "TRIM", "UPPER"
			};

			private static DataTable instance;
		}

		private new static class MetaDataCollections
		{
			public static DataTable Instance
			{
				get
				{
					if (SqlConnection.MetaDataCollections.instance == null)
					{
						SqlConnection.MetaDataCollections.instance = new DataTable("MetaDataCollections");
						foreach (SqlConnection.ColumnInfo columnInfo in SqlConnection.MetaDataCollections.columns)
						{
							SqlConnection.MetaDataCollections.instance.Columns.Add(columnInfo.name, columnInfo.type);
						}
						foreach (object[] array3 in SqlConnection.MetaDataCollections.rows)
						{
							SqlConnection.MetaDataCollections.instance.LoadDataRow(array3, true);
						}
					}
					return SqlConnection.MetaDataCollections.instance;
				}
			}

			private static readonly SqlConnection.ColumnInfo[] columns = new SqlConnection.ColumnInfo[]
			{
				new SqlConnection.ColumnInfo("CollectionName", typeof(string)),
				new SqlConnection.ColumnInfo("NumberOfRestrictions", typeof(int)),
				new SqlConnection.ColumnInfo("NumberOfIdentifierParts", typeof(int))
			};

			private static readonly object[][] rows = new object[][]
			{
				new object[] { "MetaDataCollections", 0, 0 },
				new object[] { "DataSourceInformation", 0, 0 },
				new object[] { "DataTypes", 0, 0 },
				new object[] { "Restrictions", 0, 0 },
				new object[] { "ReservedWords", 0, 0 },
				new object[] { "Users", 1, 1 },
				new object[] { "Databases", 1, 1 },
				new object[] { "Tables", 4, 3 },
				new object[] { "Columns", 4, 4 },
				new object[] { "StructuredTypeMembers", 4, 4 },
				new object[] { "Views", 3, 3 },
				new object[] { "ViewColumns", 4, 4 },
				new object[] { "ProcedureParameters", 4, 1 },
				new object[] { "Procedures", 4, 3 },
				new object[] { "ForeignKeys", 4, 3 },
				new object[] { "IndexColumns", 5, 4 },
				new object[] { "Indexes", 4, 3 },
				new object[] { "UserDefinedTypes", 2, 1 }
			};

			private static DataTable instance;
		}

		private static class DataSourceInformation
		{
			public static DataTable GetInstance(SqlConnection conn)
			{
				DataTable dataTable = new DataTable("DataSourceInformation");
				foreach (SqlConnection.ColumnInfo columnInfo in SqlConnection.DataSourceInformation.columns)
				{
					dataTable.Columns.Add(columnInfo.name, columnInfo.type);
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow[0] = "\\.";
				dataRow[1] = "Microsoft SQL Server";
				dataRow[2] = conn.ServerVersion;
				dataRow[3] = conn.ServerVersion;
				dataRow[4] = GroupByBehavior.Unrelated;
				dataRow[5] = "(^\\[\\p{Lo}\\p{Lu}\\p{Ll}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Nd}@$#_]*$)|(^\\[[^\\]\\0]|\\]\\]+\\]$)|(^\\\"[^\\\"\\0]|\\\"\\\"+\\\"$)";
				dataRow[6] = IdentifierCase.Insensitive;
				dataRow[7] = false;
				dataRow[8] = "{0}";
				dataRow[9] = "@[\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}\\p{Nd}\\uff3f_@#\\$]*(?=\\s+|$)";
				dataRow[10] = 128;
				dataRow[11] = "^[\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}\\p{Nd}\\uff3f_@#\\$]*(?=\\s+|$)";
				dataRow[12] = "(([^\\[]|\\]\\])*)";
				dataRow[13] = IdentifierCase.Insensitive;
				dataRow[14] = ";";
				dataRow[15] = "'(([^']|'')*)'";
				dataRow[16] = SupportedJoinOperators.Inner | SupportedJoinOperators.LeftOuter | SupportedJoinOperators.RightOuter | SupportedJoinOperators.FullOuter;
				dataTable.Rows.Add(dataRow);
				return dataTable;
			}

			private static readonly SqlConnection.ColumnInfo[] columns = new SqlConnection.ColumnInfo[]
			{
				new SqlConnection.ColumnInfo("CompositeIdentifierSeparatorPattern", typeof(string)),
				new SqlConnection.ColumnInfo("DataSourceProductName", typeof(string)),
				new SqlConnection.ColumnInfo("DataSourceProductVersion", typeof(string)),
				new SqlConnection.ColumnInfo("DataSourceProductVersionNormalized", typeof(string)),
				new SqlConnection.ColumnInfo("GroupByBehavior", typeof(GroupByBehavior)),
				new SqlConnection.ColumnInfo("IdentifierPattern", typeof(string)),
				new SqlConnection.ColumnInfo("IdentifierCase", typeof(IdentifierCase)),
				new SqlConnection.ColumnInfo("OrderByColumnsInSelect", typeof(bool)),
				new SqlConnection.ColumnInfo("ParameterMarkerFormat", typeof(string)),
				new SqlConnection.ColumnInfo("ParameterMarkerPattern", typeof(string)),
				new SqlConnection.ColumnInfo("ParameterNameMaxLength", typeof(int)),
				new SqlConnection.ColumnInfo("ParameterNamePattern", typeof(string)),
				new SqlConnection.ColumnInfo("QuotedIdentifierPattern", typeof(string)),
				new SqlConnection.ColumnInfo("QuotedIdentifierCase", typeof(IdentifierCase)),
				new SqlConnection.ColumnInfo("StatementSeparatorPattern", typeof(string)),
				new SqlConnection.ColumnInfo("StringLiteralPattern", typeof(string)),
				new SqlConnection.ColumnInfo("SupportedJoinOperators", typeof(SupportedJoinOperators))
			};
		}

		private static class DataTypes
		{
			// Note: this type is marked as 'beforefieldinit'.
			static DataTypes()
			{
				object[][] array = new object[30][];
				int num = 0;
				object[] array2 = new object[22];
				array2[0] = "smallint";
				array2[1] = 16;
				array2[2] = 5;
				array2[3] = "smallint";
				array2[5] = "System.Int16";
				array2[6] = true;
				array2[7] = true;
				array2[8] = false;
				array2[9] = true;
				array2[10] = true;
				array2[11] = false;
				array2[12] = true;
				array2[13] = true;
				array2[14] = false;
				array2[15] = false;
				array2[18] = false;
				array[num] = array2;
				int num2 = 1;
				object[] array3 = new object[22];
				array3[0] = "int";
				array3[1] = 8;
				array3[2] = 10;
				array3[3] = "int";
				array3[5] = "System.Int32";
				array3[6] = true;
				array3[7] = true;
				array3[8] = false;
				array3[9] = true;
				array3[10] = true;
				array3[11] = false;
				array3[12] = true;
				array3[13] = true;
				array3[14] = false;
				array3[15] = false;
				array3[18] = false;
				array[num2] = array3;
				int num3 = 2;
				object[] array4 = new object[22];
				array4[0] = "real";
				array4[1] = 13;
				array4[2] = 7;
				array4[3] = "real";
				array4[5] = "System.Single";
				array4[6] = false;
				array4[7] = true;
				array4[8] = false;
				array4[9] = true;
				array4[10] = false;
				array4[11] = false;
				array4[12] = true;
				array4[13] = true;
				array4[14] = false;
				array4[15] = false;
				array4[18] = false;
				array[num3] = array4;
				int num4 = 3;
				object[] array5 = new object[22];
				array5[0] = "float";
				array5[1] = 6;
				array5[2] = 53;
				array5[3] = "float({0})";
				array5[4] = "number of bits used to store the mantissa";
				array5[5] = "System.Double";
				array5[6] = false;
				array5[7] = true;
				array5[8] = false;
				array5[9] = true;
				array5[10] = false;
				array5[11] = false;
				array5[12] = true;
				array5[13] = true;
				array5[14] = false;
				array5[15] = false;
				array5[18] = false;
				array[num4] = array5;
				int num5 = 4;
				object[] array6 = new object[22];
				array6[0] = "money";
				array6[1] = 9;
				array6[2] = 19;
				array6[3] = "money";
				array6[5] = "System.Decimal";
				array6[6] = false;
				array6[7] = false;
				array6[8] = false;
				array6[9] = true;
				array6[10] = true;
				array6[11] = false;
				array6[12] = true;
				array6[13] = true;
				array6[14] = false;
				array6[15] = false;
				array6[18] = false;
				array[num5] = array6;
				int num6 = 5;
				object[] array7 = new object[22];
				array7[0] = "smallmoney";
				array7[1] = 17;
				array7[2] = 10;
				array7[3] = "smallmoney";
				array7[5] = "System.Decimal";
				array7[6] = false;
				array7[7] = false;
				array7[8] = false;
				array7[9] = true;
				array7[10] = true;
				array7[11] = false;
				array7[12] = true;
				array7[13] = true;
				array7[14] = false;
				array7[15] = false;
				array7[18] = false;
				array[num6] = array7;
				int num7 = 6;
				object[] array8 = new object[22];
				array8[0] = "bit";
				array8[1] = 2;
				array8[2] = 1;
				array8[3] = "bit";
				array8[5] = "System.Boolean";
				array8[6] = false;
				array8[7] = false;
				array8[8] = false;
				array8[9] = true;
				array8[10] = false;
				array8[11] = false;
				array8[12] = true;
				array8[13] = true;
				array8[14] = false;
				array8[18] = false;
				array[num7] = array8;
				int num8 = 7;
				object[] array9 = new object[22];
				array9[0] = "tinyint";
				array9[1] = 20;
				array9[2] = 3;
				array9[3] = "tinyint";
				array9[5] = "System.SByte";
				array9[6] = true;
				array9[7] = true;
				array9[8] = false;
				array9[9] = true;
				array9[10] = true;
				array9[11] = false;
				array9[12] = true;
				array9[13] = true;
				array9[14] = false;
				array9[15] = true;
				array9[18] = false;
				array[num8] = array9;
				int num9 = 8;
				object[] array10 = new object[22];
				array10[0] = "bigint";
				array10[1] = 0;
				array10[2] = 19;
				array10[3] = "bigint";
				array10[5] = "System.Int64";
				array10[6] = true;
				array10[7] = true;
				array10[8] = false;
				array10[9] = true;
				array10[10] = true;
				array10[11] = false;
				array10[12] = true;
				array10[13] = true;
				array10[14] = false;
				array10[15] = false;
				array10[18] = false;
				array[num9] = array10;
				int num10 = 9;
				object[] array11 = new object[22];
				array11[0] = "timestamp";
				array11[1] = 19;
				array11[2] = 8;
				array11[3] = "timestamp";
				array11[5] = "System.Byte[]";
				array11[6] = false;
				array11[7] = false;
				array11[8] = false;
				array11[9] = true;
				array11[10] = false;
				array11[11] = false;
				array11[12] = false;
				array11[13] = true;
				array11[14] = false;
				array11[18] = true;
				array11[20] = "0x";
				array[num10] = array11;
				int num11 = 10;
				object[] array12 = new object[22];
				array12[0] = "binary";
				array12[1] = 1;
				array12[2] = 8000;
				array12[3] = "binary({0})";
				array12[4] = "length";
				array12[5] = "System.Byte[]";
				array12[6] = false;
				array12[7] = true;
				array12[8] = false;
				array12[9] = true;
				array12[10] = false;
				array12[11] = false;
				array12[12] = true;
				array12[13] = true;
				array12[14] = false;
				array12[18] = false;
				array12[20] = "0x";
				array[num11] = array12;
				int num12 = 11;
				object[] array13 = new object[22];
				array13[0] = "image";
				array13[1] = 7;
				array13[2] = int.MaxValue;
				array13[3] = "image";
				array13[5] = "System.Byte[]";
				array13[6] = false;
				array13[7] = true;
				array13[8] = false;
				array13[9] = false;
				array13[10] = false;
				array13[11] = true;
				array13[12] = true;
				array13[13] = false;
				array13[14] = false;
				array13[18] = false;
				array13[20] = "0x";
				array[num12] = array13;
				array[12] = new object[]
				{
					"text", 18, int.MaxValue, "text", null, "System.String", false, true, false, false,
					false, true, true, false, true, null, null, null, false, null,
					"'", "'"
				};
				array[13] = new object[]
				{
					"ntext", 11, 1073741823, "ntext", null, "System.String", false, true, false, false,
					false, true, true, false, true, null, null, null, false, null,
					"N'", "'"
				};
				int num13 = 14;
				object[] array14 = new object[22];
				array14[0] = "decimal";
				array14[1] = 5;
				array14[2] = 38;
				array14[3] = "decimal({0}, {1})";
				array14[4] = "precision,scale";
				array14[5] = "System.Decimal";
				array14[6] = true;
				array14[7] = true;
				array14[8] = false;
				array14[9] = true;
				array14[10] = false;
				array14[11] = false;
				array14[12] = true;
				array14[13] = true;
				array14[14] = false;
				array14[15] = false;
				array14[16] = 38;
				array14[17] = 0;
				array14[18] = false;
				array[num13] = array14;
				int num14 = 15;
				object[] array15 = new object[22];
				array15[0] = "numeric";
				array15[1] = 5;
				array15[2] = 38;
				array15[3] = "numeric({0}, {1})";
				array15[4] = "precision,scale";
				array15[5] = "System.Decimal";
				array15[6] = true;
				array15[7] = true;
				array15[8] = false;
				array15[9] = true;
				array15[10] = false;
				array15[11] = false;
				array15[12] = true;
				array15[13] = true;
				array15[14] = false;
				array15[15] = false;
				array15[16] = 38;
				array15[17] = 0;
				array15[18] = false;
				array[num14] = array15;
				array[16] = new object[]
				{
					"datetime", 4, 23, "datetime", null, "System.DateTime", false, true, false, true,
					false, false, true, true, true, null, null, null, false, null,
					"{ts '", "'}"
				};
				array[17] = new object[]
				{
					"smalldatetime", 15, 16, "smalldatetime", null, "System.DateTime", false, true, false, true,
					false, false, true, true, true, null, null, null, false, null,
					"{ts '", "'}"
				};
				int num15 = 18;
				object[] array16 = new object[22];
				array16[0] = "sql_variant";
				array16[1] = 23;
				array16[3] = "sql_variant";
				array16[5] = "System.Object";
				array16[6] = false;
				array16[7] = true;
				array16[8] = false;
				array16[9] = false;
				array16[10] = false;
				array16[11] = false;
				array16[12] = true;
				array16[13] = true;
				array16[14] = false;
				array16[18] = false;
				array16[19] = false;
				array[num15] = array16;
				int num16 = 19;
				object[] array17 = new object[22];
				array17[0] = "xml";
				array17[1] = 25;
				array17[2] = int.MaxValue;
				array17[3] = "xml";
				array17[5] = "System.String";
				array17[6] = false;
				array17[7] = false;
				array17[8] = false;
				array17[9] = false;
				array17[10] = false;
				array17[11] = true;
				array17[12] = true;
				array17[13] = false;
				array17[14] = false;
				array17[18] = false;
				array17[19] = false;
				array[num16] = array17;
				array[20] = new object[]
				{
					"varchar", 22, int.MaxValue, "varchar({0})", "max length", "System.String", false, true, false, false,
					false, false, true, true, true, null, null, null, false, null,
					"'", "'"
				};
				array[21] = new object[]
				{
					"char", 3, int.MaxValue, "char({0})", "length", "System.String", false, true, false, true,
					false, false, true, true, true, null, null, null, false, null,
					"'", "'"
				};
				array[22] = new object[]
				{
					"nchar", 10, 1073741823, "nchar({0})", "length", "System.String", false, true, false, true,
					false, false, true, true, true, null, null, null, false, null,
					"N'", "'"
				};
				array[23] = new object[]
				{
					"nvarchar", 12, 1073741823, "nvarchar({0})", "max length", "System.String", false, true, false, false,
					false, false, true, true, true, null, null, null, false, null,
					"N'", "'"
				};
				int num17 = 24;
				object[] array18 = new object[22];
				array18[0] = "varbinary";
				array18[1] = 21;
				array18[2] = 1073741823;
				array18[3] = "varbinary({0})";
				array18[4] = "max length";
				array18[5] = "System.Byte[]";
				array18[6] = false;
				array18[7] = true;
				array18[8] = false;
				array18[9] = false;
				array18[10] = false;
				array18[11] = false;
				array18[12] = true;
				array18[13] = true;
				array18[14] = false;
				array18[18] = false;
				array18[20] = "0x";
				array[num17] = array18;
				array[25] = new object[]
				{
					"uniqueidentifier", 14, 16, "uniqueidentifier", null, "System.Guid", false, true, false, true,
					false, false, true, true, false, null, null, null, false, null,
					"'", "'"
				};
				array[26] = new object[]
				{
					"date",
					31,
					3L,
					"date",
					DBNull.Value,
					"System.DateTime",
					false,
					false,
					false,
					true,
					true,
					false,
					true,
					true,
					true,
					DBNull.Value,
					DBNull.Value,
					DBNull.Value,
					false,
					DBNull.Value,
					"{ts '",
					"'}"
				};
				array[27] = new object[]
				{
					"time",
					32,
					5L,
					"time({0})",
					"scale",
					"System.TimeSpan",
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					true,
					true,
					DBNull.Value,
					7,
					0,
					false,
					DBNull.Value,
					"{ts '",
					"'}"
				};
				array[28] = new object[]
				{
					"datetime2",
					33,
					8L,
					"datetime2({0})",
					"scale",
					"System.DateTime",
					false,
					true,
					false,
					false,
					false,
					false,
					true,
					true,
					true,
					DBNull.Value,
					7,
					0,
					false,
					DBNull.Value,
					"{ts '",
					"'}"
				};
				array[29] = new object[]
				{
					"datetimeoffset",
					34,
					10L,
					"datetimeoffset({0})",
					"scale",
					"System.DateTimeOffset",
					false,
					true,
					false,
					false,
					false,
					false,
					true,
					true,
					true,
					DBNull.Value,
					7,
					0,
					false,
					DBNull.Value,
					"{ts '",
					"'}"
				};
				SqlConnection.DataTypes.rows = array;
			}

			public static DataTable Instance
			{
				get
				{
					if (SqlConnection.DataTypes.instance == null)
					{
						SqlConnection.DataTypes.instance = new DataTable("DataTypes");
						foreach (SqlConnection.ColumnInfo columnInfo in SqlConnection.DataTypes.columns)
						{
							SqlConnection.DataTypes.instance.Columns.Add(columnInfo.name, columnInfo.type);
						}
						foreach (object[] array3 in SqlConnection.DataTypes.rows)
						{
							SqlConnection.DataTypes.instance.LoadDataRow(array3, true);
						}
					}
					return SqlConnection.DataTypes.instance;
				}
			}

			private static readonly SqlConnection.ColumnInfo[] columns = new SqlConnection.ColumnInfo[]
			{
				new SqlConnection.ColumnInfo("TypeName", typeof(string)),
				new SqlConnection.ColumnInfo("ProviderDbType", typeof(int)),
				new SqlConnection.ColumnInfo("ColumnSize", typeof(long)),
				new SqlConnection.ColumnInfo("CreateFormat", typeof(string)),
				new SqlConnection.ColumnInfo("CreateParameters", typeof(string)),
				new SqlConnection.ColumnInfo("DataType", typeof(string)),
				new SqlConnection.ColumnInfo("IsAutoIncrementable", typeof(bool)),
				new SqlConnection.ColumnInfo("IsBestMatch", typeof(bool)),
				new SqlConnection.ColumnInfo("IsCaseSensitive", typeof(bool)),
				new SqlConnection.ColumnInfo("IsFixedLength", typeof(bool)),
				new SqlConnection.ColumnInfo("IsFixedPrecisionScale", typeof(bool)),
				new SqlConnection.ColumnInfo("IsLong", typeof(bool)),
				new SqlConnection.ColumnInfo("IsNullable", typeof(bool)),
				new SqlConnection.ColumnInfo("IsSearchable", typeof(bool)),
				new SqlConnection.ColumnInfo("IsSearchableWithLike", typeof(bool)),
				new SqlConnection.ColumnInfo("IsUnsigned", typeof(bool)),
				new SqlConnection.ColumnInfo("MaximumScale", typeof(short)),
				new SqlConnection.ColumnInfo("MinimumScale", typeof(short)),
				new SqlConnection.ColumnInfo("IsConcurrencyType", typeof(bool)),
				new SqlConnection.ColumnInfo("IsLiteralSupported", typeof(bool)),
				new SqlConnection.ColumnInfo("LiteralPrefix", typeof(string)),
				new SqlConnection.ColumnInfo("LiteralSuffix", typeof(string))
			};

			private static readonly object[][] rows;

			private static DataTable instance;
		}

		private static class Restrictions
		{
			public static DataTable Instance
			{
				get
				{
					if (SqlConnection.Restrictions.instance == null)
					{
						SqlConnection.Restrictions.instance = new DataTable("Restrictions");
						foreach (SqlConnection.ColumnInfo columnInfo in SqlConnection.Restrictions.columns)
						{
							SqlConnection.Restrictions.instance.Columns.Add(columnInfo.name, columnInfo.type);
						}
						foreach (object[] array3 in SqlConnection.Restrictions.rows)
						{
							SqlConnection.Restrictions.instance.LoadDataRow(array3, true);
						}
					}
					return SqlConnection.Restrictions.instance;
				}
			}

			private static readonly SqlConnection.ColumnInfo[] columns = new SqlConnection.ColumnInfo[]
			{
				new SqlConnection.ColumnInfo("CollectionName", typeof(string)),
				new SqlConnection.ColumnInfo("RestrictionName", typeof(string)),
				new SqlConnection.ColumnInfo("ParameterName", typeof(string)),
				new SqlConnection.ColumnInfo("RestrictionDefault", typeof(string)),
				new SqlConnection.ColumnInfo("RestrictionNumber", typeof(int))
			};

			private static readonly object[][] rows = new object[][]
			{
				new object[] { "Users", "User_Name", "@Name", "name", 1 },
				new object[] { "Databases", "Name", "@Name", "Name", 1 },
				new object[] { "Tables", "Catalog", "@Catalog", "TABLE_CATALOG", 1 },
				new object[] { "Tables", "Owner", "@Owner", "TABLE_SCHEMA", 2 },
				new object[] { "Tables", "Table", "@Name", "TABLE_NAME", 3 },
				new object[] { "Tables", "TableType", "@TableType", "TABLE_TYPE", 4 },
				new object[] { "Columns", "Catalog", "@Catalog", "TABLE_CATALOG", 1 },
				new object[] { "Columns", "Owner", "@Owner", "TABLE_SCHEMA", 2 },
				new object[] { "Columns", "Table", "@Table", "TABLE_NAME", 3 },
				new object[] { "Columns", "Column", "@Column", "COLUMN_NAME", 4 },
				new object[] { "StructuredTypeMembers", "Catalog", "@Catalog", "TYPE_CATALOG", 1 },
				new object[] { "StructuredTypeMembers", "Owner", "@Owner", "TYPE_SCHEMA", 2 },
				new object[] { "StructuredTypeMembers", "Type", "@Type", "TYPE_NAME", 3 },
				new object[] { "StructuredTypeMembers", "Member", "@Member", "MEMBER_NAME", 4 },
				new object[] { "Views", "Catalog", "@Catalog", "TABLE_CATALOG", 1 },
				new object[] { "Views", "Owner", "@Owner", "TABLE_SCHEMA", 2 },
				new object[] { "Views", "Table", "@Table", "TABLE_NAME", 3 },
				new object[] { "ViewColumns", "Catalog", "@Catalog", "VIEW_CATALOG", 1 },
				new object[] { "ViewColumns", "Owner", "@Owner", "VIEW_SCHEMA", 2 },
				new object[] { "ViewColumns", "Table", "@Table", "VIEW_NAME", 3 },
				new object[] { "ViewColumns", "Column", "@Column", "COLUMN_NAME", 4 },
				new object[] { "ProcedureParameters", "Catalog", "@Catalog", "SPECIFIC_CATALOG", 1 },
				new object[] { "ProcedureParameters", "Owner", "@Owner", "SPECIFIC_SCHEMA", 2 },
				new object[] { "ProcedureParameters", "Name", "@Name", "SPECIFIC_NAME", 3 },
				new object[] { "ProcedureParameters", "Parameter", "@Parameter", "PARAMETER_NAME", 4 },
				new object[] { "Procedures", "Catalog", "@Catalog", "SPECIFIC_CATALOG", 1 },
				new object[] { "Procedures", "Owner", "@Owner", "SPECIFIC_SCHEMA", 2 },
				new object[] { "Procedures", "Name", "@Name", "SPECIFIC_NAME", 3 },
				new object[] { "Procedures", "Type", "@Type", "ROUTINE_TYPE", 4 },
				new object[] { "IndexColumns", "Catalog", "@Catalog", "db_name()", 1 },
				new object[] { "IndexColumns", "Owner", "@Owner", "user_name()", 2 },
				new object[] { "IndexColumns", "Table", "@Table", "o.name", 3 },
				new object[] { "IndexColumns", "ConstraintName", "@ConstraintName", "x.name", 4 },
				new object[] { "IndexColumns", "Column", "@Column", "c.name", 5 },
				new object[] { "Indexes", "Catalog", "@Catalog", "db_name()", 1 },
				new object[] { "Indexes", "Owner", "@Owner", "user_name()", 2 },
				new object[] { "Indexes", "Table", "@Table", "o.name", 3 },
				new object[] { "Indexes", "Name", "@Name", "x.name", 4 },
				new object[] { "UserDefinedTypes", "assembly_name", "@AssemblyName", "assemblies.name", 1 },
				new object[] { "UserDefinedTypes", "udt_name", "@UDTName", "types.assembly_class", 2 },
				new object[] { "ForeignKeys", "Catalog", "@Catalog", "CONSTRAINT_CATALOG", 1 },
				new object[] { "ForeignKeys", "Owner", "@Owner", "CONSTRAINT_SCHEMA", 2 },
				new object[] { "ForeignKeys", "Table", "@Table", "TABLE_NAME", 3 },
				new object[] { "ForeignKeys", "Name", "@Name", "CONSTRAINT_NAME", 4 }
			};

			private static DataTable instance;
		}
	}
}
