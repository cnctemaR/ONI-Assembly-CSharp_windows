using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeTextSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeTextSchemaImporterExtension()
			: base("text", "System.Data.SqlTypes.SqlString", false)
		{
		}
	}
}
