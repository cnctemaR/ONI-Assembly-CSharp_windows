using System;
using System.Data.Common;

namespace System.Data.SqlClient
{
	internal class SqlConnectionPoolKey : DbConnectionPoolKey
	{
		internal SqlConnectionPoolKey(string connectionString)
			: base(connectionString)
		{
			this.CalculateHashCode();
		}

		private SqlConnectionPoolKey(SqlConnectionPoolKey key)
			: base(key)
		{
			this.CalculateHashCode();
		}

		public override object Clone()
		{
			return new SqlConnectionPoolKey(this);
		}

		internal override string ConnectionString
		{
			get
			{
				return base.ConnectionString;
			}
			set
			{
				base.ConnectionString = value;
				this.CalculateHashCode();
			}
		}

		public override bool Equals(object obj)
		{
			SqlConnectionPoolKey sqlConnectionPoolKey = obj as SqlConnectionPoolKey;
			return sqlConnectionPoolKey != null && this.ConnectionString == sqlConnectionPoolKey.ConnectionString;
		}

		public override int GetHashCode()
		{
			return this._hashValue;
		}

		private void CalculateHashCode()
		{
			this._hashValue = base.GetHashCode();
		}

		private int _hashValue;
	}
}
