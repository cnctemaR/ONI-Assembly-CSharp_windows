using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod.Utils.Cil;

namespace MonoMod.Utils
{
	internal static class _DMDEmit
	{
		private static MethodBuilder _CreateMethodProxy(MethodBuilder context, MethodInfo target)
		{
			TypeBuilder typeBuilder = (TypeBuilder)context.DeclaringType;
			string text = string.Format(".dmdproxy<{0}>?{1}", target.Name.Replace('.', '_'), target.GetHashCode());
			Type[] array = (from param in target.GetParameters()
				select param.ParameterType).ToArray<Type>();
			MethodBuilder methodBuilder = typeBuilder.DefineMethod(text, global::System.Reflection.MethodAttributes.Private | global::System.Reflection.MethodAttributes.Static | global::System.Reflection.MethodAttributes.HideBySig, CallingConventions.Standard, target.ReturnType, array);
			ILGenerator ilgenerator = methodBuilder.GetILGenerator();
			ilgenerator.EmitReference(target);
			ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Ldnull);
			ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, array.Length);
			ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Newarr, typeof(object));
			for (int i = 0; i < array.Length; i++)
			{
				ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Dup);
				ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, i);
				ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Ldarg, i);
				Type type = array[i];
				if (type.IsByRef)
				{
					type = type.GetElementType();
				}
				if (type.IsValueType)
				{
					ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Box, type);
				}
				ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Stelem_Ref);
			}
			ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Callvirt, _DMDEmit.m_MethodBase_InvokeSimple);
			if (target.ReturnType == typeof(void))
			{
				ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Pop);
			}
			else if (target.ReturnType.IsValueType)
			{
				ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Unbox_Any, target.ReturnType);
			}
			ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Ret);
			return methodBuilder;
		}

		static _DMDEmit()
		{
			Type type = typeof(ILGenerator).Assembly.GetType("System.Reflection.Emit.DynamicILGenerator");
			_DMDEmit.f_DynILGen_m_scope = ((type != null) ? type.GetField("m_scope", BindingFlags.Instance | BindingFlags.NonPublic) : null);
			Type type2 = typeof(ILGenerator).Assembly.GetType("System.Reflection.Emit.DynamicScope");
			_DMDEmit.f_DynScope_m_tokens = ((type2 != null) ? type2.GetField("m_tokens", BindingFlags.Instance | BindingFlags.NonPublic) : null);
			_DMDEmit.CorElementTypes = new Type[]
			{
				null,
				typeof(void),
				typeof(bool),
				typeof(char),
				typeof(sbyte),
				typeof(byte),
				typeof(short),
				typeof(ushort),
				typeof(int),
				typeof(uint),
				typeof(long),
				typeof(ulong),
				typeof(string),
				typeof(IntPtr)
			};
			FieldInfo[] array = typeof(global::System.Reflection.Emit.OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
			for (int i = 0; i < array.Length; i++)
			{
				global::System.Reflection.Emit.OpCode opCode = (global::System.Reflection.Emit.OpCode)array[i].GetValue(null);
				_DMDEmit._ReflOpCodes[opCode.Value] = opCode;
			}
			array = typeof(Mono.Cecil.Cil.OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
			for (int i = 0; i < array.Length; i++)
			{
				Mono.Cecil.Cil.OpCode opCode2 = (Mono.Cecil.Cil.OpCode)array[i].GetValue(null);
				_DMDEmit._CecilOpCodes[opCode2.Value] = opCode2;
			}
		}

		public static void Generate(DynamicMethodDefinition dmd, MethodBase _mb, ILGenerator il)
		{
			MethodDefinition definition = dmd.Definition;
			DynamicMethod dynamicMethod = _mb as DynamicMethod;
			MethodBuilder mb = _mb as MethodBuilder;
			MethodBuilder mb3 = mb;
			ModuleBuilder moduleBuilder = ((mb3 != null) ? mb3.Module : null) as ModuleBuilder;
			MethodBuilder mb2 = mb;
			TypeBuilder typeBuilder = ((mb2 != null) ? mb2.DeclaringType : null) as TypeBuilder;
			AssemblyBuilder assemblyBuilder = ((typeBuilder != null) ? typeBuilder.Assembly : null) as AssemblyBuilder;
			HashSet<Assembly> hashSet = null;
			if (mb != null)
			{
				hashSet = new HashSet<Assembly>();
			}
			MethodDebugInformation defInfo = (dmd.Debug ? definition.DebugInformation : null);
			if (dynamicMethod != null)
			{
				foreach (ParameterDefinition parameterDefinition in definition.Parameters)
				{
					dynamicMethod.DefineParameter(parameterDefinition.Index + 1, (global::System.Reflection.ParameterAttributes)parameterDefinition.Attributes, parameterDefinition.Name);
				}
			}
			if (mb != null)
			{
				foreach (ParameterDefinition parameterDefinition2 in definition.Parameters)
				{
					mb.DefineParameter(parameterDefinition2.Index + 1, (global::System.Reflection.ParameterAttributes)parameterDefinition2.Attributes, parameterDefinition2.Name);
				}
			}
			LocalBuilder[] array = definition.Body.Variables.Select<VariableDefinition, LocalBuilder>(delegate(VariableDefinition var)
			{
				LocalBuilder localBuilder = il.DeclareLocal(var.VariableType.ResolveReflection(), var.IsPinned);
				string text;
				if (mb != null && defInfo != null && defInfo.TryGetName(var, out text))
				{
					localBuilder.SetLocalSymInfo(text);
				}
				return localBuilder;
			}).ToArray<LocalBuilder>();
			Dictionary<Instruction, Label> labelMap = new Dictionary<Instruction, Label>();
			foreach (Instruction instruction in definition.Body.Instructions)
			{
				Instruction[] array2 = instruction.Operand as Instruction[];
				if (array2 != null)
				{
					foreach (Instruction instruction2 in array2)
					{
						if (!labelMap.ContainsKey(instruction2))
						{
							labelMap[instruction2] = il.DefineLabel();
						}
					}
				}
				else
				{
					Instruction instruction3 = instruction.Operand as Instruction;
					if (instruction3 != null && !labelMap.ContainsKey(instruction3))
					{
						labelMap[instruction3] = il.DefineLabel();
					}
				}
			}
			Dictionary<Document, ISymbolDocumentWriter> dictionary = ((mb == null) ? null : new Dictionary<Document, ISymbolDocumentWriter>());
			int num = (definition.HasThis ? 1 : 0);
			new object[2];
			bool flag = false;
			Func<Instruction, Label> <>9__1;
			foreach (Instruction instruction4 in definition.Body.Instructions)
			{
				Label label;
				if (labelMap.TryGetValue(instruction4, out label))
				{
					il.MarkLabel(label);
				}
				MethodDebugInformation defInfo2 = defInfo;
				SequencePoint sequencePoint = ((defInfo2 != null) ? defInfo2.GetSequencePoint(instruction4) : null);
				if (mb != null && sequencePoint != null)
				{
					ISymbolDocumentWriter symbolDocumentWriter;
					if (!dictionary.TryGetValue(sequencePoint.Document, out symbolDocumentWriter))
					{
						symbolDocumentWriter = (dictionary[sequencePoint.Document] = moduleBuilder.DefineDocument(sequencePoint.Document.Url, sequencePoint.Document.LanguageGuid, sequencePoint.Document.LanguageVendorGuid, sequencePoint.Document.TypeGuid));
					}
					il.MarkSequencePoint(symbolDocumentWriter, sequencePoint.StartLine, sequencePoint.StartColumn, sequencePoint.EndLine, sequencePoint.EndColumn);
				}
				foreach (Mono.Cecil.Cil.ExceptionHandler exceptionHandler in definition.Body.ExceptionHandlers)
				{
					if (flag && exceptionHandler.HandlerEnd == instruction4)
					{
						il.EndExceptionBlock();
					}
					if (exceptionHandler.TryStart == instruction4)
					{
						il.BeginExceptionBlock();
					}
					else if (exceptionHandler.FilterStart == instruction4)
					{
						il.BeginExceptFilterBlock();
					}
					else if (exceptionHandler.HandlerStart == instruction4)
					{
						switch (exceptionHandler.HandlerType)
						{
						case ExceptionHandlerType.Catch:
							il.BeginCatchBlock(exceptionHandler.CatchType.ResolveReflection());
							break;
						case ExceptionHandlerType.Filter:
							il.BeginCatchBlock(null);
							break;
						case ExceptionHandlerType.Finally:
							il.BeginFinallyBlock();
							break;
						case ExceptionHandlerType.Fault:
							il.BeginFaultBlock();
							break;
						}
					}
					if (exceptionHandler.HandlerStart == instruction4.Next)
					{
						ExceptionHandlerType handlerType = exceptionHandler.HandlerType;
						if (handlerType != ExceptionHandlerType.Filter)
						{
							if (handlerType == ExceptionHandlerType.Finally)
							{
								if (instruction4.OpCode == Mono.Cecil.Cil.OpCodes.Endfinally)
								{
									goto IL_0881;
								}
							}
						}
						else if (instruction4.OpCode == Mono.Cecil.Cil.OpCodes.Endfilter)
						{
							goto IL_0881;
						}
					}
				}
				if (instruction4.OpCode.OperandType == Mono.Cecil.Cil.OperandType.InlineNone)
				{
					il.Emit(_DMDEmit._ReflOpCodes[instruction4.OpCode.Value]);
				}
				else
				{
					object obj = instruction4.Operand;
					Instruction[] array4 = obj as Instruction[];
					if (array4 != null)
					{
						IEnumerable<Instruction> enumerable = array4;
						Func<Instruction, Label> func;
						if ((func = <>9__1) == null)
						{
							func = (<>9__1 = (Instruction target) => labelMap[target]);
						}
						obj = enumerable.Select<Instruction, Label>(func).ToArray<Label>();
						instruction4.OpCode = instruction4.OpCode.ToLongOp();
					}
					else
					{
						Instruction instruction5 = obj as Instruction;
						if (instruction5 != null)
						{
							obj = labelMap[instruction5];
							instruction4.OpCode = instruction4.OpCode.ToLongOp();
						}
						else
						{
							VariableDefinition variableDefinition = obj as VariableDefinition;
							if (variableDefinition != null)
							{
								obj = array[variableDefinition.Index];
							}
							else
							{
								ParameterDefinition parameterDefinition3 = obj as ParameterDefinition;
								if (parameterDefinition3 != null)
								{
									obj = parameterDefinition3.Index + num;
								}
								else
								{
									MemberReference memberReference = obj as MemberReference;
									if (memberReference != null)
									{
										MemberInfo memberInfo = ((memberReference == definition) ? _mb : memberReference.ResolveReflection());
										obj = memberInfo;
										if (mb != null && memberInfo != null)
										{
											Module module = ((memberInfo != null) ? memberInfo.Module : null);
											if (module == null)
											{
												continue;
											}
											Assembly assembly = module.Assembly;
											if (assembly != null && !hashSet.Contains(assembly))
											{
												assemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(DynamicMethodDefinition.c_IgnoresAccessChecksToAttribute, new object[] { assembly.GetName().Name }));
												hashSet.Add(assembly);
											}
										}
									}
									else
									{
										Mono.Cecil.CallSite callSite = obj as Mono.Cecil.CallSite;
										if (callSite != null)
										{
											if (dynamicMethod != null)
											{
												_DMDEmit._EmitCallSite(dynamicMethod, il, _DMDEmit._ReflOpCodes[instruction4.OpCode.Value], callSite);
												continue;
											}
											obj = callSite.ResolveReflection(mb.Module);
										}
									}
								}
							}
						}
					}
					if (mb != null)
					{
						MethodBase methodBase = obj as MethodBase;
						if (methodBase != null && methodBase.DeclaringType == null)
						{
							if (!(instruction4.OpCode == Mono.Cecil.Cil.OpCodes.Call))
							{
								throw new NotSupportedException("Unsupported global method operand on opcode " + instruction4.OpCode.Name);
							}
							MethodInfo methodInfo = methodBase as MethodInfo;
							if (methodInfo != null && methodInfo.IsDynamicMethod())
							{
								obj = _DMDEmit._CreateMethodProxy(mb, methodInfo);
							}
							else
							{
								IntPtr ldftnPointer = methodBase.GetLdftnPointer();
								if (IntPtr.Size == 4)
								{
									il.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, (int)ldftnPointer);
								}
								else
								{
									il.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I8, (long)ldftnPointer);
								}
								il.Emit(global::System.Reflection.Emit.OpCodes.Conv_I);
								instruction4.OpCode = Mono.Cecil.Cil.OpCodes.Calli;
								obj = ((MethodReference)instruction4.Operand).ResolveReflectionSignature(mb.Module);
							}
						}
					}
					if (obj == null)
					{
						throw new NullReferenceException(string.Format("Unexpected null in {0} @ {1}", definition, instruction4));
					}
					il.DynEmit(_DMDEmit._ReflOpCodes[instruction4.OpCode.Value], obj);
				}
				if (!flag)
				{
					using (Collection<Mono.Cecil.Cil.ExceptionHandler>.Enumerator enumerator3 = definition.Body.ExceptionHandlers.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current.HandlerEnd == instruction4.Next)
							{
								il.EndExceptionBlock();
							}
						}
					}
				}
				flag = false;
				continue;
				IL_0881:
				flag = true;
			}
		}

		public static void ResolveWithModifiers(TypeReference typeRef, out Type type, out Type[] typeModReq, out Type[] typeModOpt, List<Type> modReq = null, List<Type> modOpt = null)
		{
			if (modReq == null)
			{
				modReq = new List<Type>();
			}
			else
			{
				modReq.Clear();
			}
			if (modOpt == null)
			{
				modOpt = new List<Type>();
			}
			else
			{
				modOpt.Clear();
			}
			TypeReference typeReference = typeRef;
			for (;;)
			{
				TypeSpecification typeSpecification = typeReference as TypeSpecification;
				if (typeSpecification == null)
				{
					break;
				}
				RequiredModifierType requiredModifierType = typeReference as RequiredModifierType;
				if (requiredModifierType == null)
				{
					OptionalModifierType optionalModifierType = typeReference as OptionalModifierType;
					if (optionalModifierType != null)
					{
						modOpt.Add(optionalModifierType.ModifierType.ResolveReflection());
					}
				}
				else
				{
					modReq.Add(requiredModifierType.ModifierType.ResolveReflection());
				}
				typeReference = typeSpecification.ElementType;
			}
			type = typeRef.ResolveReflection();
			typeModReq = modReq.ToArray();
			typeModOpt = modOpt.ToArray();
		}

		internal static void _EmitCallSite(DynamicMethod dm, ILGenerator il, global::System.Reflection.Emit.OpCode opcode, Mono.Cecil.CallSite csite)
		{
			_DMDEmit.<>c__DisplayClass17_0 CS$<>8__locals1;
			CS$<>8__locals1._tokens = null;
			CS$<>8__locals1._info = null;
			if (ReflectionHelper.IsMono)
			{
				CS$<>8__locals1._info = dm.GetDynamicILInfo();
			}
			else
			{
				CS$<>8__locals1._tokens = _DMDEmit.f_DynScope_m_tokens.GetValue(_DMDEmit.f_DynILGen_m_scope.GetValue(il)) as List<object>;
			}
			CS$<>8__locals1.signature = new byte[32];
			CS$<>8__locals1.currSig = 0;
			int num = -1;
			_DMDEmit.<_EmitCallSite>g__AddData|17_5((int)csite.CallingConvention, ref CS$<>8__locals1);
			int currSig = CS$<>8__locals1.currSig;
			CS$<>8__locals1.currSig = currSig + 1;
			num = currSig;
			List<Type> list = new List<Type>();
			List<Type> list2 = new List<Type>();
			Type type;
			Type[] array;
			Type[] array2;
			_DMDEmit.ResolveWithModifiers(csite.ReturnType, out type, out array, out array2, list, list2);
			_DMDEmit.<_EmitCallSite>g__AddArgument|17_4(type, array, array2, ref CS$<>8__locals1);
			foreach (ParameterDefinition parameterDefinition in csite.Parameters)
			{
				if (parameterDefinition.ParameterType.IsSentinel)
				{
					_DMDEmit.<_EmitCallSite>g__AddElementType|17_7(65, ref CS$<>8__locals1);
				}
				if (parameterDefinition.ParameterType.IsPinned)
				{
					_DMDEmit.<_EmitCallSite>g__AddElementType|17_7(69, ref CS$<>8__locals1);
				}
				Type type2;
				Type[] array3;
				Type[] array4;
				_DMDEmit.ResolveWithModifiers(parameterDefinition.ParameterType, out type2, out array3, out array4, list, list2);
				_DMDEmit.<_EmitCallSite>g__AddArgument|17_4(type2, array3, array4, ref CS$<>8__locals1);
			}
			_DMDEmit.<_EmitCallSite>g__AddElementType|17_7(0, ref CS$<>8__locals1);
			int currSig2 = CS$<>8__locals1.currSig;
			int num2;
			if (csite.Parameters.Count < 128)
			{
				num2 = 1;
			}
			else if (csite.Parameters.Count < 16384)
			{
				num2 = 2;
			}
			else
			{
				num2 = 4;
			}
			byte[] array5 = new byte[CS$<>8__locals1.currSig + num2 - 1];
			array5[0] = CS$<>8__locals1.signature[0];
			Buffer.BlockCopy(CS$<>8__locals1.signature, num + 1, array5, num + num2, currSig2 - (num + 1));
			CS$<>8__locals1.signature = array5;
			CS$<>8__locals1.currSig = num;
			_DMDEmit.<_EmitCallSite>g__AddData|17_5(csite.Parameters.Count, ref CS$<>8__locals1);
			CS$<>8__locals1.currSig = currSig2 + (num2 - 1);
			if (CS$<>8__locals1.signature.Length > CS$<>8__locals1.currSig)
			{
				array5 = new byte[CS$<>8__locals1.currSig];
				Array.Copy(CS$<>8__locals1.signature, array5, CS$<>8__locals1.currSig);
				CS$<>8__locals1.signature = array5;
			}
			if (_DMDEmit._ILGen_emit_int != null)
			{
				_DMDEmit._ILGen_make_room.Invoke(il, new object[] { 6 });
				_DMDEmit._ILGen_ll_emit.Invoke(il, new object[] { opcode });
				_DMDEmit._ILGen_emit_int.Invoke(il, new object[] { _DMDEmit.<_EmitCallSite>g__GetTokenForSig|17_3(CS$<>8__locals1.signature, ref CS$<>8__locals1) });
				return;
			}
			_DMDEmit._ILGen_EnsureCapacity.Invoke(il, new object[] { 7 });
			_DMDEmit._ILGen_InternalEmit.Invoke(il, new object[] { opcode });
			if (opcode.StackBehaviourPop == global::System.Reflection.Emit.StackBehaviour.Varpop)
			{
				_DMDEmit._ILGen_UpdateStackSize.Invoke(il, new object[]
				{
					opcode,
					-csite.Parameters.Count - 1
				});
			}
			_DMDEmit._ILGen_PutInteger4.Invoke(il, new object[] { _DMDEmit.<_EmitCallSite>g__GetTokenForSig|17_3(CS$<>8__locals1.signature, ref CS$<>8__locals1) });
		}

		[CompilerGenerated]
		internal static int <_EmitCallSite>g___GetTokenForType|17_0(Type v, ref _DMDEmit.<>c__DisplayClass17_0 A_1)
		{
			A_1._tokens.Add(v.TypeHandle);
			return (A_1._tokens.Count - 1) | 33554432;
		}

		[CompilerGenerated]
		internal static int <_EmitCallSite>g___GetTokenForSig|17_1(byte[] v, ref _DMDEmit.<>c__DisplayClass17_0 A_1)
		{
			A_1._tokens.Add(v);
			return (A_1._tokens.Count - 1) | 285212672;
		}

		[CompilerGenerated]
		internal static int <_EmitCallSite>g__GetTokenForType|17_2(Type v, ref _DMDEmit.<>c__DisplayClass17_0 A_1)
		{
			if (A_1._info == null)
			{
				return _DMDEmit.<_EmitCallSite>g___GetTokenForType|17_0(v, ref A_1);
			}
			return A_1._info.GetTokenFor(v.TypeHandle);
		}

		[CompilerGenerated]
		internal static int <_EmitCallSite>g__GetTokenForSig|17_3(byte[] v, ref _DMDEmit.<>c__DisplayClass17_0 A_1)
		{
			if (A_1._info == null)
			{
				return _DMDEmit.<_EmitCallSite>g___GetTokenForSig|17_1(v, ref A_1);
			}
			return A_1._info.GetTokenFor(v);
		}

		[CompilerGenerated]
		internal static void <_EmitCallSite>g__AddArgument|17_4(Type clsArgument, Type[] requiredCustomModifiers, Type[] optionalCustomModifiers, ref _DMDEmit.<>c__DisplayClass17_0 A_3)
		{
			if (optionalCustomModifiers != null)
			{
				for (int i = 0; i < optionalCustomModifiers.Length; i++)
				{
					_DMDEmit.<_EmitCallSite>g__InternalAddTypeToken|17_9(_DMDEmit.<_EmitCallSite>g__GetTokenForType|17_2(optionalCustomModifiers[i], ref A_3), 32, ref A_3);
				}
			}
			if (requiredCustomModifiers != null)
			{
				for (int i = 0; i < requiredCustomModifiers.Length; i++)
				{
					_DMDEmit.<_EmitCallSite>g__InternalAddTypeToken|17_9(_DMDEmit.<_EmitCallSite>g__GetTokenForType|17_2(requiredCustomModifiers[i], ref A_3), 31, ref A_3);
				}
			}
			_DMDEmit.<_EmitCallSite>g__AddOneArgTypeHelper|17_10(clsArgument, ref A_3);
		}

		[CompilerGenerated]
		internal static void <_EmitCallSite>g__AddData|17_5(int data, ref _DMDEmit.<>c__DisplayClass17_0 A_1)
		{
			if (A_1.currSig + 4 > A_1.signature.Length)
			{
				A_1.signature = _DMDEmit.<_EmitCallSite>g__ExpandArray|17_6(A_1.signature, -1);
			}
			if (data <= 127)
			{
				byte[] signature = A_1.signature;
				int num = A_1.currSig;
				A_1.currSig = num + 1;
				signature[num] = (byte)(data & 255);
				return;
			}
			if (data <= 16383)
			{
				byte[] signature2 = A_1.signature;
				int num = A_1.currSig;
				A_1.currSig = num + 1;
				signature2[num] = (byte)((data >> 8) | 128);
				byte[] signature3 = A_1.signature;
				num = A_1.currSig;
				A_1.currSig = num + 1;
				signature3[num] = (byte)(data & 255);
				return;
			}
			if (data <= 536870911)
			{
				byte[] signature4 = A_1.signature;
				int num = A_1.currSig;
				A_1.currSig = num + 1;
				signature4[num] = (byte)((data >> 24) | 192);
				byte[] signature5 = A_1.signature;
				num = A_1.currSig;
				A_1.currSig = num + 1;
				signature5[num] = (byte)((data >> 16) & 255);
				byte[] signature6 = A_1.signature;
				num = A_1.currSig;
				A_1.currSig = num + 1;
				signature6[num] = (byte)((data >> 8) & 255);
				byte[] signature7 = A_1.signature;
				num = A_1.currSig;
				A_1.currSig = num + 1;
				signature7[num] = (byte)(data & 255);
				return;
			}
			throw new ArgumentException("Integer or token was too large to be encoded.");
		}

		[CompilerGenerated]
		internal static byte[] <_EmitCallSite>g__ExpandArray|17_6(byte[] inArray, int requiredLength = -1)
		{
			if (requiredLength < inArray.Length)
			{
				requiredLength = inArray.Length * 2;
			}
			byte[] array = new byte[requiredLength];
			Buffer.BlockCopy(inArray, 0, array, 0, inArray.Length);
			return array;
		}

		[CompilerGenerated]
		internal static void <_EmitCallSite>g__AddElementType|17_7(byte cvt, ref _DMDEmit.<>c__DisplayClass17_0 A_1)
		{
			if (A_1.currSig + 1 > A_1.signature.Length)
			{
				A_1.signature = _DMDEmit.<_EmitCallSite>g__ExpandArray|17_6(A_1.signature, -1);
			}
			byte[] signature = A_1.signature;
			int currSig = A_1.currSig;
			A_1.currSig = currSig + 1;
			signature[currSig] = cvt;
		}

		[CompilerGenerated]
		internal static void <_EmitCallSite>g__AddToken|17_8(int token, ref _DMDEmit.<>c__DisplayClass17_0 A_1)
		{
			int num = token & 16777215;
			int num2 = token & -16777216;
			if (num > 67108863)
			{
				throw new ArgumentException("Integer or token was too large to be encoded.");
			}
			num <<= 2;
			if (num2 == 16777216)
			{
				num |= 1;
			}
			else if (num2 == 452984832)
			{
				num |= 2;
			}
			_DMDEmit.<_EmitCallSite>g__AddData|17_5(num, ref A_1);
		}

		[CompilerGenerated]
		internal static void <_EmitCallSite>g__InternalAddTypeToken|17_9(int clsToken, byte CorType, ref _DMDEmit.<>c__DisplayClass17_0 A_2)
		{
			_DMDEmit.<_EmitCallSite>g__AddElementType|17_7(CorType, ref A_2);
			_DMDEmit.<_EmitCallSite>g__AddToken|17_8(clsToken, ref A_2);
		}

		[CompilerGenerated]
		internal static void <_EmitCallSite>g__AddOneArgTypeHelper|17_10(Type clsArgument, ref _DMDEmit.<>c__DisplayClass17_0 A_1)
		{
			_DMDEmit.<_EmitCallSite>g__AddOneArgTypeHelperWorker|17_11(clsArgument, false, ref A_1);
		}

		[CompilerGenerated]
		internal static void <_EmitCallSite>g__AddOneArgTypeHelperWorker|17_11(Type clsArgument, bool lastWasGenericInst, ref _DMDEmit.<>c__DisplayClass17_0 A_2)
		{
			if (clsArgument.IsGenericType && (!clsArgument.IsGenericTypeDefinition || !lastWasGenericInst))
			{
				_DMDEmit.<_EmitCallSite>g__AddElementType|17_7(21, ref A_2);
				_DMDEmit.<_EmitCallSite>g__AddOneArgTypeHelperWorker|17_11(clsArgument.GetGenericTypeDefinition(), true, ref A_2);
				Type[] genericArguments = clsArgument.GetGenericArguments();
				_DMDEmit.<_EmitCallSite>g__AddData|17_5(genericArguments.Length, ref A_2);
				Type[] array = genericArguments;
				for (int i = 0; i < array.Length; i++)
				{
					_DMDEmit.<_EmitCallSite>g__AddOneArgTypeHelper|17_10(array[i], ref A_2);
				}
				return;
			}
			if (clsArgument.IsByRef)
			{
				_DMDEmit.<_EmitCallSite>g__AddElementType|17_7(16, ref A_2);
				clsArgument = clsArgument.GetElementType();
				_DMDEmit.<_EmitCallSite>g__AddOneArgTypeHelper|17_10(clsArgument, ref A_2);
				return;
			}
			if (clsArgument.IsPointer)
			{
				_DMDEmit.<_EmitCallSite>g__AddElementType|17_7(15, ref A_2);
				_DMDEmit.<_EmitCallSite>g__AddOneArgTypeHelper|17_10(clsArgument.GetElementType(), ref A_2);
				return;
			}
			if (clsArgument.IsArray)
			{
				_DMDEmit.<_EmitCallSite>g__AddElementType|17_7(20, ref A_2);
				_DMDEmit.<_EmitCallSite>g__AddOneArgTypeHelper|17_10(clsArgument.GetElementType(), ref A_2);
				int arrayRank = clsArgument.GetArrayRank();
				_DMDEmit.<_EmitCallSite>g__AddData|17_5(arrayRank, ref A_2);
				_DMDEmit.<_EmitCallSite>g__AddData|17_5(0, ref A_2);
				_DMDEmit.<_EmitCallSite>g__AddData|17_5(arrayRank, ref A_2);
				for (int j = 0; j < arrayRank; j++)
				{
					_DMDEmit.<_EmitCallSite>g__AddData|17_5(0, ref A_2);
				}
				return;
			}
			byte b = 0;
			for (int k = 0; k < _DMDEmit.CorElementTypes.Length; k++)
			{
				if (clsArgument == _DMDEmit.CorElementTypes[k])
				{
					b = (byte)k;
					break;
				}
			}
			if (b == 0)
			{
				if (clsArgument == typeof(object))
				{
					b = 28;
				}
				else if (clsArgument.IsValueType)
				{
					b = 17;
				}
				else
				{
					b = 18;
				}
			}
			if (b <= 14 || b == 22 || b == 24 || b == 25 || b == 28)
			{
				_DMDEmit.<_EmitCallSite>g__AddElementType|17_7(b, ref A_2);
				return;
			}
			if (clsArgument.IsValueType)
			{
				_DMDEmit.<_EmitCallSite>g__InternalAddTypeToken|17_9(_DMDEmit.<_EmitCallSite>g__GetTokenForType|17_2(clsArgument, ref A_2), 17, ref A_2);
				return;
			}
			_DMDEmit.<_EmitCallSite>g__InternalAddTypeToken|17_9(_DMDEmit.<_EmitCallSite>g__GetTokenForType|17_2(clsArgument, ref A_2), 18, ref A_2);
		}

		private static readonly MethodInfo m_MethodBase_InvokeSimple = typeof(MethodBase).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public, null, new Type[]
		{
			typeof(object),
			typeof(object[])
		}, null);

		private static readonly Dictionary<short, global::System.Reflection.Emit.OpCode> _ReflOpCodes = new Dictionary<short, global::System.Reflection.Emit.OpCode>();

		private static readonly Dictionary<short, Mono.Cecil.Cil.OpCode> _CecilOpCodes = new Dictionary<short, Mono.Cecil.Cil.OpCode>();

		private static readonly MethodInfo _ILGen_make_room = typeof(ILGenerator).GetMethod("make_room", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo _ILGen_emit_int = typeof(ILGenerator).GetMethod("emit_int", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo _ILGen_ll_emit = typeof(ILGenerator).GetMethod("ll_emit", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo _ILGen_EnsureCapacity = typeof(ILGenerator).GetMethod("EnsureCapacity", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo _ILGen_PutInteger4 = typeof(ILGenerator).GetMethod("PutInteger4", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo _ILGen_InternalEmit = typeof(ILGenerator).GetMethod("InternalEmit", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo _ILGen_UpdateStackSize = typeof(ILGenerator).GetMethod("UpdateStackSize", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly FieldInfo f_DynILGen_m_scope;

		private static readonly FieldInfo f_DynScope_m_tokens;

		private static readonly Type[] CorElementTypes;
	}
}
