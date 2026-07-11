using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Harmony.ILCopying;

namespace Harmony
{
	public static class DynamicTools
	{
		public static DynamicMethod CreateDynamicMethod(MethodBase original, string suffix)
		{
			bool flag = original == null;
			if (flag)
			{
				throw new ArgumentNullException("original cannot be null");
			}
			string text = original.Name + suffix;
			text = text.Replace("<>", "");
			ParameterInfo[] parameters = original.GetParameters();
			List<Type> list = parameters.Types().ToList<Type>();
			bool flag2 = !original.IsStatic;
			if (flag2)
			{
				list.Insert(0, typeof(object));
			}
			Type[] array = list.ToArray();
			Type returnedType = AccessTools.GetReturnedType(original);
			bool flag3 = returnedType == null || returnedType.IsByRef;
			DynamicMethod dynamicMethod;
			if (flag3)
			{
				dynamicMethod = null;
			}
			else
			{
				DynamicMethod dynamicMethod2;
				try
				{
					dynamicMethod2 = new DynamicMethod(text, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, returnedType, array, original.DeclaringType, true);
				}
				catch (Exception)
				{
					return null;
				}
				for (int i = 0; i < parameters.Length; i++)
				{
					dynamicMethod2.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);
				}
				dynamicMethod = dynamicMethod2;
			}
			return dynamicMethod;
		}

		public static ILGenerator CreateSaveableMethod(MethodBase original, string suffix, out AssemblyBuilder assemblyBuilder, out TypeBuilder typeBuilder)
		{
			AssemblyName assemblyName = new AssemblyName("DebugAssembly");
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, folderPath);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");
			typeBuilder = moduleBuilder.DefineType("Debug" + original.DeclaringType.Name, TypeAttributes.Public);
			bool flag = original == null;
			if (flag)
			{
				throw new ArgumentNullException("original cannot be null");
			}
			string text = original.Name + suffix;
			text = text.Replace("<>", "");
			ParameterInfo[] parameters = original.GetParameters();
			List<Type> list = parameters.Types().ToList<Type>();
			bool flag2 = !original.IsStatic;
			if (flag2)
			{
				list.Insert(0, typeof(object));
			}
			Type[] array = list.ToArray();
			MethodBuilder methodBuilder = typeBuilder.DefineMethod(text, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, AccessTools.GetReturnedType(original), array);
			return methodBuilder.GetILGenerator();
		}

		public static void SaveMethod(AssemblyBuilder assemblyBuilder, TypeBuilder typeBuilder)
		{
			Type type = typeBuilder.CreateType();
			assemblyBuilder.Save("HarmonyDebugAssembly.dll");
		}

		public static LocalBuilder[] DeclareLocalVariables(MethodBase original, ILGenerator il, bool logOutput = true)
		{
			MethodBody methodBody = original.GetMethodBody();
			IList<LocalVariableInfo> list = ((methodBody != null) ? methodBody.LocalVariables : null);
			bool flag = list == null;
			LocalBuilder[] array;
			if (flag)
			{
				array = new LocalBuilder[0];
			}
			else
			{
				array = list.Select<LocalVariableInfo, LocalBuilder>(delegate(LocalVariableInfo lvi)
				{
					LocalBuilder localBuilder = il.DeclareLocal(lvi.LocalType, lvi.IsPinned);
					bool logOutput2 = logOutput;
					if (logOutput2)
					{
						Emitter.LogLocalVariable(il, localBuilder);
					}
					return localBuilder;
				}).ToArray<LocalBuilder>();
			}
			return array;
		}

		public static LocalBuilder DeclareLocalVariable(ILGenerator il, Type type)
		{
			bool isByRef = type.IsByRef;
			if (isByRef)
			{
				type = type.GetElementType();
			}
			bool flag = AccessTools.IsClass(type);
			LocalBuilder localBuilder2;
			if (flag)
			{
				LocalBuilder localBuilder = il.DeclareLocal(type);
				Emitter.LogLocalVariable(il, localBuilder);
				Emitter.Emit(il, OpCodes.Ldnull);
				Emitter.Emit(il, OpCodes.Stloc, localBuilder);
				localBuilder2 = localBuilder;
			}
			else
			{
				bool flag2 = AccessTools.IsStruct(type);
				if (flag2)
				{
					LocalBuilder localBuilder3 = il.DeclareLocal(type);
					Emitter.LogLocalVariable(il, localBuilder3);
					Emitter.Emit(il, OpCodes.Ldloca, localBuilder3);
					Emitter.Emit(il, OpCodes.Initobj, type);
					localBuilder2 = localBuilder3;
				}
				else
				{
					bool flag3 = AccessTools.IsValue(type);
					if (flag3)
					{
						LocalBuilder localBuilder4 = il.DeclareLocal(type);
						Emitter.LogLocalVariable(il, localBuilder4);
						bool flag4 = type == typeof(float);
						if (flag4)
						{
							Emitter.Emit(il, OpCodes.Ldc_R4, 0f);
						}
						else
						{
							bool flag5 = type == typeof(double);
							if (flag5)
							{
								Emitter.Emit(il, OpCodes.Ldc_R8, 0.0);
							}
							else
							{
								bool flag6 = type == typeof(long);
								if (flag6)
								{
									Emitter.Emit(il, OpCodes.Ldc_I8, 0L);
								}
								else
								{
									Emitter.Emit(il, OpCodes.Ldc_I4, 0);
								}
							}
						}
						Emitter.Emit(il, OpCodes.Stloc, localBuilder4);
						localBuilder2 = localBuilder4;
					}
					else
					{
						localBuilder2 = null;
					}
				}
			}
			return localBuilder2;
		}

		public static void PrepareDynamicMethod(DynamicMethod method)
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			BindingFlags bindingFlags2 = BindingFlags.Static | BindingFlags.NonPublic;
			MethodInfo method2 = typeof(DynamicMethod).GetMethod("CreateDynMethod", bindingFlags);
			bool flag = method2 != null;
			if (flag)
			{
				method2.Invoke(method, new object[0]);
			}
			else
			{
				MethodInfo method3 = typeof(RuntimeHelpers).GetMethod("_CompileMethod", bindingFlags2);
				MethodInfo method4 = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", bindingFlags);
				RuntimeMethodHandle runtimeMethodHandle = (RuntimeMethodHandle)method4.Invoke(method, new object[0]);
				MethodInfo method5 = typeof(RuntimeMethodHandle).GetMethod("GetMethodInfo", bindingFlags);
				bool flag2 = method5 != null;
				if (flag2)
				{
					object obj = method5.Invoke(runtimeMethodHandle, new object[0]);
					try
					{
						method3.Invoke(null, new object[] { obj });
						return;
					}
					catch (Exception)
					{
					}
				}
				bool flag3 = method3.GetParameters()[0].ParameterType.IsAssignableFrom(runtimeMethodHandle.Value.GetType());
				if (flag3)
				{
					method3.Invoke(null, new object[] { runtimeMethodHandle.Value });
				}
				else
				{
					bool flag4 = method3.GetParameters()[0].ParameterType.IsAssignableFrom(runtimeMethodHandle.GetType());
					if (flag4)
					{
						method3.Invoke(null, new object[] { runtimeMethodHandle });
					}
				}
			}
		}
	}
}
