using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace HarmonyLib
{
	internal static class MethodCreatorTools
	{
		internal static List<CodeInstruction> GenerateVariableInit(this MethodCreator _, LocalBuilder variable, bool isReturnValue = false)
		{
			List<CodeInstruction> list = new List<CodeInstruction>();
			Type type = variable.LocalType;
			if (type.IsByRef)
			{
				if (isReturnValue)
				{
					list.Add(Code.Ldc_I4_1);
					list.Add(Code.Newarr[type.GetElementType(), null]);
					list.Add(Code.Ldc_I4_0);
					list.Add(Code.Ldelema[type.GetElementType(), null]);
					list.Add(Code.Stloc[variable, null]);
					return list;
				}
				type = type.GetElementType();
			}
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			if (AccessTools.IsClass(type))
			{
				list.Add(Code.Ldnull);
				list.Add(Code.Stloc[variable, null]);
				return list;
			}
			if (AccessTools.IsStruct(type))
			{
				list.Add(Code.Ldloca[variable, null]);
				list.Add(Code.Initobj[type, null]);
				return list;
			}
			if (AccessTools.IsValue(type))
			{
				if (type == typeof(float))
				{
					list.Add(Code.Ldc_R4[0f, null]);
				}
				else if (type == typeof(double))
				{
					list.Add(Code.Ldc_R8[0.0, null]);
				}
				else if (type == typeof(long) || type == typeof(ulong))
				{
					list.Add(Code.Ldc_I8[0L, null]);
				}
				else
				{
					list.Add(Code.Ldc_I4[0, null]);
				}
				list.Add(Code.Stloc[variable, null]);
				return list;
			}
			return list;
		}

		internal static List<CodeInstruction> PrepareArgumentArray(this MethodCreator creator)
		{
			List<CodeInstruction> list = new List<CodeInstruction>();
			MethodBase original = creator.config.original;
			bool isStatic = original.IsStatic;
			ParameterInfo[] parameters = original.GetParameters();
			int num = 0;
			foreach (ParameterInfo parameterInfo in parameters)
			{
				int num2 = num++ + ((!isStatic) ? 1 : 0);
				if (parameterInfo.IsOut || parameterInfo.IsRetval)
				{
					list.AddRange(MethodCreatorTools.InitializeOutParameter(num2, parameterInfo.ParameterType));
				}
			}
			list.Add(Code.Ldc_I4[parameters.Length, null]);
			list.Add(Code.Newarr[typeof(object), null]);
			num = 0;
			int num3 = 0;
			foreach (ParameterInfo parameterInfo2 in parameters)
			{
				int num4 = num++ + ((!isStatic) ? 1 : 0);
				Type type = parameterInfo2.ParameterType;
				bool isByRef = type.IsByRef;
				if (isByRef)
				{
					type = type.GetElementType();
				}
				list.Add(Code.Dup);
				list.Add(Code.Ldc_I4[num3++, null]);
				list.Add(Code.Ldarg[num4, null]);
				if (isByRef)
				{
					if (AccessTools.IsStruct(type))
					{
						list.Add(Code.Ldobj[type, null]);
					}
					else
					{
						list.Add(MethodCreatorTools.LoadIndOpCodeFor(type));
					}
				}
				if (type.IsValueType)
				{
					list.Add(Code.Box[type, null]);
				}
				list.Add(Code.Stelem_Ref);
			}
			return list;
		}

		internal static bool AffectsOriginal(this MethodCreator creator, MethodInfo fix)
		{
			if (fix.ReturnType == typeof(bool))
			{
				return true;
			}
			List<InjectedParameter> list;
			if (!creator.config.injections.TryGetValue(fix, out list))
			{
				return false;
			}
			return list.Any<InjectedParameter>(delegate(InjectedParameter parameter)
			{
				if (parameter.injectionType == InjectionType.Instance)
				{
					return false;
				}
				if (parameter.injectionType == InjectionType.OriginalMethod)
				{
					return false;
				}
				if (parameter.injectionType == InjectionType.State)
				{
					return false;
				}
				ParameterInfo parameterInfo = parameter.parameterInfo;
				if (parameterInfo.IsOut || parameterInfo.IsRetval)
				{
					return true;
				}
				Type parameterType = parameterInfo.ParameterType;
				return parameterType.IsByRef || (!AccessTools.IsValue(parameterType) && !AccessTools.IsStruct(parameterType));
			});
		}

		internal static CodeInstruction MarkBlock(this MethodCreator _, ExceptionBlockType blockType)
		{
			return Code.Nop.WithBlocks(new ExceptionBlock[]
			{
				new ExceptionBlock(blockType, null)
			});
		}

		internal static List<CodeInstruction> EmitCallParameter(this MethodCreator creator, MethodInfo patch, bool allowFirsParamPassthrough, out LocalBuilder tmpInstanceBoxingVar, out LocalBuilder tmpObjectVar, out bool refResultUsed, List<KeyValuePair<LocalBuilder, Type>> tmpBoxVars)
		{
			tmpInstanceBoxingVar = null;
			tmpObjectVar = null;
			refResultUsed = false;
			List<CodeInstruction> list = new List<CodeInstruction>();
			MethodCreatorConfig config = creator.config;
			MethodBase original = config.original;
			bool isStatic = original.IsStatic;
			Type returnType = config.returnType;
			List<InjectedParameter> list2 = config.injections[patch].ToList<InjectedParameter>();
			bool flag = !isStatic;
			ParameterInfo[] parameters = original.GetParameters();
			string[] array = parameters.Select<ParameterInfo, string>((ParameterInfo p) => p.Name).ToArray<string>();
			Type declaringType = original.DeclaringType;
			List<ParameterInfo> list3 = patch.GetParameters().ToList<ParameterInfo>();
			if (allowFirsParamPassthrough && patch.ReturnType != typeof(void) && list3.Count > 0 && list3[0].ParameterType == patch.ReturnType)
			{
				list2.RemoveAt(0);
				list3.RemoveAt(0);
			}
			foreach (InjectedParameter injectedParameter in list2)
			{
				InjectionType injectionType = injectedParameter.injectionType;
				string realName = injectedParameter.realName;
				Type parameterType = injectedParameter.parameterInfo.ParameterType;
				LocalBuilder localBuilder3;
				if (injectionType == InjectionType.OriginalMethod)
				{
					if (!MethodCreatorTools.EmitOriginalBaseMethod(original, list))
					{
						list.Add(Code.Ldnull);
					}
				}
				else if (injectionType == InjectionType.Exception)
				{
					if (config.exceptionVariable != null)
					{
						list.Add(Code.Ldloc[config.exceptionVariable, null]);
					}
					else
					{
						list.Add(Code.Ldnull);
					}
				}
				else if (injectionType == InjectionType.RunOriginal)
				{
					if (config.runOriginalVariable != null)
					{
						list.Add(Code.Ldloc[config.runOriginalVariable, null]);
					}
					else
					{
						list.Add(Code.Ldc_I4_0);
					}
				}
				else if (injectionType == InjectionType.Instance)
				{
					if (isStatic)
					{
						list.Add(Code.Ldnull);
					}
					else
					{
						bool isByRef = parameterType.IsByRef;
						bool flag2 = parameterType == typeof(object) || parameterType == typeof(object).MakeByRefType();
						if (AccessTools.IsStruct(declaringType))
						{
							if (flag2)
							{
								if (isByRef)
								{
									list.Add(Code.Ldarg_0);
									list.Add(Code.Ldobj[declaringType, null]);
									list.Add(Code.Box[declaringType, null]);
									tmpInstanceBoxingVar = config.DeclareLocal(typeof(object), false);
									list.Add(Code.Stloc[tmpInstanceBoxingVar, null]);
									list.Add(Code.Ldloca[tmpInstanceBoxingVar, null]);
								}
								else
								{
									list.Add(Code.Ldarg_0);
									list.Add(Code.Ldobj[declaringType, null]);
									list.Add(Code.Box[declaringType, null]);
								}
							}
							else if (isByRef)
							{
								list.Add(Code.Ldarg_0);
							}
							else
							{
								list.Add(Code.Ldarg_0);
								list.Add(Code.Ldobj[declaringType, null]);
							}
						}
						else if (isByRef)
						{
							list.Add(Code.Ldarga[0, null]);
						}
						else
						{
							list.Add(Code.Ldarg_0);
						}
					}
				}
				else if (injectionType == InjectionType.ArgsArray)
				{
					LocalBuilder localBuilder;
					if (config.localVariables.TryGetValue(InjectionType.ArgsArray, out localBuilder))
					{
						list.Add(Code.Ldloc[localBuilder, null]);
					}
					else
					{
						list.Add(Code.Ldnull);
					}
				}
				else if (realName.StartsWith("___", StringComparison.Ordinal))
				{
					string text = realName.Substring("___".Length);
					IEnumerable<char> enumerable = text;
					Func<char, bool> func;
					if ((func = MethodCreatorTools.<>O.<0>__IsDigit) == null)
					{
						func = (MethodCreatorTools.<>O.<0>__IsDigit = new Func<char, bool>(char.IsDigit));
					}
					FieldInfo fieldInfo;
					if (enumerable.All<char>(func))
					{
						fieldInfo = AccessTools.DeclaredField(declaringType, int.Parse(text));
						if (fieldInfo == null)
						{
							throw new ArgumentException("No field found at given index in class " + (((declaringType != null) ? declaringType.AssemblyQualifiedName : null) ?? "null"), text);
						}
					}
					else
					{
						fieldInfo = AccessTools.Field(declaringType, text);
						if (fieldInfo == null)
						{
							throw new ArgumentException("No such field defined in class " + (((declaringType != null) ? declaringType.AssemblyQualifiedName : null) ?? "null"), text);
						}
					}
					if (fieldInfo.IsStatic)
					{
						list.Add(parameterType.IsByRef ? Code.Ldsflda[fieldInfo, null] : Code.Ldsfld[fieldInfo, null]);
					}
					else
					{
						list.Add(Code.Ldarg_0);
						list.Add(parameterType.IsByRef ? Code.Ldflda[fieldInfo, null] : Code.Ldfld[fieldInfo, null]);
					}
				}
				else if (injectionType == InjectionType.State)
				{
					global::System.Reflection.Emit.OpCode opCode = (parameterType.IsByRef ? global::System.Reflection.Emit.OpCodes.Ldloca : global::System.Reflection.Emit.OpCodes.Ldloc);
					VariableState localVariables = config.localVariables;
					Type declaringType2 = patch.DeclaringType;
					LocalBuilder localBuilder2;
					if (localVariables.TryGetValue(((declaringType2 != null) ? declaringType2.AssemblyQualifiedName : null) ?? "null", out localBuilder2))
					{
						list.Add(new CodeInstruction(opCode, localBuilder2));
					}
					else
					{
						list.Add(Code.Ldnull);
					}
				}
				else if (injectionType == InjectionType.Result)
				{
					if (returnType == typeof(void))
					{
						throw new Exception("Cannot get result from void method " + original.FullDescription());
					}
					Type type = parameterType;
					if (type.IsByRef && !returnType.IsByRef)
					{
						type = type.GetElementType();
					}
					if (!type.IsAssignableFrom(returnType))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 4);
						defaultInterpolatedStringHandler.AppendLiteral("Cannot assign method return type ");
						defaultInterpolatedStringHandler.AppendFormatted(returnType.FullName);
						defaultInterpolatedStringHandler.AppendLiteral(" to ");
						defaultInterpolatedStringHandler.AppendFormatted("__result");
						defaultInterpolatedStringHandler.AppendLiteral(" type ");
						defaultInterpolatedStringHandler.AppendFormatted(type.FullName);
						defaultInterpolatedStringHandler.AppendLiteral(" for method ");
						defaultInterpolatedStringHandler.AppendFormatted(original.FullDescription());
						throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					global::System.Reflection.Emit.OpCode opCode2 = ((parameterType.IsByRef && !returnType.IsByRef) ? global::System.Reflection.Emit.OpCodes.Ldloca : global::System.Reflection.Emit.OpCodes.Ldloc);
					if (returnType.IsValueType && parameterType == typeof(object).MakeByRefType())
					{
						opCode2 = global::System.Reflection.Emit.OpCodes.Ldloc;
					}
					list.Add(new CodeInstruction(opCode2, config.GetLocal(InjectionType.Result)));
					if (returnType.IsValueType)
					{
						if (parameterType == typeof(object))
						{
							list.Add(Code.Box[returnType, null]);
						}
						else if (parameterType == typeof(object).MakeByRefType())
						{
							list.Add(Code.Box[returnType, null]);
							tmpObjectVar = config.DeclareLocal(typeof(object), false);
							list.Add(Code.Stloc[tmpObjectVar, null]);
							list.Add(Code.Ldloca[tmpObjectVar, null]);
						}
					}
				}
				else if (injectionType == InjectionType.ResultRef)
				{
					if (!returnType.IsByRef)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 3);
						defaultInterpolatedStringHandler.AppendLiteral("Cannot use ");
						defaultInterpolatedStringHandler.AppendFormatted<InjectionType>(InjectionType.ResultRef);
						defaultInterpolatedStringHandler.AppendLiteral(" with non-ref return type ");
						defaultInterpolatedStringHandler.AppendFormatted(returnType.FullName);
						defaultInterpolatedStringHandler.AppendLiteral(" of method ");
						defaultInterpolatedStringHandler.AppendFormatted(original.FullDescription());
						throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					Type type2 = parameterType;
					Type type3 = typeof(RefResult<>).MakeGenericType(new Type[] { returnType.GetElementType() }).MakeByRefType();
					if (type2 != type3)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 4);
						defaultInterpolatedStringHandler.AppendLiteral("Wrong type of ");
						defaultInterpolatedStringHandler.AppendFormatted("__resultRef");
						defaultInterpolatedStringHandler.AppendLiteral(" for method ");
						defaultInterpolatedStringHandler.AppendFormatted(original.FullDescription());
						defaultInterpolatedStringHandler.AppendLiteral(". Expected ");
						defaultInterpolatedStringHandler.AppendFormatted(type3.FullName);
						defaultInterpolatedStringHandler.AppendLiteral(", got ");
						defaultInterpolatedStringHandler.AppendFormatted(type2.FullName);
						throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					list.Add(Code.Ldloca[config.GetLocal(InjectionType.ResultRef), null]);
					refResultUsed = true;
				}
				else if (config.localVariables.TryGetValue(realName, out localBuilder3))
				{
					global::System.Reflection.Emit.OpCode opCode3 = (parameterType.IsByRef ? global::System.Reflection.Emit.OpCodes.Ldloca : global::System.Reflection.Emit.OpCodes.Ldloc);
					list.Add(new CodeInstruction(opCode3, localBuilder3));
				}
				else
				{
					int argumentIndex;
					if (realName.StartsWith("__", StringComparison.Ordinal))
					{
						string text2 = realName.Substring("__".Length);
						if (!int.TryParse(text2, out argumentIndex))
						{
							throw new Exception("Parameter " + realName + " does not contain a valid index");
						}
						if (argumentIndex < 0 || argumentIndex >= parameters.Length)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
							defaultInterpolatedStringHandler.AppendLiteral("No parameter found at index ");
							defaultInterpolatedStringHandler.AppendFormatted<int>(argumentIndex);
							throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
						}
					}
					else
					{
						argumentIndex = patch.GetArgumentIndex(array, injectedParameter.parameterInfo);
						if (argumentIndex == -1)
						{
							HarmonyMethod mergedFromType = HarmonyMethodExtensions.GetMergedFromType(parameterType);
							HarmonyMethod harmonyMethod = mergedFromType;
							MethodType methodType = harmonyMethod.methodType.GetValueOrDefault();
							if (harmonyMethod.methodType == null)
							{
								methodType = MethodType.Normal;
								harmonyMethod.methodType = new MethodType?(methodType);
							}
							MethodBase originalMethod = mergedFromType.GetOriginalMethod();
							MethodInfo methodInfo = originalMethod as MethodInfo;
							if (methodInfo != null)
							{
								ConstructorInfo constructor = parameterType.GetConstructor(new Type[]
								{
									typeof(object),
									typeof(IntPtr)
								});
								if (constructor != null)
								{
									if (methodInfo.IsStatic)
									{
										list.Add(Code.Ldnull);
									}
									else
									{
										list.Add(Code.Ldarg_0);
										if (declaringType != null && declaringType.IsValueType)
										{
											list.Add(Code.Ldobj[declaringType, null]);
											list.Add(Code.Box[declaringType, null]);
										}
									}
									if (!methodInfo.IsStatic && !mergedFromType.nonVirtualDelegate)
									{
										list.Add(Code.Dup);
										list.Add(Code.Ldvirtftn[methodInfo, null]);
									}
									else
									{
										list.Add(Code.Ldftn[methodInfo, null]);
									}
									list.Add(Code.Newobj[constructor, null]);
									continue;
								}
							}
							throw new Exception("Parameter \"" + realName + "\" not found in method " + original.FullDescription());
						}
					}
					Type parameterType2 = parameters[argumentIndex].ParameterType;
					Type type4 = (parameterType2.IsByRef ? parameterType2.GetElementType() : parameterType2);
					Type type5 = parameterType;
					Type type6 = (type5.IsByRef ? type5.GetElementType() : type5);
					bool flag3 = !parameters[argumentIndex].IsOut && !parameterType2.IsByRef;
					bool flag4 = !injectedParameter.parameterInfo.IsOut && !type5.IsByRef;
					bool flag5 = type4.IsValueType && !type6.IsValueType;
					int num = argumentIndex + ((flag > false) ? 1 : 0);
					if (flag3 == flag4)
					{
						list.Add(Code.Ldarg[num, null]);
						if (flag5)
						{
							if (flag4)
							{
								list.Add(Code.Box[type4, null]);
							}
							else
							{
								list.Add(Code.Ldobj[type4, null]);
								list.Add(Code.Box[type4, null]);
								LocalBuilder localBuilder4 = config.DeclareLocal(type6, false);
								list.Add(Code.Stloc[localBuilder4, null]);
								list.Add(Code.Ldloca_S[localBuilder4, null]);
								tmpBoxVars.Add(new KeyValuePair<LocalBuilder, Type>(localBuilder4, type4));
							}
						}
					}
					else if (flag3 && !flag4)
					{
						if (flag5)
						{
							list.Add(Code.Ldarg[num, null]);
							list.Add(Code.Box[type4, null]);
							LocalBuilder localBuilder5 = config.DeclareLocal(type6, false);
							list.Add(Code.Stloc[localBuilder5, null]);
							list.Add(Code.Ldloca_S[localBuilder5, null]);
						}
						else
						{
							list.Add(Code.Ldarga[num, null]);
						}
					}
					else
					{
						list.Add(Code.Ldarg[num, null]);
						if (flag5)
						{
							list.Add(Code.Ldobj[type4, null]);
							list.Add(Code.Box[type4, null]);
						}
						else if (type4.IsValueType)
						{
							list.Add(Code.Ldobj[type4, null]);
						}
						else
						{
							list.Add(new CodeInstruction(MethodCreatorTools.LoadIndOpCodeFor(parameters[argumentIndex].ParameterType)));
						}
					}
				}
			}
			return list;
		}

		internal static LocalBuilder[] DeclareOriginalLocalVariables(this MethodCreator creator, MethodBase member)
		{
			global::System.Reflection.MethodBody methodBody = member.GetMethodBody();
			IList<LocalVariableInfo> list = ((methodBody != null) ? methodBody.LocalVariables : null);
			if (list == null)
			{
				return Array.Empty<LocalBuilder>();
			}
			return list.Select<LocalVariableInfo, LocalBuilder>((LocalVariableInfo lvi) => creator.config.il.DeclareLocal(lvi.LocalType, lvi.IsPinned)).ToArray<LocalBuilder>();
		}

		internal static List<CodeInstruction> RestoreArgumentArray(this MethodCreator creator)
		{
			List<CodeInstruction> list = new List<CodeInstruction>();
			MethodBase original = creator.config.original;
			bool isStatic = original.IsStatic;
			ParameterInfo[] parameters = original.GetParameters();
			int num = 0;
			int num2 = 0;
			foreach (ParameterInfo parameterInfo in parameters)
			{
				int num3 = num++ + ((!isStatic) ? 1 : 0);
				Type type = parameterInfo.ParameterType;
				if (type.IsByRef)
				{
					type = type.GetElementType();
					list.Add(Code.Ldarg[num3, null]);
					list.Add(Code.Ldloc[creator.config.GetLocal(InjectionType.ArgsArray), null]);
					list.Add(Code.Ldc_I4[num2, null]);
					list.Add(Code.Ldelem_Ref);
					if (type.IsValueType)
					{
						list.Add(Code.Unbox_Any[type, null]);
						if (AccessTools.IsStruct(type))
						{
							list.Add(Code.Stobj[type, null]);
						}
						else
						{
							list.Add(MethodCreatorTools.StoreIndOpCodeFor(type));
						}
					}
					else
					{
						list.Add(Code.Castclass[type, null]);
						list.Add(Code.Stind_Ref);
					}
				}
				else
				{
					list.Add(Code.Ldloc[creator.config.GetLocal(InjectionType.ArgsArray), null]);
					list.Add(Code.Ldc_I4[num2, null]);
					list.Add(Code.Ldelem_Ref);
					if (type.IsValueType)
					{
						list.Add(Code.Unbox_Any[type, null]);
					}
					else
					{
						list.Add(Code.Castclass[type, null]);
					}
					list.Add(Code.Starg[num3, null]);
				}
				num2++;
			}
			return list;
		}

		internal static IEnumerable<CodeInstruction> CleanupCodes(this MethodCreator creator, IEnumerable<CodeInstruction> instructions, List<Label> endLabels)
		{
			MethodCreatorTools.<CleanupCodes>d__10 <CleanupCodes>d__ = new MethodCreatorTools.<CleanupCodes>d__10(-2);
			<CleanupCodes>d__.<>3__creator = creator;
			<CleanupCodes>d__.<>3__instructions = instructions;
			<CleanupCodes>d__.<>3__endLabels = endLabels;
			return <CleanupCodes>d__;
		}

		internal static void LogCodes(this MethodCreator _, Emitter emitter, List<CodeInstruction> codeInstructions)
		{
			int codePos = emitter.CurrentPos();
			IEnumerable<VariableDefinition> enumerable = emitter.Variables();
			Action<VariableDefinition> action;
			if ((action = MethodCreatorTools.<>O.<1>__LogIL) == null)
			{
				action = (MethodCreatorTools.<>O.<1>__LogIL = new Action<VariableDefinition>(FileLog.LogIL));
			}
			enumerable.Do<VariableDefinition>(action);
			Action<Label> <>9__1;
			Action<ExceptionBlock> <>9__2;
			Action<ExceptionBlock> <>9__3;
			codeInstructions.Do<CodeInstruction>(delegate(CodeInstruction codeInstruction)
			{
				IEnumerable<Label> labels = codeInstruction.labels;
				Action<Label> action2;
				if ((action2 = <>9__1) == null)
				{
					action2 = (<>9__1 = delegate(Label label)
					{
						FileLog.LogIL(codePos, label);
					});
				}
				labels.Do<Label>(action2);
				IEnumerable<ExceptionBlock> blocks = codeInstruction.blocks;
				Action<ExceptionBlock> action3;
				if ((action3 = <>9__2) == null)
				{
					action3 = (<>9__2 = delegate(ExceptionBlock block)
					{
						FileLog.LogILBlockBegin(codePos, block);
					});
				}
				blocks.Do<ExceptionBlock>(action3);
				global::System.Reflection.Emit.OpCode opcode = codeInstruction.opcode;
				object operand = codeInstruction.operand;
				bool flag = true;
				global::System.Reflection.Emit.OperandType operandType = opcode.OperandType;
				if (operandType != global::System.Reflection.Emit.OperandType.InlineNone)
				{
					if (operandType != global::System.Reflection.Emit.OperandType.InlineSig)
					{
						FileLog.LogIL(codePos, opcode, operand);
					}
					else
					{
						FileLog.LogIL(codePos, opcode, (ICallSiteGenerator)operand);
					}
				}
				else
				{
					string text = codeInstruction.IsAnnotation();
					if (text != null)
					{
						FileLog.LogILComment(codePos, text);
						flag = false;
					}
					else
					{
						FileLog.LogIL(codePos, opcode);
					}
				}
				IEnumerable<ExceptionBlock> blocks2 = codeInstruction.blocks;
				Action<ExceptionBlock> action4;
				if ((action4 = <>9__3) == null)
				{
					action4 = (<>9__3 = delegate(ExceptionBlock block)
					{
						FileLog.LogILBlockEnd(codePos, block);
					});
				}
				blocks2.Do<ExceptionBlock>(action4);
				if (flag)
				{
					codePos += codeInstruction.GetSize();
				}
			});
			FileLog.FlushBuffer();
		}

		internal static void EmitCodes(this MethodCreator _, Emitter emitter, List<CodeInstruction> codeInstructions)
		{
			Action<Label> <>9__1;
			Action<ExceptionBlock> <>9__2;
			Action<ExceptionBlock> <>9__3;
			codeInstructions.Do<CodeInstruction>(delegate(CodeInstruction codeInstruction)
			{
				IEnumerable<Label> labels = codeInstruction.labels;
				Action<Label> action;
				if ((action = <>9__1) == null)
				{
					action = (<>9__1 = delegate(Label label)
					{
						emitter.MarkLabel(label);
					});
				}
				labels.Do<Label>(action);
				IEnumerable<ExceptionBlock> blocks = codeInstruction.blocks;
				Action<ExceptionBlock> action2;
				if ((action2 = <>9__2) == null)
				{
					action2 = (<>9__2 = delegate(ExceptionBlock block)
					{
						Label? label;
						emitter.MarkBlockBefore(block, out label);
					});
				}
				blocks.Do<ExceptionBlock>(action2);
				global::System.Reflection.Emit.OpCode opcode = codeInstruction.opcode;
				object operand = codeInstruction.operand;
				global::System.Reflection.Emit.OperandType operandType = opcode.OperandType;
				if (operandType != global::System.Reflection.Emit.OperandType.InlineNone)
				{
					if (operandType != global::System.Reflection.Emit.OperandType.InlineSig)
					{
						if (operand == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
							defaultInterpolatedStringHandler.AppendLiteral("Wrong null argument: ");
							defaultInterpolatedStringHandler.AppendFormatted<CodeInstruction>(codeInstruction);
							throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						emitter.DynEmit(opcode, operand);
					}
					else
					{
						if (operand == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
							defaultInterpolatedStringHandler.AppendLiteral("Wrong null argument: ");
							defaultInterpolatedStringHandler.AppendFormatted<CodeInstruction>(codeInstruction);
							throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						if (!(operand is ICallSiteGenerator))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 2);
							defaultInterpolatedStringHandler.AppendLiteral("Wrong Emit argument type ");
							defaultInterpolatedStringHandler.AppendFormatted<Type>(operand.GetType());
							defaultInterpolatedStringHandler.AppendLiteral(" in ");
							defaultInterpolatedStringHandler.AppendFormatted<CodeInstruction>(codeInstruction);
							throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						emitter.Emit(opcode, (ICallSiteGenerator)operand);
					}
				}
				else if (codeInstruction.IsAnnotation() == null)
				{
					emitter.Emit(opcode);
				}
				IEnumerable<ExceptionBlock> blocks2 = codeInstruction.blocks;
				Action<ExceptionBlock> action3;
				if ((action3 = <>9__3) == null)
				{
					action3 = (<>9__3 = delegate(ExceptionBlock block)
					{
						emitter.MarkBlockAfter(block);
					});
				}
				blocks2.Do<ExceptionBlock>(action3);
			});
		}

		private static List<CodeInstruction> InitializeOutParameter(int argIndex, Type type)
		{
			List<CodeInstruction> list = new List<CodeInstruction>();
			if (type.IsByRef)
			{
				type = type.GetElementType();
			}
			list.Add(Code.Ldarg[argIndex, null]);
			if (AccessTools.IsStruct(type))
			{
				list.Add(Code.Initobj[type, null]);
				return list;
			}
			if (!AccessTools.IsValue(type))
			{
				list.Add(Code.Ldnull);
				list.Add(Code.Stind_Ref);
				return list;
			}
			if (type == typeof(float))
			{
				list.Add(Code.Ldc_R4[0f, null]);
				list.Add(Code.Stind_R4);
				return list;
			}
			if (type == typeof(double))
			{
				list.Add(Code.Ldc_R8[0.0, null]);
				list.Add(Code.Stind_R8);
				return list;
			}
			if (type == typeof(long))
			{
				list.Add(Code.Ldc_I8[0L, null]);
				list.Add(Code.Stind_I8);
				return list;
			}
			list.Add(Code.Ldc_I4[0, null]);
			list.Add(Code.Stind_I4);
			return list;
		}

		private static CodeInstruction LoadIndOpCodeFor(Type type)
		{
			if (MethodCreatorTools.PrimitivesWithObjectTypeCode.Contains(type))
			{
				return Code.Ldind_I;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Empty:
			case TypeCode.Object:
			case TypeCode.DBNull:
			case TypeCode.String:
				return Code.Ldind_Ref;
			case TypeCode.Boolean:
			case TypeCode.SByte:
			case TypeCode.Byte:
				return Code.Ldind_I1;
			case TypeCode.Char:
			case TypeCode.Int16:
			case TypeCode.UInt16:
				return Code.Ldind_I2;
			case TypeCode.Int32:
			case TypeCode.UInt32:
				return Code.Ldind_I4;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return Code.Ldind_I8;
			case TypeCode.Single:
				return Code.Ldind_R4;
			case TypeCode.Double:
				return Code.Ldind_R8;
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				throw new NotSupportedException();
			}
			return Code.Ldind_Ref;
		}

		private static bool EmitOriginalBaseMethod(MethodBase original, List<CodeInstruction> codes)
		{
			MethodInfo methodInfo = original as MethodInfo;
			if (methodInfo != null)
			{
				codes.Add(Code.Ldtoken[methodInfo, null]);
			}
			else
			{
				ConstructorInfo constructorInfo = original as ConstructorInfo;
				if (constructorInfo == null)
				{
					return false;
				}
				codes.Add(Code.Ldtoken[constructorInfo, null]);
			}
			Type reflectedType = original.ReflectedType;
			if (reflectedType.IsGenericType)
			{
				codes.Add(Code.Ldtoken[reflectedType, null]);
			}
			codes.Add(Code.Call[reflectedType.IsGenericType ? MethodCreatorTools.m_GetMethodFromHandle2 : MethodCreatorTools.m_GetMethodFromHandle1, null]);
			return true;
		}

		private static CodeInstruction StoreIndOpCodeFor(Type type)
		{
			if (MethodCreatorTools.PrimitivesWithObjectTypeCode.Contains(type))
			{
				return Code.Stind_I;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Empty:
			case TypeCode.Object:
			case TypeCode.DBNull:
			case TypeCode.String:
				return Code.Stind_Ref;
			case TypeCode.Boolean:
			case TypeCode.SByte:
			case TypeCode.Byte:
				return Code.Stind_I1;
			case TypeCode.Char:
			case TypeCode.Int16:
			case TypeCode.UInt16:
				return Code.Stind_I2;
			case TypeCode.Int32:
			case TypeCode.UInt32:
				return Code.Stind_I4;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return Code.Stind_I8;
			case TypeCode.Single:
				return Code.Stind_R4;
			case TypeCode.Double:
				return Code.Stind_R8;
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				throw new NotSupportedException();
			}
			return Code.Stind_Ref;
		}

		internal const string PARAM_INDEX_PREFIX = "__";

		private const string INSTANCE_FIELD_PREFIX = "___";

		private static readonly Dictionary<global::System.Reflection.Emit.OpCode, global::System.Reflection.Emit.OpCode> shortJumps = new Dictionary<global::System.Reflection.Emit.OpCode, global::System.Reflection.Emit.OpCode>
		{
			{
				global::System.Reflection.Emit.OpCodes.Leave_S,
				global::System.Reflection.Emit.OpCodes.Leave
			},
			{
				global::System.Reflection.Emit.OpCodes.Brfalse_S,
				global::System.Reflection.Emit.OpCodes.Brfalse
			},
			{
				global::System.Reflection.Emit.OpCodes.Brtrue_S,
				global::System.Reflection.Emit.OpCodes.Brtrue
			},
			{
				global::System.Reflection.Emit.OpCodes.Beq_S,
				global::System.Reflection.Emit.OpCodes.Beq
			},
			{
				global::System.Reflection.Emit.OpCodes.Bge_S,
				global::System.Reflection.Emit.OpCodes.Bge
			},
			{
				global::System.Reflection.Emit.OpCodes.Bgt_S,
				global::System.Reflection.Emit.OpCodes.Bgt
			},
			{
				global::System.Reflection.Emit.OpCodes.Ble_S,
				global::System.Reflection.Emit.OpCodes.Ble
			},
			{
				global::System.Reflection.Emit.OpCodes.Blt_S,
				global::System.Reflection.Emit.OpCodes.Blt
			},
			{
				global::System.Reflection.Emit.OpCodes.Bne_Un_S,
				global::System.Reflection.Emit.OpCodes.Bne_Un
			},
			{
				global::System.Reflection.Emit.OpCodes.Bge_Un_S,
				global::System.Reflection.Emit.OpCodes.Bge_Un
			},
			{
				global::System.Reflection.Emit.OpCodes.Bgt_Un_S,
				global::System.Reflection.Emit.OpCodes.Bgt_Un
			},
			{
				global::System.Reflection.Emit.OpCodes.Ble_Un_S,
				global::System.Reflection.Emit.OpCodes.Ble_Un
			},
			{
				global::System.Reflection.Emit.OpCodes.Br_S,
				global::System.Reflection.Emit.OpCodes.Br
			},
			{
				global::System.Reflection.Emit.OpCodes.Blt_Un_S,
				global::System.Reflection.Emit.OpCodes.Blt_Un
			}
		};

		private static readonly MethodInfo m_GetMethodFromHandle1 = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) });

		private static readonly MethodInfo m_GetMethodFromHandle2 = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[]
		{
			typeof(RuntimeMethodHandle),
			typeof(RuntimeTypeHandle)
		});

		private static readonly HashSet<Type> PrimitivesWithObjectTypeCode = new HashSet<Type>
		{
			typeof(IntPtr),
			typeof(UIntPtr),
			typeof(IntPtr),
			typeof(UIntPtr)
		};

		[CompilerGenerated]
		private static class <>O
		{
			public static Func<char, bool> <0>__IsDigit;

			public static Action<VariableDefinition> <1>__LogIL;
		}
	}
}
