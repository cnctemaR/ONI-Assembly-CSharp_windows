using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;

internal class XmlTableWriter
{
	internal static void WriteTables(XmlWriter writer, XmlWriteMode mode, List<DataTable> tables, List<DataRelation> relations, string mainDataTable, string dataSetName)
	{
		if (mode == XmlWriteMode.DiffGram)
		{
			foreach (DataTable dataTable in tables)
			{
				dataTable.SetRowsID();
			}
			DataSet.WriteDiffGramElement(writer);
		}
		bool flag = mode != XmlWriteMode.DiffGram;
		int num = 0;
		while (num < tables.Count && !flag)
		{
			flag = tables[num].Rows.Count > 0;
			num++;
		}
		if (flag)
		{
			DataSet.WriteStartElement(writer, mode, tables[0].Namespace, tables[0].Prefix, XmlHelper.Encode(dataSetName));
			if (mode == XmlWriteMode.WriteSchema)
			{
				DataTable[] array = new DataTable[tables.Count];
				tables.CopyTo(array);
				DataRelation[] array2 = new DataRelation[relations.Count];
				relations.CopyTo(array2);
				DataTable dataTable2 = array[0];
				new XmlSchemaWriter(writer, array, array2, mainDataTable, dataSetName, (!dataTable2.LocaleSpecified) ? null : dataTable2.Locale).WriteSchema();
			}
			XmlTableWriter.WriteTableList(writer, mode, tables, DataRowVersion.Default);
			writer.WriteEndElement();
		}
		if (mode == XmlWriteMode.DiffGram)
		{
			List<DataTable> list = new List<DataTable>();
			foreach (DataTable dataTable3 in tables)
			{
				DataTable changes = dataTable3.GetChanges(DataRowState.Deleted | DataRowState.Modified);
				if (changes != null && changes.Rows.Count > 0)
				{
					list.Add(changes);
				}
			}
			if (list.Count > 0)
			{
				DataSet.WriteStartElement(writer, XmlWriteMode.DiffGram, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "before");
				XmlTableWriter.WriteTableList(writer, mode, list, DataRowVersion.Original);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
		writer.Flush();
	}

	internal static void WriteTableList(XmlWriter writer, XmlWriteMode mode, List<DataTable> tables, DataRowVersion version)
	{
		foreach (DataTable dataTable in tables)
		{
			DataSet.WriteTable(writer, dataTable, mode, version);
		}
	}
}
