using System;

namespace System.Xml.Schema
{
	internal class RedefineEntry
	{
		public RedefineEntry(XmlSchemaRedefine external, XmlSchema schema)
		{
			this.redefine = external;
			this.schemaToUpdate = schema;
		}

		internal XmlSchemaRedefine redefine;

		internal XmlSchema schemaToUpdate;
	}
}
