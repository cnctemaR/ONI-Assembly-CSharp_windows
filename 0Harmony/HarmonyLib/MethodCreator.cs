using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	internal class MethodCreator
	{
		internal MethodCreator(MethodCreatorConfig config)
		{
			if (config.original == null)
			{
				throw new ArgumentNullException("config.original");
			}
			this.config = config;
			if (config.debug)
			{
				FileLog.LogBuffered("### Patch: " + config.original.FullDescription());
				FileLog.FlushBuffer();
			}
			if (!config.Prepare())
			{
				throw new Exception("Could not create replacement method");
			}
		}

		internal ValueTuple<MethodInfo, Dictionary<int, CodeInstruction>> CreateReplacement()
		{
			this.config.originalVariables = this.DeclareOriginalLocalVariables(this.config.MethodBase);
			this.config.localVariables = new VariableState();
			if (this.config.Fixes.Any<MethodInfo>() && this.config.returnType != typeof(void))
			{
				this.config.resultVariable = this.config.DeclareLocal(this.config.returnType, false);
				this.config.AddLocal(InjectionType.Result, this.config.resultVariable);
				this.config.AddCodes(this.GenerateVariableInit(this.config.resultVariable, true));
			}
			if (this.config.AnyFixHas(InjectionType.ResultRef) && this.config.returnType.IsByRef)
			{
				Type type = typeof(RefResult<>).MakeGenericType(new Type[] { this.config.returnType.GetElementType() });
				LocalBuilder localBuilder = this.config.DeclareLocal(type, false);
				this.config.AddLocal(InjectionType.ResultRef, localBuilder);
				this.config.AddCodes(new <>z__ReadOnlyArray<CodeInstruction>(new CodeInstruction[]
				{
					Code.Ldnull,
					Code.Stloc[localBuilder, null]
				}));
			}
			if (this.config.AnyFixHas(InjectionType.ArgsArray))
			{
				LocalBuilder localBuilder2 = this.config.DeclareLocal(typeof(object[]), false);
				this.config.AddLocal(InjectionType.ArgsArray, localBuilder2);
				this.config.AddCodes(this.PrepareArgumentArray());
				this.config.AddCode(Code.Stloc[localBuilder2, null]);
			}
			this.config.skipOriginalLabel = null;
			bool flag = this.config.prefixes.Any<MethodInfo>(new Func<MethodInfo, bool>(base.AffectsOriginal));
			bool flag2 = this.config.AnyFixHas(InjectionType.RunOriginal);
			if (flag || flag2)
			{
				this.config.runOriginalVariable = this.config.DeclareLocal(typeof(bool), false);
				this.config.AddCodes(new <>z__ReadOnlyArray<CodeInstruction>(new CodeInstruction[]
				{
					Code.Ldc_I4_1,
					Code.Stloc[this.config.runOriginalVariable, null]
				}));
				if (flag)
				{
					this.config.skipOriginalLabel = new Label?(this.config.DefineLabel());
				}
			}
			this.config.WithFixes(delegate(MethodInfo fix)
			{
				Type declaringType = fix.DeclaringType;
				if (declaringType == null)
				{
					return;
				}
				string assemblyQualifiedName = declaringType.AssemblyQualifiedName;
				LocalBuilder localBuilder3;
				this.config.localVariables.TryGetValue(assemblyQualifiedName, out localBuilder3);
				foreach (InjectedParameter injectedParameter in this.config.InjectionsFor(fix, InjectionType.State))
				{
					Type parameterType = injectedParameter.parameterInfo.ParameterType;
					Type type2 = (parameterType.IsByRef ? parameterType.GetElementType() : parameterType);
					if (localBuilder3 != null)
					{
						if (!type2.IsAssignableFrom(localBuilder3.LocalType))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(97, 4);
							defaultInterpolatedStringHandler.AppendLiteral("__state type mismatch in patch \"");
							defaultInterpolatedStringHandler.AppendFormatted(fix.DeclaringType.FullName);
							defaultInterpolatedStringHandler.AppendLiteral(".");
							defaultInterpolatedStringHandler.AppendFormatted(fix.Name);
							defaultInterpolatedStringHandler.AppendLiteral("\": ");
							defaultInterpolatedStringHandler.AppendLiteral("previous __state was declared as \"");
							defaultInterpolatedStringHandler.AppendFormatted(localBuilder3.LocalType.FullName);
							defaultInterpolatedStringHandler.AppendLiteral("\" but this patch expects \"");
							defaultInterpolatedStringHandler.AppendFormatted(type2.FullName);
							defaultInterpolatedStringHandler.AppendLiteral("\"");
							string text = defaultInterpolatedStringHandler.ToStringAndClear();
							throw new HarmonyException(text);
						}
					}
					else
					{
						LocalBuilder localBuilder4 = this.config.DeclareLocal(type2, false);
						this.config.AddLocal(assemblyQualifiedName, localBuilder4);
						this.config.AddCodes(this.GenerateVariableInit(localBuilder4, false));
					}
				}
			});
			this.config.finalizedVariable = null;
			if (this.config.finalizers.Count > 0)
			{
				this.config.finalizedVariable = this.config.DeclareLocal(typeof(bool), false);
				this.config.AddCodes(this.GenerateVariableInit(this.config.finalizedVariable, false));
				this.config.exceptionVariable = this.config.DeclareLocal(typeof(Exception), false);
				this.config.AddLocal(InjectionType.Exception, this.config.exceptionVariable);
				this.config.AddCodes(this.GenerateVariableInit(this.config.exceptionVariable, false));
				this.config.AddCode(this.MarkBlock(ExceptionBlockType.BeginExceptionBlock));
			}
			this.AddPrefixes();
			if (this.config.skipOriginalLabel != null)
			{
				this.config.AddCodes(new <>z__ReadOnlyArray<CodeInstruction>(new CodeInstruction[]
				{
					Code.Ldloc[this.config.runOriginalVariable, null],
					Code.Brfalse[this.config.skipOriginalLabel.Value, null]
				}));
			}
			MethodCopier methodCopier = new MethodCopier(this.config);
			foreach (MethodInfo methodInfo in this.config.transpilers)
			{
				methodCopier.AddTranspiler(methodInfo);
			}
			methodCopier.AddTranspiler(PatchTools.m_GetExecutingAssemblyReplacementTranspiler);
			List<Label> list = new List<Label>();
			bool flag3;
			bool flag4;
			List<CodeInstruction> list2 = methodCopier.Finalize(true, out flag3, out flag4, list);
			list2 = this.AddInfixes(list2).ToList<CodeInstruction>();
			this.config.AddCode(Code.Nop["start original", null]);
			this.config.AddCodes(this.CleanupCodes(list2, list));
			this.config.AddCode(Code.Nop["end original", null]);
			if (list.Count > 0)
			{
				this.config.AddCode(Code.Nop.WithLabels(list));
			}
			if (this.config.resultVariable != null && flag3)
			{
				this.config.AddCode(Code.Stloc[this.config.resultVariable, null]);
			}
			if (this.config.skipOriginalLabel != null)
			{
				this.config.AddCode(Code.Nop.WithLabels(new Label[] { this.config.skipOriginalLabel.Value }));
			}
			this.AddPostfixes(false);
			if (this.config.resultVariable != null && (flag3 || (flag4 && this.config.skipOriginalLabel != null)))
			{
				this.config.AddCode(Code.Ldloc[this.config.resultVariable, null]);
			}
			bool flag5 = this.AddPostfixes(true);
			if (this.config.finalizers.Count > 0)
			{
				LocalBuilder local = this.config.GetLocal(InjectionType.Exception);
				if (flag5)
				{
					this.config.AddCode(Code.Stloc[this.config.resultVariable, null]);
					this.config.AddCode(Code.Ldloc[this.config.resultVariable, null]);
				}
				this.AddFinalizers(false);
				this.config.AddCode(Code.Ldc_I4_1);
				this.config.AddCode(Code.Stloc[this.config.finalizedVariable, null]);
				Label label = this.config.DefineLabel();
				this.config.AddCode(Code.Ldloc[local, null]);
				this.config.AddCode(Code.Brfalse[label, null]);
				this.config.AddCode(Code.Ldloc[local, null]);
				this.config.AddCode(Code.Throw);
				this.config.AddCode(Code.Nop.WithLabels(new Label[] { label }));
				this.config.AddCode(this.MarkBlock(ExceptionBlockType.BeginCatchBlock));
				this.config.AddCode(Code.Stloc[local, null]);
				this.config.AddCode(Code.Ldloc[this.config.finalizedVariable, null]);
				Label label2 = this.config.DefineLabel();
				this.config.AddCode(Code.Brtrue[label2, null]);
				bool flag6 = this.AddFinalizers(true);
				this.config.AddCode(Code.Nop.WithLabels(new Label[] { label2 }));
				Label label3 = this.config.DefineLabel();
				this.config.AddCode(Code.Ldloc[local, null]);
				this.config.AddCode(Code.Brfalse[label3, null]);
				if (flag6)
				{
					this.config.AddCode(Code.Rethrow);
				}
				else
				{
					this.config.AddCode(Code.Ldloc[local, null]);
					this.config.AddCode(Code.Throw);
				}
				this.config.AddCode(Code.Nop.WithLabels(new Label[] { label3 }));
				this.config.AddCode(this.MarkBlock(ExceptionBlockType.EndExceptionBlock));
				if (this.config.resultVariable != null)
				{
					this.config.AddCode(Code.Ldloc[this.config.resultVariable, null]);
				}
			}
			if (flag4)
			{
				Label? skipOriginalLabel = this.config.skipOriginalLabel;
				if (skipOriginalLabel == null && this.config.finalizers.Count <= 0 && this.config.postfixes.Count <= 0)
				{
					goto IL_0860;
				}
			}
			this.config.AddCode(Code.Ret);
			IL_0860:
			this.config.instructions = FaultBlockRewriter.Rewrite(this.config.instructions, this.config.il);
			if (this.config.debug)
			{
				Emitter emitter = new Emitter(this.config.il);
				this.LogCodes(emitter, this.config.instructions);
			}
			Emitter emitter2 = new Emitter(this.config.il);
			this.EmitCodes(emitter2, this.config.instructions);
			MethodInfo methodInfo2 = this.config.patch.Generate();
			if (this.config.debug)
			{
				FileLog.LogBuffered("DONE");
				FileLog.LogBuffered("");
				FileLog.FlushBuffer();
			}
			return new ValueTuple<MethodInfo, Dictionary<int, CodeInstruction>>(methodInfo2, emitter2.GetInstructions());
		}

		internal void AddPrefixes()
		{
			foreach (MethodInfo methodInfo in this.config.prefixes)
			{
				Label? label = (this.AffectsOriginal(methodInfo) ? new Label?(this.config.DefineLabel()) : null);
				if (label != null)
				{
					this.config.AddCodes(new <>z__ReadOnlyArray<CodeInstruction>(new CodeInstruction[]
					{
						Code.Ldloc[this.config.runOriginalVariable, null],
						Code.Brfalse[label.Value, null]
					}));
				}
				List<KeyValuePair<LocalBuilder, Type>> list = new List<KeyValuePair<LocalBuilder, Type>>();
				LocalBuilder localBuilder;
				LocalBuilder localBuilder2;
				bool flag;
				this.config.AddCodes(this.EmitCallParameter(methodInfo, false, out localBuilder, out localBuilder2, out flag, list));
				this.config.AddCode(Code.Call[methodInfo, null]);
				if (MethodPatcherTools.OriginalParameters(methodInfo).Any<ValueTuple<ParameterInfo, string>>(([TupleElementNames(new string[] { "info", "realName" })] ValueTuple<ParameterInfo, string> pair) => pair.Item2 == "__args"))
				{
					this.config.AddCodes(this.RestoreArgumentArray());
				}
				if (localBuilder != null)
				{
					this.config.AddCode(Code.Ldarg_0);
					this.config.AddCode(Code.Ldloc[localBuilder, null]);
					this.config.AddCode(Code.Unbox_Any[this.config.original.DeclaringType, null]);
					this.config.AddCode(Code.Stobj[this.config.original.DeclaringType, null]);
				}
				if (flag)
				{
					Label label2 = this.config.DefineLabel();
					this.config.AddCode(Code.Ldloc[this.config.GetLocal(InjectionType.ResultRef), null]);
					this.config.AddCode(Code.Brfalse_S[label2, null]);
					this.config.AddCode(Code.Ldloc[this.config.GetLocal(InjectionType.ResultRef), null]);
					this.config.AddCode(Code.Callvirt[AccessTools.Method(this.config.GetLocal(InjectionType.ResultRef).LocalType, "Invoke", null, null), null]);
					this.config.AddCode(Code.Stloc[this.config.GetLocal(InjectionType.Result), null]);
					this.config.AddCode(Code.Ldnull);
					this.config.AddCode(Code.Stloc[this.config.GetLocal(InjectionType.ResultRef), null]);
					this.config.AddCode(Code.Nop.WithLabels(new Label[] { label2 }));
				}
				else if (localBuilder2 != null)
				{
					this.config.AddCode(Code.Ldloc[localBuilder2, null]);
					this.config.AddCode(Code.Unbox_Any[AccessTools.GetReturnedType(this.config.original), null]);
					this.config.AddCode(Code.Stloc[this.config.GetLocal(InjectionType.Result), null]);
				}
				list.Do<KeyValuePair<LocalBuilder, Type>>(delegate(KeyValuePair<LocalBuilder, Type> tmpBoxVar)
				{
					this.config.AddCode(new CodeInstruction(this.config.OriginalIsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1, null));
					this.config.AddCode(Code.Ldloc[tmpBoxVar.Key, null]);
					this.config.AddCode(Code.Unbox_Any[tmpBoxVar.Value, null]);
					this.config.AddCode(Code.Stobj[tmpBoxVar.Value, null]);
				});
				Type returnType = methodInfo.ReturnType;
				if (returnType != typeof(void))
				{
					if (returnType != typeof(bool))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Prefix patch ");
						defaultInterpolatedStringHandler.AppendFormatted<MethodInfo>(methodInfo);
						defaultInterpolatedStringHandler.AppendLiteral(" has not \"bool\" or \"void\" return type: ");
						defaultInterpolatedStringHandler.AppendFormatted<Type>(methodInfo.ReturnType);
						throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					this.config.AddCode(Code.Stloc[this.config.runOriginalVariable, null]);
				}
				if (label != null)
				{
					this.config.AddCode(Code.Nop.WithLabels(new Label[] { label.Value }));
				}
			}
		}

		internal bool AddPostfixes(bool passthroughPatches)
		{
			bool flag = false;
			MethodBase original = this.config.original;
			bool originalIsStatic = original.IsStatic;
			IEnumerable<MethodInfo> postfixes = this.config.postfixes;
			Func<MethodInfo, bool> <>9__0;
			Func<MethodInfo, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (MethodInfo fix) => passthroughPatches == (fix.ReturnType != typeof(void)));
			}
			Action<KeyValuePair<LocalBuilder, Type>> <>9__2;
			foreach (MethodInfo methodInfo in postfixes.Where<MethodInfo>(func))
			{
				List<KeyValuePair<LocalBuilder, Type>> list = new List<KeyValuePair<LocalBuilder, Type>>();
				LocalBuilder localBuilder;
				LocalBuilder localBuilder2;
				bool flag2;
				this.config.AddCodes(this.EmitCallParameter(methodInfo, true, out localBuilder, out localBuilder2, out flag2, list));
				this.config.AddCode(Code.Call[methodInfo, null]);
				if (MethodPatcherTools.OriginalParameters(methodInfo).Any<ValueTuple<ParameterInfo, string>>(([TupleElementNames(new string[] { "info", "realName" })] ValueTuple<ParameterInfo, string> pair) => pair.Item2 == "__args"))
				{
					this.config.AddCodes(this.RestoreArgumentArray());
				}
				if (localBuilder != null)
				{
					this.config.AddCode(Code.Ldarg_0);
					this.config.AddCode(Code.Ldloc[localBuilder, null]);
					this.config.AddCode(Code.Unbox_Any[original.DeclaringType, null]);
					this.config.AddCode(Code.Stobj[original.DeclaringType, null]);
				}
				if (flag2)
				{
					Label label = this.config.DefineLabel();
					this.config.AddCode(Code.Ldloc[this.config.GetLocal(InjectionType.ResultRef), null]);
					this.config.AddCode(Code.Brfalse_S[label, null]);
					this.config.AddCode(Code.Ldloc[this.config.GetLocal(InjectionType.ResultRef), null]);
					this.config.AddCode(Code.Callvirt[AccessTools.Method(this.config.GetLocal(InjectionType.ResultRef).LocalType, "Invoke", null, null), null]);
					this.config.AddCode(Code.Stloc[this.config.GetLocal(InjectionType.Result), null]);
					this.config.AddCode(Code.Ldnull);
					this.config.AddCode(Code.Stloc[this.config.GetLocal(InjectionType.ResultRef), null]);
					this.config.AddCode(Code.Nop.WithLabels(new Label[] { label }));
				}
				else if (localBuilder2 != null)
				{
					this.config.AddCode(Code.Ldloc[localBuilder2, null]);
					this.config.AddCode(Code.Unbox_Any[AccessTools.GetReturnedType(original), null]);
					this.config.AddCode(Code.Stloc[this.config.GetLocal(InjectionType.Result), null]);
				}
				IEnumerable<KeyValuePair<LocalBuilder, Type>> enumerable = list;
				Action<KeyValuePair<LocalBuilder, Type>> action;
				if ((action = <>9__2) == null)
				{
					action = (<>9__2 = delegate(KeyValuePair<LocalBuilder, Type> tmpBoxVar)
					{
						this.config.AddCode(new CodeInstruction(originalIsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1, null));
						this.config.AddCode(Code.Ldloc[tmpBoxVar.Key, null]);
						this.config.AddCode(Code.Unbox_Any[tmpBoxVar.Value, null]);
						this.config.AddCode(Code.Stobj[tmpBoxVar.Value, null]);
					});
				}
				enumerable.Do<KeyValuePair<LocalBuilder, Type>>(action);
				if (methodInfo.ReturnType != typeof(void))
				{
					ParameterInfo parameterInfo = methodInfo.GetParameters().FirstOrDefault<ParameterInfo>();
					bool flag3 = parameterInfo != null && methodInfo.ReturnType == parameterInfo.ParameterType;
					if (flag3)
					{
						flag = true;
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
						if (parameterInfo != null)
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(79, 1);
							defaultInterpolatedStringHandler.AppendLiteral("Return type of pass through postfix ");
							defaultInterpolatedStringHandler.AppendFormatted<MethodInfo>(methodInfo);
							defaultInterpolatedStringHandler.AppendLiteral(" does not match type of its first parameter");
							throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Postfix patch ");
						defaultInterpolatedStringHandler.AppendFormatted<MethodInfo>(methodInfo);
						defaultInterpolatedStringHandler.AppendLiteral(" must have a \"void\" return type");
						throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
			}
			return flag;
		}

		internal bool AddFinalizers(bool catchExceptions)
		{
			bool rethrowPossible = true;
			MethodBase original = this.config.original;
			bool originalIsStatic = original.IsStatic;
			Action<KeyValuePair<LocalBuilder, Type>> <>9__2;
			this.config.finalizers.Do<MethodInfo>(delegate(MethodInfo fix)
			{
				if (catchExceptions)
				{
					this.config.AddCode(this.MarkBlock(ExceptionBlockType.BeginExceptionBlock));
				}
				List<KeyValuePair<LocalBuilder, Type>> list = new List<KeyValuePair<LocalBuilder, Type>>();
				LocalBuilder localBuilder;
				LocalBuilder localBuilder2;
				bool flag;
				this.config.AddCodes(this.EmitCallParameter(fix, false, out localBuilder, out localBuilder2, out flag, list));
				this.config.AddCode(Code.Call[fix, null]);
				if (MethodPatcherTools.OriginalParameters(fix).Any<ValueTuple<ParameterInfo, string>>(([TupleElementNames(new string[] { "info", "realName" })] ValueTuple<ParameterInfo, string> pair) => pair.Item2 == "__args"))
				{
					this.config.AddCodes(this.RestoreArgumentArray());
				}
				if (localBuilder != null)
				{
					this.config.AddCode(Code.Ldarg_0);
					this.config.AddCode(Code.Ldloc[localBuilder, null]);
					this.config.AddCode(Code.Unbox_Any[original.DeclaringType, null]);
					this.config.AddCode(Code.Stobj[original.DeclaringType, null]);
				}
				if (flag)
				{
					Label label = this.config.DefineLabel();
					this.config.AddCode(Code.Ldloc[this.config.GetLocal(InjectionType.ResultRef), null]);
					this.config.AddCode(Code.Brfalse_S[label, null]);
					this.config.AddCode(Code.Ldloc[this.config.GetLocal(InjectionType.ResultRef), null]);
					this.config.AddCode(Code.Callvirt[AccessTools.Method(this.config.GetLocal(InjectionType.ResultRef).LocalType, "Invoke", null, null), null]);
					this.config.AddCode(Code.Stloc[this.config.GetLocal(InjectionType.Result), null]);
					this.config.AddCode(Code.Ldnull);
					this.config.AddCode(Code.Stloc[this.config.GetLocal(InjectionType.ResultRef), null]);
					this.config.AddCode(Code.Nop.WithLabels(new Label[] { label }));
				}
				else if (localBuilder2 != null)
				{
					this.config.AddCode(Code.Ldloc[localBuilder2, null]);
					this.config.AddCode(Code.Unbox_Any[AccessTools.GetReturnedType(original), null]);
					this.config.AddCode(Code.Stloc[this.config.GetLocal(InjectionType.Result), null]);
				}
				IEnumerable<KeyValuePair<LocalBuilder, Type>> enumerable = list;
				Action<KeyValuePair<LocalBuilder, Type>> action;
				if ((action = <>9__2) == null)
				{
					action = (<>9__2 = delegate(KeyValuePair<LocalBuilder, Type> tmpBoxVar)
					{
						this.config.AddCode(new CodeInstruction(originalIsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1, null));
						this.config.AddCode(Code.Ldloc[tmpBoxVar.Key, null]);
						this.config.AddCode(Code.Unbox_Any[tmpBoxVar.Value, null]);
						this.config.AddCode(Code.Stobj[tmpBoxVar.Value, null]);
					});
				}
				enumerable.Do<KeyValuePair<LocalBuilder, Type>>(action);
				if (fix.ReturnType != typeof(void))
				{
					this.config.AddCode(Code.Stloc[this.config.GetLocal(InjectionType.Exception), null]);
					rethrowPossible = false;
				}
				if (catchExceptions)
				{
					this.config.AddCode(this.MarkBlock(ExceptionBlockType.BeginCatchBlock));
					this.config.AddCode(Code.Pop);
					this.config.AddCode(this.MarkBlock(ExceptionBlockType.EndExceptionBlock));
				}
			});
			return rethrowPossible;
		}

		private IEnumerable<CodeInstruction> AddInfixes(IEnumerable<CodeInstruction> instructions)
		{
			IEnumerable<IGrouping<MethodInfo, CodeInstruction>> enumerable = from ins in instructions
				where ins.opcode == OpCodes.Call || ins.opcode == OpCodes.Callvirt
				where ins.operand is MethodInfo
				group ins by (MethodInfo)ins.operand;
			Dictionary<CodeInstruction, CodeInstruction[]> replacements = new Dictionary<CodeInstruction, CodeInstruction[]>();
			Func<Infix, IEnumerable<CodeInstruction>> <>9__5;
			Func<Infix, IEnumerable<CodeInstruction>> <>9__6;
			foreach (ValueTuple<MethodInfo, List<CodeInstruction>> valueTuple in enumerable.Select<IGrouping<MethodInfo, CodeInstruction>, ValueTuple<MethodInfo, List<CodeInstruction>>>((IGrouping<MethodInfo, CodeInstruction> g) => new ValueTuple<MethodInfo, List<CodeInstruction>>(g.Key, g.ToList<CodeInstruction>())))
			{
				MethodInfo item = valueTuple.Item1;
				List<CodeInstruction> item2 = valueTuple.Item2;
				int count = item2.Count;
				for (int i = 0; i < count; i++)
				{
					CodeInstruction codeInstruction = item2[i];
					IEnumerable<Infix> enumerable2 = this.config.innerprefixes.FilterAndSort(item, i + 1, count, this.config.debug);
					Func<Infix, IEnumerable<CodeInstruction>> func;
					if ((func = <>9__5) == null)
					{
						func = (<>9__5 = (Infix fix) => fix.Apply(this.config, true));
					}
					IEnumerable<CodeInstruction> enumerable3 = enumerable2.SelectMany<Infix, CodeInstruction>(func);
					IEnumerable<Infix> enumerable4 = this.config.innerpostfixes.FilterAndSort(item, i + 1, count, this.config.debug);
					Func<Infix, IEnumerable<CodeInstruction>> func2;
					if ((func2 = <>9__6) == null)
					{
						func2 = (<>9__6 = (Infix fix) => fix.Apply(this.config, false));
					}
					IEnumerable<CodeInstruction> enumerable5 = enumerable4.SelectMany<Infix, CodeInstruction>(func2);
					Dictionary<CodeInstruction, CodeInstruction[]> replacements2 = replacements;
					CodeInstruction codeInstruction2 = codeInstruction;
					List<CodeInstruction> list = new List<CodeInstruction>();
					list.AddRange(enumerable3);
					list.Add(codeInstruction);
					list.AddRange(enumerable5);
					replacements2[codeInstruction2] = list.ToArray();
				}
			}
			return instructions.SelectMany<CodeInstruction, CodeInstruction>(delegate(CodeInstruction instruction)
			{
				CodeInstruction[] array;
				if (!replacements.TryGetValue(instruction, out array))
				{
					return new CodeInstruction[] { instruction };
				}
				return array;
			});
		}

		internal MethodCreatorConfig config;
	}
}
