using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Cecil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace HarmonyLib
{
	internal class MethodPatcher
	{
		internal MethodPatcher(MethodBase original, MethodBase source, List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers, List<MethodInfo> finalizers, bool debug)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			this.debug = debug;
			this.original = original;
			this.source = source;
			this.prefixes = prefixes;
			this.postfixes = postfixes;
			this.transpilers = transpilers;
			this.finalizers = finalizers;
			Memory.MarkForNoInlining(original);
			if (debug)
			{
				FileLog.LogBuffered("### Patch: " + original.FullDescription());
				FileLog.FlushBuffer();
			}
			this.idx = prefixes.Count + postfixes.Count + finalizers.Count;
			this.useStructReturnBuffer = StructReturnBuffer.NeedsFix(original);
			if (debug && this.useStructReturnBuffer)
			{
				FileLog.Log("### Note: A buffer for the returned struct is used. That requires an extra IntPtr argument before the first real argument");
			}
			this.returnType = AccessTools.GetReturnedType(original);
			this.patch = MethodPatcher.CreateDynamicMethod(original, string.Format("_Patch{0}", this.idx), debug);
			if (this.patch == null)
			{
				throw new Exception("Could not create replacement method");
			}
			this.il = this.patch.GetILGenerator();
			this.emitter = new Emitter(this.il, debug);
		}

		internal MethodInfo CreateReplacement(out Dictionary<int, CodeInstruction> finalInstructions)
		{
			LocalBuilder[] array = MethodPatcher.DeclareLocalVariables(this.il, this.source ?? this.original);
			Dictionary<string, LocalBuilder> privateVars = new Dictionary<string, LocalBuilder>();
			List<MethodInfo> list = this.prefixes.Union<MethodInfo>(this.postfixes).Union<MethodInfo>(this.finalizers).ToList<MethodInfo>();
			LocalBuilder localBuilder = null;
			if (this.idx > 0)
			{
				localBuilder = this.DeclareLocalVariable(this.returnType, true);
				privateVars["__result"] = localBuilder;
			}
			if (list.Any<MethodInfo>((MethodInfo fix) => fix.GetParameters().Any<ParameterInfo>((ParameterInfo p) => p.Name == "__args")))
			{
				this.PrepareArgumentArray();
				LocalBuilder localBuilder2 = this.il.DeclareLocal(typeof(object[]));
				this.emitter.Emit(OpCodes.Stloc, localBuilder2);
				privateVars["__args"] = localBuilder2;
			}
			Label? label = null;
			LocalBuilder localBuilder3 = null;
			bool flag = this.prefixes.Any<MethodInfo>((MethodInfo fix) => MethodPatcher.PrefixAffectsOriginal(fix));
			bool flag2 = list.Any<MethodInfo>((MethodInfo fix) => fix.GetParameters().Any<ParameterInfo>((ParameterInfo p) => p.Name == "__runOriginal"));
			if (flag || flag2)
			{
				localBuilder3 = this.DeclareLocalVariable(typeof(bool), false);
				this.emitter.Emit(OpCodes.Ldc_I4_1);
				this.emitter.Emit(OpCodes.Stloc, localBuilder3);
				if (flag)
				{
					label = new Label?(this.il.DefineLabel());
				}
			}
			list.ForEach(delegate(MethodInfo fix)
			{
				if (fix.DeclaringType != null && !privateVars.ContainsKey(fix.DeclaringType.AssemblyQualifiedName))
				{
					(from patchParam in fix.GetParameters()
						where patchParam.Name == "__state"
						select patchParam).Do<ParameterInfo>(delegate(ParameterInfo patchParam)
					{
						LocalBuilder localBuilder6 = this.DeclareLocalVariable(patchParam.ParameterType, false);
						privateVars[fix.DeclaringType.AssemblyQualifiedName] = localBuilder6;
					});
				}
			});
			LocalBuilder localBuilder4 = null;
			if (this.finalizers.Any<MethodInfo>())
			{
				localBuilder4 = this.DeclareLocalVariable(typeof(bool), false);
				privateVars["__exception"] = this.DeclareLocalVariable(typeof(Exception), false);
				Label? label2;
				this.emitter.MarkBlockBefore(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock, null), out label2);
			}
			this.AddPrefixes(privateVars, localBuilder3);
			if (label != null)
			{
				this.emitter.Emit(OpCodes.Ldloc, localBuilder3);
				this.emitter.Emit(OpCodes.Brfalse, label.Value);
			}
			MethodCopier methodCopier = new MethodCopier(this.source ?? this.original, this.il, array);
			methodCopier.SetArgumentShift(this.useStructReturnBuffer);
			methodCopier.SetDebugging(this.debug);
			foreach (MethodInfo methodInfo in this.transpilers)
			{
				methodCopier.AddTranspiler(methodInfo);
			}
			List<Label> list2 = new List<Label>();
			bool flag3;
			methodCopier.Finalize(this.emitter, list2, out flag3);
			foreach (Label label3 in list2)
			{
				this.emitter.MarkLabel(label3);
			}
			if (localBuilder != null && flag3)
			{
				this.emitter.Emit(OpCodes.Stloc, localBuilder);
			}
			if (label != null)
			{
				this.emitter.MarkLabel(label.Value);
			}
			this.AddPostfixes(privateVars, localBuilder3, false);
			if (localBuilder != null && flag3)
			{
				this.emitter.Emit(OpCodes.Ldloc, localBuilder);
			}
			bool flag4 = this.AddPostfixes(privateVars, localBuilder3, true);
			bool flag5 = this.finalizers.Any<MethodInfo>();
			if (flag5)
			{
				if (flag4)
				{
					this.emitter.Emit(OpCodes.Stloc, localBuilder);
					this.emitter.Emit(OpCodes.Ldloc, localBuilder);
				}
				this.AddFinalizers(privateVars, localBuilder3, false);
				this.emitter.Emit(OpCodes.Ldc_I4_1);
				this.emitter.Emit(OpCodes.Stloc, localBuilder4);
				Label label4 = this.il.DefineLabel();
				this.emitter.Emit(OpCodes.Ldloc, privateVars["__exception"]);
				this.emitter.Emit(OpCodes.Brfalse, label4);
				this.emitter.Emit(OpCodes.Ldloc, privateVars["__exception"]);
				this.emitter.Emit(OpCodes.Throw);
				this.emitter.MarkLabel(label4);
				Label? label5;
				this.emitter.MarkBlockBefore(new ExceptionBlock(ExceptionBlockType.BeginCatchBlock, null), out label5);
				this.emitter.Emit(OpCodes.Stloc, privateVars["__exception"]);
				this.emitter.Emit(OpCodes.Ldloc, localBuilder4);
				Label label6 = this.il.DefineLabel();
				this.emitter.Emit(OpCodes.Brtrue, label6);
				bool flag6 = this.AddFinalizers(privateVars, localBuilder3, true);
				this.emitter.MarkLabel(label6);
				Label label7 = this.il.DefineLabel();
				this.emitter.Emit(OpCodes.Ldloc, privateVars["__exception"]);
				this.emitter.Emit(OpCodes.Brfalse, label7);
				if (flag6)
				{
					this.emitter.Emit(OpCodes.Rethrow);
				}
				else
				{
					this.emitter.Emit(OpCodes.Ldloc, privateVars["__exception"]);
					this.emitter.Emit(OpCodes.Throw);
				}
				this.emitter.MarkLabel(label7);
				this.emitter.MarkBlockAfter(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock, null));
				if (localBuilder != null)
				{
					this.emitter.Emit(OpCodes.Ldloc, localBuilder);
				}
			}
			if (this.useStructReturnBuffer)
			{
				LocalBuilder localBuilder5 = this.DeclareLocalVariable(this.returnType, false);
				this.emitter.Emit(OpCodes.Stloc, localBuilder5);
				this.emitter.Emit(this.original.IsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1);
				this.emitter.Emit(OpCodes.Ldloc, localBuilder5);
				this.emitter.Emit(OpCodes.Stobj, this.returnType);
			}
			if (flag5 || flag3)
			{
				this.emitter.Emit(OpCodes.Ret);
			}
			finalInstructions = this.emitter.GetInstructions();
			if (this.debug)
			{
				FileLog.LogBuffered("DONE");
				FileLog.LogBuffered("");
				FileLog.FlushBuffer();
			}
			return this.patch.Generate().Pin<MethodInfo>();
		}

		internal static DynamicMethodDefinition CreateDynamicMethod(MethodBase original, string suffix, bool debug)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			bool flag = StructReturnBuffer.NeedsFix(original);
			Type declaringType = original.DeclaringType;
			string text = (((declaringType != null) ? declaringType.FullName : null) ?? "GLOBALTYPE") + "." + original.Name + suffix;
			text = text.Replace("<>", "");
			ParameterInfo[] parameters = original.GetParameters();
			List<Type> list = new List<Type>();
			list.AddRange(parameters.Types());
			if (flag)
			{
				list.Insert(0, typeof(IntPtr));
			}
			if (!original.IsStatic)
			{
				if (AccessTools.IsStruct(original.DeclaringType))
				{
					list.Insert(0, original.DeclaringType.MakeByRefType());
				}
				else
				{
					list.Insert(0, original.DeclaringType);
				}
			}
			Type type = (flag ? typeof(void) : AccessTools.GetReturnedType(original));
			DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(text, type, list.ToArray())
			{
				OwnerType = original.DeclaringType
			};
			int num = (original.IsStatic ? 0 : 1) + (flag ? 1 : 0);
			if (flag)
			{
				dynamicMethodDefinition.Definition.Parameters[original.IsStatic ? 0 : 1].Name = "retbuf";
			}
			if (!original.IsStatic)
			{
				dynamicMethodDefinition.Definition.Parameters[0].Name = "this";
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterDefinition parameterDefinition = dynamicMethodDefinition.Definition.Parameters[i + num];
				parameterDefinition.Attributes = (Mono.Cecil.ParameterAttributes)parameters[i].Attributes;
				parameterDefinition.Name = parameters[i].Name;
			}
			if (debug)
			{
				List<string> list2 = list.Select<Type, string>((Type p) => p.FullDescription()).ToList<string>();
				if (list.Count == dynamicMethodDefinition.Definition.Parameters.Count)
				{
					for (int j = 0; j < list.Count; j++)
					{
						List<string> list3 = list2;
						int num2 = j;
						list3[num2] = list3[num2] + " " + dynamicMethodDefinition.Definition.Parameters[j].Name;
					}
				}
				string[] array = new string[9];
				array[0] = "### Replacement: static ";
				array[1] = type.FullDescription();
				array[2] = " ";
				int num3 = 3;
				Type declaringType2 = original.DeclaringType;
				array[num3] = ((declaringType2 != null) ? declaringType2.FullName : null) ?? "GLOBALTYPE";
				array[4] = "::";
				array[5] = text;
				array[6] = "(";
				array[7] = list2.Join<string>(null, ", ");
				array[8] = ")";
				FileLog.Log(string.Concat(array));
			}
			return dynamicMethodDefinition;
		}

		internal static LocalBuilder[] DeclareLocalVariables(ILGenerator il, MethodBase member)
		{
			MethodBody methodBody = member.GetMethodBody();
			IList<LocalVariableInfo> list = ((methodBody != null) ? methodBody.LocalVariables : null);
			if (list == null)
			{
				return new LocalBuilder[0];
			}
			return list.Select<LocalVariableInfo, LocalBuilder>((LocalVariableInfo lvi) => il.DeclareLocal(lvi.LocalType, lvi.IsPinned)).ToArray<LocalBuilder>();
		}

		private LocalBuilder DeclareLocalVariable(Type type, bool isReturnValue = false)
		{
			if (type.IsByRef && !isReturnValue)
			{
				type = type.GetElementType();
			}
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			if (AccessTools.IsClass(type))
			{
				LocalBuilder localBuilder = this.il.DeclareLocal(type);
				this.emitter.Emit(OpCodes.Ldnull);
				this.emitter.Emit(OpCodes.Stloc, localBuilder);
				return localBuilder;
			}
			if (AccessTools.IsStruct(type))
			{
				LocalBuilder localBuilder2 = this.il.DeclareLocal(type);
				this.emitter.Emit(OpCodes.Ldloca, localBuilder2);
				this.emitter.Emit(OpCodes.Initobj, type);
				return localBuilder2;
			}
			if (AccessTools.IsValue(type))
			{
				LocalBuilder localBuilder3 = this.il.DeclareLocal(type);
				if (type == typeof(float))
				{
					this.emitter.Emit(OpCodes.Ldc_R4, 0f);
				}
				else if (type == typeof(double))
				{
					this.emitter.Emit(OpCodes.Ldc_R8, 0.0);
				}
				else if (type == typeof(long) || type == typeof(ulong))
				{
					this.emitter.Emit(OpCodes.Ldc_I8, 0L);
				}
				else
				{
					this.emitter.Emit(OpCodes.Ldc_I4, 0);
				}
				this.emitter.Emit(OpCodes.Stloc, localBuilder3);
				return localBuilder3;
			}
			return null;
		}

		private static OpCode LoadIndOpCodeFor(Type type)
		{
			if (type.IsEnum)
			{
				return OpCodes.Ldind_I4;
			}
			if (type == typeof(float))
			{
				return OpCodes.Ldind_R4;
			}
			if (type == typeof(double))
			{
				return OpCodes.Ldind_R8;
			}
			if (type == typeof(byte))
			{
				return OpCodes.Ldind_U1;
			}
			if (type == typeof(ushort))
			{
				return OpCodes.Ldind_U2;
			}
			if (type == typeof(uint))
			{
				return OpCodes.Ldind_U4;
			}
			if (type == typeof(ulong))
			{
				return OpCodes.Ldind_I8;
			}
			if (type == typeof(sbyte))
			{
				return OpCodes.Ldind_I1;
			}
			if (type == typeof(short))
			{
				return OpCodes.Ldind_I2;
			}
			if (type == typeof(int))
			{
				return OpCodes.Ldind_I4;
			}
			if (type == typeof(long))
			{
				return OpCodes.Ldind_I8;
			}
			return OpCodes.Ldind_Ref;
		}

		private static OpCode StoreIndOpCodeFor(Type type)
		{
			if (type.IsEnum)
			{
				return OpCodes.Stind_I4;
			}
			if (type == typeof(float))
			{
				return OpCodes.Stind_R4;
			}
			if (type == typeof(double))
			{
				return OpCodes.Stind_R8;
			}
			if (type == typeof(byte))
			{
				return OpCodes.Stind_I1;
			}
			if (type == typeof(ushort))
			{
				return OpCodes.Stind_I2;
			}
			if (type == typeof(uint))
			{
				return OpCodes.Stind_I4;
			}
			if (type == typeof(ulong))
			{
				return OpCodes.Stind_I8;
			}
			if (type == typeof(sbyte))
			{
				return OpCodes.Stind_I1;
			}
			if (type == typeof(short))
			{
				return OpCodes.Stind_I2;
			}
			if (type == typeof(int))
			{
				return OpCodes.Stind_I4;
			}
			if (type == typeof(long))
			{
				return OpCodes.Stind_I8;
			}
			return OpCodes.Stind_Ref;
		}

		private void InitializeOutParameter(int argIndex, Type type)
		{
			if (type.IsByRef)
			{
				type = type.GetElementType();
			}
			this.emitter.Emit(OpCodes.Ldarg, argIndex);
			if (AccessTools.IsStruct(type))
			{
				this.emitter.Emit(OpCodes.Initobj, type);
				return;
			}
			if (!AccessTools.IsValue(type))
			{
				this.emitter.Emit(OpCodes.Ldnull);
				this.emitter.Emit(OpCodes.Stind_Ref);
				return;
			}
			if (type == typeof(float))
			{
				this.emitter.Emit(OpCodes.Ldc_R4, 0f);
				this.emitter.Emit(OpCodes.Stind_R4);
				return;
			}
			if (type == typeof(double))
			{
				this.emitter.Emit(OpCodes.Ldc_R8, 0.0);
				this.emitter.Emit(OpCodes.Stind_R8);
				return;
			}
			if (type == typeof(long))
			{
				this.emitter.Emit(OpCodes.Ldc_I8, 0L);
				this.emitter.Emit(OpCodes.Stind_I8);
				return;
			}
			this.emitter.Emit(OpCodes.Ldc_I4, 0);
			this.emitter.Emit(OpCodes.Stind_I4);
		}

		private bool EmitOriginalBaseMethod()
		{
			MethodInfo methodInfo = this.original as MethodInfo;
			if (methodInfo != null)
			{
				this.emitter.Emit(OpCodes.Ldtoken, methodInfo);
			}
			else
			{
				ConstructorInfo constructorInfo = this.original as ConstructorInfo;
				if (constructorInfo == null)
				{
					return false;
				}
				this.emitter.Emit(OpCodes.Ldtoken, constructorInfo);
			}
			Type reflectedType = this.original.ReflectedType;
			if (reflectedType.IsGenericType)
			{
				this.emitter.Emit(OpCodes.Ldtoken, reflectedType);
			}
			this.emitter.Emit(OpCodes.Call, reflectedType.IsGenericType ? MethodPatcher.m_GetMethodFromHandle2 : MethodPatcher.m_GetMethodFromHandle1);
			return true;
		}

		private void EmitCallParameter(MethodInfo patch, Dictionary<string, LocalBuilder> variables, LocalBuilder runOriginalVariable, bool allowFirsParamPassthrough, out LocalBuilder tmpObjectVar, List<KeyValuePair<LocalBuilder, Type>> tmpBoxVars)
		{
			tmpObjectVar = null;
			bool flag = !this.original.IsStatic;
			ParameterInfo[] parameters = this.original.GetParameters();
			string[] array = parameters.Select<ParameterInfo, string>((ParameterInfo p) => p.Name).ToArray<string>();
			List<ParameterInfo> list = patch.GetParameters().ToList<ParameterInfo>();
			if (allowFirsParamPassthrough && patch.ReturnType != typeof(void) && list.Count > 0 && list[0].ParameterType == patch.ReturnType)
			{
				list.RemoveRange(0, 1);
			}
			foreach (ParameterInfo parameterInfo in list)
			{
				LocalBuilder localBuilder3;
				if (parameterInfo.Name == "__originalMethod")
				{
					if (!this.EmitOriginalBaseMethod())
					{
						this.emitter.Emit(OpCodes.Ldnull);
					}
				}
				else if (parameterInfo.Name == "__runOriginal")
				{
					if (runOriginalVariable != null)
					{
						this.emitter.Emit(OpCodes.Ldloc, runOriginalVariable);
					}
					else
					{
						this.emitter.Emit(OpCodes.Ldc_I4_0);
					}
				}
				else if (parameterInfo.Name == "__instance")
				{
					if (this.original.IsStatic)
					{
						this.emitter.Emit(OpCodes.Ldnull);
					}
					else
					{
						object obj = this.original.DeclaringType != null && AccessTools.IsStruct(this.original.DeclaringType);
						bool isByRef = parameterInfo.ParameterType.IsByRef;
						object obj2 = obj;
						if (obj2 == isByRef)
						{
							this.emitter.Emit(OpCodes.Ldarg_0);
						}
						if (obj2 != null && !isByRef)
						{
							this.emitter.Emit(OpCodes.Ldarg_0);
							this.emitter.Emit(OpCodes.Ldobj, this.original.DeclaringType);
						}
						if (obj2 == 0 && isByRef)
						{
							this.emitter.Emit(OpCodes.Ldarga, 0);
						}
					}
				}
				else if (parameterInfo.Name == "__args")
				{
					LocalBuilder localBuilder;
					if (variables.TryGetValue("__args", out localBuilder))
					{
						this.emitter.Emit(OpCodes.Ldloc, localBuilder);
					}
					else
					{
						this.emitter.Emit(OpCodes.Ldnull);
					}
				}
				else if (parameterInfo.Name.StartsWith("___", StringComparison.Ordinal))
				{
					string text = parameterInfo.Name.Substring("___".Length);
					FieldInfo fieldInfo;
					if (text.All<char>(new Func<char, bool>(char.IsDigit)))
					{
						fieldInfo = AccessTools.DeclaredField(this.original.DeclaringType, int.Parse(text));
						if (fieldInfo == null)
						{
							string text2 = "No field found at given index in class ";
							Type declaringType = this.original.DeclaringType;
							throw new ArgumentException(text2 + (((declaringType != null) ? declaringType.AssemblyQualifiedName : null) ?? "null"), text);
						}
					}
					else
					{
						fieldInfo = AccessTools.Field(this.original.DeclaringType, text);
						if (fieldInfo == null)
						{
							string text3 = "No such field defined in class ";
							Type declaringType2 = this.original.DeclaringType;
							throw new ArgumentException(text3 + (((declaringType2 != null) ? declaringType2.AssemblyQualifiedName : null) ?? "null"), text);
						}
					}
					if (fieldInfo.IsStatic)
					{
						this.emitter.Emit(parameterInfo.ParameterType.IsByRef ? OpCodes.Ldsflda : OpCodes.Ldsfld, fieldInfo);
					}
					else
					{
						this.emitter.Emit(OpCodes.Ldarg_0);
						this.emitter.Emit(parameterInfo.ParameterType.IsByRef ? OpCodes.Ldflda : OpCodes.Ldfld, fieldInfo);
					}
				}
				else if (parameterInfo.Name == "__state")
				{
					OpCode opCode = (parameterInfo.ParameterType.IsByRef ? OpCodes.Ldloca : OpCodes.Ldloc);
					Type declaringType3 = patch.DeclaringType;
					LocalBuilder localBuilder2;
					if (variables.TryGetValue(((declaringType3 != null) ? declaringType3.AssemblyQualifiedName : null) ?? "null", out localBuilder2))
					{
						this.emitter.Emit(opCode, localBuilder2);
					}
					else
					{
						this.emitter.Emit(OpCodes.Ldnull);
					}
				}
				else if (parameterInfo.Name == "__result")
				{
					Type returnedType = AccessTools.GetReturnedType(this.original);
					if (returnedType == typeof(void))
					{
						throw new Exception("Cannot get result from void method " + this.original.FullDescription());
					}
					Type type = parameterInfo.ParameterType;
					if (type.IsByRef && !returnedType.IsByRef)
					{
						type = type.GetElementType();
					}
					if (!type.IsAssignableFrom(returnedType))
					{
						throw new Exception(string.Concat(new string[]
						{
							"Cannot assign method return type ",
							returnedType.FullName,
							" to __result type ",
							type.FullName,
							" for method ",
							this.original.FullDescription()
						}));
					}
					OpCode opCode2 = ((parameterInfo.ParameterType.IsByRef && !returnedType.IsByRef) ? OpCodes.Ldloca : OpCodes.Ldloc);
					if (returnedType.IsValueType && parameterInfo.ParameterType == typeof(object).MakeByRefType())
					{
						opCode2 = OpCodes.Ldloc;
					}
					this.emitter.Emit(opCode2, variables["__result"]);
					if (returnedType.IsValueType)
					{
						if (parameterInfo.ParameterType == typeof(object))
						{
							this.emitter.Emit(OpCodes.Box, returnedType);
						}
						else if (parameterInfo.ParameterType == typeof(object).MakeByRefType())
						{
							this.emitter.Emit(OpCodes.Box, returnedType);
							tmpObjectVar = this.il.DeclareLocal(typeof(object));
							this.emitter.Emit(OpCodes.Stloc, tmpObjectVar);
							this.emitter.Emit(OpCodes.Ldloca, tmpObjectVar);
						}
					}
				}
				else if (variables.TryGetValue(parameterInfo.Name, out localBuilder3))
				{
					OpCode opCode3 = (parameterInfo.ParameterType.IsByRef ? OpCodes.Ldloca : OpCodes.Ldloc);
					this.emitter.Emit(opCode3, localBuilder3);
				}
				else
				{
					int argumentIndex;
					if (parameterInfo.Name.StartsWith("__", StringComparison.Ordinal))
					{
						if (!int.TryParse(parameterInfo.Name.Substring("__".Length), out argumentIndex))
						{
							throw new Exception("Parameter " + parameterInfo.Name + " does not contain a valid index");
						}
						if (argumentIndex < 0 || argumentIndex >= parameters.Length)
						{
							throw new Exception(string.Format("No parameter found at index {0}", argumentIndex));
						}
					}
					else
					{
						argumentIndex = patch.GetArgumentIndex(array, parameterInfo);
						if (argumentIndex == -1)
						{
							HarmonyMethod mergedFromType = HarmonyMethodExtensions.GetMergedFromType(parameterInfo.ParameterType);
							MethodType? methodType = mergedFromType.methodType;
							if (methodType == null)
							{
								mergedFromType.methodType = new MethodType?(MethodType.Normal);
							}
							MethodInfo methodInfo = mergedFromType.GetOriginalMethod() as MethodInfo;
							if (methodInfo != null)
							{
								ConstructorInfo constructor = parameterInfo.ParameterType.GetConstructor(new Type[]
								{
									typeof(object),
									typeof(IntPtr)
								});
								if (constructor != null)
								{
									Type declaringType4 = this.original.DeclaringType;
									if (methodInfo.IsStatic)
									{
										this.emitter.Emit(OpCodes.Ldnull);
									}
									else
									{
										this.emitter.Emit(OpCodes.Ldarg_0);
										if (declaringType4 != null && declaringType4.IsValueType)
										{
											this.emitter.Emit(OpCodes.Ldobj, declaringType4);
											this.emitter.Emit(OpCodes.Box, declaringType4);
										}
									}
									if (!methodInfo.IsStatic && !mergedFromType.nonVirtualDelegate)
									{
										this.emitter.Emit(OpCodes.Dup);
										this.emitter.Emit(OpCodes.Ldvirtftn, methodInfo);
									}
									else
									{
										this.emitter.Emit(OpCodes.Ldftn, methodInfo);
									}
									this.emitter.Emit(OpCodes.Newobj, constructor);
									continue;
								}
							}
							throw new Exception("Parameter \"" + parameterInfo.Name + "\" not found in method " + this.original.FullDescription());
						}
					}
					Type parameterType = parameters[argumentIndex].ParameterType;
					Type type2 = (parameterType.IsByRef ? parameterType.GetElementType() : parameterType);
					Type parameterType2 = parameterInfo.ParameterType;
					Type type3 = (parameterType2.IsByRef ? parameterType2.GetElementType() : parameterType2);
					bool flag2 = !parameters[argumentIndex].IsOut && !parameterType.IsByRef;
					bool flag3 = !parameterInfo.IsOut && !parameterType2.IsByRef;
					bool flag4 = type2.IsValueType && !type3.IsValueType;
					int num = argumentIndex + (flag ? 1 : 0) + (this.useStructReturnBuffer ? 1 : 0);
					if (flag2 == flag3)
					{
						this.emitter.Emit(OpCodes.Ldarg, num);
						if (flag4)
						{
							if (flag3)
							{
								this.emitter.Emit(OpCodes.Box, type2);
							}
							else
							{
								this.emitter.Emit(OpCodes.Ldobj, type2);
								this.emitter.Emit(OpCodes.Box, type2);
								LocalBuilder localBuilder4 = this.il.DeclareLocal(type3);
								this.emitter.Emit(OpCodes.Stloc, localBuilder4);
								this.emitter.Emit(OpCodes.Ldloca_S, localBuilder4);
								tmpBoxVars.Add(new KeyValuePair<LocalBuilder, Type>(localBuilder4, type2));
							}
						}
					}
					else if (flag2 && !flag3)
					{
						if (flag4)
						{
							this.emitter.Emit(OpCodes.Ldarg, num);
							this.emitter.Emit(OpCodes.Box, type2);
							LocalBuilder localBuilder5 = this.il.DeclareLocal(type3);
							this.emitter.Emit(OpCodes.Stloc, localBuilder5);
							this.emitter.Emit(OpCodes.Ldloca_S, localBuilder5);
						}
						else
						{
							this.emitter.Emit(OpCodes.Ldarga, num);
						}
					}
					else
					{
						this.emitter.Emit(OpCodes.Ldarg, num);
						if (flag4)
						{
							this.emitter.Emit(OpCodes.Ldobj, type2);
							this.emitter.Emit(OpCodes.Box, type2);
						}
						else if (type2.IsValueType)
						{
							this.emitter.Emit(OpCodes.Ldobj, type2);
						}
						else
						{
							this.emitter.Emit(MethodPatcher.LoadIndOpCodeFor(parameters[argumentIndex].ParameterType));
						}
					}
				}
			}
		}

		private static bool PrefixAffectsOriginal(MethodInfo fix)
		{
			if (fix.ReturnType == typeof(bool))
			{
				return true;
			}
			return fix.GetParameters().Any<ParameterInfo>(delegate(ParameterInfo p)
			{
				string name = p.Name;
				Type parameterType = p.ParameterType;
				return !(name == "__instance") && !(name == "__originalMethod") && !(name == "__state") && (p.IsOut || p.IsRetval || parameterType.IsByRef || (!AccessTools.IsValue(parameterType) && !AccessTools.IsStruct(parameterType)));
			});
		}

		private void AddPrefixes(Dictionary<string, LocalBuilder> variables, LocalBuilder runOriginalVariable)
		{
			Action<KeyValuePair<LocalBuilder, Type>> <>9__2;
			this.prefixes.Do<MethodInfo>(delegate(MethodInfo fix)
			{
				Label? label = (MethodPatcher.PrefixAffectsOriginal(fix) ? new Label?(this.il.DefineLabel()) : null);
				if (label != null)
				{
					this.emitter.Emit(OpCodes.Ldloc, runOriginalVariable);
					this.emitter.Emit(OpCodes.Brfalse, label.Value);
				}
				List<KeyValuePair<LocalBuilder, Type>> list = new List<KeyValuePair<LocalBuilder, Type>>();
				LocalBuilder localBuilder;
				this.EmitCallParameter(fix, variables, runOriginalVariable, false, out localBuilder, list);
				this.emitter.Emit(OpCodes.Call, fix);
				if (fix.GetParameters().Any<ParameterInfo>((ParameterInfo p) => p.Name == "__args"))
				{
					this.RestoreArgumentArray(variables);
				}
				if (localBuilder != null)
				{
					this.emitter.Emit(OpCodes.Ldloc, localBuilder);
					this.emitter.Emit(OpCodes.Unbox_Any, AccessTools.GetReturnedType(this.original));
					this.emitter.Emit(OpCodes.Stloc, variables["__result"]);
				}
				IEnumerable<KeyValuePair<LocalBuilder, Type>> enumerable = list;
				Action<KeyValuePair<LocalBuilder, Type>> action;
				if ((action = <>9__2) == null)
				{
					action = (<>9__2 = delegate(KeyValuePair<LocalBuilder, Type> tmpBoxVar)
					{
						this.emitter.Emit(this.original.IsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1);
						this.emitter.Emit(OpCodes.Ldloc, tmpBoxVar.Key);
						this.emitter.Emit(OpCodes.Unbox_Any, tmpBoxVar.Value);
						this.emitter.Emit(OpCodes.Stobj, tmpBoxVar.Value);
					});
				}
				enumerable.Do<KeyValuePair<LocalBuilder, Type>>(action);
				Type type = fix.ReturnType;
				if (type != typeof(void))
				{
					if (type != typeof(bool))
					{
						throw new Exception(string.Format("Prefix patch {0} has not \"bool\" or \"void\" return type: {1}", fix, fix.ReturnType));
					}
					this.emitter.Emit(OpCodes.Stloc, runOriginalVariable);
				}
				if (label != null)
				{
					this.emitter.MarkLabel(label.Value);
					this.emitter.Emit(OpCodes.Nop);
				}
			});
		}

		private bool AddPostfixes(Dictionary<string, LocalBuilder> variables, LocalBuilder runOriginalVariable, bool passthroughPatches)
		{
			bool result = false;
			Action<KeyValuePair<LocalBuilder, Type>> <>9__3;
			this.postfixes.Where<MethodInfo>((MethodInfo fix) => passthroughPatches == (fix.ReturnType != typeof(void))).Do<MethodInfo>(delegate(MethodInfo fix)
			{
				List<KeyValuePair<LocalBuilder, Type>> list = new List<KeyValuePair<LocalBuilder, Type>>();
				LocalBuilder localBuilder;
				this.EmitCallParameter(fix, variables, runOriginalVariable, true, out localBuilder, list);
				this.emitter.Emit(OpCodes.Call, fix);
				if (fix.GetParameters().Any<ParameterInfo>((ParameterInfo p) => p.Name == "__args"))
				{
					this.RestoreArgumentArray(variables);
				}
				if (localBuilder != null)
				{
					this.emitter.Emit(OpCodes.Ldloc, localBuilder);
					this.emitter.Emit(OpCodes.Unbox_Any, AccessTools.GetReturnedType(this.original));
					this.emitter.Emit(OpCodes.Stloc, variables["__result"]);
				}
				IEnumerable<KeyValuePair<LocalBuilder, Type>> enumerable = list;
				Action<KeyValuePair<LocalBuilder, Type>> action;
				if ((action = <>9__3) == null)
				{
					action = (<>9__3 = delegate(KeyValuePair<LocalBuilder, Type> tmpBoxVar)
					{
						this.emitter.Emit(this.original.IsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1);
						this.emitter.Emit(OpCodes.Ldloc, tmpBoxVar.Key);
						this.emitter.Emit(OpCodes.Unbox_Any, tmpBoxVar.Value);
						this.emitter.Emit(OpCodes.Stobj, tmpBoxVar.Value);
					});
				}
				enumerable.Do<KeyValuePair<LocalBuilder, Type>>(action);
				if (!(fix.ReturnType != typeof(void)))
				{
					return;
				}
				ParameterInfo parameterInfo = fix.GetParameters().FirstOrDefault<ParameterInfo>();
				if (parameterInfo != null && fix.ReturnType == parameterInfo.ParameterType)
				{
					result = true;
					return;
				}
				if (parameterInfo != null)
				{
					throw new Exception(string.Format("Return type of pass through postfix {0} does not match type of its first parameter", fix));
				}
				throw new Exception(string.Format("Postfix patch {0} must have a \"void\" return type", fix));
			});
			return result;
		}

		private bool AddFinalizers(Dictionary<string, LocalBuilder> variables, LocalBuilder runOriginalVariable, bool catchExceptions)
		{
			bool rethrowPossible = true;
			Action<KeyValuePair<LocalBuilder, Type>> <>9__2;
			this.finalizers.Do<MethodInfo>(delegate(MethodInfo fix)
			{
				if (catchExceptions)
				{
					Label? label;
					this.emitter.MarkBlockBefore(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock, null), out label);
				}
				List<KeyValuePair<LocalBuilder, Type>> list = new List<KeyValuePair<LocalBuilder, Type>>();
				LocalBuilder localBuilder;
				this.EmitCallParameter(fix, variables, runOriginalVariable, false, out localBuilder, list);
				this.emitter.Emit(OpCodes.Call, fix);
				if (fix.GetParameters().Any<ParameterInfo>((ParameterInfo p) => p.Name == "__args"))
				{
					this.RestoreArgumentArray(variables);
				}
				if (localBuilder != null)
				{
					this.emitter.Emit(OpCodes.Ldloc, localBuilder);
					this.emitter.Emit(OpCodes.Unbox_Any, AccessTools.GetReturnedType(this.original));
					this.emitter.Emit(OpCodes.Stloc, variables["__result"]);
				}
				IEnumerable<KeyValuePair<LocalBuilder, Type>> enumerable = list;
				Action<KeyValuePair<LocalBuilder, Type>> action;
				if ((action = <>9__2) == null)
				{
					action = (<>9__2 = delegate(KeyValuePair<LocalBuilder, Type> tmpBoxVar)
					{
						this.emitter.Emit(this.original.IsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1);
						this.emitter.Emit(OpCodes.Ldloc, tmpBoxVar.Key);
						this.emitter.Emit(OpCodes.Unbox_Any, tmpBoxVar.Value);
						this.emitter.Emit(OpCodes.Stobj, tmpBoxVar.Value);
					});
				}
				enumerable.Do<KeyValuePair<LocalBuilder, Type>>(action);
				if (fix.ReturnType != typeof(void))
				{
					this.emitter.Emit(OpCodes.Stloc, variables["__exception"]);
					rethrowPossible = false;
				}
				if (catchExceptions)
				{
					Label? label2;
					this.emitter.MarkBlockBefore(new ExceptionBlock(ExceptionBlockType.BeginCatchBlock, null), out label2);
					this.emitter.Emit(OpCodes.Pop);
					this.emitter.MarkBlockAfter(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock, null));
				}
			});
			return rethrowPossible;
		}

		private void PrepareArgumentArray()
		{
			ParameterInfo[] parameters = this.original.GetParameters();
			int num = 0;
			foreach (ParameterInfo parameterInfo in parameters)
			{
				int num2 = num++ + (this.original.IsStatic ? 0 : 1);
				if (parameterInfo.IsOut || parameterInfo.IsRetval)
				{
					this.InitializeOutParameter(num2, parameterInfo.ParameterType);
				}
			}
			this.emitter.Emit(OpCodes.Ldc_I4, parameters.Length);
			this.emitter.Emit(OpCodes.Newarr, typeof(object));
			num = 0;
			int num3 = 0;
			foreach (ParameterInfo parameterInfo2 in parameters)
			{
				int num4 = num++ + (this.original.IsStatic ? 0 : 1);
				Type type = parameterInfo2.ParameterType;
				bool isByRef = type.IsByRef;
				if (isByRef)
				{
					type = type.GetElementType();
				}
				this.emitter.Emit(OpCodes.Dup);
				this.emitter.Emit(OpCodes.Ldc_I4, num3++);
				this.emitter.Emit(OpCodes.Ldarg, num4);
				if (isByRef)
				{
					if (AccessTools.IsStruct(type))
					{
						this.emitter.Emit(OpCodes.Ldobj, type);
					}
					else
					{
						this.emitter.Emit(MethodPatcher.LoadIndOpCodeFor(type));
					}
				}
				if (type.IsValueType)
				{
					this.emitter.Emit(OpCodes.Box, type);
				}
				this.emitter.Emit(OpCodes.Stelem_Ref);
			}
		}

		private void RestoreArgumentArray(Dictionary<string, LocalBuilder> variables)
		{
			ParameterInfo[] parameters = this.original.GetParameters();
			int num = 0;
			int num2 = 0;
			foreach (ParameterInfo parameterInfo in parameters)
			{
				int num3 = num++ + (this.original.IsStatic ? 0 : 1);
				Type type = parameterInfo.ParameterType;
				if (type.IsByRef)
				{
					type = type.GetElementType();
					this.emitter.Emit(OpCodes.Ldarg, num3);
					this.emitter.Emit(OpCodes.Ldloc, variables["__args"]);
					this.emitter.Emit(OpCodes.Ldc_I4, num2);
					this.emitter.Emit(OpCodes.Ldelem_Ref);
					if (type.IsValueType)
					{
						this.emitter.Emit(OpCodes.Unbox_Any, type);
						if (AccessTools.IsStruct(type))
						{
							this.emitter.Emit(OpCodes.Stobj, type);
						}
						else
						{
							this.emitter.Emit(MethodPatcher.StoreIndOpCodeFor(type));
						}
					}
					else
					{
						this.emitter.Emit(OpCodes.Castclass, type);
						this.emitter.Emit(OpCodes.Stind_Ref);
					}
				}
				num2++;
			}
		}

		private const string INSTANCE_PARAM = "__instance";

		private const string ORIGINAL_METHOD_PARAM = "__originalMethod";

		private const string ARGS_ARRAY_VAR = "__args";

		private const string RESULT_VAR = "__result";

		private const string STATE_VAR = "__state";

		private const string EXCEPTION_VAR = "__exception";

		private const string RUN_ORIGINAL_VAR = "__runOriginal";

		private const string PARAM_INDEX_PREFIX = "__";

		private const string INSTANCE_FIELD_PREFIX = "___";

		private readonly bool debug;

		private readonly MethodBase original;

		private readonly MethodBase source;

		private readonly List<MethodInfo> prefixes;

		private readonly List<MethodInfo> postfixes;

		private readonly List<MethodInfo> transpilers;

		private readonly List<MethodInfo> finalizers;

		private readonly int idx;

		private readonly bool useStructReturnBuffer;

		private readonly Type returnType;

		private readonly DynamicMethodDefinition patch;

		private readonly ILGenerator il;

		private readonly Emitter emitter;

		private static readonly MethodInfo m_GetMethodFromHandle1 = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) });

		private static readonly MethodInfo m_GetMethodFromHandle2 = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[]
		{
			typeof(RuntimeMethodHandle),
			typeof(RuntimeTypeHandle)
		});
	}
}
