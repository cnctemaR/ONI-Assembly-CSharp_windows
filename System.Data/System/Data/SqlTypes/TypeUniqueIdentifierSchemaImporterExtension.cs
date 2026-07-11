using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeUniqueIdentifierSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeUniqueIdentifierSchemaImporterExtension()
			: base("uniqueidentifier", "System.Data.SqlTypes.SqlGuid")
		{
		}
	}
}
