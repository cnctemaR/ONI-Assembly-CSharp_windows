using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeVarCharSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeVarCharSchemaImporterExtension()
			: base("varchar", "System.Data.SqlTypes.SqlString", false)
		{
		}
	}
}
