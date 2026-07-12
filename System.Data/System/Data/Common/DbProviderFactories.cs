using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace System.Data.Common
{
	public static class DbProviderFactories
	{
		public static DbProviderFactory GetFactory(string providerInvariantName)
		{
			return DbProviderFactories.GetFactory(providerInvariantName, true);
		}

		public static DbProviderFactory GetFactory(string providerInvariantName, bool throwOnError)
		{
			if (throwOnError)
			{
				ADP.CheckArgumentLength(providerInvariantName, "providerInvariantName");
			}
			DataTable providerTable = DbProviderFactories.GetProviderTable();
			if (providerTable != null)
			{
				DataRow dataRow = providerTable.Rows.Find(providerInvariantName);
				if (dataRow != null)
				{
					return DbProviderFactories.GetFactory(dataRow);
				}
			}
			if (throwOnError)
			{
				throw ADP.ConfigProviderNotFound();
			}
			return null;
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
							DataSet dataSet = global::System.Configuration.PrivilegedConfigurationManager.GetSection("system.data") as DataSet;
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

		public static bool TryGetFactory(string providerInvariantName, out DbProviderFactory factory)
		{
			factory = DbProviderFactories.GetFactory(providerInvariantName, false);
			return factory != null;
		}

		public static IEnumerable<string> GetProviderInvariantNames()
		{
			return DbProviderFactories._registeredFactories.Keys.ToList<string>();
		}

		public static void RegisterFactory(string providerInvariantName, string factoryTypeAssemblyQualifiedName)
		{
			ADP.CheckArgumentLength(providerInvariantName, "providerInvariantName");
			ADP.CheckArgumentLength(factoryTypeAssemblyQualifiedName, "factoryTypeAssemblyQualifiedName");
			DbProviderFactories._registeredFactories[providerInvariantName] = new DbProviderFactories.ProviderRegistration(factoryTypeAssemblyQualifiedName, null);
		}

		private static DbProviderFactory GetFactoryInstance(Type providerFactoryClass)
		{
			ADP.CheckArgumentNull(providerFactoryClass, "providerFactoryClass");
			if (!providerFactoryClass.IsSubclassOf(typeof(DbProviderFactory)))
			{
				throw ADP.Argument(SR.Format("The type '{0}' doesn't inherit from DbProviderFactory.", providerFactoryClass.FullName));
			}
			FieldInfo field = providerFactoryClass.GetField("Instance", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
			if (null == field)
			{
				throw ADP.InvalidOperation("The requested .NET Data Provider's implementation does not have an Instance field of a System.Data.Common.DbProviderFactory derived type.");
			}
			if (!field.FieldType.IsSubclassOf(typeof(DbProviderFactory)))
			{
				throw ADP.InvalidOperation("The requested .NET Data Provider's implementation does not have an Instance field of a System.Data.Common.DbProviderFactory derived type.");
			}
			object value = field.GetValue(null);
			if (value == null)
			{
				throw ADP.InvalidOperation("The requested .NET Data Provider's implementation does not have an Instance field of a System.Data.Common.DbProviderFactory derived type.");
			}
			return (DbProviderFactory)value;
		}

		public static void RegisterFactory(string providerInvariantName, Type providerFactoryClass)
		{
			DbProviderFactories.RegisterFactory(providerInvariantName, DbProviderFactories.GetFactoryInstance(providerFactoryClass));
		}

		public static void RegisterFactory(string providerInvariantName, DbProviderFactory factory)
		{
			ADP.CheckArgumentLength(providerInvariantName, "providerInvariantName");
			ADP.CheckArgumentNull(factory, "factory");
			DbProviderFactories._registeredFactories[providerInvariantName] = new DbProviderFactories.ProviderRegistration(factory.GetType().AssemblyQualifiedName, factory);
		}

		public static bool UnregisterFactory(string providerInvariantName)
		{
			DbProviderFactories.ProviderRegistration providerRegistration;
			return !string.IsNullOrWhiteSpace(providerInvariantName) && DbProviderFactories._registeredFactories.TryRemove(providerInvariantName, out providerRegistration);
		}

		private const string AssemblyQualifiedName = "AssemblyQualifiedName";

		private const string Instance = "Instance";

		private const string InvariantName = "InvariantName";

		private const string Name = "Name";

		private const string Description = "Description";

		private const string InstanceFieldName = "Instance";

		private static ConcurrentDictionary<string, DbProviderFactories.ProviderRegistration> _registeredFactories = new ConcurrentDictionary<string, DbProviderFactories.ProviderRegistration>();

		private static ConnectionState _initState;

		private static DataTable _providerTable;

		private static object _lockobj = new object();

		private struct ProviderRegistration
		{
			internal ProviderRegistration(string factoryTypeAssemblyQualifiedName, DbProviderFactory factoryInstance)
			{
				this.FactoryTypeAssemblyQualifiedName = factoryTypeAssemblyQualifiedName;
				this.FactoryInstance = factoryInstance;
			}

			internal readonly string FactoryTypeAssemblyQualifiedName { get; }

			internal readonly DbProviderFactory FactoryInstance { get; }
		}
	}
}
