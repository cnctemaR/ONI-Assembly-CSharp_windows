using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeDateTimeSchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeDateTimeSchemaImporterExtension()
			: base("datetime", "System.Data.SqlTypes.SqlDateTime")
		{
		}
	}
}
