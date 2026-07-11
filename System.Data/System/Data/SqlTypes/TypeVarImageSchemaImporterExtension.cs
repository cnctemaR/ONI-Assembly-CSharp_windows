using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeVarImageSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeVarImageSchemaImporterExtension()
			: base("image", "System.Data.SqlTypes.SqlBinary", false)
		{
		}
	}
}
