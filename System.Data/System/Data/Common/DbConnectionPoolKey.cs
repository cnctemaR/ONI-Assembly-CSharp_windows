using System;

namespace System.Data.Common
{
	internal class DbConnectionPoolKey : ICloneable
	{
		internal DbConnectionPoolKey(string connectionString)
		{
			this._connectionString = connectionString;
		}

		protected DbConnectionPoolKey(DbConnectionPoolKey key)
		{
			this._connectionString = key.ConnectionString;
		}

		public virtual object Clone()
		{
			return new DbConnectionPoolKey(this);
		}

		internal virtual string ConnectionString
		{
			get
			{
				return this._connectionString;
			}
			set
			{
				this._connectionString = value;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != typeof(DbConnectionPoolKey))
			{
				return false;
			}
			DbConnectionPoolKey dbConnectionPoolKey = obj as DbConnectionPoolKey;
			return dbConnectionPoolKey != null && this._connectionString == dbConnectionPoolKey._connectionString;
		}

		public override int GetHashCode()
		{
			if (this._connectionString != null)
			{
				return this._connectionString.GetHashCode();
			}
			return 0;
		}

		private string _connectionString;
	}
}
