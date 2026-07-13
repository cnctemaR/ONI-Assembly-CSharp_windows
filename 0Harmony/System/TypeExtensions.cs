using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class TypeExtensions
	{
		[NullableContext(1)]
		public static bool IsByRefLike(this Type type)
		{
			ThrowHelper.ThrowIfArgumentNull(type, ExceptionArgument.type);
			if (type == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.type);
			}
			object[] customAttributes = type.GetCustomAttributes(false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				if (customAttributes[i].GetType().FullName == "System.Runtime.CompilerServices.IsByRefLikeAttribute")
				{
					return true;
				}
			}
			return false;
		}
	}
}
