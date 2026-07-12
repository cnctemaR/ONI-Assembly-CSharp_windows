using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace MonoMod.Utils.Cil
{
	public sealed class CecilILGenerator : ILGeneratorShim
	{
		unsafe static CecilILGenerator()
		{
			FieldInfo[] fields = typeof(Mono.Cecil.Cil.OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
			for (int i = 0; i < fields.Length; i++)
			{
				Mono.Cecil.Cil.OpCode opCode = (Mono.Cecil.Cil.OpCode)fields[i].GetValue(null);
				CecilILGenerator._MCCOpCodes[opCode.Value] = opCode;
			}
			Label label = default(Label);
			*(int*)(&label) = -1;
			CecilILGenerator.NullLabel = label;
		}

		public CecilILGenerator(ILProcessor il)
		{
			this.IL = il;
		}

		private Mono.Cecil.Cil.OpCode _(global::System.Reflection.Emit.OpCode opcode)
		{
			return CecilILGenerator._MCCOpCodes[opcode.Value];
		}

		private CecilILGenerator.LabelInfo _(Label handle)
		{
			CecilILGenerator.LabelInfo labelInfo;
			if (!this._LabelInfos.TryGetValue(handle, out labelInfo))
			{
				return null;
			}
			return labelInfo;
		}

		private VariableDefinition _(LocalBuilder handle)
		{
			return this._Variables[handle];
		}

		private TypeReference _(Type info)
		{
			return this.IL.Body.Method.Module.ImportReference(info);
		}

		private FieldReference _(FieldInfo info)
		{
			return this.IL.Body.Method.Module.ImportReference(info);
		}

		private MethodReference _(MethodBase info)
		{
			return this.IL.Body.Method.Module.ImportReference(info);
		}

		public override int ILOffset
		{
			get
			{
				return this._ILOffset;
			}
		}

		private Instruction ProcessLabels(Instruction ins)
		{
			if (this._LabelsToMark.Count != 0)
			{
				foreach (CecilILGenerator.LabelInfo labelInfo in this._LabelsToMark)
				{
					foreach (Instruction instruction in labelInfo.Branches)
					{
						object operand = instruction.Operand;
						if (!(operand is Instruction))
						{
							Instruction[] array = operand as Instruction[];
							if (array != null)
							{
								for (int i = 0; i < array.Length; i++)
								{
									if (array[i] == labelInfo.Instruction)
									{
										array[i] = ins;
										break;
									}
								}
							}
						}
						else
						{
							instruction.Operand = ins;
						}
					}
					labelInfo.Emitted = true;
					labelInfo.Instruction = ins;
				}
				this._LabelsToMark.Clear();
			}
			if (this._ExceptionHandlersToMark.Count != 0)
			{
				foreach (CecilILGenerator.LabelledExceptionHandler labelledExceptionHandler in this._ExceptionHandlersToMark)
				{
					Collection<Mono.Cecil.Cil.ExceptionHandler> exceptionHandlers = this.IL.Body.ExceptionHandlers;
					Mono.Cecil.Cil.ExceptionHandler exceptionHandler = new Mono.Cecil.Cil.ExceptionHandler(labelledExceptionHandler.HandlerType);
					CecilILGenerator.LabelInfo labelInfo2 = this._(labelledExceptionHandler.TryStart);
					exceptionHandler.TryStart = ((labelInfo2 != null) ? labelInfo2.Instruction : null);
					CecilILGenerator.LabelInfo labelInfo3 = this._(labelledExceptionHandler.TryEnd);
					exceptionHandler.TryEnd = ((labelInfo3 != null) ? labelInfo3.Instruction : null);
					CecilILGenerator.LabelInfo labelInfo4 = this._(labelledExceptionHandler.HandlerStart);
					exceptionHandler.HandlerStart = ((labelInfo4 != null) ? labelInfo4.Instruction : null);
					CecilILGenerator.LabelInfo labelInfo5 = this._(labelledExceptionHandler.HandlerEnd);
					exceptionHandler.HandlerEnd = ((labelInfo5 != null) ? labelInfo5.Instruction : null);
					CecilILGenerator.LabelInfo labelInfo6 = this._(labelledExceptionHandler.FilterStart);
					exceptionHandler.FilterStart = ((labelInfo6 != null) ? labelInfo6.Instruction : null);
					exceptionHandler.CatchType = labelledExceptionHandler.ExceptionType;
					exceptionHandlers.Add(exceptionHandler);
				}
				this._ExceptionHandlersToMark.Clear();
			}
			return ins;
		}

		public unsafe override Label DefineLabel()
		{
			Label label = default(Label);
			ref int ptr = ref *(int*)(&label);
			int num = this.labelCounter;
			this.labelCounter = num + 1;
			ptr = num;
			this._LabelInfos[label] = new CecilILGenerator.LabelInfo();
			return label;
		}

		public override void MarkLabel(Label loc)
		{
			CecilILGenerator.LabelInfo labelInfo;
			if (!this._LabelInfos.TryGetValue(loc, out labelInfo) || labelInfo.Emitted)
			{
				return;
			}
			this._LabelsToMark.Add(labelInfo);
		}

		public override LocalBuilder DeclareLocal(Type type)
		{
			return this.DeclareLocal(type, false);
		}

		public override LocalBuilder DeclareLocal(Type type, bool pinned)
		{
			int count = this.IL.Body.Variables.Count;
			object obj;
			if (CecilILGenerator.c_LocalBuilder_params != 4)
			{
				if (CecilILGenerator.c_LocalBuilder_params != 3)
				{
					if (CecilILGenerator.c_LocalBuilder_params != 2)
					{
						if (CecilILGenerator.c_LocalBuilder_params != 0)
						{
							throw new NotSupportedException();
						}
						obj = CecilILGenerator.c_LocalBuilder.Invoke(new object[0]);
					}
					else
					{
						ConstructorInfo constructorInfo = CecilILGenerator.c_LocalBuilder;
						object[] array = new object[2];
						array[0] = type;
						obj = constructorInfo.Invoke(array);
					}
				}
				else
				{
					ConstructorInfo constructorInfo2 = CecilILGenerator.c_LocalBuilder;
					object[] array2 = new object[3];
					array2[0] = count;
					array2[1] = type;
					obj = constructorInfo2.Invoke(array2);
				}
			}
			else
			{
				obj = CecilILGenerator.c_LocalBuilder.Invoke(new object[] { count, type, null, pinned });
			}
			LocalBuilder localBuilder = (LocalBuilder)obj;
			FieldInfo fieldInfo = CecilILGenerator.f_LocalBuilder_position;
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(localBuilder, (ushort)count);
			}
			FieldInfo fieldInfo2 = CecilILGenerator.f_LocalBuilder_is_pinned;
			if (fieldInfo2 != null)
			{
				fieldInfo2.SetValue(localBuilder, pinned);
			}
			TypeReference typeReference = this._(type);
			if (pinned)
			{
				typeReference = new PinnedType(typeReference);
			}
			VariableDefinition variableDefinition = new VariableDefinition(typeReference);
			this.IL.Body.Variables.Add(variableDefinition);
			this._Variables[localBuilder] = variableDefinition;
			return localBuilder;
		}

		private void Emit(Instruction ins)
		{
			ins.Offset = this._ILOffset;
			this._ILOffset += ins.GetSize();
			this.IL.Append(this.ProcessLabels(ins));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode)
		{
			this.Emit(this.IL.Create(this._(opcode)));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, byte arg)
		{
			if (opcode.OperandType == global::System.Reflection.Emit.OperandType.ShortInlineVar || opcode.OperandType == global::System.Reflection.Emit.OperandType.InlineVar)
			{
				this._EmitInlineVar(this._(opcode), (int)arg);
				return;
			}
			this.Emit(this.IL.Create(this._(opcode), arg));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, sbyte arg)
		{
			if (opcode.OperandType == global::System.Reflection.Emit.OperandType.ShortInlineVar || opcode.OperandType == global::System.Reflection.Emit.OperandType.InlineVar)
			{
				this._EmitInlineVar(this._(opcode), (int)arg);
				return;
			}
			this.Emit(this.IL.Create(this._(opcode), arg));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, short arg)
		{
			if (opcode.OperandType == global::System.Reflection.Emit.OperandType.ShortInlineVar || opcode.OperandType == global::System.Reflection.Emit.OperandType.InlineVar)
			{
				this._EmitInlineVar(this._(opcode), (int)arg);
				return;
			}
			this.Emit(this.IL.Create(this._(opcode), (int)arg));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, int arg)
		{
			if (opcode.OperandType == global::System.Reflection.Emit.OperandType.ShortInlineVar || opcode.OperandType == global::System.Reflection.Emit.OperandType.InlineVar)
			{
				this._EmitInlineVar(this._(opcode), arg);
				return;
			}
			if (opcode.Name.EndsWith(".s", StringComparison.Ordinal))
			{
				this.Emit(this.IL.Create(this._(opcode), (sbyte)arg));
				return;
			}
			this.Emit(this.IL.Create(this._(opcode), arg));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, long arg)
		{
			this.Emit(this.IL.Create(this._(opcode), arg));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, float arg)
		{
			this.Emit(this.IL.Create(this._(opcode), arg));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, double arg)
		{
			this.Emit(this.IL.Create(this._(opcode), arg));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, string arg)
		{
			this.Emit(this.IL.Create(this._(opcode), arg));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, Type arg)
		{
			this.Emit(this.IL.Create(this._(opcode), this._(arg)));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, FieldInfo arg)
		{
			this.Emit(this.IL.Create(this._(opcode), this._(arg)));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, ConstructorInfo arg)
		{
			this.Emit(this.IL.Create(this._(opcode), this._(arg)));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, MethodInfo arg)
		{
			this.Emit(this.IL.Create(this._(opcode), this._(arg)));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, Label label)
		{
			CecilILGenerator.LabelInfo labelInfo = this._(label);
			Instruction instruction = this.IL.Create(this._(opcode), this._(label).Instruction);
			labelInfo.Branches.Add(instruction);
			this.Emit(this.ProcessLabels(instruction));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, Label[] labels)
		{
			IEnumerable<CecilILGenerator.LabelInfo> enumerable = labels.Distinct<Label>().Select<Label, CecilILGenerator.LabelInfo>(new Func<Label, CecilILGenerator.LabelInfo>(this._));
			Instruction instruction = this.IL.Create(this._(opcode), enumerable.Select<CecilILGenerator.LabelInfo, Instruction>((CecilILGenerator.LabelInfo labelInfo) => labelInfo.Instruction).ToArray<Instruction>());
			foreach (CecilILGenerator.LabelInfo labelInfo2 in enumerable)
			{
				labelInfo2.Branches.Add(instruction);
			}
			this.Emit(this.ProcessLabels(instruction));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, LocalBuilder local)
		{
			this.Emit(this.IL.Create(this._(opcode), this._(local)));
		}

		public override void Emit(global::System.Reflection.Emit.OpCode opcode, SignatureHelper signature)
		{
			this.Emit(this.IL.Create(this._(opcode), this.IL.Body.Method.Module.ImportCallSite(signature)));
		}

		public void Emit(global::System.Reflection.Emit.OpCode opcode, ICallSiteGenerator signature)
		{
			this.Emit(this.IL.Create(this._(opcode), this.IL.Body.Method.Module.ImportCallSite(signature)));
		}

		private void _EmitInlineVar(Mono.Cecil.Cil.OpCode opcode, int index)
		{
			switch (opcode.OperandType)
			{
			case Mono.Cecil.Cil.OperandType.InlineVar:
			case Mono.Cecil.Cil.OperandType.ShortInlineVar:
				this.Emit(this.IL.Create(opcode, this.IL.Body.Variables[index]));
				return;
			case Mono.Cecil.Cil.OperandType.InlineArg:
			case Mono.Cecil.Cil.OperandType.ShortInlineArg:
				this.Emit(this.IL.Create(opcode, this.IL.Body.Method.Parameters[index]));
				return;
			}
			throw new NotSupportedException(string.Format("Unsupported SRE InlineVar -> Cecil {0} for {1} {2}", opcode.OperandType, opcode, index));
		}

		public override void EmitCall(global::System.Reflection.Emit.OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
		{
			this.Emit(this.IL.Create(this._(opcode), this._(methodInfo)));
		}

		public override void EmitCalli(global::System.Reflection.Emit.OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			throw new NotSupportedException();
		}

		public override void EmitCalli(global::System.Reflection.Emit.OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
		{
			throw new NotSupportedException();
		}

		public override void EmitWriteLine(FieldInfo field)
		{
			if (field.IsStatic)
			{
				this.Emit(this.IL.Create(Mono.Cecil.Cil.OpCodes.Ldsfld, this._(field)));
			}
			else
			{
				this.Emit(this.IL.Create(Mono.Cecil.Cil.OpCodes.Ldarg_0));
				this.Emit(this.IL.Create(Mono.Cecil.Cil.OpCodes.Ldfld, this._(field)));
			}
			this.Emit(this.IL.Create(Mono.Cecil.Cil.OpCodes.Call, this._(typeof(Console).GetMethod("WriteLine", new Type[] { field.FieldType }))));
		}

		public override void EmitWriteLine(LocalBuilder localBuilder)
		{
			this.Emit(this.IL.Create(Mono.Cecil.Cil.OpCodes.Ldloc, this._(localBuilder)));
			this.Emit(this.IL.Create(Mono.Cecil.Cil.OpCodes.Call, this._(typeof(Console).GetMethod("WriteLine", new Type[] { localBuilder.LocalType }))));
		}

		public override void EmitWriteLine(string value)
		{
			this.Emit(this.IL.Create(Mono.Cecil.Cil.OpCodes.Ldstr, value));
			this.Emit(this.IL.Create(Mono.Cecil.Cil.OpCodes.Call, this._(typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }))));
		}

		public override void ThrowException(Type type)
		{
			this.Emit(this.IL.Create(Mono.Cecil.Cil.OpCodes.Newobj, this._(type.GetConstructor(Type.EmptyTypes))));
			this.Emit(this.IL.Create(Mono.Cecil.Cil.OpCodes.Throw));
		}

		public override Label BeginExceptionBlock()
		{
			CecilILGenerator.ExceptionHandlerChain exceptionHandlerChain = new CecilILGenerator.ExceptionHandlerChain(this);
			this._ExceptionHandlers.Push(exceptionHandlerChain);
			return exceptionHandlerChain.SkipAll;
		}

		public override void BeginCatchBlock(Type exceptionType)
		{
			this._ExceptionHandlers.Peek().BeginHandler(ExceptionHandlerType.Catch).ExceptionType = ((exceptionType == null) ? null : this._(exceptionType));
		}

		public override void BeginExceptFilterBlock()
		{
			this._ExceptionHandlers.Peek().BeginHandler(ExceptionHandlerType.Filter);
		}

		public override void BeginFaultBlock()
		{
			this._ExceptionHandlers.Peek().BeginHandler(ExceptionHandlerType.Fault);
		}

		public override void BeginFinallyBlock()
		{
			this._ExceptionHandlers.Peek().BeginHandler(ExceptionHandlerType.Finally);
		}

		public override void EndExceptionBlock()
		{
			this._ExceptionHandlers.Pop().End();
		}

		public override void BeginScope()
		{
		}

		public override void EndScope()
		{
		}

		public override void UsingNamespace(string usingNamespace)
		{
		}

		private static readonly ConstructorInfo c_LocalBuilder = (from c in typeof(LocalBuilder).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			orderby c.GetParameters().Length descending
			select c).First<ConstructorInfo>();

		private static readonly FieldInfo f_LocalBuilder_position = typeof(LocalBuilder).GetField("position", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly FieldInfo f_LocalBuilder_is_pinned = typeof(LocalBuilder).GetField("is_pinned", BindingFlags.Instance | BindingFlags.NonPublic);

		private static int c_LocalBuilder_params = CecilILGenerator.c_LocalBuilder.GetParameters().Length;

		private static readonly Dictionary<short, Mono.Cecil.Cil.OpCode> _MCCOpCodes = new Dictionary<short, Mono.Cecil.Cil.OpCode>();

		private static Label NullLabel;

		public readonly ILProcessor IL;

		private readonly Dictionary<Label, CecilILGenerator.LabelInfo> _LabelInfos = new Dictionary<Label, CecilILGenerator.LabelInfo>();

		private readonly List<CecilILGenerator.LabelInfo> _LabelsToMark = new List<CecilILGenerator.LabelInfo>();

		private readonly List<CecilILGenerator.LabelledExceptionHandler> _ExceptionHandlersToMark = new List<CecilILGenerator.LabelledExceptionHandler>();

		private readonly Dictionary<LocalBuilder, VariableDefinition> _Variables = new Dictionary<LocalBuilder, VariableDefinition>();

		private readonly Stack<CecilILGenerator.ExceptionHandlerChain> _ExceptionHandlers = new Stack<CecilILGenerator.ExceptionHandlerChain>();

		private int labelCounter;

		private int _ILOffset;

		private class LabelInfo
		{
			public bool Emitted;

			public Instruction Instruction = Instruction.Create(Mono.Cecil.Cil.OpCodes.Nop);

			public readonly List<Instruction> Branches = new List<Instruction>();
		}

		private class LabelledExceptionHandler
		{
			public Label TryStart = CecilILGenerator.NullLabel;

			public Label TryEnd = CecilILGenerator.NullLabel;

			public Label HandlerStart = CecilILGenerator.NullLabel;

			public Label HandlerEnd = CecilILGenerator.NullLabel;

			public Label FilterStart = CecilILGenerator.NullLabel;

			public ExceptionHandlerType HandlerType;

			public TypeReference ExceptionType;
		}

		private class ExceptionHandlerChain
		{
			public ExceptionHandlerChain(CecilILGenerator il)
			{
				this.IL = il;
				this._Start = il.DefineLabel();
				il.MarkLabel(this._Start);
				this.SkipAll = il.DefineLabel();
			}

			public CecilILGenerator.LabelledExceptionHandler BeginHandler(ExceptionHandlerType type)
			{
				CecilILGenerator.LabelledExceptionHandler labelledExceptionHandler = (this._Prev = this._Handler);
				if (labelledExceptionHandler != null)
				{
					this.EndHandler(labelledExceptionHandler);
				}
				this.IL.Emit(global::System.Reflection.Emit.OpCodes.Leave, this._SkipHandler = this.IL.DefineLabel());
				Label label = this.IL.DefineLabel();
				this.IL.MarkLabel(label);
				CecilILGenerator.LabelledExceptionHandler labelledExceptionHandler2 = new CecilILGenerator.LabelledExceptionHandler();
				labelledExceptionHandler2.TryStart = this._Start;
				labelledExceptionHandler2.TryEnd = label;
				labelledExceptionHandler2.HandlerType = type;
				labelledExceptionHandler2.HandlerEnd = this._SkipHandler;
				CecilILGenerator.LabelledExceptionHandler labelledExceptionHandler3 = labelledExceptionHandler2;
				this._Handler = labelledExceptionHandler2;
				CecilILGenerator.LabelledExceptionHandler labelledExceptionHandler4 = labelledExceptionHandler3;
				if (type == ExceptionHandlerType.Filter)
				{
					labelledExceptionHandler4.FilterStart = label;
				}
				else
				{
					labelledExceptionHandler4.HandlerStart = label;
				}
				return labelledExceptionHandler4;
			}

			public void EndHandler(CecilILGenerator.LabelledExceptionHandler handler)
			{
				Label skipHandler = this._SkipHandler;
				ExceptionHandlerType handlerType = handler.HandlerType;
				if (handlerType != ExceptionHandlerType.Filter)
				{
					if (handlerType != ExceptionHandlerType.Finally)
					{
						this.IL.Emit(global::System.Reflection.Emit.OpCodes.Leave, skipHandler);
					}
					else
					{
						this.IL.Emit(global::System.Reflection.Emit.OpCodes.Endfinally);
					}
				}
				else
				{
					this.IL.Emit(global::System.Reflection.Emit.OpCodes.Endfilter);
				}
				this.IL.MarkLabel(skipHandler);
				this.IL._ExceptionHandlersToMark.Add(handler);
			}

			public void End()
			{
				this.EndHandler(this._Handler);
				this.IL.MarkLabel(this.SkipAll);
			}

			private readonly CecilILGenerator IL;

			private readonly Label _Start;

			public readonly Label SkipAll;

			private Label _SkipHandler;

			private CecilILGenerator.LabelledExceptionHandler _Prev;

			private CecilILGenerator.LabelledExceptionHandler _Handler;
		}
	}
}
