using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Systems
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class SystemVABI
	{
		public static TypeClassification ClassifyAMD64(Type type, bool isReturn)
		{
			int managedSize = type.GetManagedSize();
			if (managedSize > 16)
			{
				if (managedSize > 32)
				{
					if (!isReturn)
					{
						return TypeClassification.OnStack;
					}
					return TypeClassification.ByReference;
				}
				else if (true)
				{
					if (!isReturn)
					{
						return TypeClassification.OnStack;
					}
					return TypeClassification.ByReference;
				}
			}
			return TypeClassification.InRegister;
		}

		public static TypeClassification ClassifyARM64(Type type, bool isReturn)
		{
			int managedSize = type.GetManagedSize();
			if (managedSize > 16)
			{
				if (managedSize > 32)
				{
					if (!isReturn)
					{
						return TypeClassification.OnStack;
					}
					return TypeClassification.ByReference;
				}
				else if (SystemVABI.AnyFieldsNotFloat(type))
				{
					if (!isReturn)
					{
						return TypeClassification.OnStack;
					}
					return TypeClassification.ByReference;
				}
			}
			return TypeClassification.InRegister;
		}

		private static bool AnyFieldsNotFloat(Type type)
		{
			return SystemVABI.SysVIsMemoryCache.GetValue(type, delegate(Type type)
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				for (int i = 0; i < fields.Length; i++)
				{
					Type fieldType = fields[i].FieldType;
					if (fieldType != null && !fieldType.IsPrimitive && fieldType.IsValueType && SystemVABI.AnyFieldsNotFloat(fieldType))
					{
						return SystemVABI.SBTrue;
					}
					TypeCode typeCode = Type.GetTypeCode(fieldType);
					if (typeCode != TypeCode.Single && typeCode != TypeCode.Double)
					{
						return SystemVABI.SBTrue;
					}
				}
				return SystemVABI.SBFalse;
			}).Value;
		}

		private static readonly ConditionalWeakTable<Type, StrongBox<bool>> SysVIsMemoryCache = new ConditionalWeakTable<Type, StrongBox<bool>>();

		private static readonly StrongBox<bool> SBTrue = new StrongBox<bool>(true);

		private static readonly StrongBox<bool> SBFalse = new StrongBox<bool>(false);
	}
}
