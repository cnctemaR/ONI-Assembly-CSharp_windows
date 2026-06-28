using System;
using System.Collections;
using System.Data.Common;

namespace System.Data
{
	internal class TableAdapterSchemaInfo
	{
		public TableAdapterSchemaInfo(DbProviderFactory provider)
		{
			this.Provider = provider;
			this.Adapter = provider.CreateDataAdapter();
			this.Connection = provider.CreateConnection();
			this.Commands = new ArrayList();
			this.ShortCommands = false;
		}

		public TableAdapterSchemaInfo()
		{
			this.Commands = new ArrayList();
			this.ShortCommands = false;
		}

		public DbProviderFactory Provider;

		public DbDataAdapter Adapter;

		public DbConnection Connection;

		public string ConnectionString;

		public string BaseClass;

		public string Name;

		public bool ShortCommands;

		public ArrayList Commands;
	}
}
