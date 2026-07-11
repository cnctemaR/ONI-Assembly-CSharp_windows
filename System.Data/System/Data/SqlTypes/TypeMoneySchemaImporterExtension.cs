using System;

namespace System.Data.SqlTypes
{
	public sealed class TypeMoneySchemaImporterExtension : SqlTypesSchemaImporterExtensionHelper
	{
		public TypeMoneySchemaImporterExtension()
			: base("money", "System.Data.SqlTypes.SqlMoney")
		{
		}
	}
}
