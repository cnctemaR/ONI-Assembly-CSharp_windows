using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony.ILCopying;

namespace Harmony
{
	public static class MethodPatcher
	{
		[UpgradeToLatestVersion(1)]
		public static DynamicMethod CreatePatchedMethod(MethodBase original, List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers)
		{
			return MethodPatcher.CreatePatchedMethod(original, "HARMONY_PATCH_1.1.1", prefixes, postfixes, transpilers);
		}

		public static DynamicMethod CreatePatchedMethod(MethodBase original, string harmonyInstanceID, List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers)
		{
			DynamicMethod dynamicMethod2;
			try
			{
				bool debug = HarmonyInstance.DEBUG;
				if (debug)
				{
					FileLog.LogBuffered(string.Concat(new object[] { "### Patch ", original.DeclaringType, ", ", original }));
				}
				int num = prefixes.Count<MethodInfo>() + postfixes.Count<MethodInfo>();
				DynamicMethod dynamicMethod = DynamicTools.CreateDynamicMethod(original, "_Patch" + num);
				bool flag = dynamicMethod == null;
				if (flag)
				{
					dynamicMethod2 = null;
				}
				else
				{
					ILGenerator il = dynamicMethod.GetILGenerator();
					AssemblyBuilder assemblyBuilder = null;
					TypeBuilder typeBuilder = null;
					bool debug_METHOD_GENERATION_BY_DLL_CREATION = MethodPatcher.DEBUG_METHOD_GENERATION_BY_DLL_CREATION;
					if (debug_METHOD_GENERATION_BY_DLL_CREATION)
					{
						il = DynamicTools.CreateSaveableMethod(original, "_Patch" + num, out assemblyBuilder, out typeBuilder);
					}
					LocalBuilder[] array = DynamicTools.DeclareLocalVariables(original, il, true);
					Dictionary<string, LocalBuilder> privateVars = new Dictionary<string, LocalBuilder>();
					LocalBuilder localBuilder = null;
					bool flag2 = num > 0;
					if (flag2)
					{
						localBuilder = DynamicTools.DeclareLocalVariable(il, AccessTools.GetReturnedType(original));
						privateVars[MethodPatcher.RESULT_VAR] = localBuilder;
					}
					prefixes.ForEach(delegate(MethodInfo prefix)
					{
						(from patchParam in prefix.GetParameters()
							where patchParam.Name == MethodPatcher.STATE_VAR
							select patchParam).Do<ParameterInfo>(delegate(ParameterInfo patchParam)
						{
							LocalBuilder localBuilder2 = DynamicTools.DeclareLocalVariable(il, patchParam.ParameterType);
							privateVars[prefix.DeclaringType.FullName] = localBuilder2;
						});
					});
					Label label = il.DefineLabel();
					bool flag3 = MethodPatcher.AddPrefixes(il, original, prefixes, privateVars, label);
					MethodCopier methodCopier = new MethodCopier(original, il, array);
					foreach (MethodInfo methodInfo in transpilers)
					{
						methodCopier.AddTranspiler(methodInfo);
					}
					List<Label> list = new List<Label>();
					List<ExceptionBlock> list2 = new List<ExceptionBlock>();
					methodCopier.Finalize(list, list2);
					foreach (Label label2 in list)
					{
						Emitter.MarkLabel(il, label2);
					}
					foreach (ExceptionBlock exceptionBlock in list2)
					{
						Emitter.MarkBlockAfter(il, exceptionBlock);
					}
					bool flag4 = localBuilder != null;
					if (flag4)
					{
						Emitter.Emit(il, OpCodes.Stloc, localBuilder);
					}
					bool flag5 = flag3;
					if (flag5)
					{
						Emitter.MarkLabel(il, label);
					}
					MethodPatcher.AddPostfixes(il, original, postfixes, privateVars, false);
					bool flag6 = localBuilder != null;
					if (flag6)
					{
						Emitter.Emit(il, OpCodes.Ldloc, localBuilder);
					}
					MethodPatcher.AddPostfixes(il, original, postfixes, privateVars, true);
					Emitter.Emit(il, OpCodes.Ret);
					bool debug2 = HarmonyInstance.DEBUG;
					if (debug2)
					{
						FileLog.LogBuffered("DONE");
						FileLog.LogBuffered("");
						FileLog.FlushBuffer();
					}
					bool debug_METHOD_GENERATION_BY_DLL_CREATION2 = MethodPatcher.DEBUG_METHOD_GENERATION_BY_DLL_CREATION;
					if (debug_METHOD_GENERATION_BY_DLL_CREATION2)
					{
						DynamicTools.SaveMethod(assemblyBuilder, typeBuilder);
						dynamicMethod2 = null;
					}
					else
					{
						DynamicTools.PrepareDynamicMethod(dynamicMethod);
						dynamicMethod2 = dynamicMethod;
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception from HarmonyInstance \"" + harmonyInstanceID + "\"", ex);
			}
			finally
			{
				bool debug3 = HarmonyInstance.DEBUG;
				if (debug3)
				{
					FileLog.FlushBuffer();
				}
			}
			return dynamicMethod2;
		}

		private static OpCode LoadIndOpCodeFor(Type type)
		{
			bool isEnum = type.IsEnum;
			OpCode opCode;
			if (isEnum)
			{
				opCode = OpCodes.Ldind_I4;
			}
			else
			{
				bool flag = type == typeof(float);
				if (flag)
				{
					opCode = OpCodes.Ldind_R4;
				}
				else
				{
					bool flag2 = type == typeof(double);
					if (flag2)
					{
						opCode = OpCodes.Ldind_R8;
					}
					else
					{
						bool flag3 = type == typeof(byte);
						if (flag3)
						{
							opCode = OpCodes.Ldind_U1;
						}
						else
						{
							bool flag4 = type == typeof(ushort);
							if (flag4)
							{
								opCode = OpCodes.Ldind_U2;
							}
							else
							{
								bool flag5 = type == typeof(uint);
								if (flag5)
								{
									opCode = OpCodes.Ldind_U4;
								}
								else
								{
									bool flag6 = type == typeof(ulong);
									if (flag6)
									{
										opCode = OpCodes.Ldind_I8;
									}
									else
									{
										bool flag7 = type == typeof(sbyte);
										if (flag7)
										{
											opCode = OpCodes.Ldind_I1;
										}
										else
										{
											bool flag8 = type == typeof(short);
											if (flag8)
											{
												opCode = OpCodes.Ldind_I2;
											}
											else
											{
												bool flag9 = type == typeof(int);
												if (flag9)
												{
													opCode = OpCodes.Ldind_I4;
												}
												else
												{
													bool flag10 = type == typeof(long);
													if (flag10)
													{
														opCode = OpCodes.Ldind_I8;
													}
													else
													{
														opCode = OpCodes.Ldind_Ref;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return opCode;
		}

		private static HarmonyArgument GetArgumentAttribute(this ParameterInfo parameter)
		{
			return parameter.GetCustomAttributes(false).FirstOrDefault<object>((object attr) => attr is HarmonyArgument) as HarmonyArgument;
		}

		private static HarmonyArgument[] GetArgumentAttributes(this MethodInfo method)
		{
			return (from attr in method.GetCustomAttributes(false)
				where attr is HarmonyArgument
				select attr).Cast<HarmonyArgument>().ToArray<HarmonyArgument>();
		}

		private static HarmonyArgument[] GetArgumentAttributes(this Type type)
		{
			return (from attr in type.GetCustomAttributes(false)
				where attr is HarmonyArgument
				select attr).Cast<HarmonyArgument>().ToArray<HarmonyArgument>();
		}

		private static string GetOriginalArgumentName(this ParameterInfo parameter, string[] originalParameterNames)
		{
			HarmonyArgument argumentAttribute = parameter.GetArgumentAttribute();
			bool flag = argumentAttribute == null;
			string text;
			if (flag)
			{
				text = null;
			}
			else
			{
				bool flag2 = !string.IsNullOrEmpty(argumentAttribute.OriginalName);
				if (flag2)
				{
					text = argumentAttribute.OriginalName;
				}
				else
				{
					bool flag3 = argumentAttribute.Index >= 0 && argumentAttribute.Index < originalParameterNames.Length;
					if (flag3)
					{
						text = originalParameterNames[argumentAttribute.Index];
					}
					else
					{
						text = null;
					}
				}
			}
			return text;
		}

		private static string GetOriginalArgumentName(HarmonyArgument[] attributes, string name, string[] originalParameterNames)
		{
			bool flag = attributes.Length == 0;
			string text;
			if (flag)
			{
				text = null;
			}
			else
			{
				HarmonyArgument harmonyArgument = attributes.SingleOrDefault<HarmonyArgument>((HarmonyArgument p) => p.NewName == name);
				bool flag2 = harmonyArgument == null;
				if (flag2)
				{
					text = null;
				}
				else
				{
					bool flag3 = !string.IsNullOrEmpty(harmonyArgument.OriginalName);
					if (flag3)
					{
						text = harmonyArgument.OriginalName;
					}
					else
					{
						bool flag4 = harmonyArgument.Index >= 0 && harmonyArgument.Index < originalParameterNames.Length;
						if (flag4)
						{
							text = originalParameterNames[harmonyArgument.Index];
						}
						else
						{
							text = null;
						}
					}
				}
			}
			return text;
		}

		private static string GetOriginalArgumentName(this MethodInfo method, string[] originalParameterNames, string name)
		{
			string text = MethodPatcher.GetOriginalArgumentName(method.GetArgumentAttributes(), name, originalParameterNames);
			bool flag = text != null;
			string text2;
			if (flag)
			{
				text2 = text;
			}
			else
			{
				text = MethodPatcher.GetOriginalArgumentName(method.DeclaringType.GetArgumentAttributes(), name, originalParameterNames);
				bool flag2 = text != null;
				if (flag2)
				{
					text2 = text;
				}
				else
				{
					text2 = name;
				}
			}
			return text2;
		}

		private static int GetArgumentIndex(MethodInfo patch, string[] originalParameterNames, ParameterInfo patchParam)
		{
			string text = patchParam.GetOriginalArgumentName(originalParameterNames);
			bool flag = text != null;
			int num;
			if (flag)
			{
				num = Array.IndexOf<string>(originalParameterNames, text);
			}
			else
			{
				string name = patchParam.Name;
				text = patch.GetOriginalArgumentName(originalParameterNames, name);
				bool flag2 = text != null;
				if (flag2)
				{
					num = Array.IndexOf<string>(originalParameterNames, text);
				}
				else
				{
					num = -1;
				}
			}
			return num;
		}

		private static void EmitCallParameter(ILGenerator il, MethodBase original, MethodInfo patch, Dictionary<string, LocalBuilder> variables, bool allowFirsParamPassthrough)
		{
			bool flag = !original.IsStatic;
			ParameterInfo[] parameters = original.GetParameters();
			string[] array = parameters.Select<ParameterInfo, string>((ParameterInfo p) => p.Name).ToArray<string>();
			List<ParameterInfo> list = patch.GetParameters().ToList<ParameterInfo>();
			bool flag2 = allowFirsParamPassthrough && patch.ReturnType != typeof(void) && list.Count > 0 && list[0].ParameterType == patch.ReturnType;
			if (flag2)
			{
				list.RemoveRange(0, 1);
			}
			foreach (ParameterInfo parameterInfo in list)
			{
				bool flag3 = parameterInfo.Name == MethodPatcher.ORIGINAL_METHOD_PARAM;
				if (flag3)
				{
					ConstructorInfo constructorInfo = original as ConstructorInfo;
					bool flag4 = constructorInfo != null;
					if (flag4)
					{
						Emitter.Emit(il, OpCodes.Ldtoken, constructorInfo);
						Emitter.Emit(il, OpCodes.Call, MethodPatcher.getMethodMethod);
					}
					else
					{
						MethodInfo methodInfo = original as MethodInfo;
						bool flag5 = methodInfo != null;
						if (flag5)
						{
							Emitter.Emit(il, OpCodes.Ldtoken, methodInfo);
							Emitter.Emit(il, OpCodes.Call, MethodPatcher.getMethodMethod);
						}
						else
						{
							Emitter.Emit(il, OpCodes.Ldnull);
						}
					}
				}
				else
				{
					bool flag6 = parameterInfo.Name == MethodPatcher.INSTANCE_PARAM;
					if (flag6)
					{
						bool isStatic = original.IsStatic;
						if (isStatic)
						{
							Emitter.Emit(il, OpCodes.Ldnull);
						}
						else
						{
							bool isByRef = parameterInfo.ParameterType.IsByRef;
							if (isByRef)
							{
								Emitter.Emit(il, OpCodes.Ldarga, 0);
							}
							else
							{
								Emitter.Emit(il, OpCodes.Ldarg_0);
							}
						}
					}
					else
					{
						bool flag7 = parameterInfo.Name.StartsWith(MethodPatcher.INSTANCE_FIELD_PREFIX);
						if (flag7)
						{
							string text = parameterInfo.Name.Substring(MethodPatcher.INSTANCE_FIELD_PREFIX.Length);
							bool flag8 = text.All<char>(new Func<char, bool>(char.IsDigit));
							FieldInfo fieldInfo;
							if (flag8)
							{
								fieldInfo = AccessTools.Field(original.DeclaringType, int.Parse(text));
								bool flag9 = fieldInfo == null;
								if (flag9)
								{
									throw new ArgumentException("No field found at given index in class " + original.DeclaringType.FullName, text);
								}
							}
							else
							{
								fieldInfo = AccessTools.Field(original.DeclaringType, text);
								bool flag10 = fieldInfo == null;
								if (flag10)
								{
									throw new ArgumentException("No such field defined in class " + original.DeclaringType.FullName, text);
								}
							}
							bool isStatic2 = fieldInfo.IsStatic;
							if (isStatic2)
							{
								bool isByRef2 = parameterInfo.ParameterType.IsByRef;
								if (isByRef2)
								{
									Emitter.Emit(il, OpCodes.Ldsflda, fieldInfo);
								}
								else
								{
									Emitter.Emit(il, OpCodes.Ldsfld, fieldInfo);
								}
							}
							else
							{
								bool isByRef3 = parameterInfo.ParameterType.IsByRef;
								if (isByRef3)
								{
									Emitter.Emit(il, OpCodes.Ldarg_0);
									Emitter.Emit(il, OpCodes.Ldflda, fieldInfo);
								}
								else
								{
									Emitter.Emit(il, OpCodes.Ldarg_0);
									Emitter.Emit(il, OpCodes.Ldfld, fieldInfo);
								}
							}
						}
						else
						{
							bool flag11 = parameterInfo.Name == MethodPatcher.STATE_VAR;
							if (flag11)
							{
								OpCode opCode = (parameterInfo.ParameterType.IsByRef ? OpCodes.Ldloca : OpCodes.Ldloc);
								Emitter.Emit(il, opCode, variables[patch.DeclaringType.FullName]);
							}
							else
							{
								bool flag12 = parameterInfo.Name == MethodPatcher.RESULT_VAR;
								if (flag12)
								{
									bool flag13 = AccessTools.GetReturnedType(original) == typeof(void);
									if (flag13)
									{
										throw new Exception("Cannot get result from void method " + original.FullDescription());
									}
									OpCode opCode2 = (parameterInfo.ParameterType.IsByRef ? OpCodes.Ldloca : OpCodes.Ldloc);
									Emitter.Emit(il, opCode2, variables[MethodPatcher.RESULT_VAR]);
								}
								else
								{
									bool flag14 = parameterInfo.Name.StartsWith(MethodPatcher.PARAM_INDEX_PREFIX);
									int argumentIndex;
									if (flag14)
									{
										string text2 = parameterInfo.Name.Substring(MethodPatcher.PARAM_INDEX_PREFIX.Length);
										bool flag15 = !int.TryParse(text2, out argumentIndex);
										if (flag15)
										{
											throw new Exception("Parameter " + parameterInfo.Name + " does not contain a valid index");
										}
										bool flag16 = argumentIndex < 0 || argumentIndex >= parameters.Length;
										if (flag16)
										{
											throw new Exception("No parameter found at index " + argumentIndex);
										}
									}
									else
									{
										argumentIndex = MethodPatcher.GetArgumentIndex(patch, array, parameterInfo);
										bool flag17 = argumentIndex == -1;
										if (flag17)
										{
											throw new Exception("Parameter \"" + parameterInfo.Name + "\" not found in method " + original.FullDescription());
										}
									}
									bool flag18 = !parameters[argumentIndex].IsOut && !parameters[argumentIndex].ParameterType.IsByRef;
									bool flag19 = !parameterInfo.IsOut && !parameterInfo.ParameterType.IsByRef;
									int num = argumentIndex + (flag ? 1 : 0);
									bool flag20 = flag18 == flag19;
									if (flag20)
									{
										Emitter.Emit(il, OpCodes.Ldarg, num);
									}
									else
									{
										bool flag21 = flag18 && !flag19;
										if (flag21)
										{
											Emitter.Emit(il, OpCodes.Ldarga, num);
										}
										else
										{
											Emitter.Emit(il, OpCodes.Ldarg, num);
											Emitter.Emit(il, MethodPatcher.LoadIndOpCodeFor(parameters[argumentIndex].ParameterType));
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private static bool AddPrefixes(ILGenerator il, MethodBase original, List<MethodInfo> prefixes, Dictionary<string, LocalBuilder> variables, Label label)
		{
			bool canHaveJump = false;
			prefixes.ForEach(delegate(MethodInfo fix)
			{
				MethodPatcher.EmitCallParameter(il, original, fix, variables, false);
				Emitter.Emit(il, OpCodes.Call, fix);
				bool flag = fix.ReturnType != typeof(void);
				if (flag)
				{
					bool flag2 = fix.ReturnType != typeof(bool);
					if (flag2)
					{
						throw new Exception(string.Concat(new object[] { "Prefix patch ", fix, " has not \"bool\" or \"void\" return type: ", fix.ReturnType }));
					}
					Emitter.Emit(il, OpCodes.Brfalse, label);
					canHaveJump = true;
				}
			});
			return canHaveJump;
		}

		private static void AddPostfixes(ILGenerator il, MethodBase original, List<MethodInfo> postfixes, Dictionary<string, LocalBuilder> variables, bool passthroughPatches)
		{
			postfixes.Where<MethodInfo>((MethodInfo fix) => passthroughPatches == (fix.ReturnType != typeof(void))).Do<MethodInfo>(delegate(MethodInfo fix)
			{
				MethodPatcher.EmitCallParameter(il, original, fix, variables, true);
				Emitter.Emit(il, OpCodes.Call, fix);
				bool flag = fix.ReturnType != typeof(void);
				if (flag)
				{
					ParameterInfo parameterInfo = fix.GetParameters().FirstOrDefault<ParameterInfo>();
					bool flag2 = parameterInfo != null && fix.ReturnType == parameterInfo.ParameterType;
					bool flag3 = !flag2;
					if (flag3)
					{
						bool flag4 = parameterInfo != null;
						if (flag4)
						{
							throw new Exception("Return type of postfix patch " + fix + " does match type of its first parameter");
						}
						throw new Exception("Postfix patch " + fix + " must have a \"void\" return type");
					}
				}
			});
		}

		public static string INSTANCE_PARAM = "__instance";

		public static string ORIGINAL_METHOD_PARAM = "__originalMethod";

		public static string RESULT_VAR = "__result";

		public static string STATE_VAR = "__state";

		public static string PARAM_INDEX_PREFIX = "__";

		public static string INSTANCE_FIELD_PREFIX = "___";

		private static readonly bool DEBUG_METHOD_GENERATION_BY_DLL_CREATION = false;

		private static MethodInfo getMethodMethod = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) });
	}
}
