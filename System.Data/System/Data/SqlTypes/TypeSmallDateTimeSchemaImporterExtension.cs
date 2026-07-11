using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeSmallDateTimeSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeSmallDateTimeSchemaImporterExtension()
			: base("smalldatetime", "System.Data.SqlTypes.SqlDateTime")
		{
		}
	}
}
