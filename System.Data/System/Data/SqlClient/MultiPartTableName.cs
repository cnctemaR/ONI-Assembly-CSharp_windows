using System;
using System.Data.Common;

namespace System.Data.SqlClient
{
	internal struct MultiPartTableName
	{
		internal MultiPartTableName(string[] parts)
		{
			this._multipartName = null;
			this._serverName = parts[0];
			this._catalogName = parts[1];
			this._schemaName = parts[2];
			this._tableName = parts[3];
		}

		internal MultiPartTableName(string multipartName)
		{
			this._multipartName = multipartName;
			this._serverName = null;
			this._catalogName = null;
			this._schemaName = null;
			this._tableName = null;
		}

		internal string ServerName
		{
			get
			{
				this.ParseMultipartName();
				return this._serverName;
			}
			set
			{
				this._serverName = value;
			}
		}

		internal string CatalogName
		{
			get
			{
				this.ParseMultipartName();
				return this._catalogName;
			}
			set
			{
				this._catalogName = value;
			}
		}

		internal string SchemaName
		{
			get
			{
				this.ParseMultipartName();
				return this._schemaName;
			}
			set
			{
				this._schemaName = value;
			}
		}

		internal string TableName
		{
			get
			{
				this.ParseMultipartName();
				return this._tableName;
			}
			set
			{
				this._tableName = value;
			}
		}

		private void ParseMultipartName()
		{
			if (this._multipartName != null)
			{
				string[] array = MultipartIdentifier.ParseMultipartIdentifier(this._multipartName, "[\"", "]\"", "Processing of results from SQL Server failed because of an invalid multipart name", false);
				this._serverName = array[0];
				this._catalogName = array[1];
				this._schemaName = array[2];
				this._tableName = array[3];
				this._multipartName = null;
			}
		}

		private string _multipartName;

		private string _serverName;

		private string _catalogName;

		private string _schemaName;

		private string _tableName;

		internal static readonly MultiPartTableName Null = new MultiPartTableName(new string[4]);
	}
}
