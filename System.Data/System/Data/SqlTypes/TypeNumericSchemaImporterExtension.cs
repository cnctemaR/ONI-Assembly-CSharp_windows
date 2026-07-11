using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeNumericSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeNumericSchemaImporterExtension()
			: base("numeric", "System.Data.SqlTypes.SqlDecimal", false)
		{
		}
	}
}
