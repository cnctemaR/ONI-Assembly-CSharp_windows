using System;
using System.Configuration;
using System.Xml;

namespace System.Data.Common
{
	public class DbProviderFactoriesConfigurationHandler : IConfigurationSectionHandler
	{
		public virtual object Create(object parent, object configContext, XmlNode section)
		{
			DataSet dataSet = (parent as DataSet) ?? this.CreateDataSet();
			this.FillDataTables(dataSet, section);
			return dataSet;
		}

		private DataSet CreateDataSet()
		{
			DataSet dataSet = new DataSet("system.data");
			DataTable dataTable = dataSet.Tables.Add("DbProviderFactories");
			DataColumn[] array = new DataColumn[]
			{
				new DataColumn("Name", typeof(string)),
				new DataColumn("Description", typeof(string)),
				new DataColumn("InvariantName", typeof(string)),
				new DataColumn("AssemblyQualifiedName", typeof(string))
			};
			dataTable.Columns.AddRange(array);
			dataTable.PrimaryKey = new DataColumn[] { array[2] };
			return dataSet;
		}

		private void FillDataTables(DataSet ds, XmlNode section)
		{
			DataTable dataTable = ds.Tables[0];
			foreach (object obj in section.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					if (xmlNode.Name == "DbProviderFactories")
					{
						foreach (object obj2 in xmlNode.ChildNodes)
						{
							XmlNode xmlNode2 = (XmlNode)obj2;
							if (xmlNode2.NodeType == XmlNodeType.Element)
							{
								string name = xmlNode2.Name;
								switch (name)
								{
								case "add":
									this.AddRow(dataTable, xmlNode2);
									continue;
								case "clear":
									dataTable.Rows.Clear();
									continue;
								case "remove":
									this.RemoveRow(dataTable, xmlNode2);
									continue;
								}
								throw new ConfigurationErrorsException("Unrecognized element.", xmlNode2);
							}
						}
					}
				}
			}
		}

		private string GetAttributeValue(XmlNode node, string name, bool required)
		{
			XmlAttribute xmlAttribute = node.Attributes[name];
			if (xmlAttribute == null)
			{
				if (!required)
				{
					return null;
				}
				throw new ConfigurationErrorsException("Required Attribute '" + name + "' is  missing!", node);
			}
			else
			{
				string value = xmlAttribute.Value;
				if (value == string.Empty)
				{
					throw new ConfigurationException("Attribute '" + name + "' cannot be empty!", node);
				}
				return value;
			}
		}

		private void AddRow(DataTable dt, XmlNode addNode)
		{
			string attributeValue = this.GetAttributeValue(addNode, "name", true);
			string attributeValue2 = this.GetAttributeValue(addNode, "description", true);
			string attributeValue3 = this.GetAttributeValue(addNode, "invariant", true);
			string attributeValue4 = this.GetAttributeValue(addNode, "type", true);
			DataRow dataRow = dt.NewRow();
			dataRow[0] = attributeValue;
			dataRow[1] = attributeValue2;
			dataRow[2] = attributeValue3;
			dataRow[3] = attributeValue4;
			dt.Rows.Add(dataRow);
		}

		private void RemoveRow(DataTable dt, XmlNode removeNode)
		{
			string attributeValue = this.GetAttributeValue(removeNode, "invariant", true);
			DataRow dataRow = dt.Rows.Find(attributeValue);
			if (dataRow != null)
			{
				dataRow.Delete();
			}
		}
	}
}
