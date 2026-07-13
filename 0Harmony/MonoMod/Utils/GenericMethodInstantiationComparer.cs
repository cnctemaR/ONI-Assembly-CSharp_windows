using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class GenericMethodInstantiationComparer : IEqualityComparer<MethodBase>
	{
		public GenericMethodInstantiationComparer()
			: this(new GenericTypeInstantiationComparer())
		{
		}

		public GenericMethodInstantiationComparer(IEqualityComparer<Type> typeComparer)
		{
			this.genericTypeComparer = typeComparer;
		}

		[NullableContext(2)]
		public bool Equals(MethodBase x, MethodBase y)
		{
			if (x == null && y == null)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			bool flag;
			if (!x.IsGenericMethod || x.ContainsGenericParameters)
			{
				Type declaringType = x.DeclaringType;
				flag = declaringType != null && declaringType.IsGenericType;
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			bool flag3;
			if (!y.IsGenericMethod || y.ContainsGenericParameters)
			{
				Type declaringType2 = y.DeclaringType;
				flag3 = declaringType2 != null && declaringType2.IsGenericType;
			}
			else
			{
				flag3 = true;
			}
			bool flag4 = flag3;
			if (flag2 != flag4)
			{
				return false;
			}
			if (!flag2)
			{
				return x.Equals(y);
			}
			if (!this.genericTypeComparer.Equals(x.DeclaringType, y.DeclaringType))
			{
				return false;
			}
			MethodInfo methodInfo = x as MethodInfo;
			MethodBase methodBase;
			if (methodInfo != null)
			{
				methodBase = methodInfo.GetActualGenericMethodDefinition();
			}
			else
			{
				methodBase = x.GetUnfilledMethodOnGenericType();
			}
			MethodInfo methodInfo2 = y as MethodInfo;
			MethodBase methodBase2;
			if (methodInfo2 != null)
			{
				methodBase2 = methodInfo2.GetActualGenericMethodDefinition();
			}
			else
			{
				methodBase2 = y.GetUnfilledMethodOnGenericType();
			}
			if (!methodBase.Equals(methodBase2))
			{
				return false;
			}
			if (methodBase.Name != methodBase2.Name)
			{
				return false;
			}
			ParameterInfo[] parameters = x.GetParameters();
			ParameterInfo[] parameters2 = y.GetParameters();
			if (parameters.Length != parameters2.Length)
			{
				return false;
			}
			ParameterInfo[] parameters3 = methodBase.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				Type type = parameters[i].ParameterType;
				Type type2 = parameters2[i].ParameterType;
				if (parameters3[i].ParameterType.IsGenericParameter)
				{
					if (!type.IsValueType)
					{
						type = GenericMethodInstantiationComparer.CannonicalFillType ?? typeof(object);
					}
					if (!type2.IsValueType)
					{
						type2 = GenericMethodInstantiationComparer.CannonicalFillType ?? typeof(object);
					}
				}
				if (!this.genericTypeComparer.Equals(type, type2))
				{
					return false;
				}
			}
			return true;
		}

		public int GetHashCode(MethodBase obj)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(obj, "obj");
			if (!obj.IsGenericMethod || obj.ContainsGenericParameters)
			{
				Type declaringType = obj.DeclaringType;
				if (declaringType == null || !declaringType.IsGenericType)
				{
					return obj.GetHashCode();
				}
			}
			int num = -559038737;
			if (obj.DeclaringType != null)
			{
				num ^= obj.DeclaringType.Assembly.GetHashCode();
				num ^= this.genericTypeComparer.GetHashCode(obj.DeclaringType);
			}
			num ^= obj.Name.GetHashCode(StringComparison.Ordinal);
			ParameterInfo[] parameters = obj.GetParameters();
			int num2 = parameters.Length;
			num2 ^= num2 << 4;
			num2 ^= num2 << 8;
			num2 ^= num2 << 16;
			num ^= num2;
			if (obj.IsGenericMethod)
			{
				Type[] genericArguments = obj.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					int num3 = i % 32;
					Type type = genericArguments[i];
					int num4;
					if (!type.IsValueType)
					{
						Type cannonicalFillType = GenericMethodInstantiationComparer.CannonicalFillType;
						num4 = ((cannonicalFillType != null) ? cannonicalFillType.GetHashCode() : 1431655765);
					}
					else
					{
						num4 = this.genericTypeComparer.GetHashCode(type);
					}
					int num5 = num4;
					num5 = (num5 << num3) | (num5 >> 32 - num3);
					num ^= num5;
				}
			}
			MethodInfo methodInfo = obj as MethodInfo;
			MethodBase methodBase;
			if (methodInfo != null)
			{
				methodBase = methodInfo.GetActualGenericMethodDefinition();
			}
			else
			{
				methodBase = obj.GetUnfilledMethodOnGenericType();
			}
			ParameterInfo[] parameters2 = methodBase.GetParameters();
			for (int j = 0; j < parameters.Length; j++)
			{
				int num6 = j % 32;
				Type parameterType = parameters[j].ParameterType;
				int num7 = this.genericTypeComparer.GetHashCode(parameterType);
				if (parameters2[j].ParameterType.IsGenericParameter && !parameterType.IsValueType)
				{
					Type cannonicalFillType2 = GenericMethodInstantiationComparer.CannonicalFillType;
					num7 = ((cannonicalFillType2 != null) ? cannonicalFillType2.GetHashCode() : 1431655765);
				}
				num7 = (num7 >> num6) | (num7 << 32 - num6);
				num ^= num7;
			}
			return num;
		}

		[Nullable(2)]
		internal static Type CannonicalFillType = typeof(object).Assembly.GetType("System.__Canon");

		private readonly IEqualityComparer<Type> genericTypeComparer;
	}
}
