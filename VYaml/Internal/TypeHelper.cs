using System;
using System.Runtime.CompilerServices;

namespace VYaml.Internal
{
	internal static class TypeHelper
	{
		[NullableContext(1)]
		public static bool IsAnonymous(Type type)
		{
			return type.Namespace == null && type.IsSealed && (type.Name.StartsWith("<>f__AnonymousType", StringComparison.Ordinal) || type.Name.StartsWith("<>__AnonType", StringComparison.Ordinal) || type.Name.StartsWith("VB$AnonymousType_", StringComparison.Ordinal)) && type.IsDefined(typeof(CompilerGeneratedAttribute), false);
		}
	}
}
