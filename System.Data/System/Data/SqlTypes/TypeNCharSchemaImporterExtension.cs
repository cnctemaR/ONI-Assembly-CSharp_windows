using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeNCharSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeNCharSchemaImporterExtension()
			: base("nchar", "System.Data.SqlTypes.SqlString", false)
		{
		}
	}
}
