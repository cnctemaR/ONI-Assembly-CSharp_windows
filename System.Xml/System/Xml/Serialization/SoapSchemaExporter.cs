using System;

namespace System.Xml.Serialization
{
	public class SoapSchemaExporter
	{
		public SoapSchemaExporter(XmlSchemas schemas)
		{
			this._exporter = new XmlSchemaExporter(schemas, true);
		}

		public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping)
		{
			this._exporter.ExportMembersMapping(xmlMembersMapping, false);
		}

		public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping, bool exportEnclosingType)
		{
			this._exporter.ExportMembersMapping(xmlMembersMapping, exportEnclosingType);
		}

		public void ExportTypeMapping(XmlTypeMapping xmlTypeMapping)
		{
			this._exporter.ExportTypeMapping(xmlTypeMapping);
		}

		private XmlSchemaExporter _exporter;
	}
}
