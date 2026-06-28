using System;
using System.Data;
using System.Data.SqlClient;

namespace FileHelpers.DataLink
{
	public sealed class SqlServerStorage : DatabaseStorage
	{
		public SqlServerStorage(Type recordType)
			: this(recordType, string.Empty)
		{
		}

		public SqlServerStorage(Type recordType, string connectionStr)
			: base(recordType)
		{
			base.ConnectionString = connectionStr;
		}

		public SqlServerStorage(Type recordType, string server, string database)
			: this(recordType, server, database, string.Empty, string.Empty)
		{
		}

		public SqlServerStorage(Type recordType, string server, string database, string user, string pass)
			: this(recordType, DataBaseHelper.SqlConnectionString(server, database, user, pass))
		{
			this.mServerName = server;
			this.mDatabaseName = database;
			this.mUserName = user;
			this.mUserPass = pass;
		}

		protected sealed override IDbConnection CreateConnection()
		{
			string text;
			if (base.ConnectionString == string.Empty)
			{
				if (this.mServerName == null || this.mServerName == string.Empty)
				{
					throw new BadUsageException("The ServerName can't be null or empty.");
				}
				if (this.mDatabaseName == null || this.mDatabaseName == string.Empty)
				{
					throw new BadUsageException("The DatabaseName can't be null or empty.");
				}
				text = DataBaseHelper.SqlConnectionString(this.ServerName, this.DatabaseName, this.UserName, this.UserPass);
			}
			else
			{
				text = base.ConnectionString;
			}
			return new SqlConnection(text);
		}

		public string ServerName
		{
			get
			{
				return this.mServerName;
			}
			set
			{
				this.mServerName = value;
				base.ConnectionString = DataBaseHelper.SqlConnectionString(this.ServerName, this.DatabaseName, this.UserName, this.UserPass);
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.mDatabaseName;
			}
			set
			{
				this.mDatabaseName = value;
				base.ConnectionString = DataBaseHelper.SqlConnectionString(this.ServerName, this.DatabaseName, this.UserName, this.UserPass);
			}
		}

		public string UserName
		{
			get
			{
				return this.mUserName;
			}
			set
			{
				this.mUserName = value;
				base.ConnectionString = DataBaseHelper.SqlConnectionString(this.ServerName, this.DatabaseName, this.UserName, this.UserPass);
			}
		}

		public string UserPass
		{
			get
			{
				return this.mUserPass;
			}
			set
			{
				this.mUserPass = value;
				base.ConnectionString = DataBaseHelper.SqlConnectionString(this.ServerName, this.DatabaseName, this.UserName, this.UserPass);
			}
		}

		protected override bool ExecuteInBatch
		{
			get
			{
				return true;
			}
		}

		private string mServerName = string.Empty;

		private string mDatabaseName = string.Empty;

		private string mUserName = string.Empty;

		private string mUserPass = string.Empty;
	}
}
