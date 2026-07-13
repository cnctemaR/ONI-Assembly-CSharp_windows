using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod.Utils.Cil;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class _DMDEmit
	{
		private static MethodBuilder _CreateMethodProxy(MethodBuilder context, MethodInfo target)
		{
			TypeBuilder typeBuilder = (TypeBuilder)context.DeclaringType;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
			defaultInterpolatedStringHandler.AppendLiteral(".dmdproxy<");
			defaultInterpolatedStringHandler.AppendFormatted(target.Name.Replace('.', '_'));
			defaultInterpolatedStringHandler.AppendLiteral(">?");
			defaultInterpolatedStringHandler.AppendFormatted<int>(target.GetHashCode());
			string text = defaultInterpolatedStringHandler.ToStringAndClear();
			Type[] array = (from param in target.GetParameters()
				select param.ParameterType).ToArray<Type>();
			MethodBuilder methodBuilder = typeBuilder.DefineMethod(text, global::System.Reflection.MethodAttributes.Private | global::System.Reflection.MethodAttributes.Static | global::System.Reflection.MethodAttributes.HideBySig, CallingConventions.Standard, target.ReturnType, array);
			ILGenerator ilgenerator = methodBuilder.GetILGenerator();
			DynamicReferenceCell dynamicReferenceCell;
			ilgenerator.EmitNewTypedReference(target, out dynamicReferenceCell);
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
					type = type.GetElementType() ?? type;
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
			MethodInfo methodInfo = _DMDEmit.mDynamicMethod_AddRef;
			_DMDEmit.DynamicMethod_AddRef = ((methodInfo != null) ? methodInfo.CreateDelegate<Func<DynamicMethod, object, int>>() : null);
			_DMDEmit.TRuntimeILGenerator = Type.GetType("System.Reflection.Emit.RuntimeILGenerator");
			MethodInfo methodInfo2;
			if ((methodInfo2 = typeof(ILGenerator).GetMethod("EnsureCapacity", BindingFlags.Instance | BindingFlags.NonPublic)) == null)
			{
				Type truntimeILGenerator = _DMDEmit.TRuntimeILGenerator;
				methodInfo2 = ((truntimeILGenerator != null) ? truntimeILGenerator.GetMethod("EnsureCapacity", BindingFlags.Instance | BindingFlags.NonPublic) : null);
			}
			_DMDEmit._ILGen_EnsureCapacity = methodInfo2;
			MethodInfo methodInfo3;
			if ((methodInfo3 = typeof(ILGenerator).GetMethod("PutInteger4", BindingFlags.Instance | BindingFlags.NonPublic)) == null)
			{
				Type truntimeILGenerator2 = _DMDEmit.TRuntimeILGenerator;
				methodInfo3 = ((truntimeILGenerator2 != null) ? truntimeILGenerator2.GetMethod("PutInteger4", BindingFlags.Instance | BindingFlags.NonPublic) : null);
			}
			_DMDEmit._ILGen_PutInteger4 = methodInfo3;
			MethodInfo methodInfo4;
			if ((methodInfo4 = typeof(ILGenerator).GetMethod("InternalEmit", BindingFlags.Instance | BindingFlags.NonPublic)) == null)
			{
				Type truntimeILGenerator3 = _DMDEmit.TRuntimeILGenerator;
				methodInfo4 = ((truntimeILGenerator3 != null) ? truntimeILGenerator3.GetMethod("InternalEmit", BindingFlags.Instance | BindingFlags.NonPublic) : null);
			}
			_DMDEmit._ILGen_InternalEmit = methodInfo4;
			MethodInfo methodInfo5;
			if ((methodInfo5 = typeof(ILGenerator).GetMethod("UpdateStackSize", BindingFlags.Instance | BindingFlags.NonPublic)) == null)
			{
				Type truntimeILGenerator4 = _DMDEmit.TRuntimeILGenerator;
				methodInfo5 = ((truntimeILGenerator4 != null) ? truntimeILGenerator4.GetMethod("UpdateStackSize", BindingFlags.Instance | BindingFlags.NonPublic) : null);
			}
			_DMDEmit._ILGen_UpdateStackSize = methodInfo5;
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
				typeof(float),
				typeof(double),
				typeof(string),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				typeof(IntPtr),
				typeof(UIntPtr),
				null,
				null,
				typeof(object)
			};
			_DMDEmit.callSiteEmitter = ((_DMDEmit.DynamicMethod_AddRef != null) ? new _DMDEmit.MonoCallSiteEmitter() : new _DMDEmit.NetCallSiteEmitter());
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
			if (definition == null)
			{
				throw new InvalidOperationException();
			}
			MethodDefinition methodDefinition = definition;
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
			MethodDebugInformation defInfo = (dmd.Debug ? methodDefinition.DebugInformation : null);
			if (dynamicMethod != null)
			{
				foreach (ParameterDefinition parameterDefinition in methodDefinition.Parameters)
				{
					dynamicMethod.DefineParameter(parameterDefinition.Index + 1, (global::System.Reflection.ParameterAttributes)parameterDefinition.Attributes, parameterDefinition.Name);
				}
			}
			if (mb != null)
			{
				foreach (ParameterDefinition parameterDefinition2 in methodDefinition.Parameters)
				{
					mb.DefineParameter(parameterDefinition2.Index + 1, (global::System.Reflection.ParameterAttributes)parameterDefinition2.Attributes, parameterDefinition2.Name);
				}
			}
			LocalBuilder[] array = methodDefinition.Body.Variables.Select<VariableDefinition, LocalBuilder>(delegate(VariableDefinition var)
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
			foreach (Instruction instruction in methodDefinition.Body.Instructions)
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
			int num = ((methodDefinition.HasThis > false) ? 1 : 0);
			new object[2];
			bool flag = false;
			Func<Instruction, Label> <>9__1;
			foreach (Instruction instruction4 in methodDefinition.Body.Instructions)
			{
				Label label;
				if (labelMap.TryGetValue(instruction4, out label))
				{
					il.MarkLabel(label);
				}
				MethodDebugInformation defInfo2 = defInfo;
				SequencePoint sequencePoint = ((defInfo2 != null) ? defInfo2.GetSequencePoint(instruction4) : null);
				if (mb != null && sequencePoint != null && dictionary != null && moduleBuilder != null)
				{
					ISymbolDocumentWriter symbolDocumentWriter;
					if (!dictionary.TryGetValue(sequencePoint.Document, out symbolDocumentWriter))
					{
						symbolDocumentWriter = (dictionary[sequencePoint.Document] = moduleBuilder.DefineDocument(sequencePoint.Document.Url, sequencePoint.Document.LanguageGuid, sequencePoint.Document.LanguageVendorGuid, sequencePoint.Document.TypeGuid));
					}
					il.MarkSequencePoint(symbolDocumentWriter, sequencePoint.StartLine, sequencePoint.StartColumn, sequencePoint.EndLine, sequencePoint.EndColumn);
				}
				foreach (Mono.Cecil.Cil.ExceptionHandler exceptionHandler in methodDefinition.Body.ExceptionHandlers)
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
									goto IL_08A3;
								}
							}
						}
						else if (instruction4.OpCode == Mono.Cecil.Cil.OpCodes.Endfilter)
						{
							goto IL_08A3;
						}
					}
				}
				if (instruction4.OpCode.OperandType == Mono.Cecil.Cil.OperandType.InlineNone)
				{
					il.Emit(_DMDEmit._ReflOpCodes[instruction4.OpCode.Value]);
				}
				else
				{
					Mono.Cecil.Cil.OpCode opCode = instruction4.OpCode;
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
						opCode = opCode.ToLongOp();
					}
					else
					{
						Instruction instruction5 = obj as Instruction;
						if (instruction5 != null)
						{
							obj = labelMap[instruction5];
							opCode = opCode.ToLongOp();
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
										MemberInfo memberInfo = ((memberReference == methodDefinition) ? _mb : memberReference.ResolveReflection());
										obj = memberInfo;
										if (mb != null && memberInfo != null)
										{
											Module module = memberInfo.Module;
											if (module == null)
											{
												continue;
											}
											Assembly assembly = module.Assembly;
											if (assembly != null && hashSet != null && assemblyBuilder != null && !hashSet.Contains(assembly))
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
												_DMDEmit._EmitCallSite(dynamicMethod, il, _DMDEmit._ReflOpCodes[opCode.Value], callSite);
												continue;
											}
											if (mb == null)
											{
												throw new NotSupportedException();
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
							if (!(opCode == Mono.Cecil.Cil.OpCodes.Call))
							{
								throw new NotSupportedException("Unsupported global method operand on opcode " + opCode.Name);
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
								opCode = Mono.Cecil.Cil.OpCodes.Calli;
								obj = ((MethodReference)instruction4.Operand).ResolveReflectionSignature(mb.Module);
							}
						}
					}
					if (obj == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Unexpected null in ");
						defaultInterpolatedStringHandler.AppendFormatted<MethodDefinition>(methodDefinition);
						defaultInterpolatedStringHandler.AppendLiteral(" @ ");
						defaultInterpolatedStringHandler.AppendFormatted<Instruction>(instruction4);
						throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					il.DynEmit(_DMDEmit._ReflOpCodes[opCode.Value], obj);
				}
				if (!flag)
				{
					using (Collection<Mono.Cecil.Cil.ExceptionHandler>.Enumerator enumerator3 = methodDefinition.Body.ExceptionHandlers.GetEnumerator())
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
				IL_08A3:
				flag = true;
			}
		}

		public static void ResolveWithModifiers(TypeReference typeRef, out Type type, out Type[] typeModReq, out Type[] typeModOpt, [Nullable(new byte[] { 2, 1 })] List<Type> modReq = null, [Nullable(new byte[] { 2, 1 })] List<Type> modOpt = null)
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
			_DMDEmit.callSiteEmitter.EmitCallSite(dm, il, opcode, csite);
		}

		private static readonly MethodInfo m_MethodBase_InvokeSimple = typeof(MethodBase).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public, null, new Type[]
		{
			typeof(object),
			typeof(object[])
		}, null);

		private static readonly Dictionary<short, global::System.Reflection.Emit.OpCode> _ReflOpCodes = new Dictionary<short, global::System.Reflection.Emit.OpCode>();

		private static readonly Dictionary<short, Mono.Cecil.Cil.OpCode> _CecilOpCodes = new Dictionary<short, Mono.Cecil.Cil.OpCode>();

		[Nullable(2)]
		private static readonly MethodInfo _ILGen_make_room = typeof(ILGenerator).GetMethod("make_room", BindingFlags.Instance | BindingFlags.NonPublic);

		[Nullable(2)]
		private static readonly MethodInfo _ILGen_emit_int = typeof(ILGenerator).GetMethod("emit_int", BindingFlags.Instance | BindingFlags.NonPublic);

		[Nullable(2)]
		private static readonly MethodInfo _ILGen_ll_emit = typeof(ILGenerator).GetMethod("ll_emit", BindingFlags.Instance | BindingFlags.NonPublic);

		[Nullable(2)]
		private static readonly MethodInfo mDynamicMethod_AddRef = typeof(DynamicMethod).GetMethod("AddRef", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(object) }, null);

		[Nullable(new byte[] { 2, 1, 2 })]
		private static readonly Func<DynamicMethod, object, int> DynamicMethod_AddRef;

		[Nullable(2)]
		private static readonly Type TRuntimeILGenerator;

		[Nullable(2)]
		private static readonly MethodInfo _ILGen_EnsureCapacity;

		[Nullable(2)]
		private static readonly MethodInfo _ILGen_PutInteger4;

		[Nullable(2)]
		private static readonly MethodInfo _ILGen_InternalEmit;

		[Nullable(2)]
		private static readonly MethodInfo _ILGen_UpdateStackSize;

		[Nullable(2)]
		private static readonly FieldInfo f_DynILGen_m_scope;

		[Nullable(2)]
		private static readonly FieldInfo f_DynScope_m_tokens;

		[Nullable(new byte[] { 1, 2 })]
		private static readonly Type[] CorElementTypes;

		private static readonly _DMDEmit.CallSiteEmitter callSiteEmitter;

		[Nullable(0)]
		private abstract class TokenCreator
		{
			public abstract int GetTokenForType(Type type);

			public abstract int GetTokenForSig(byte[] sig);
		}

		[Nullable(0)]
		private sealed class NetTokenCreator : _DMDEmit.TokenCreator
		{
			public NetTokenCreator(ILGenerator il)
			{
				Helpers.Assert(_DMDEmit.f_DynScope_m_tokens != null, null, "f_DynScope_m_tokens is not null");
				Helpers.Assert(_DMDEmit.f_DynILGen_m_scope != null, null, "f_DynILGen_m_scope is not null");
				List<object> list = (List<object>)_DMDEmit.f_DynScope_m_tokens.GetValue(_DMDEmit.f_DynILGen_m_scope.GetValue(il));
				Helpers.Assert(list != null, "DynamicMethod object list is null!", "list is not null");
				this.tokens = list;
			}

			public override int GetTokenForType(Type type)
			{
				this.tokens.Add(type.TypeHandle);
				return (this.tokens.Count - 1) | 33554432;
			}

			public override int GetTokenForSig(byte[] sig)
			{
				this.tokens.Add(sig);
				return (this.tokens.Count - 1) | 285212672;
			}

			private readonly List<object> tokens;
		}

		[Nullable(0)]
		private sealed class MonoTokenCreator : _DMDEmit.TokenCreator
		{
			public MonoTokenCreator(DynamicMethod dm)
			{
				Helpers.Assert(_DMDEmit.DynamicMethod_AddRef != null, null, "DynamicMethod_AddRef is not null");
				this.addRef = _DMDEmit.DynamicMethod_AddRef;
				this.dm = dm;
			}

			public override int GetTokenForType(Type type)
			{
				return this.addRef(this.dm, type);
			}

			public override int GetTokenForSig(byte[] sig)
			{
				return this.addRef(this.dm, sig);
			}

			private readonly DynamicMethod dm;

			[Nullable(new byte[] { 1, 1, 2 })]
			private readonly Func<DynamicMethod, object, int> addRef;
		}

		[NullableContext(0)]
		private abstract class CallSiteEmitter
		{
			[NullableContext(1)]
			public abstract void EmitCallSite(DynamicMethod dm, ILGenerator il, global::System.Reflection.Emit.OpCode opcode, Mono.Cecil.CallSite csite);
		}

		[NullableContext(0)]
		private sealed class NetCallSiteEmitter : _DMDEmit.CallSiteEmitter
		{
			[NullableContext(1)]
			public override void EmitCallSite(DynamicMethod dm, ILGenerator il, global::System.Reflection.Emit.OpCode opcode, Mono.Cecil.CallSite csite)
			{
				_DMDEmit.NetCallSiteEmitter.<>c__DisplayClass0_0 CS$<>8__locals1;
				CS$<>8__locals1.tokenCreator = ((_DMDEmit.DynamicMethod_AddRef != null) ? new _DMDEmit.MonoTokenCreator(dm) : new _DMDEmit.NetTokenCreator(il));
				CS$<>8__locals1.signature = new byte[32];
				CS$<>8__locals1.currSig = 0;
				int num = -1;
				_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddData|0_1((int)(csite.CallingConvention | (csite.HasThis ? ((MethodCallingConvention)32) : MethodCallingConvention.Default) | (csite.ExplicitThis ? ((MethodCallingConvention)64) : MethodCallingConvention.Default)), ref CS$<>8__locals1);
				int currSig = CS$<>8__locals1.currSig;
				CS$<>8__locals1.currSig = currSig + 1;
				num = currSig;
				List<Type> list = new List<Type>();
				List<Type> list2 = new List<Type>();
				Type type;
				Type[] array;
				Type[] array2;
				_DMDEmit.ResolveWithModifiers(csite.ReturnType, out type, out array, out array2, list, list2);
				_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddArgument|0_0(type, array, array2, ref CS$<>8__locals1);
				foreach (ParameterDefinition parameterDefinition in csite.Parameters)
				{
					if (parameterDefinition.ParameterType.IsSentinel)
					{
						_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddElementType|0_3(65, ref CS$<>8__locals1);
					}
					if (parameterDefinition.ParameterType.IsPinned)
					{
						_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddElementType|0_3(69, ref CS$<>8__locals1);
					}
					Type type2;
					Type[] array3;
					Type[] array4;
					_DMDEmit.ResolveWithModifiers(parameterDefinition.ParameterType, out type2, out array3, out array4, list, list2);
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddArgument|0_0(type2, array3, array4, ref CS$<>8__locals1);
				}
				_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddElementType|0_3(0, ref CS$<>8__locals1);
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
				_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddData|0_1(csite.Parameters.Count, ref CS$<>8__locals1);
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
					_DMDEmit._ILGen_emit_int.Invoke(il, new object[] { CS$<>8__locals1.tokenCreator.GetTokenForSig(CS$<>8__locals1.signature) });
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
				_DMDEmit._ILGen_PutInteger4.Invoke(il, new object[] { CS$<>8__locals1.tokenCreator.GetTokenForSig(CS$<>8__locals1.signature) });
			}

			[NullableContext(1)]
			[CompilerGenerated]
			internal static void <EmitCallSite>g__AddArgument|0_0(Type clsArgument, Type[] requiredCustomModifiers, Type[] optionalCustomModifiers, ref _DMDEmit.NetCallSiteEmitter.<>c__DisplayClass0_0 A_3)
			{
				if (optionalCustomModifiers != null)
				{
					foreach (Type type in optionalCustomModifiers)
					{
						_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__InternalAddTypeToken|0_5(A_3.tokenCreator.GetTokenForType(type), 32, ref A_3);
					}
				}
				if (requiredCustomModifiers != null)
				{
					foreach (Type type2 in requiredCustomModifiers)
					{
						_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__InternalAddTypeToken|0_5(A_3.tokenCreator.GetTokenForType(type2), 31, ref A_3);
					}
				}
				_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddOneArgTypeHelper|0_6(clsArgument, ref A_3);
			}

			[CompilerGenerated]
			internal static void <EmitCallSite>g__AddData|0_1(int data, ref _DMDEmit.NetCallSiteEmitter.<>c__DisplayClass0_0 A_1)
			{
				if (A_1.currSig + 4 > A_1.signature.Length)
				{
					A_1.signature = _DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__ExpandArray|0_2(A_1.signature, -1);
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

			[NullableContext(1)]
			[CompilerGenerated]
			internal static byte[] <EmitCallSite>g__ExpandArray|0_2(byte[] inArray, int requiredLength = -1)
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
			internal static void <EmitCallSite>g__AddElementType|0_3(byte cvt, ref _DMDEmit.NetCallSiteEmitter.<>c__DisplayClass0_0 A_1)
			{
				if (A_1.currSig + 1 > A_1.signature.Length)
				{
					A_1.signature = _DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__ExpandArray|0_2(A_1.signature, -1);
				}
				byte[] signature = A_1.signature;
				int currSig = A_1.currSig;
				A_1.currSig = currSig + 1;
				signature[currSig] = cvt;
			}

			[CompilerGenerated]
			internal static void <EmitCallSite>g__AddToken|0_4(int token, ref _DMDEmit.NetCallSiteEmitter.<>c__DisplayClass0_0 A_1)
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
				_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddData|0_1(num, ref A_1);
			}

			[CompilerGenerated]
			internal static void <EmitCallSite>g__InternalAddTypeToken|0_5(int clsToken, byte CorType, ref _DMDEmit.NetCallSiteEmitter.<>c__DisplayClass0_0 A_2)
			{
				_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddElementType|0_3(CorType, ref A_2);
				_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddToken|0_4(clsToken, ref A_2);
			}

			[NullableContext(1)]
			[CompilerGenerated]
			internal static void <EmitCallSite>g__AddOneArgTypeHelper|0_6(Type clsArgument, ref _DMDEmit.NetCallSiteEmitter.<>c__DisplayClass0_0 A_1)
			{
				_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddOneArgTypeHelperWorker|0_7(clsArgument, false, ref A_1);
			}

			[NullableContext(1)]
			[CompilerGenerated]
			internal static void <EmitCallSite>g__AddOneArgTypeHelperWorker|0_7(Type clsArgument, bool lastWasGenericInst, ref _DMDEmit.NetCallSiteEmitter.<>c__DisplayClass0_0 A_2)
			{
				if (clsArgument.IsGenericType && (!clsArgument.IsGenericTypeDefinition || !lastWasGenericInst))
				{
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddElementType|0_3(21, ref A_2);
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddOneArgTypeHelperWorker|0_7(clsArgument.GetGenericTypeDefinition(), true, ref A_2);
					Type[] genericArguments = clsArgument.GetGenericArguments();
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddData|0_1(genericArguments.Length, ref A_2);
					Type[] array = genericArguments;
					for (int i = 0; i < array.Length; i++)
					{
						_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddOneArgTypeHelper|0_6(array[i], ref A_2);
					}
					return;
				}
				if (clsArgument.IsByRef)
				{
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddElementType|0_3(16, ref A_2);
					clsArgument = clsArgument.GetElementType() ?? clsArgument;
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddOneArgTypeHelper|0_6(clsArgument, ref A_2);
					return;
				}
				if (clsArgument.IsPointer)
				{
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddElementType|0_3(15, ref A_2);
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddOneArgTypeHelper|0_6(clsArgument.GetElementType() ?? clsArgument, ref A_2);
					return;
				}
				if (clsArgument.IsArray)
				{
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddElementType|0_3(20, ref A_2);
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddOneArgTypeHelper|0_6(clsArgument.GetElementType() ?? clsArgument, ref A_2);
					int arrayRank = clsArgument.GetArrayRank();
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddData|0_1(arrayRank, ref A_2);
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddData|0_1(0, ref A_2);
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddData|0_1(arrayRank, ref A_2);
					for (int j = 0; j < arrayRank; j++)
					{
						_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddData|0_1(0, ref A_2);
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
					_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddElementType|0_3(b, ref A_2);
					return;
				}
				_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__InternalAddRuntimeType|0_8(clsArgument, ref A_2);
			}

			[NullableContext(1)]
			[CompilerGenerated]
			internal unsafe static void <EmitCallSite>g__InternalAddRuntimeType|0_8(Type type, ref _DMDEmit.NetCallSiteEmitter.<>c__DisplayClass0_0 A_1)
			{
				_DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__AddElementType|0_3(33, ref A_1);
				IntPtr value = type.TypeHandle.Value;
				if (A_1.currSig + sizeof(void*) > A_1.signature.Length)
				{
					A_1.signature = _DMDEmit.NetCallSiteEmitter.<EmitCallSite>g__ExpandArray|0_2(A_1.signature, -1);
				}
				byte* ptr = (byte*)(&value);
				for (int i = 0; i < sizeof(void*); i++)
				{
					byte[] signature = A_1.signature;
					int currSig = A_1.currSig;
					A_1.currSig = currSig + 1;
					signature[currSig] = ptr[i];
				}
			}
		}

		[Nullable(0)]
		private sealed class MonoCallSiteEmitter : _DMDEmit.CallSiteEmitter
		{
			public MonoCallSiteEmitter()
			{
				FieldInfo field = typeof(SignatureHelper).GetField("callConv", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field2 = typeof(SignatureHelper).GetField("unmanagedCallConv", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field3 = typeof(SignatureHelper).GetField("arguments", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field4 = typeof(SignatureHelper).GetField("modreqs", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field5 = typeof(SignatureHelper).GetField("modopts", BindingFlags.Instance | BindingFlags.NonPublic);
				Helpers.Assert(field != null, null, "callConv is not null");
				Helpers.Assert(field2 != null, null, "unmanagedCallConv is not null");
				Helpers.Assert(field3 != null, null, "arguments is not null");
				Helpers.Assert(field4 != null, null, "modreqs is not null");
				Helpers.Assert(field5 != null, null, "modopts is not null");
				this.SigHelper_callConv = field;
				this.SigHelper_unmanagedCallConv = field2;
				this.SigHelper_arguments = field3;
				this.SigHelper_modreqs = field4;
				this.SigHelper_modopts = field5;
			}

			public override void EmitCallSite(DynamicMethod dm, ILGenerator il, global::System.Reflection.Emit.OpCode opcode, Mono.Cecil.CallSite csite)
			{
				List<Type> list = new List<Type>();
				List<Type> list2 = new List<Type>();
				Type type;
				Type[] array;
				Type[] array2;
				_DMDEmit.ResolveWithModifiers(csite.ReturnType, out type, out array, out array2, list, list2);
				SignatureHelper methodSigHelper = SignatureHelper.GetMethodSigHelper(CallingConventions.Standard, type);
				Type[] array3 = new Type[csite.Parameters.Count];
				Type[][] array4 = new Type[csite.Parameters.Count][];
				Type[][] array5 = new Type[csite.Parameters.Count][];
				CallingConventions callingConventions;
				if (csite.CallingConvention == MethodCallingConvention.VarArg)
				{
					callingConventions = CallingConventions.VarArgs;
				}
				else
				{
					callingConventions = CallingConventions.Standard;
				}
				CallingConventions callingConventions2 = callingConventions;
				if (csite.HasThis)
				{
					callingConventions2 |= CallingConventions.HasThis;
				}
				if (csite.ExplicitThis)
				{
					callingConventions2 |= CallingConventions.ExplicitThis;
				}
				CallingConvention callingConvention;
				switch (csite.CallingConvention)
				{
				case MethodCallingConvention.C:
					callingConvention = CallingConvention.Cdecl;
					break;
				case MethodCallingConvention.StdCall:
					callingConvention = CallingConvention.StdCall;
					break;
				case MethodCallingConvention.ThisCall:
					callingConvention = CallingConvention.ThisCall;
					break;
				case MethodCallingConvention.FastCall:
					callingConvention = CallingConvention.FastCall;
					break;
				default:
					callingConvention = (CallingConvention)0;
					break;
				}
				CallingConvention callingConvention2 = callingConvention;
				for (int i = 0; i < csite.Parameters.Count; i++)
				{
					_DMDEmit.ResolveWithModifiers(csite.Parameters[i].ParameterType, out array3[i], out array4[i], out array5[i], list, list2);
				}
				this.SigHelper_callConv.SetValue(methodSigHelper, callingConventions2);
				this.SigHelper_unmanagedCallConv.SetValue(methodSigHelper, callingConvention2);
				this.SigHelper_arguments.SetValue(methodSigHelper, array3);
				this.SigHelper_modreqs.SetValue(methodSigHelper, array4);
				this.SigHelper_modopts.SetValue(methodSigHelper, array5);
				_DMDEmit._ILGen_make_room.Invoke(il, new object[] { 6 });
				_DMDEmit._ILGen_ll_emit.Invoke(il, new object[] { opcode });
				_DMDEmit._ILGen_emit_int.Invoke(il, new object[] { _DMDEmit.DynamicMethod_AddRef(dm, methodSigHelper) });
			}

			private FieldInfo SigHelper_callConv;

			private FieldInfo SigHelper_unmanagedCallConv;

			private FieldInfo SigHelper_arguments;

			private FieldInfo SigHelper_modreqs;

			private FieldInfo SigHelper_modopts;
		}
	}
}
