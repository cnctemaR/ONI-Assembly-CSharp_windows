using System;
using System.Configuration;
using System.Reflection;
using System.Threading;

namespace System.Data.Common
{
	public static class DbProviderFactories
	{
		public static DbProviderFactory GetFactory(DataRow providerRow)
		{
			string text = (string)providerRow["AssemblyQualifiedName"];
			Type type = Type.GetType(text, false, true);
			if (type != null && type.IsSubclassOf(typeof(DbProviderFactory)))
			{
				FieldInfo field = type.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
				if (field != null)
				{
					return field.GetValue(null) as DbProviderFactory;
				}
			}
			throw new ConfigurationErrorsException("Failed to find or load the registered .Net Framework Data Provider.");
		}

		public static DbProviderFactory GetFactory(string providerInvariantName)
		{
			DataTable factoryClasses = DbProviderFactories.GetFactoryClasses();
			if (factoryClasses != null)
			{
				DataRow dataRow = factoryClasses.Rows.Find(providerInvariantName);
				if (dataRow != null)
				{
					return DbProviderFactories.GetFactory(dataRow);
				}
			}
			throw new ConfigurationErrorsException(string.Format("Failed to find or load the registered .Net Framework Data Provider '{0}'.", providerInvariantName));
		}

		public static DataTable GetFactoryClasses()
		{
			DataSet dataSet = DbProviderFactories.GetConfigEntries();
			DataTable dataTable = ((dataSet == null) ? null : dataSet.Tables["DbProviderFactories"]);
			if (dataTable != null)
			{
				dataTable = dataTable.Copy();
			}
			return dataTable;
		}

		internal static DataSet GetConfigEntries()
		{
			if (DbProviderFactories.configEntries != null)
			{
				return DbProviderFactories.configEntries as DataSet;
			}
			DataSet dataSet = (DataSet)ConfigurationManager.GetSection("system.data");
			Interlocked.CompareExchange(ref DbProviderFactories.configEntries, dataSet, null);
			return DbProviderFactories.configEntries as DataSet;
		}

		internal const string CONFIG_SECTION_NAME = "system.data";

		internal const string CONFIG_SEC_TABLE_NAME = "DbProviderFactories";

		private static object configEntries;
	}
}
