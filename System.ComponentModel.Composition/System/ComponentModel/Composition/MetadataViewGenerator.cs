using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
	internal static class MetadataViewGenerator
	{
		private static AssemblyBuilder CreateProxyAssemblyBuilder(ConstructorInfo constructorInfo)
		{
			return AppDomain.CurrentDomain.DefineDynamicAssembly(MetadataViewGenerator.ProxyAssemblyName, AssemblyBuilderAccess.Run);
		}

		private static ModuleBuilder GetProxyModuleBuilder(bool requiresCritical)
		{
			if (MetadataViewGenerator.transparentProxyModuleBuilder == null)
			{
				MetadataViewGenerator.transparentProxyModuleBuilder = MetadataViewGenerator.CreateProxyAssemblyBuilder(typeof(SecurityTransparentAttribute).GetConstructor(Type.EmptyTypes)).DefineDynamicModule("MetadataViewProxiesModule");
			}
			return MetadataViewGenerator.transparentProxyModuleBuilder;
		}

		public static Type GenerateView(Type viewType)
		{
			Assumes.NotNull<Type>(viewType);
			Assumes.IsTrue(viewType.IsInterface);
			Type type;
			bool flag;
			using (new ReadLock(MetadataViewGenerator._lock))
			{
				flag = MetadataViewGenerator._proxies.TryGetValue(viewType, out type);
			}
			if (!flag)
			{
				Type type2 = MetadataViewGenerator.GenerateInterfaceViewProxyType(viewType);
				Assumes.NotNull<Type>(type2);
				using (new WriteLock(MetadataViewGenerator._lock))
				{
					if (!MetadataViewGenerator._proxies.TryGetValue(viewType, out type))
					{
						type = type2;
						MetadataViewGenerator._proxies.Add(viewType, type);
					}
				}
			}
			return type;
		}

		private static void GenerateLocalAssignmentFromDefaultAttribute(this ILGenerator IL, DefaultValueAttribute[] attrs, LocalBuilder local)
		{
			if (attrs.Length != 0)
			{
				DefaultValueAttribute defaultValueAttribute = attrs[0];
				IL.LoadValue(defaultValueAttribute.Value);
				if (defaultValueAttribute.Value != null && defaultValueAttribute.Value.GetType().IsValueType)
				{
					IL.Emit(OpCodes.Box, defaultValueAttribute.Value.GetType());
				}
				IL.Emit(OpCodes.Stloc, local);
			}
		}

		private static void GenerateFieldAssignmentFromLocalValue(this ILGenerator IL, LocalBuilder local, FieldBuilder field)
		{
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldloc, local);
			IL.Emit(field.FieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, field.FieldType);
			IL.Emit(OpCodes.Stfld, field);
		}

		private static void GenerateLocalAssignmentFromFlag(this ILGenerator IL, LocalBuilder local, bool flag)
		{
			IL.Emit(flag ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
			IL.Emit(OpCodes.Stloc, local);
		}

		private static Type GenerateInterfaceViewProxyType(Type viewType)
		{
			Type[] array = new Type[] { viewType };
			TypeBuilder typeBuilder = MetadataViewGenerator.GetProxyModuleBuilder(false).DefineType(string.Format(CultureInfo.InvariantCulture, "_proxy_{0}_{1}", viewType.FullName, Guid.NewGuid()), TypeAttributes.Public, typeof(object), array);
			ILGenerator ilgenerator = typeBuilder.CreateGeneratorForPublicConstructor(MetadataViewGenerator.CtorArgumentTypes);
			LocalBuilder localBuilder = ilgenerator.DeclareLocal(typeof(Exception));
			LocalBuilder localBuilder2 = ilgenerator.DeclareLocal(typeof(IDictionary));
			LocalBuilder localBuilder3 = ilgenerator.DeclareLocal(typeof(Type));
			LocalBuilder localBuilder4 = ilgenerator.DeclareLocal(typeof(object));
			LocalBuilder localBuilder5 = ilgenerator.DeclareLocal(typeof(bool));
			Label label = ilgenerator.BeginExceptionBlock();
			foreach (PropertyInfo propertyInfo in viewType.GetAllProperties())
			{
				string text = string.Format(CultureInfo.InvariantCulture, "_{0}_{1}", propertyInfo.Name, Guid.NewGuid());
				string text2 = string.Format(CultureInfo.InvariantCulture, "{0}", propertyInfo.Name);
				Type[] array2 = new Type[] { propertyInfo.PropertyType };
				Type[] array3 = null;
				Type[] array4 = null;
				FieldBuilder fieldBuilder = typeBuilder.DefineField(text, propertyInfo.PropertyType, FieldAttributes.Private);
				PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(text2, PropertyAttributes.None, propertyInfo.PropertyType, array2);
				Label label2 = ilgenerator.BeginExceptionBlock();
				DefaultValueAttribute[] attributes = propertyInfo.GetAttributes<DefaultValueAttribute>(false);
				if (attributes.Length != 0)
				{
					ilgenerator.BeginExceptionBlock();
				}
				Label label3 = ilgenerator.DefineLabel();
				ilgenerator.GenerateLocalAssignmentFromFlag(localBuilder5, true);
				ilgenerator.Emit(OpCodes.Ldarg_1);
				ilgenerator.Emit(OpCodes.Ldstr, propertyInfo.Name);
				ilgenerator.Emit(OpCodes.Ldloca, localBuilder4);
				ilgenerator.Emit(OpCodes.Callvirt, MetadataViewGenerator._mdvDictionaryTryGet);
				ilgenerator.Emit(OpCodes.Brtrue, label3);
				ilgenerator.GenerateLocalAssignmentFromFlag(localBuilder5, false);
				ilgenerator.GenerateLocalAssignmentFromDefaultAttribute(attributes, localBuilder4);
				ilgenerator.MarkLabel(label3);
				ilgenerator.GenerateFieldAssignmentFromLocalValue(localBuilder4, fieldBuilder);
				ilgenerator.Emit(OpCodes.Leave, label2);
				if (attributes.Length != 0)
				{
					ilgenerator.BeginCatchBlock(typeof(InvalidCastException));
					Label label4 = ilgenerator.DefineLabel();
					ilgenerator.Emit(OpCodes.Ldloc, localBuilder5);
					ilgenerator.Emit(OpCodes.Brtrue, label4);
					ilgenerator.Emit(OpCodes.Rethrow);
					ilgenerator.MarkLabel(label4);
					ilgenerator.GenerateLocalAssignmentFromDefaultAttribute(attributes, localBuilder4);
					ilgenerator.GenerateFieldAssignmentFromLocalValue(localBuilder4, fieldBuilder);
					ilgenerator.EndExceptionBlock();
				}
				ilgenerator.BeginCatchBlock(typeof(NullReferenceException));
				ilgenerator.Emit(OpCodes.Stloc, localBuilder);
				ilgenerator.GetExceptionDataAndStoreInLocal(localBuilder, localBuilder2);
				ilgenerator.AddItemToLocalDictionary(localBuilder2, "MetadataItemKey", text2);
				ilgenerator.AddItemToLocalDictionary(localBuilder2, "MetadataItemTargetType", propertyInfo.PropertyType);
				ilgenerator.Emit(OpCodes.Rethrow);
				ilgenerator.BeginCatchBlock(typeof(InvalidCastException));
				ilgenerator.Emit(OpCodes.Stloc, localBuilder);
				ilgenerator.GetExceptionDataAndStoreInLocal(localBuilder, localBuilder2);
				ilgenerator.AddItemToLocalDictionary(localBuilder2, "MetadataItemKey", text2);
				ilgenerator.AddItemToLocalDictionary(localBuilder2, "MetadataItemTargetType", propertyInfo.PropertyType);
				ilgenerator.Emit(OpCodes.Rethrow);
				ilgenerator.EndExceptionBlock();
				if (propertyInfo.CanWrite)
				{
					throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Strings.InvalidSetterOnMetadataField, viewType, text2));
				}
				if (propertyInfo.CanRead)
				{
					MethodBuilder methodBuilder = typeBuilder.DefineMethod(string.Format(CultureInfo.InvariantCulture, "get_{0}", text2), MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.SpecialName, CallingConventions.HasThis, propertyInfo.PropertyType, array4, array3, Type.EmptyTypes, null, null);
					typeBuilder.DefineMethodOverride(methodBuilder, propertyInfo.GetGetMethod());
					ILGenerator ilgenerator2 = methodBuilder.GetILGenerator();
					ilgenerator2.Emit(OpCodes.Ldarg_0);
					ilgenerator2.Emit(OpCodes.Ldfld, fieldBuilder);
					ilgenerator2.Emit(OpCodes.Ret);
					propertyBuilder.SetGetMethod(methodBuilder);
				}
			}
			ilgenerator.Emit(OpCodes.Leave, label);
			ilgenerator.BeginCatchBlock(typeof(NullReferenceException));
			ilgenerator.Emit(OpCodes.Stloc, localBuilder);
			ilgenerator.GetExceptionDataAndStoreInLocal(localBuilder, localBuilder2);
			ilgenerator.AddItemToLocalDictionary(localBuilder2, "MetadataViewType", viewType);
			ilgenerator.Emit(OpCodes.Rethrow);
			ilgenerator.BeginCatchBlock(typeof(InvalidCastException));
			ilgenerator.Emit(OpCodes.Stloc, localBuilder);
			ilgenerator.GetExceptionDataAndStoreInLocal(localBuilder, localBuilder2);
			ilgenerator.Emit(OpCodes.Ldloc, localBuilder4);
			ilgenerator.Emit(OpCodes.Call, MetadataViewGenerator.ObjectGetType);
			ilgenerator.Emit(OpCodes.Stloc, localBuilder3);
			ilgenerator.AddItemToLocalDictionary(localBuilder2, "MetadataViewType", viewType);
			ilgenerator.AddLocalToLocalDictionary(localBuilder2, "MetadataItemSourceType", localBuilder3);
			ilgenerator.AddLocalToLocalDictionary(localBuilder2, "MetadataItemValue", localBuilder4);
			ilgenerator.Emit(OpCodes.Rethrow);
			ilgenerator.EndExceptionBlock();
			ilgenerator.Emit(OpCodes.Ret);
			return typeBuilder.CreateType();
		}

		public const string MetadataViewType = "MetadataViewType";

		public const string MetadataItemKey = "MetadataItemKey";

		public const string MetadataItemTargetType = "MetadataItemTargetType";

		public const string MetadataItemSourceType = "MetadataItemSourceType";

		public const string MetadataItemValue = "MetadataItemValue";

		private static Lock _lock = new Lock();

		private static Dictionary<Type, Type> _proxies = new Dictionary<Type, Type>();

		private static AssemblyName ProxyAssemblyName = new AssemblyName(string.Format(CultureInfo.InvariantCulture, "MetadataViewProxies_{0}", Guid.NewGuid()));

		private static ModuleBuilder transparentProxyModuleBuilder;

		private static Type[] CtorArgumentTypes = new Type[] { typeof(IDictionary<string, object>) };

		private static MethodInfo _mdvDictionaryTryGet = MetadataViewGenerator.CtorArgumentTypes[0].GetMethod("TryGetValue");

		private static readonly MethodInfo ObjectGetType = typeof(object).GetMethod("GetType", Type.EmptyTypes);
	}
}
