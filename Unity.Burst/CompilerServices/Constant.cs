using System;
using System.Runtime.CompilerServices;

namespace Unity.Burst.CompilerServices
{
	public static class Constant
	{
		public static bool IsConstantExpression<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T t) where T : struct, ValueType
		{
			return false;
		}

		public unsafe static bool IsConstantExpression(void* t)
		{
			return false;
		}
	}
}
