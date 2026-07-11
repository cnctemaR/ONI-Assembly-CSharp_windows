using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeNVarCharSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeNVarCharSchemaImporterExtension()
			: base("nvarchar", "System.Data.SqlTypes.SqlString", false)
		{
		}
	}
}
