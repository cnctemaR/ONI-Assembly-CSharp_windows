using System;
using System.Configuration;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Reflection;

namespace System.Data.Common
{
	public static class DbProviderFactories
	{
		public static DbProviderFactory GetFactory(string providerInvariantName)
		{
			ADP.CheckArgumentLength(providerInvariantName, "providerInvariantName");
			DataTable providerTable = DbProviderFactories.GetProviderTable();
			if (providerTable != null)
			{
				DataRow dataRow = providerTable.Rows.Find(providerInvariantName);
				if (dataRow != null)
				{
					return DbProviderFactories.GetFactory(dataRow);
				}
			}
			throw ADP.ConfigProviderNotFound();
		}

		public static DbProviderFactory GetFactory(DataRow providerRow)
		{
			ADP.CheckArgumentNull(providerRow, "providerRow");
			DataColumn dataColumn = providerRow.Table.Columns["AssemblyQualifiedName"];
			if (dataColumn != null)
			{
				string text = providerRow[dataColumn] as string;
				if (!ADP.IsEmpty(text))
				{
					Type type = Type.GetType(text);
					if (null != type)
					{
						FieldInfo field = type.GetField("Instance", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
						if (null != field && field.FieldType.IsSubclassOf(typeof(DbProviderFactory)))
						{
							object value = field.GetValue(null);
							if (value != null)
							{
								return (DbProviderFactory)value;
							}
						}
						throw ADP.ConfigProviderInvalid();
					}
					throw ADP.ConfigProviderNotInstalled();
				}
			}
			throw ADP.ConfigProviderMissing();
		}

		public static DbProviderFactory GetFactory(DbConnection connection)
		{
			ADP.CheckArgumentNull(connection, "connection");
			return connection.ProviderFactory;
		}

		public static DataTable GetFactoryClasses()
		{
			DataTable dataTable = DbProviderFactories.GetProviderTable();
			if (dataTable != null)
			{
				dataTable = dataTable.Copy();
			}
			else
			{
				dataTable = DbProviderFactoriesConfigurationHandler.CreateProviderDataTable();
			}
			return dataTable;
		}

		private static DataTable IncludeFrameworkFactoryClasses(DataTable configDataTable)
		{
			DataTable dataTable = DbProviderFactoriesConfigurationHandler.CreateProviderDataTable();
			string text = typeof(SqlClientFactory).AssemblyQualifiedName.ToString().Replace("System.Data.SqlClient.SqlClientFactory, System.Data,", "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient,");
			DbProviderFactoryConfigSection[] array = new DbProviderFactoryConfigSection[]
			{
				new DbProviderFactoryConfigSection(typeof(OdbcFactory), "Odbc Data Provider", ".Net Framework Data Provider for Odbc"),
				new DbProviderFactoryConfigSection(typeof(OleDbFactory), "OleDb Data Provider", ".Net Framework Data Provider for OleDb"),
				new DbProviderFactoryConfigSection("OracleClient Data Provider", "System.Data.OracleClient", ".Net Framework Data Provider for Oracle", text),
				new DbProviderFactoryConfigSection(typeof(SqlClientFactory), "SqlClient Data Provider", ".Net Framework Data Provider for SqlServer")
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].IsNull())
				{
					bool flag = false;
					if (i == 2)
					{
						Type type = Type.GetType(array[i].AssemblyQualifiedName);
						if (type != null)
						{
							FieldInfo field = type.GetField("Instance", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
							if (null != field && field.FieldType.IsSubclassOf(typeof(DbProviderFactory)))
							{
								object value = field.GetValue(null);
								if (value != null)
								{
									flag = true;
								}
							}
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["Name"] = array[i].Name;
						dataRow["InvariantName"] = array[i].InvariantName;
						dataRow["Description"] = array[i].Description;
						dataRow["AssemblyQualifiedName"] = array[i].AssemblyQualifiedName;
						dataTable.Rows.Add(dataRow);
					}
				}
			}
			int num = 0;
			while (configDataTable != null && num < configDataTable.Rows.Count)
			{
				try
				{
					bool flag2 = false;
					if (configDataTable.Rows[num]["AssemblyQualifiedName"].ToString().ToLowerInvariant().Contains("System.Data.OracleClient".ToString().ToLowerInvariant()))
					{
						Type type2 = Type.GetType(configDataTable.Rows[num]["AssemblyQualifiedName"].ToString());
						if (type2 != null)
						{
							FieldInfo field2 = type2.GetField("Instance", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
							if (null != field2 && field2.FieldType.IsSubclassOf(typeof(DbProviderFactory)))
							{
								object value2 = field2.GetValue(null);
								if (value2 != null)
								{
									flag2 = true;
								}
							}
						}
					}
					else
					{
						flag2 = true;
					}
					if (flag2)
					{
						dataTable.Rows.Add(configDataTable.Rows[num].ItemArray);
					}
				}
				catch (ConstraintException)
				{
				}
				num++;
			}
			return dataTable;
		}

		private static DataTable GetProviderTable()
		{
			DbProviderFactories.Initialize();
			return DbProviderFactories._providerTable;
		}

		private static void Initialize()
		{
			if (ConnectionState.Open != DbProviderFactories._initState)
			{
				object lockobj = DbProviderFactories._lockobj;
				lock (lockobj)
				{
					ConnectionState initState = DbProviderFactories._initState;
					if (initState != ConnectionState.Closed)
					{
						if (initState - ConnectionState.Open > 1)
						{
						}
					}
					else
					{
						DbProviderFactories._initState = ConnectionState.Connecting;
						try
						{
							DataSet dataSet = PrivilegedConfigurationManager.GetSection("system.data") as DataSet;
							DbProviderFactories._providerTable = ((dataSet != null) ? DbProviderFactories.IncludeFrameworkFactoryClasses(dataSet.Tables["DbProviderFactories"]) : DbProviderFactories.IncludeFrameworkFactoryClasses(null));
						}
						finally
						{
							DbProviderFactories._initState = ConnectionState.Open;
						}
					}
				}
			}
		}

		private const string AssemblyQualifiedName = "AssemblyQualifiedName";

		private const string Instance = "Instance";

		private const string InvariantName = "InvariantName";

		private const string Name = "Name";

		private const string Description = "Description";

		private static ConnectionState _initState;

		private static DataTable _providerTable;

		private static object _lockobj = new object();
	}
}
