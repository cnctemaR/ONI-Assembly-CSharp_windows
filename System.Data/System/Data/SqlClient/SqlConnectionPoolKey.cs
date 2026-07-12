using System;
using System.Data.Common;

namespace System.Data.SqlClient
{
	internal class SqlConnectionPoolKey : DbConnectionPoolKey
	{
		internal SqlConnectionPoolKey(string connectionString, SqlCredential credential, string accessToken)
			: base(connectionString)
		{
			this._credential = credential;
			this._accessToken = accessToken;
			this.CalculateHashCode();
		}

		private SqlConnectionPoolKey(SqlConnectionPoolKey key)
			: base(key)
		{
			this._credential = key.Credential;
			this._accessToken = key.AccessToken;
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

		internal SqlCredential Credential
		{
			get
			{
				return this._credential;
			}
		}

		internal string AccessToken
		{
			get
			{
				return this._accessToken;
			}
		}

		public override bool Equals(object obj)
		{
			SqlConnectionPoolKey sqlConnectionPoolKey = obj as SqlConnectionPoolKey;
			return sqlConnectionPoolKey != null && this._credential == sqlConnectionPoolKey._credential && this.ConnectionString == sqlConnectionPoolKey.ConnectionString && this._accessToken == sqlConnectionPoolKey._accessToken;
		}

		public override int GetHashCode()
		{
			return this._hashValue;
		}

		private void CalculateHashCode()
		{
			this._hashValue = base.GetHashCode();
			if (this._credential != null)
			{
				this._hashValue = this._hashValue * 17 + this._credential.GetHashCode();
				return;
			}
			if (this._accessToken != null)
			{
				this._hashValue = this._hashValue * 17 + this._accessToken.GetHashCode();
			}
		}

		private int _hashValue;

		private SqlCredential _credential;

		private readonly string _accessToken;
	}
}
