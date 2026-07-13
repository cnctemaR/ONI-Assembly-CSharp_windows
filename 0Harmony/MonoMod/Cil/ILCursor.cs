using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod.SourceGen.Attributes;
using MonoMod.Utils;

namespace MonoMod.Cil
{
	[NullableContext(1)]
	[Nullable(0)]
	[EmitILOverloads("ILOpcodes.txt", "ILCursor")]
	internal sealed class ILCursor
	{
		public ILContext Context { get; }

		[Nullable(2)]
		public Instruction Next
		{
			[NullableContext(2)]
			get
			{
				return this._next;
			}
			[NullableContext(2)]
			set
			{
				this.Goto(value, MoveType.Before, false);
			}
		}

		public Instruction Prev
		{
			get
			{
				if (this.Next != null)
				{
					return this.Next.Previous;
				}
				return this.Instrs[this.Instrs.Count - 1];
			}
			set
			{
				this.Goto(value, MoveType.After, false);
			}
		}

		public Instruction Previous
		{
			get
			{
				return this.Prev;
			}
			set
			{
				this.Prev = value;
			}
		}

		public int Index
		{
			get
			{
				return this.Context.IndexOf(this.Next);
			}
			set
			{
				this.Goto(value, MoveType.Before, false);
			}
		}

		public SearchTarget SearchTarget
		{
			get
			{
				return this._searchTarget;
			}
			set
			{
				if ((value == SearchTarget.Next && this.Next == null) || (value == SearchTarget.Prev && this.Prev == null))
				{
					value = SearchTarget.None;
				}
				this._searchTarget = value;
			}
		}

		public IEnumerable<ILLabel> IncomingLabels
		{
			get
			{
				return this.Context.GetIncomingLabels(this.Next);
			}
		}

		public MethodDefinition Method
		{
			get
			{
				return this.Context.Method;
			}
		}

		public ILProcessor IL
		{
			get
			{
				return this.Context.IL;
			}
		}

		public Mono.Cecil.Cil.MethodBody Body
		{
			get
			{
				return this.Context.Body;
			}
		}

		public ModuleDefinition Module
		{
			get
			{
				return this.Context.Module;
			}
		}

		public Collection<Instruction> Instrs
		{
			get
			{
				return this.Context.Instrs;
			}
		}

		public ILCursor(ILContext context)
		{
			this.Context = context;
			this.Index = 0;
		}

		public ILCursor(ILCursor c)
		{
			Helpers.ThrowIfArgumentNull<ILCursor>(c, "c");
			this.Context = c.Context;
			this._next = c._next;
			this._searchTarget = c._searchTarget;
			this._afterLabels = c._afterLabels;
			this._afterHandlerStarts = c._afterHandlerStarts;
			this._afterHandlerEnds = c._afterHandlerEnds;
		}

		public ILCursor Clone()
		{
			return new ILCursor(this);
		}

		public bool IsBefore(Instruction instr)
		{
			return this.Index <= this.Context.IndexOf(instr);
		}

