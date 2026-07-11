using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeIntSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeIntSchemaImporterExtension()
			: base("int", "System.Data.SqlTypes.SqlInt32")
		{
		}
	}
}
