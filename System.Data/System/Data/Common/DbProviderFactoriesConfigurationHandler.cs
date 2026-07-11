using System;
using System.Configuration;
using System.Globalization;
using System.Xml;

namespace System.Data.Common
{
	public class DbProviderFactoriesConfigurationHandler : IConfigurationSectionHandler
	{
		public virtual object Create(object parent, object configContext, XmlNode section)
		{
			return DbProviderFactoriesConfigurationHandler.CreateStatic(parent, configContext, section);
		}

		internal static object CreateStatic(object parent, object configContext, XmlNode section)
		{
			object obj = parent;
			if (section != null)
			{
				obj = HandlerBase.CloneParent(parent as DataSet, false);
				bool flag = false;
				HandlerBase.CheckForUnrecognizedAttributes(section);
				foreach (object obj2 in section.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj2;
					if (!HandlerBase.IsIgnorableAlsoCheckForNonElement(xmlNode))
					{
						string name = xmlNode.Name;
						if (!(name == "DbProviderFactories"))
						{
							throw ADP.ConfigUnrecognizedElement(xmlNode);
						}
						if (flag)
						{
							throw ADP.ConfigSectionsUnique("DbProviderFactories");
						}
						flag = true;
						DbProviderFactoriesConfigurationHandler.HandleProviders(obj as DataSet, configContext, xmlNode, name);
					}
				}
			}
			return obj;
		}

		private static void HandleProviders(DataSet config, object configContext, XmlNode section, string sectionName)
		{
			DataTableCollection tables = config.Tables;
			DataTable dataTable = tables[sectionName];
			bool flag = dataTable != null;
			dataTable = DbProviderFactoriesConfigurationHandler.DbProviderDictionarySectionHandler.CreateStatic(dataTable, configContext, section);
			if (!flag)
			{
				tables.Add(dataTable);
			}
		}

		internal static DataTable CreateProviderDataTable()
		{
			DataColumn dataColumn = new DataColumn("Name", typeof(string));
			dataColumn.ReadOnly = true;
			DataColumn dataColumn2 = new DataColumn("Description", typeof(string));
			dataColumn2.ReadOnly = true;
			DataColumn dataColumn3 = new DataColumn("InvariantName", typeof(string));
			dataColumn3.ReadOnly = true;
			DataColumn dataColumn4 = new DataColumn("AssemblyQualifiedName", typeof(string));
			dataColumn4.ReadOnly = true;
			DataColumn[] array = new DataColumn[] { dataColumn3 };
			DataColumn[] array2 = new DataColumn[] { dataColumn, dataColumn2, dataColumn3, dataColumn4 };
			DataTable dataTable = new DataTable("DbProviderFactories");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.AddRange(array2);
			dataTable.PrimaryKey = array;
			return dataTable;
		}

		internal const string sectionName = "system.data";

		internal const string providerGroup = "DbProviderFactories";

		internal const string odbcProviderName = "Odbc Data Provider";

		internal const string odbcProviderDescription = ".Net Framework Data Provider for Odbc";

		internal const string oledbProviderName = "OleDb Data Provider";

		internal const string oledbProviderDescription = ".Net Framework Data Provider for OleDb";

		internal const string oracleclientProviderName = "OracleClient Data Provider";

		internal const string oracleclientProviderNamespace = "System.Data.OracleClient";

		internal const string oracleclientProviderDescription = ".Net Framework Data Provider for Oracle";

		internal const string sqlclientProviderName = "SqlClient Data Provider";

		internal const string sqlclientProviderDescription = ".Net Framework Data Provider for SqlServer";

		internal const string sqlclientPartialAssemblyQualifiedName = "System.Data.SqlClient.SqlClientFactory, System.Data,";

		internal const string oracleclientPartialAssemblyQualifiedName = "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient,";

		private static class DbProviderDictionarySectionHandler
		{
			internal static DataTable CreateStatic(DataTable config, object context, XmlNode section)
			{
				if (section != null)
				{
					HandlerBase.CheckForUnrecognizedAttributes(section);
					if (config == null)
					{
						config = DbProviderFactoriesConfigurationHandler.CreateProviderDataTable();
					}
					foreach (object obj in section.ChildNodes)
					{
						XmlNode xmlNode = (XmlNode)obj;
						if (!HandlerBase.IsIgnorableAlsoCheckForNonElement(xmlNode))
						{
							string name = xmlNode.Name;
							if (!(name == "add"))
							{
								if (!(name == "remove"))
								{
									if (!(name == "clear"))
									{
										throw ADP.ConfigUnrecognizedElement(xmlNode);
									}
									DbProviderFactoriesConfigurationHandler.DbProviderDictionarySectionHandler.HandleClear(xmlNode, config);
								}
								else
								{
									DbProviderFactoriesConfigurationHandler.DbProviderDictionarySectionHandler.HandleRemove(xmlNode, config);
								}
							}
							else
							{
								DbProviderFactoriesConfigurationHandler.DbProviderDictionarySectionHandler.HandleAdd(xmlNode, config);
							}
						}
					}
					config.AcceptChanges();
				}
				return config;
			}

			private static void HandleAdd(XmlNode child, DataTable config)
			{
				HandlerBase.CheckForChildNodes(child);
				DataRow dataRow = config.NewRow();
				dataRow[0] = HandlerBase.RemoveAttribute(child, "name", true, false);
				dataRow[1] = HandlerBase.RemoveAttribute(child, "description", true, false);
				dataRow[2] = HandlerBase.RemoveAttribute(child, "invariant", true, false);
				dataRow[3] = HandlerBase.RemoveAttribute(child, "type", true, false);
				HandlerBase.RemoveAttribute(child, "support", false, false);
				HandlerBase.CheckForUnrecognizedAttributes(child);
				config.Rows.Add(dataRow);
			}

			private static void HandleRemove(XmlNode child, DataTable config)
			{
				HandlerBase.CheckForChildNodes(child);
				string text = HandlerBase.RemoveAttribute(child, "invariant", true, false);
				HandlerBase.CheckForUnrecognizedAttributes(child);
				DataRow dataRow = config.Rows.Find(text);
				if (dataRow != null)
				{
					dataRow.Delete();
				}
			}

			private static void HandleClear(XmlNode child, DataTable config)
			{
				HandlerBase.CheckForChildNodes(child);
				HandlerBase.CheckForUnrecognizedAttributes(child);
				config.Clear();
			}
		}
	}
}