		public bool IsAfter(Instruction instr)
		{
			return this.Index > this.Context.IndexOf(instr);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 3);
			defaultInterpolatedStringHandler.AppendLiteral("// ILCursor: ");
			defaultInterpolatedStringHandler.AppendFormatted<MethodDefinition>(this.Method);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.Index);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted<SearchTarget>(this.SearchTarget);
			stringBuilder.AppendLine(defaultInterpolatedStringHandler.ToStringAndClear());
			ILContext.ToString(stringBuilder, this.Prev);
			ILContext.ToString(stringBuilder, this.Next);
			return stringBuilder.ToString();
		}

		public ILCursor Goto([Nullable(2)] Instruction insn, MoveType moveType = MoveType.Before, bool setTarget = false)
		{
			if (moveType == MoveType.After)
			{
				this._next = ((insn != null) ? insn.Next : null);
			}
			else
			{
				this._next = insn;
			}
			if (setTarget)
			{
				this._searchTarget = ((moveType == MoveType.After) ? SearchTarget.Prev : SearchTarget.Next);
			}
			else
			{
				this._searchTarget = SearchTarget.None;
			}
			if (moveType == MoveType.AfterLabel)
			{
				this.MoveAfterLabels();
			}
			else
			{
				this.MoveBeforeLabels();
			}
			return this;
		}

		public ILCursor MoveAfterLabels()
		{
			this.MoveAfterLabels(true);
			return this;
		}

		public ILCursor MoveAfterLabels(bool intoEHRanges)
		{
			this._afterLabels = this.IncomingLabels.ToArray<ILLabel>();
			this._afterHandlerStarts = intoEHRanges;
			this._afterHandlerEnds = true;
			return this;
		}

		public ILCursor MoveBeforeLabels()
		{
			this._afterLabels = null;
			this._afterHandlerStarts = false;
			this._afterHandlerEnds = false;
			return this;
		}

		public ILCursor Goto(int index, MoveType moveType = MoveType.Before, bool setTarget = false)
		{
			if (index < 0)
			{
				index += this.Instrs.Count;
			}
			return this.Goto((index == this.Instrs.Count) ? null : this.Instrs[index], moveType, setTarget);
		}

		public ILCursor GotoLabel(ILLabel label, MoveType moveType = MoveType.AfterLabel, bool setTarget = false)
		{
			return this.Goto(Helpers.ThrowIfNull<ILLabel>(label, "label").Target, moveType, setTarget);
		}

		public ILCursor GotoNext(MoveType moveType = MoveType.Before, params Func<Instruction, bool>[] predicates)
		{
			if (!this.TryGotoNext(moveType, predicates))
			{
				throw new KeyNotFoundException();
			}
			return this;
		}

		public bool TryGotoNext(MoveType moveType = MoveType.Before, params Func<Instruction, bool>[] predicates)
		{
			Helpers.ThrowIfArgumentNull<Func<Instruction, bool>[]>(predicates, "predicates");
			Collection<Instruction> instrs = this.Instrs;
			int num = this.Index;
			if (this.SearchTarget == SearchTarget.Next)
			{
				num++;
			}
			IL_006D:
			while (num + predicates.Length <= instrs.Count)
			{
				for (int i = 0; i < predicates.Length; i++)
				{
					Func<Instruction, bool> func = predicates[i];
					if (func != null && !func(instrs[num + i]))
					{
						num++;
						goto IL_006D;
					}
				}
				this.Goto((moveType == MoveType.After) ? (num + predicates.Length - 1) : num, moveType, true);
				return true;
			}
			return false;
		}

		public ILCursor GotoPrev(MoveType moveType = MoveType.Before, params Func<Instruction, bool>[] predicates)
		{
			if (!this.TryGotoPrev(moveType, predicates))
			{
				throw new KeyNotFoundException();
			}
			return this;
		}

		public bool TryGotoPrev(MoveType moveType = MoveType.Before, params Func<Instruction, bool>[] predicates)
		{
			Helpers.ThrowIfArgumentNull<Func<Instruction, bool>[]>(predicates, "predicates");
			Collection<Instruction> instrs = this.Instrs;
			int i = this.Index - 1;
			if (this.SearchTarget == SearchTarget.Prev)
			{
				i--;
			}
			i = Math.Min(i, instrs.Count - predicates.Length);
			IL_0080:
			while (i >= 0)
			{
				for (int j = 0; j < predicates.Length; j++)
				{
					Func<Instruction, bool> func = predicates[j];
					if (func != null && !func(instrs[i + j]))
					{
						i--;
						goto IL_0080;
					}
				}
				this.Goto((moveType == MoveType.After) ? (i + predicates.Length - 1) : i, moveType, true);
				return true;
			}
			return false;
		}

		public ILCursor GotoNext(params Func<Instruction, bool>[] predicates)
		{
			return this.GotoNext(MoveType.Before, predicates);
		}

		public bool TryGotoNext(params Func<Instruction, bool>[] predicates)
		{
			return this.TryGotoNext(MoveType.Before, predicates);
		}

		public ILCursor GotoPrev(params Func<Instruction, bool>[] predicates)
		{
			return this.GotoPrev(MoveType.Before, predicates);
		}

		public bool TryGotoPrev(params Func<Instruction, bool>[] predicates)
		{
			return this.TryGotoPrev(MoveType.Before, predicates);
		}

		public void FindNext(out ILCursor[] cursors, params Func<Instruction, bool>[] predicates)
		{
			if (!this.TryFindNext(out cursors, predicates))
			{
				throw new KeyNotFoundException();
			}
		}

		public bool TryFindNext(out ILCursor[] cursors, params Func<Instruction, bool>[] predicates)
		{
			Helpers.ThrowIfArgumentNull<Func<Instruction, bool>[]>(predicates, "predicates");
			cursors = new ILCursor[predicates.Length];
			ILCursor ilcursor = this;
			for (int i = 0; i < predicates.Length; i++)
			{
				ilcursor = ilcursor.Clone();
				if (!ilcursor.TryGotoNext(new Func<Instruction, bool>[] { predicates[i] }))
				{
					return false;
				}
				cursors[i] = ilcursor;
			}
			return true;
		}

		public void FindPrev(out ILCursor[] cursors, params Func<Instruction, bool>[] predicates)
		{
			if (!this.TryFindPrev(out cursors, predicates))
			{
				throw new KeyNotFoundException();
			}
		}

		public bool TryFindPrev(out ILCursor[] cursors, params Func<Instruction, bool>[] predicates)
		{
			Helpers.ThrowIfArgumentNull<Func<Instruction, bool>[]>(predicates, "predicates");
			cursors = new ILCursor[predicates.Length];
			ILCursor ilcursor = this;
			for (int i = predicates.Length - 1; i >= 0; i--)
			{
				ilcursor = ilcursor.Clone();
				if (!ilcursor.TryGotoPrev(new Func<Instruction, bool>[] { predicates[i] }))
				{
					return false;
				}
				cursors[i] = ilcursor;
			}
			return true;
		}

		[NullableContext(2)]
		public void MarkLabel(ILLabel label)
		{
			if (label == null)
			{
				label = new ILLabel(this.Context);
			}
			label.Target = this.Next;
			if (this._afterLabels != null)
			{
				Array.Resize<ILLabel>(ref this._afterLabels, this._afterLabels.Length + 1);
				this._afterLabels[this._afterLabels.Length - 1] = label;
				return;
			}
			this._afterLabels = new ILLabel[] { label };
		}

		public ILLabel MarkLabel(Instruction inst)
		{
			ILLabel illabel = this.Context.DefineLabel();
			if (inst == this.Next)
			{
				this.MarkLabel(illabel);
				return illabel;
			}
			illabel.Target = inst;
			return illabel;
		}

		public ILLabel MarkLabel()
		{
			ILLabel illabel = this.DefineLabel();
			this.MarkLabel(illabel);
			return illabel;
		}

		public ILLabel DefineLabel()
		{
			return this.Context.DefineLabel();
		}

		public VariableDefinition CreateLocal<[Nullable(2)] T>()
		{
			return this.Context.CreateLocal<T>();
		}

		public VariableDefinition CreateLocal(Type type)
		{
			return this.Context.CreateLocal(type);
		}

		public VariableDefinition CreateLocal(TypeReference typeRef)
		{
			return this.Context.CreateLocal(typeRef);
		}

		private ILCursor _Insert(Instruction instr)
		{
			if (this._afterLabels != null)
			{
				ILLabel[] afterLabels = this._afterLabels;
				for (int i = 0; i < afterLabels.Length; i++)
				{
					afterLabels[i].Target = instr;
				}
			}
			if (this._afterHandlerStarts)
			{
				foreach (ExceptionHandler exceptionHandler in this.Body.ExceptionHandlers)
				{
					if (exceptionHandler.TryStart == this.Next)
					{
						exceptionHandler.TryStart = instr;
					}
					if (exceptionHandler.HandlerStart == this.Next)
					{
						exceptionHandler.HandlerStart = instr;
					}
					if (exceptionHandler.FilterStart == this.Next)
					{
						exceptionHandler.FilterStart = instr;
					}
				}
			}
			if (this._afterHandlerEnds)
			{
				foreach (ExceptionHandler exceptionHandler2 in this.Body.ExceptionHandlers)
				{
					if (exceptionHandler2.TryEnd == this.Next)
					{
						exceptionHandler2.TryEnd = instr;
					}
					if (exceptionHandler2.HandlerEnd == this.Next)
					{
						exceptionHandler2.HandlerEnd = instr;
					}
				}
			}
			this.Instrs.Insert(this.Index, instr);
			this.Goto(instr, MoveType.After, false);
			return this;
		}

		public ILCursor Remove()
		{
			return this.RemoveRange(1);
		}

		public ILCursor RemoveRange(int num)
		{
			int index = this.Index;
			Instruction instruction = ((index + num < this.Instrs.Count) ? this.Instrs[index + num] : null);
			foreach (ILLabel illabel in this.IncomingLabels)
			{
				illabel.Target = instruction;
			}
			using (Collection<ExceptionHandler>.Enumerator enumerator2 = this.Body.ExceptionHandlers.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					ExceptionHandler exceptionHandler = enumerator2.Current;
					if (exceptionHandler.TryStart == this.Next)
					{
						exceptionHandler.TryStart = instruction;
					}
					if (exceptionHandler.TryEnd == this.Next)
					{
						exceptionHandler.TryEnd = instruction;
					}
					if (exceptionHandler.HandlerStart == this.Next)
					{
						exceptionHandler.HandlerStart = instruction;
					}
					if (exceptionHandler.FilterStart == this.Next)
					{
						exceptionHandler.FilterStart = instruction;
					}
					if (exceptionHandler.HandlerEnd == this.Next)
					{
						exceptionHandler.HandlerEnd = instruction;
					}
				}
				goto IL_010E;
			}
			IL_0102:
			this.Instrs.RemoveAt(index);
			IL_010E:
			if (num-- <= 0)
			{
				this._searchTarget = SearchTarget.None;
				this._next = instruction;
				return this;
			}
			goto IL_0102;
		}

		public ILCursor Emit(OpCode opcode, ParameterDefinition parameter)
		{
			return this._Insert(this.IL.Create(opcode, parameter));
		}

		public ILCursor Emit(OpCode opcode, VariableDefinition variable)
		{
			return this._Insert(this.IL.Create(opcode, variable));
		}

		public ILCursor Emit(OpCode opcode, Instruction[] targets)
		{
			return this._Insert(this.IL.Create(opcode, targets));
		}

		public ILCursor Emit(OpCode opcode, Instruction target)
		{
			return this._Insert(this.IL.Create(opcode, target));
		}

		public ILCursor Emit(OpCode opcode, double value)
		{
			return this._Insert(this.IL.Create(opcode, value));
		}

		public ILCursor Emit(OpCode opcode, float value)
		{
			return this._Insert(this.IL.Create(opcode, value));
		}

		public ILCursor Emit(OpCode opcode, long value)
		{
			return this._Insert(this.IL.Create(opcode, value));
		}

		public ILCursor Emit(OpCode opcode, sbyte value)
		{
			return this._Insert(this.IL.Create(opcode, value));
		}

		public ILCursor Emit(OpCode opcode, byte value)
		{
			return this._Insert(this.IL.Create(opcode, value));
		}

		public ILCursor Emit(OpCode opcode, string value)
		{
			return this._Insert(this.IL.Create(opcode, value));
		}

		public ILCursor Emit(OpCode opcode, FieldReference field)
		{
			return this._Insert(this.IL.Create(opcode, field));
		}

		public ILCursor Emit(OpCode opcode, Mono.Cecil.CallSite site)
		{
			return this._Insert(this.IL.Create(opcode, site));
		}

		public ILCursor Emit(OpCode opcode, TypeReference type)
		{
			return this._Insert(this.IL.Create(opcode, type));
		}

		public ILCursor Emit(OpCode opcode)
		{
			return this._Insert(this.IL.Create(opcode));
		}

		public ILCursor Emit(OpCode opcode, int value)
		{
			return this._Insert(this.IL.Create(opcode, value));
		}

		public ILCursor Emit(OpCode opcode, MethodReference method)
		{
			return this._Insert(this.IL.Create(opcode, method));
		}

		public ILCursor Emit(OpCode opcode, FieldInfo field)
		{
			return this._Insert(this.IL.Create(opcode, field));
		}

		public ILCursor Emit(OpCode opcode, MethodBase method)
		{
			return this._Insert(this.IL.Create(opcode, method));
		}

		public ILCursor Emit(OpCode opcode, Type type)
		{
			return this._Insert(this.IL.Create(opcode, type));
		}

		public ILCursor Emit(OpCode opcode, object operand)
		{
			return this._Insert(this.IL.Create(opcode, operand));
		}

		public ILCursor Emit<[Nullable(2)] T>(OpCode opcode, string memberName)
		{
			return this._Insert(this.IL.Create(opcode, typeof(T).GetMember(memberName, (BindingFlags)(-1)).First<MemberInfo>()));
		}

		public int AddReference<[Nullable(2)] T>(in T t)
		{
			return this.Context.AddReference<T>(in t);
		}

		[NullableContext(2)]
		public void EmitGetReference<T>(int id)
		{
			this.EmitLoadTypedReference(this.Context.GetReferenceCell(id), typeof(T));
		}

		[NullableContext(2)]
		public int EmitReference<T>(in T t)
		{
			int num = this.AddReference<T>(in t);
			this.EmitLoadTypedReferenceUnsafe(this.Context.GetReferenceCell(num), typeof(T));
			return num;
		}

		public int EmitDelegate<[Nullable(0)] T>(T cb) where T : Delegate
		{
			Helpers.ThrowIfArgumentNull<T>(cb, "cb");
			if (cb.GetInvocationList().Length == 1 && cb.Target == null)
			{
				this.Emit(OpCodes.Call, cb.Method);
				return -1;
			}
			ValueTuple<MethodInfo, Type>? delegateInvoker = FastDelegateInvokers.GetDelegateInvoker(typeof(T));
			int num;
			if (delegateInvoker != null)
			{
				ValueTuple<MethodInfo, Type> valueOrDefault = delegateInvoker.GetValueOrDefault();
				Delegate @delegate = cb.CastDelegate(valueOrDefault.Item2);
				num = this.EmitReference<Delegate>(in @delegate);
				this.AddReference<MethodInfo>(in valueOrDefault.Item1);
				this.Emit(OpCodes.Call, valueOrDefault.Item1);
			}
			else
			{
				num = this.EmitReference<T>(in cb);
				MethodInfo method = typeof(T).GetMethod("Invoke");
				this.Emit(OpCodes.Callvirt, method);
			}
			return num;
		}

		public ILCursor EmitAdd()
		{
			return this._Insert(this.IL.Create(OpCodes.Add));
		}

		public ILCursor EmitAddOvf()
		{
			return this._Insert(this.IL.Create(OpCodes.Add_Ovf));
		}

		public ILCursor EmitAddOvfUn()
		{
			return this._Insert(this.IL.Create(OpCodes.Add_Ovf_Un));
		}

		public ILCursor EmitAnd()
		{
			return this._Insert(this.IL.Create(OpCodes.And));
		}

		public ILCursor EmitArglist()
		{
			return this._Insert(this.IL.Create(OpCodes.Arglist));
		}

		public ILCursor EmitBeq(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Beq, operand));
		}

		public ILCursor EmitBeq(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Beq, this.MarkLabel(operand)));
		}

		public ILCursor EmitBge(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Bge, operand));
		}

		public ILCursor EmitBge(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Bge, this.MarkLabel(operand)));
		}

		public ILCursor EmitBgeUn(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Bge_Un, operand));
		}

		public ILCursor EmitBgeUn(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Bge_Un, this.MarkLabel(operand)));
		}

		public ILCursor EmitBgt(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Bgt, operand));
		}

		public ILCursor EmitBgt(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Bgt, this.MarkLabel(operand)));
		}

		public ILCursor EmitBgtUn(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Bgt_Un, operand));
		}

		public ILCursor EmitBgtUn(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Bgt_Un, this.MarkLabel(operand)));
		}

		public ILCursor EmitBle(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ble, operand));
		}

		public ILCursor EmitBle(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ble, this.MarkLabel(operand)));
		}

		public ILCursor EmitBleUn(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ble_Un, operand));
		}

		public ILCursor EmitBleUn(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ble_Un, this.MarkLabel(operand)));
		}

		public ILCursor EmitBlt(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Blt, operand));
		}

		public ILCursor EmitBlt(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Blt, this.MarkLabel(operand)));
		}

		public ILCursor EmitBltUn(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Blt_Un, operand));
		}

		public ILCursor EmitBltUn(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Blt_Un, this.MarkLabel(operand)));
		}

		public ILCursor EmitBneUn(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Bne_Un, operand));
		}

		public ILCursor EmitBneUn(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Bne_Un, this.MarkLabel(operand)));
		}

		public ILCursor EmitBox(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Box, operand));
		}

		public ILCursor EmitBox(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Box, this.Context.Import(operand)));
		}

		public ILCursor EmitBr(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Br, operand));
		}

		public ILCursor EmitBr(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Br, this.MarkLabel(operand)));
		}

		public ILCursor EmitBreak()
		{
			return this._Insert(this.IL.Create(OpCodes.Break));
		}

		public ILCursor EmitBrfalse(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Brfalse, operand));
		}

		public ILCursor EmitBrfalse(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Brfalse, this.MarkLabel(operand)));
		}

		public ILCursor EmitBrtrue(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Brtrue, operand));
		}

		public ILCursor EmitBrtrue(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Brtrue, this.MarkLabel(operand)));
		}

		public ILCursor EmitCall(MethodReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Call, operand));
		}

		public ILCursor EmitCall(MethodBase operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Call, this.Context.Import(operand)));
		}

		public ILCursor EmitCalli(IMethodSignature operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Calli, operand));
		}

		public ILCursor EmitCallvirt(MethodReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Callvirt, operand));
		}

		public ILCursor EmitCallvirt(MethodBase operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Callvirt, this.Context.Import(operand)));
		}

		public ILCursor EmitCastclass(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Castclass, operand));
		}

		public ILCursor EmitCastclass(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Castclass, this.Context.Import(operand)));
		}

		public ILCursor EmitCeq()
		{
			return this._Insert(this.IL.Create(OpCodes.Ceq));
		}

		public ILCursor EmitCgt()
		{
			return this._Insert(this.IL.Create(OpCodes.Cgt));
		}

		public ILCursor EmitCgtUn()
		{
			return this._Insert(this.IL.Create(OpCodes.Cgt_Un));
		}

		public ILCursor EmitCkfinite()
		{
			return this._Insert(this.IL.Create(OpCodes.Ckfinite));
		}

		public ILCursor EmitClt()
		{
			return this._Insert(this.IL.Create(OpCodes.Clt));
		}

		public ILCursor EmitCltUn()
		{
			return this._Insert(this.IL.Create(OpCodes.Clt_Un));
		}

		public ILCursor EmitConstrained(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Constrained, operand));
		}

		public ILCursor EmitConstrained(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Constrained, this.Context.Import(operand)));
		}

		public ILCursor EmitConvI()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_I));
		}

		public ILCursor EmitConvI1()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_I1));
		}

		public ILCursor EmitConvI2()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_I2));
		}

		public ILCursor EmitConvI4()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_I4));
		}

		public ILCursor EmitConvI8()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_I8));
		}

		public ILCursor EmitConvOvfI()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_I));
		}

		public ILCursor EmitConvOvfIUn()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_I_Un));
		}

		public ILCursor EmitConvOvfI1()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_I1));
		}

		public ILCursor EmitConvOvfI1Un()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_I1_Un));
		}

		public ILCursor EmitConvOvfI2()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_I2));
		}

		public ILCursor EmitConvOvfI2Un()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_I2_Un));
		}

		public ILCursor EmitConvOvfI4()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_I4));
		}

		public ILCursor EmitConvOvfI4Un()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_I4_Un));
		}

		public ILCursor EmitConvOvfI8()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_I8));
		}

		public ILCursor EmitConvOvfI8Un()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_I8_Un));
		}

		public ILCursor EmitConvOvfU()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_U));
		}

		public ILCursor EmitConvOvfUUn()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_U_Un));
		}

		public ILCursor EmitConvOvfU1()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_U1));
		}

		public ILCursor EmitConvOvfU1Un()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_U1_Un));
		}

		public ILCursor EmitConvOvfU2()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_U2));
		}

		public ILCursor EmitConvOvfU2Un()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_U2_Un));
		}

		public ILCursor EmitConvOvfU4()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_U4));
		}

		public ILCursor EmitConvOvfU4Un()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_U4_Un));
		}

		public ILCursor EmitConvOvfU8()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_U8));
		}

		public ILCursor EmitConvOvfU8Un()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_Ovf_U8_Un));
		}

		public ILCursor EmitConvRUn()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_R_Un));
		}

		public ILCursor EmitConvR4()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_R4));
		}

		public ILCursor EmitConvR8()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_R8));
		}

		public ILCursor EmitConvU()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_U));
		}

		public ILCursor EmitConvU1()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_U1));
		}

		public ILCursor EmitConvU2()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_U2));
		}

		public ILCursor EmitConvU4()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_U4));
		}

		public ILCursor EmitConvU8()
		{
			return this._Insert(this.IL.Create(OpCodes.Conv_U8));
		}

		public ILCursor EmitCpblk()
		{
			return this._Insert(this.IL.Create(OpCodes.Cpblk));
		}

		public ILCursor EmitCpobj(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Cpobj, operand));
		}

		public ILCursor EmitCpobj(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Cpobj, this.Context.Import(operand)));
		}

		public ILCursor EmitDiv()
		{
			return this._Insert(this.IL.Create(OpCodes.Div));
		}

		public ILCursor EmitDivUn()
		{
			return this._Insert(this.IL.Create(OpCodes.Div_Un));
		}

		public ILCursor EmitDup()
		{
			return this._Insert(this.IL.Create(OpCodes.Dup));
		}

		public ILCursor EmitEndfilter()
		{
			return this._Insert(this.IL.Create(OpCodes.Endfilter));
		}

		public ILCursor EmitEndfinally()
		{
			return this._Insert(this.IL.Create(OpCodes.Endfinally));
		}

		public ILCursor EmitInitblk()
		{
			return this._Insert(this.IL.Create(OpCodes.Initblk));
		}

		public ILCursor EmitInitobj(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Initobj, operand));
		}

		public ILCursor EmitInitobj(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Initobj, this.Context.Import(operand)));
		}

		public ILCursor EmitIsinst(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Isinst, operand));
		}

		public ILCursor EmitIsinst(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Isinst, this.Context.Import(operand)));
		}

		public ILCursor EmitJmp(MethodReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Jmp, operand));
		}

		public ILCursor EmitJmp(MethodBase operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Jmp, this.Context.Import(operand)));
		}

		public ILCursor EmitLdarg0()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldarg_0));
		}

		public ILCursor EmitLdarg1()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldarg_1));
		}

		public ILCursor EmitLdarg2()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldarg_2));
		}

		public ILCursor EmitLdarg3()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldarg_3));
		}

		public ILCursor EmitLdarg(int operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldarg, operand));
		}

		public ILCursor EmitLdarg(uint operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldarg, (int)operand));
		}

		public ILCursor EmitLdarg(ParameterReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldarg, operand));
		}

		public ILCursor EmitLdarga(int operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldarga, operand));
		}

		public ILCursor EmitLdarga(uint operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldarga, (int)operand));
		}

		public ILCursor EmitLdarga(ParameterReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldarga, operand));
		}

		public ILCursor EmitLdcI4(int operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldc_I4, operand));
		}

		public ILCursor EmitLdcI4(uint operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldc_I4, (int)operand));
		}

		public ILCursor EmitLdcI8(long operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldc_I8, operand));
		}

		public ILCursor EmitLdcI8(ulong operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldc_I8, (long)operand));
		}

		public ILCursor EmitLdcR4(float operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldc_R4, operand));
		}

		public ILCursor EmitLdcR8(double operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldc_R8, operand));
		}

		public ILCursor EmitLdelemAny(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_Any, operand));
		}

		public ILCursor EmitLdelemAny(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_Any, this.Context.Import(operand)));
		}

		public ILCursor EmitLdelemI()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_I));
		}

		public ILCursor EmitLdelemI1()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_I1));
		}

		public ILCursor EmitLdelemI2()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_I2));
		}

		public ILCursor EmitLdelemI4()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_I4));
		}

		public ILCursor EmitLdelemI8()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_I8));
		}

		public ILCursor EmitLdelemR4()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_R4));
		}

		public ILCursor EmitLdelemR8()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_R8));
		}

		public ILCursor EmitLdelemRef()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_Ref));
		}

		public ILCursor EmitLdelemU1()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_U1));
		}

		public ILCursor EmitLdelemU2()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_U2));
		}

		public ILCursor EmitLdelemU4()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelem_U4));
		}

		public ILCursor EmitLdelema(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelema, operand));
		}

		public ILCursor EmitLdelema(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldelema, this.Context.Import(operand)));
		}

		public ILCursor EmitLdfld(FieldReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldfld, operand));
		}

		public ILCursor EmitLdfld(FieldInfo operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldfld, this.Context.Import(operand)));
		}

		public ILCursor EmitLdflda(FieldReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldflda, operand));
		}

		public ILCursor EmitLdflda(FieldInfo operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldflda, this.Context.Import(operand)));
		}

		public ILCursor EmitLdftn(MethodReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldftn, operand));
		}

		public ILCursor EmitLdftn(MethodBase operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldftn, this.Context.Import(operand)));
		}

		public ILCursor EmitLdindI()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldind_I));
		}

		public ILCursor EmitLdindI1()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldind_I1));
		}

		public ILCursor EmitLdindI2()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldind_I2));
		}

		public ILCursor EmitLdindI4()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldind_I4));
		}

		public ILCursor EmitLdindI8()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldind_I8));
		}

		public ILCursor EmitLdindR4()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldind_R4));
		}

		public ILCursor EmitLdindR8()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldind_R8));
		}

		public ILCursor EmitLdindRef()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldind_Ref));
		}

		public ILCursor EmitLdindU1()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldind_U1));
		}

		public ILCursor EmitLdindU2()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldind_U2));
		}

		public ILCursor EmitLdindU4()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldind_U4));
		}

		public ILCursor EmitLdlen()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldlen));
		}

		public ILCursor EmitLdloc0()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldloc_0));
		}

		public ILCursor EmitLdloc1()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldloc_1));
		}

		public ILCursor EmitLdloc2()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldloc_2));
		}

		public ILCursor EmitLdloc3()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldloc_3));
		}

		public ILCursor EmitLdloc(int operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldloc, operand));
		}

		public ILCursor EmitLdloc(uint operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldloc, (int)operand));
		}

		public ILCursor EmitLdloc(VariableReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldloc, operand));
		}

		public ILCursor EmitLdloca(int operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldloca, operand));
		}

		public ILCursor EmitLdloca(uint operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldloca, (int)operand));
		}

		public ILCursor EmitLdloca(VariableReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldloca, operand));
		}

		public ILCursor EmitLdnull()
		{
			return this._Insert(this.IL.Create(OpCodes.Ldnull));
		}

		public ILCursor EmitLdobj(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldobj, operand));
		}

		public ILCursor EmitLdobj(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldobj, this.Context.Import(operand)));
		}

		public ILCursor EmitLdsfld(FieldReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldsfld, operand));
		}

		public ILCursor EmitLdsfld(FieldInfo operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldsfld, this.Context.Import(operand)));
		}

		public ILCursor EmitLdsflda(FieldReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldsflda, operand));
		}

		public ILCursor EmitLdsflda(FieldInfo operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldsflda, this.Context.Import(operand)));
		}

		public ILCursor EmitLdstr(string operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldstr, operand));
		}

		public ILCursor EmitLdtoken(IMetadataTokenProvider operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldtoken, operand));
		}

		public ILCursor EmitLdtoken(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldtoken, this.Context.Import(operand)));
		}

		public ILCursor EmitLdtoken(FieldInfo operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldtoken, this.Context.Import(operand)));
		}

		public ILCursor EmitLdtoken(MethodBase operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldtoken, this.Context.Import(operand)));
		}

		public ILCursor EmitLdvirtftn(MethodReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldvirtftn, operand));
		}

		public ILCursor EmitLdvirtftn(MethodBase operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Ldvirtftn, this.Context.Import(operand)));
		}

		public ILCursor EmitLeave(ILLabel operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Leave, operand));
		}

		public ILCursor EmitLeave(Instruction operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Leave, this.MarkLabel(operand)));
		}

		public ILCursor EmitLocalloc()
		{
			return this._Insert(this.IL.Create(OpCodes.Localloc));
		}

		public ILCursor EmitMkrefany(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Mkrefany, operand));
		}

		public ILCursor EmitMkrefany(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Mkrefany, this.Context.Import(operand)));
		}

		public ILCursor EmitMul()
		{
			return this._Insert(this.IL.Create(OpCodes.Mul));
		}

		public ILCursor EmitMulOvf()
		{
			return this._Insert(this.IL.Create(OpCodes.Mul_Ovf));
		}

		public ILCursor EmitMulOvfUn()
		{
			return this._Insert(this.IL.Create(OpCodes.Mul_Ovf_Un));
		}

		public ILCursor EmitNeg()
		{
			return this._Insert(this.IL.Create(OpCodes.Neg));
		}

		public ILCursor EmitNewarr(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Newarr, operand));
		}

		public ILCursor EmitNewarr(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Newarr, this.Context.Import(operand)));
		}

		public ILCursor EmitNewobj(MethodReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Newobj, operand));
		}

		public ILCursor EmitNewobj(MethodBase operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Newobj, this.Context.Import(operand)));
		}

		public ILCursor EmitNop()
		{
			return this._Insert(this.IL.Create(OpCodes.Nop));
		}

		public ILCursor EmitNot()
		{
			return this._Insert(this.IL.Create(OpCodes.Not));
		}

		public ILCursor EmitOr()
		{
			return this._Insert(this.IL.Create(OpCodes.Or));
		}

		public ILCursor EmitPop()
		{
			return this._Insert(this.IL.Create(OpCodes.Pop));
		}

		public ILCursor EmitReadonly()
		{
			return this._Insert(this.IL.Create(OpCodes.Readonly));
		}

		public ILCursor EmitRefanytype()
		{
			return this._Insert(this.IL.Create(OpCodes.Refanytype));
		}

		public ILCursor EmitRefanyval(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Refanyval, operand));
		}

		public ILCursor EmitRefanyval(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Refanyval, this.Context.Import(operand)));
		}

		public ILCursor EmitRem()
		{
			return this._Insert(this.IL.Create(OpCodes.Rem));
		}

		public ILCursor EmitRemUn()
		{
			return this._Insert(this.IL.Create(OpCodes.Rem_Un));
		}

		public ILCursor EmitRet()
		{
			return this._Insert(this.IL.Create(OpCodes.Ret));
		}

		public ILCursor EmitRethrow()
		{
			return this._Insert(this.IL.Create(OpCodes.Rethrow));
		}

		public ILCursor EmitShl()
		{
			return this._Insert(this.IL.Create(OpCodes.Shl));
		}

		public ILCursor EmitShr()
		{
			return this._Insert(this.IL.Create(OpCodes.Shr));
		}

		public ILCursor EmitShrUn()
		{
			return this._Insert(this.IL.Create(OpCodes.Shr_Un));
		}

		public ILCursor EmitSizeof(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Sizeof, operand));
		}

		public ILCursor EmitSizeof(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Sizeof, this.Context.Import(operand)));
		}

		public ILCursor EmitStarg(int operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Starg, operand));
		}

		public ILCursor EmitStarg(uint operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Starg, (int)operand));
		}

		public ILCursor EmitStarg(ParameterReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Starg, operand));
		}

		public ILCursor EmitStelemAny(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Stelem_Any, operand));
		}

		public ILCursor EmitStelemAny(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Stelem_Any, this.Context.Import(operand)));
		}

		public ILCursor EmitStelemI()
		{
			return this._Insert(this.IL.Create(OpCodes.Stelem_I));
		}

		public ILCursor EmitStelemI1()
		{
			return this._Insert(this.IL.Create(OpCodes.Stelem_I1));
		}

		public ILCursor EmitStelemI2()
		{
			return this._Insert(this.IL.Create(OpCodes.Stelem_I2));
		}

		public ILCursor EmitStelemI4()
		{
			return this._Insert(this.IL.Create(OpCodes.Stelem_I4));
		}

		public ILCursor EmitStelemI8()
		{
			return this._Insert(this.IL.Create(OpCodes.Stelem_I8));
		}

		public ILCursor EmitStelemR4()
		{
			return this._Insert(this.IL.Create(OpCodes.Stelem_R4));
		}

		public ILCursor EmitStelemR8()
		{
			return this._Insert(this.IL.Create(OpCodes.Stelem_R8));
		}

		public ILCursor EmitStelemRef()
		{
			return this._Insert(this.IL.Create(OpCodes.Stelem_Ref));
		}

		public ILCursor EmitStfld(FieldReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Stfld, operand));
		}

		public ILCursor EmitStfld(FieldInfo operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Stfld, this.Context.Import(operand)));
		}

		public ILCursor EmitStindI()
		{
			return this._Insert(this.IL.Create(OpCodes.Stind_I));
		}

		public ILCursor EmitStindI1()
		{
			return this._Insert(this.IL.Create(OpCodes.Stind_I1));
		}

		public ILCursor EmitStindI2()
		{
			return this._Insert(this.IL.Create(OpCodes.Stind_I2));
		}

		public ILCursor EmitStindI4()
		{
			return this._Insert(this.IL.Create(OpCodes.Stind_I4));
		}

		public ILCursor EmitStindI8()
		{
			return this._Insert(this.IL.Create(OpCodes.Stind_I8));
		}

		public ILCursor EmitStindR4()
		{
			return this._Insert(this.IL.Create(OpCodes.Stind_R4));
		}

		public ILCursor EmitStindR8()
		{
			return this._Insert(this.IL.Create(OpCodes.Stind_R8));
		}

		public ILCursor EmitStindRef()
		{
			return this._Insert(this.IL.Create(OpCodes.Stind_Ref));
		}

		public ILCursor EmitStloc0()
		{
			return this._Insert(this.IL.Create(OpCodes.Stloc_0));
		}

		public ILCursor EmitStloc1()
		{
			return this._Insert(this.IL.Create(OpCodes.Stloc_1));
		}

		public ILCursor EmitStloc2()
		{
			return this._Insert(this.IL.Create(OpCodes.Stloc_2));
		}

		public ILCursor EmitStloc3()
		{
			return this._Insert(this.IL.Create(OpCodes.Stloc_3));
		}

		public ILCursor EmitStloc(int operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Stloc, operand));
		}

		public ILCursor EmitStloc(uint operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Stloc, (int)operand));
		}

		public ILCursor EmitStloc(VariableReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Stloc, operand));
		}

		public ILCursor EmitStobj(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Stobj, operand));
		}

		public ILCursor EmitStobj(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Stobj, this.Context.Import(operand)));
		}

		public ILCursor EmitStsfld(FieldReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Stsfld, operand));
		}

		public ILCursor EmitStsfld(FieldInfo operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Stsfld, this.Context.Import(operand)));
		}

		public ILCursor EmitSub()
		{
			return this._Insert(this.IL.Create(OpCodes.Sub));
		}

		public ILCursor EmitSubOvf()
		{
			return this._Insert(this.IL.Create(OpCodes.Sub_Ovf));
		}

		public ILCursor EmitSubOvfUn()
		{
			return this._Insert(this.IL.Create(OpCodes.Sub_Ovf_Un));
		}

		public ILCursor EmitSwitch(ILLabel[] operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Switch, operand));
		}

		public ILCursor EmitSwitch(Instruction[] operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Switch, operand.Select<Instruction, ILLabel>(new Func<Instruction, ILLabel>(this.MarkLabel)).ToArray<ILLabel>()));
		}

		public ILCursor EmitTail()
		{
			return this._Insert(this.IL.Create(OpCodes.Tail));
		}

		public ILCursor EmitThrow()
		{
			return this._Insert(this.IL.Create(OpCodes.Throw));
		}

		public ILCursor EmitUnaligned(byte operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Unaligned, operand));
		}

		public ILCursor EmitUnbox(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Unbox, operand));
		}

		public ILCursor EmitUnbox(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Unbox, this.Context.Import(operand)));
		}

		public ILCursor EmitUnboxAny(TypeReference operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Unbox_Any, operand));
		}

		public ILCursor EmitUnboxAny(Type operand)
		{
			return this._Insert(this.IL.Create(OpCodes.Unbox_Any, this.Context.Import(operand)));
		}

		public ILCursor EmitVolatile()
		{
			return this._Insert(this.IL.Create(OpCodes.Volatile));
		}

		public ILCursor EmitXor()
		{
			return this._Insert(this.IL.Create(OpCodes.Xor));
		}

		[Nullable(2)]
		private Instruction _next;

		[Nullable(new byte[] { 2, 1 })]
		private ILLabel[] _afterLabels;

		private bool _afterHandlerStarts;

		private bool _afterHandlerEnds;

		private SearchTarget _searchTarget;
	}
}
