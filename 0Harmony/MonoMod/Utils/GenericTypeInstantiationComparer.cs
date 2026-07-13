using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class GenericTypeInstantiationComparer : IEqualityComparer<Type>
	{
		public bool Equals(Type x, Type y)
		{
			if (x == null && y == null)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			bool isGenericType = x.IsGenericType;
			bool isGenericType2 = y.IsGenericType;
			if (isGenericType != isGenericType2)
			{
				return false;
			}
			if (!isGenericType)
			{
				return x.Equals(y);
			}
			Type genericTypeDefinition = x.GetGenericTypeDefinition();
			Type genericTypeDefinition2 = y.GetGenericTypeDefinition();
			if (!genericTypeDefinition.Equals(genericTypeDefinition2))
			{
				return false;
			}
			Type[] genericArguments = x.GetGenericArguments();
			Type[] genericArguments2 = y.GetGenericArguments();
			if (genericArguments.Length != genericArguments2.Length)
			{
				return false;
			}
			for (int i = 0; i < genericArguments.Length; i++)
			{
				Type type = genericArguments[i];
				Type type2 = genericArguments2[i];
				if (!type.IsValueType)
				{
					type = GenericTypeInstantiationComparer.CannonicalFillType ?? typeof(object);
				}
				if (!type2.IsValueType)
				{
					type2 = GenericTypeInstantiationComparer.CannonicalFillType ?? typeof(object);
				}
				if (!this.Equals(type, type2))
				{
					return false;
				}
			}
			return true;
		}

		[NullableContext(1)]
		public int GetHashCode(Type obj)
		{
			Helpers.ThrowIfArgumentNull<Type>(obj, "obj");
			if (!obj.IsGenericType)
			{
				return obj.GetHashCode();
			}
			int num = -559038737;
			num ^= obj.Assembly.GetHashCode();
			num ^= (num << 16) | (num >> 16);
			if (obj.Namespace != null)
			{
				num ^= obj.Namespace.GetHashCode(StringComparison.Ordinal);
			}
			num ^= obj.Name.GetHashCode(StringComparison.Ordinal);
			Type[] genericArguments = obj.GetGenericArguments();
			for (int i = 0; i < genericArguments.Length; i++)
			{
				int num2 = i % 8 * 4;
				Type type = genericArguments[i];
				int num3;
				if (!type.IsValueType)
				{
					Type cannonicalFillType = GenericTypeInstantiationComparer.CannonicalFillType;
					num3 = ((cannonicalFillType != null) ? cannonicalFillType.GetHashCode() : (-1717986919));
				}
				else
				{
					num3 = this.GetHashCode(type);
				}
				int num4 = num3;
				num ^= (num4 << num2) | (num4 >> 32 - num2);
			}
			return num;
		}

		private static Type CannonicalFillType = GenericMethodInstantiationComparer.CannonicalFillType;
	}
}
