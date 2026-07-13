using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using MonoMod.Logs;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1 })]
	internal sealed class DMDEmitMethodBuilderGenerator : DMDGenerator<DMDEmitMethodBuilderGenerator>
	{
		protected override MethodInfo GenerateCore(DynamicMethodDefinition dmd, [Nullable(2)] object context)
		{
			TypeBuilder typeBuilder = context as TypeBuilder;
			MethodBuilder methodBuilder = DMDEmitMethodBuilderGenerator.GenerateMethodBuilder(dmd, typeBuilder);
			typeBuilder = (TypeBuilder)methodBuilder.DeclaringType;
			Type type = typeBuilder.CreateType();
			object obj;
			if (!string.IsNullOrEmpty(Switches.TryGetSwitchValue("DMDDumpTo", out obj) ? (obj as string) : null))
			{
				string fullyQualifiedName = methodBuilder.Module.FullyQualifiedName;
				string fileName = Path.GetFileName(fullyQualifiedName);
				string directoryName = Path.GetDirectoryName(fullyQualifiedName);
				if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				if (File.Exists(fullyQualifiedName))
				{
					File.Delete(fullyQualifiedName);
				}
				((AssemblyBuilder)typeBuilder.Assembly).Save(fileName);
			}
			return type.GetMethod(methodBuilder.Name, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static MethodBuilder GenerateMethodBuilder(DynamicMethodDefinition dmd, [Nullable(2)] TypeBuilder typeBuilder)
		{
			Helpers.ThrowIfArgumentNull<DynamicMethodDefinition>(dmd, "dmd");
			MethodBase originalMethod = dmd.OriginalMethod;
			MethodDefinition definition = dmd.Definition;
			if (typeBuilder == null)
			{
				object obj;
				string text = (Switches.TryGetSwitchValue("DMDDumpTo", out obj) ? (obj as string) : null);
				if (string.IsNullOrEmpty(text))
				{
					text = null;
				}
				else
				{
					text = Path.GetFullPath(text);
				}
				bool flag = string.IsNullOrEmpty(text) && DMDEmitMethodBuilderGenerator._MBCanRunAndCollect;
				AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName
				{
					Name = dmd.GetDumpName("MethodBuilder")
				}, flag ? AssemblyBuilderAccess.RunAndCollect : AssemblyBuilderAccess.RunAndSave, text);
				assemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(DynamicMethodDefinition.c_UnverifiableCodeAttribute, new object[0]));
				if (dmd.Debug)
				{
					assemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(DynamicMethodDefinition.c_DebuggableAttribute, new object[] { DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations }));
				}
				ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyBuilder.GetName().Name + ".dll", assemblyBuilder.GetName().Name + ".dll", dmd.Debug);
				FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(6, 2);
				formatInterpolatedStringHandler.AppendLiteral("DMD<");
				formatInterpolatedStringHandler.AppendFormatted<MethodBase>(originalMethod);
				formatInterpolatedStringHandler.AppendLiteral(">?");
				formatInterpolatedStringHandler.AppendFormatted<int>(dmd.GetHashCode());
				typeBuilder = moduleBuilder.DefineType(DebugFormatter.Format(ref formatInterpolatedStringHandler), global::System.Reflection.TypeAttributes.Public | global::System.Reflection.TypeAttributes.Abstract | global::System.Reflection.TypeAttributes.Sealed);
			}
			Type[] array;
			Type[][] array2;
			Type[][] array3;
			if (originalMethod != null)
			{
				ParameterInfo[] parameters = originalMethod.GetParameters();
				int num = 0;
				if (!originalMethod.IsStatic)
				{
					num++;
					array = new Type[parameters.Length + 1];
					array2 = new Type[parameters.Length + 1][];
					array3 = new Type[parameters.Length + 1][];
					array[0] = originalMethod.GetThisParamType();
					array2[0] = Type.EmptyTypes;
					array3[0] = Type.EmptyTypes;
				}
				else
				{
					array = new Type[parameters.Length];
					array2 = new Type[parameters.Length][];
					array3 = new Type[parameters.Length][];
				}
				for (int i = 0; i < parameters.Length; i++)
				{
					array[i + num] = parameters[i].ParameterType;
					array2[i + num] = parameters[i].GetRequiredCustomModifiers();
					array3[i + num] = parameters[i].GetOptionalCustomModifiers();
				}
			}
			else
			{
				int num2 = 0;
				if (definition.HasThis)
				{
					num2++;
					array = new Type[definition.Parameters.Count + 1];
					array2 = new Type[definition.Parameters.Count + 1][];
					array3 = new Type[definition.Parameters.Count + 1][];
					Type type = definition.DeclaringType.ResolveReflection();
					if (type.IsValueType)
					{
						type = type.MakeByRefType();
					}
					array[0] = type;
					array2[0] = Type.EmptyTypes;
					array3[0] = Type.EmptyTypes;
				}
				else
				{
					array = new Type[definition.Parameters.Count];
					array2 = new Type[definition.Parameters.Count][];
					array3 = new Type[definition.Parameters.Count][];
				}
				List<Type> list = new List<Type>();
				List<Type> list2 = new List<Type>();
				for (int j = 0; j < definition.Parameters.Count; j++)
				{
					Type type2;
					Type[] array4;
					Type[] array5;
					_DMDEmit.ResolveWithModifiers(definition.Parameters[j].ParameterType, out type2, out array4, out array5, list, list2);
					array[j + num2] = type2;
					array2[j + num2] = array4;
					array3[j + num2] = array5;
				}
			}
			Type type3;
			Type[] array6;
			Type[] array7;
			_DMDEmit.ResolveWithModifiers(definition.ReturnType, out type3, out array6, out array7, null, null);
			TypeBuilder typeBuilder2 = typeBuilder;
			string text2;
			if ((text2 = dmd.Name) == null)
			{
				text2 = (((originalMethod != null) ? originalMethod.Name : null) ?? definition.Name).Replace('.', '_');
			}
			MethodBuilder methodBuilder = typeBuilder2.DefineMethod(text2, global::System.Reflection.MethodAttributes.FamANDAssem | global::System.Reflection.MethodAttributes.Family | global::System.Reflection.MethodAttributes.Static | global::System.Reflection.MethodAttributes.HideBySig, CallingConventions.Standard, type3, array6, array7, array, array2, array3);
			ILGenerator ilgenerator = methodBuilder.GetILGenerator();
			_DMDEmit.Generate(dmd, methodBuilder, ilgenerator);
			return methodBuilder;
		}

		private static readonly bool _MBCanRunAndCollect = Enum.IsDefined(typeof(AssemblyBuilderAccess), "RunAndCollect");
	}
}
