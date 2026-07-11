using System;
using System.Collections;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_ILGenerator))]
	[ClassInterface(ClassInterfaceType.None)]
	public class ILGenerator : _ILGenerator
	{
		internal ILGenerator(Module m, TokenGenerator token_gen, int size)
		{
			if (size < 0)
			{
				size = 128;
			}
			this.code = new byte[size];
			this.token_fixups = new ILTokenInfo[8];
			this.module = m;
			this.token_gen = token_gen;
		}

		void _ILGenerator.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _ILGenerator.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _ILGenerator.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _ILGenerator.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		private void add_token_fixup(MemberInfo mi)
		{
			if (this.num_token_fixups == this.token_fixups.Length)
			{
				ILTokenInfo[] array = new ILTokenInfo[this.num_token_fixups * 2];
				this.token_fixups.CopyTo(array, 0);
				this.token_fixups = array;
			}
			this.token_fixups[this.num_token_fixups].member = mi;
			this.token_fixups[this.num_token_fixups++].code_pos = this.code_len;
		}

		private void make_room(int nbytes)
		{
			if (this.code_len + nbytes < this.code.Length)
			{
				return;
			}
			byte[] array = new byte[(this.code_len + nbytes) * 2 + 128];
			Array.Copy(this.code, 0, array, 0, this.code.Length);
			this.code = array;
		}

		private void emit_int(int val)
		{
			this.code[this.code_len++] = (byte)(val & 255);
			this.code[this.code_len++] = (byte)((val >> 8) & 255);
			this.code[this.code_len++] = (byte)((val >> 16) & 255);
			this.code[this.code_len++] = (byte)((val >> 24) & 255);
		}

		private void ll_emit(OpCode opcode)
		{
			if (opcode.Size == 2)
			{
				this.code[this.code_len++] = opcode.op1;
			}
			this.code[this.code_len++] = opcode.op2;
			switch (opcode.StackBehaviourPush)
			{
			case StackBehaviour.Push1:
			case StackBehaviour.Pushi:
			case StackBehaviour.Pushi8:
			case StackBehaviour.Pushr4:
			case StackBehaviour.Pushr8:
			case StackBehaviour.Pushref:
			case StackBehaviour.Varpush:
				this.cur_stack++;
				break;
			case StackBehaviour.Push1_push1:
				this.cur_stack += 2;
				break;
			}
			if (this.max_stack < this.cur_stack)
			{
				this.max_stack = this.cur_stack;
			}
			switch (opcode.StackBehaviourPop)
			{
			case StackBehaviour.Pop1:
			case StackBehaviour.Popi:
			case StackBehaviour.Popref:
				this.cur_stack--;
				break;
			case StackBehaviour.Pop1_pop1:
			case StackBehaviour.Popi_pop1:
			case StackBehaviour.Popi_popi:
			case StackBehaviour.Popi_popi8:
			case StackBehaviour.Popi_popr4:
			case StackBehaviour.Popi_popr8:
			case StackBehaviour.Popref_pop1:
			case StackBehaviour.Popref_popi:
				this.cur_stack -= 2;
				break;
			case StackBehaviour.Popi_popi_popi:
			case StackBehaviour.Popref_popi_popi:
			case StackBehaviour.Popref_popi_popi8:
			case StackBehaviour.Popref_popi_popr4:
			case StackBehaviour.Popref_popi_popr8:
			case StackBehaviour.Popref_popi_popref:
				this.cur_stack -= 3;
				break;
			}
		}

		private static int target_len(OpCode opcode)
		{
			if (opcode.OperandType == OperandType.InlineBrTarget)
			{
				return 4;
			}
			return 1;
		}

		private void InternalEndClause()
		{
			int num = this.ex_handlers[this.cur_block].LastClauseType();
			switch (num + 1)
			{
			case 0:
			case 1:
			case 2:
				this.Emit(OpCodes.Leave, this.ex_handlers[this.cur_block].end);
				break;
			case 3:
			case 5:
				this.Emit(OpCodes.Endfinally);
				break;
			}
		}

		public virtual void BeginCatchBlock(Type exceptionType)
		{
			if (this.open_blocks == null)
			{
				this.open_blocks = new Stack(2);
			}
			if (this.open_blocks.Count <= 0)
			{
				throw new NotSupportedException("Not in an exception block");
			}
			if (exceptionType != null && exceptionType.IsUserType)
			{
				throw new NotSupportedException("User defined subclasses of System.Type are not yet supported.");
			}
			if (this.ex_handlers[this.cur_block].LastClauseType() == -1)
			{
				if (exceptionType != null)
				{
					throw new ArgumentException("Do not supply an exception type for filter clause");
				}
				this.Emit(OpCodes.Endfilter);
				this.ex_handlers[this.cur_block].PatchFilterClause(this.code_len);
			}
			else
			{
				this.InternalEndClause();
				this.ex_handlers[this.cur_block].AddCatch(exceptionType, this.code_len);
			}
			this.cur_stack = 1;
			if (this.max_stack < this.cur_stack)
			{
				this.max_stack = this.cur_stack;
			}
		}

		public virtual void BeginExceptFilterBlock()
		{
			if (this.open_blocks == null)
			{
				this.open_blocks = new Stack(2);
			}
			if (this.open_blocks.Count <= 0)
			{
				throw new NotSupportedException("Not in an exception block");
			}
			this.InternalEndClause();
			this.ex_handlers[this.cur_block].AddFilter(this.code_len);
		}

		public virtual Label BeginExceptionBlock()
		{
			if (this.open_blocks == null)
			{
				this.open_blocks = new Stack(2);
			}
			if (this.ex_handlers != null)
			{
				this.cur_block = this.ex_handlers.Length;
				ILExceptionInfo[] array = new ILExceptionInfo[this.cur_block + 1];
				Array.Copy(this.ex_handlers, array, this.cur_block);
				this.ex_handlers = array;
			}
			else
			{
				this.ex_handlers = new ILExceptionInfo[1];
				this.cur_block = 0;
			}
			this.open_blocks.Push(this.cur_block);
			this.ex_handlers[this.cur_block].start = this.code_len;
			return this.ex_handlers[this.cur_block].end = this.DefineLabel();
		}

		public virtual void BeginFaultBlock()
		{
			if (this.open_blocks == null)
			{
				this.open_blocks = new Stack(2);
			}
			if (this.open_blocks.Count <= 0)
			{
				throw new NotSupportedException("Not in an exception block");
			}
			if (this.ex_handlers[this.cur_block].LastClauseType() == -1)
			{
				this.Emit(OpCodes.Leave, this.ex_handlers[this.cur_block].end);
				this.ex_handlers[this.cur_block].PatchFilterClause(this.code_len);
			}
			this.InternalEndClause();
			this.ex_handlers[this.cur_block].AddFault(this.code_len);
		}

		public virtual void BeginFinallyBlock()
		{
			if (this.open_blocks == null)
			{
				this.open_blocks = new Stack(2);
			}
			if (this.open_blocks.Count <= 0)
			{
				throw new NotSupportedException("Not in an exception block");
			}
			this.InternalEndClause();
			if (this.ex_handlers[this.cur_block].LastClauseType() == -1)
			{
				this.Emit(OpCodes.Leave, this.ex_handlers[this.cur_block].end);
				this.ex_handlers[this.cur_block].PatchFilterClause(this.code_len);
			}
			this.ex_handlers[this.cur_block].AddFinally(this.code_len);
		}

		public virtual void BeginScope()
		{
		}

		public virtual LocalBuilder DeclareLocal(Type localType)
		{
			return this.DeclareLocal(localType, false);
		}

		public virtual LocalBuilder DeclareLocal(Type localType, bool pinned)
		{
			if (localType == null)
			{
				throw new ArgumentNullException("localType");
			}
			if (localType.IsUserType)
			{
				throw new NotSupportedException("User defined subclasses of System.Type are not yet supported.");
			}
			LocalBuilder localBuilder = new LocalBuilder(localType, this);
			localBuilder.is_pinned = pinned;
			if (this.locals != null)
			{
				LocalBuilder[] array = new LocalBuilder[this.locals.Length + 1];
				Array.Copy(this.locals, array, this.locals.Length);
				array[this.locals.Length] = localBuilder;
				this.locals = array;
			}
			else
			{
				this.locals = new LocalBuilder[1];
				this.locals[0] = localBuilder;
			}
			localBuilder.position = (ushort)(this.locals.Length - 1);
			return localBuilder;
		}

		public virtual Label DefineLabel()
		{
			if (this.labels == null)
			{
				this.labels = new ILGenerator.LabelData[4];
			}
			else if (this.num_labels >= this.labels.Length)
			{
				ILGenerator.LabelData[] array = new ILGenerator.LabelData[this.labels.Length * 2];
				Array.Copy(this.labels, array, this.labels.Length);
				this.labels = array;
			}
			this.labels[this.num_labels] = new ILGenerator.LabelData(-1, 0);
			return new Label(this.num_labels++);
		}

		public virtual void Emit(OpCode opcode)
		{
			this.make_room(2);
			this.ll_emit(opcode);
		}

		public virtual void Emit(OpCode opcode, byte arg)
		{
			this.make_room(3);
			this.ll_emit(opcode);
			this.code[this.code_len++] = arg;
		}

		[ComVisible(true)]
		public virtual void Emit(OpCode opcode, ConstructorInfo con)
		{
			int token = this.token_gen.GetToken(con);
			this.make_room(6);
			this.ll_emit(opcode);
			if (con.DeclaringType.Module == this.module)
			{
				this.add_token_fixup(con);
			}
			this.emit_int(token);
			if (opcode.StackBehaviourPop == StackBehaviour.Varpop)
			{
				this.cur_stack -= con.GetParameterCount();
			}
		}

		public virtual void Emit(OpCode opcode, double arg)
		{
			byte[] bytes = BitConverter.GetBytes(arg);
			this.make_room(10);
			this.ll_emit(opcode);
			if (BitConverter.IsLittleEndian)
			{
				Array.Copy(bytes, 0, this.code, this.code_len, 8);
				this.code_len += 8;
			}
			else
			{
				this.code[this.code_len++] = bytes[7];
				this.code[this.code_len++] = bytes[6];
				this.code[this.code_len++] = bytes[5];
				this.code[this.code_len++] = bytes[4];
				this.code[this.code_len++] = bytes[3];
				this.code[this.code_len++] = bytes[2];
				this.code[this.code_len++] = bytes[1];
				this.code[this.code_len++] = bytes[0];
			}
		}

		public virtual void Emit(OpCode opcode, FieldInfo field)
		{
			int token = this.token_gen.GetToken(field);
			this.make_room(6);
			this.ll_emit(opcode);
			if (field.DeclaringType.Module == this.module)
			{
				this.add_token_fixup(field);
			}
			this.emit_int(token);
		}

		public virtual void Emit(OpCode opcode, short arg)
		{
			this.make_room(4);
			this.ll_emit(opcode);
			this.code[this.code_len++] = (byte)(arg & 255);
			this.code[this.code_len++] = (byte)((arg >> 8) & 255);
		}

		public virtual void Emit(OpCode opcode, int arg)
		{
			this.make_room(6);
			this.ll_emit(opcode);
			this.emit_int(arg);
		}

		public virtual void Emit(OpCode opcode, long arg)
		{
			this.make_room(10);
			this.ll_emit(opcode);
			this.code[this.code_len++] = (byte)(arg & 255L);
			this.code[this.code_len++] = (byte)((arg >> 8) & 255L);
			this.code[this.code_len++] = (byte)((arg >> 16) & 255L);
			this.code[this.code_len++] = (byte)((arg >> 24) & 255L);
			this.code[this.code_len++] = (byte)((arg >> 32) & 255L);
			this.code[this.code_len++] = (byte)((arg >> 40) & 255L);
			this.code[this.code_len++] = (byte)((arg >> 48) & 255L);
			this.code[this.code_len++] = (byte)((arg >> 56) & 255L);
		}

		public virtual void Emit(OpCode opcode, Label label)
		{
			int num = ILGenerator.target_len(opcode);
			this.make_room(6);
			this.ll_emit(opcode);
			if (this.cur_stack > this.labels[label.label].maxStack)
			{
				this.labels[label.label].maxStack = this.cur_stack;
			}
			if (this.fixups == null)
			{
				this.fixups = new ILGenerator.LabelFixup[4];
			}
			else if (this.num_fixups >= this.fixups.Length)
			{
				ILGenerator.LabelFixup[] array = new ILGenerator.LabelFixup[this.fixups.Length * 2];
				Array.Copy(this.fixups, array, this.fixups.Length);
				this.fixups = array;
			}
			this.fixups[this.num_fixups].offset = num;
			this.fixups[this.num_fixups].pos = this.code_len;
			this.fixups[this.num_fixups].label_idx = label.label;
			this.num_fixups++;
			this.code_len += num;
		}

		public virtual void Emit(OpCode opcode, Label[] labels)
		{
			if (labels == null)
			{
				throw new ArgumentNullException("labels");
			}
			int num = labels.Length;
			this.make_room(6 + num * 4);
			this.ll_emit(opcode);
			for (int i = 0; i < num; i++)
			{
				if (this.cur_stack > this.labels[labels[i].label].maxStack)
				{
					this.labels[labels[i].label].maxStack = this.cur_stack;
				}
			}
			this.emit_int(num);
			if (this.fixups == null)
			{
				this.fixups = new ILGenerator.LabelFixup[4 + num];
			}
			else if (this.num_fixups + num >= this.fixups.Length)
			{
				ILGenerator.LabelFixup[] array = new ILGenerator.LabelFixup[num + this.fixups.Length * 2];
				Array.Copy(this.fixups, array, this.fixups.Length);
				this.fixups = array;
			}
			int j = 0;
			int num2 = num * 4;
			while (j < num)
			{
				this.fixups[this.num_fixups].offset = num2;
				this.fixups[this.num_fixups].pos = this.code_len;
				this.fixups[this.num_fixups].label_idx = labels[j].label;
				this.num_fixups++;
				this.code_len += 4;
				j++;
				num2 -= 4;
			}
		}

		public virtual void Emit(OpCode opcode, LocalBuilder local)
		{
			if (local == null)
			{
				throw new ArgumentNullException("local");
			}
			uint position = (uint)local.position;
			bool flag = false;
			bool flag2 = false;
			this.make_room(6);
			if (local.ilgen != this)
			{
				throw new ArgumentException("Trying to emit a local from a different ILGenerator.");
			}
			if (opcode.StackBehaviourPop == StackBehaviour.Pop1)
			{
				this.cur_stack--;
				flag2 = true;
			}
			else
			{
				this.cur_stack++;
				if (this.cur_stack > this.max_stack)
				{
					this.max_stack = this.cur_stack;
				}
				flag = opcode.StackBehaviourPush == StackBehaviour.Pushi;
			}
			if (flag)
			{
				if (position < 256U)
				{
					this.code[this.code_len++] = 18;
					this.code[this.code_len++] = (byte)position;
				}
				else
				{
					this.code[this.code_len++] = 254;
					this.code[this.code_len++] = 13;
					this.code[this.code_len++] = (byte)(position & 255U);
					this.code[this.code_len++] = (byte)((position >> 8) & 255U);
				}
			}
			else if (flag2)
			{
				if (position < 4U)
				{
					this.code[this.code_len++] = (byte)(10U + position);
				}
				else if (position < 256U)
				{
					this.code[this.code_len++] = 19;
					this.code[this.code_len++] = (byte)position;
				}
				else
				{
					this.code[this.code_len++] = 254;
					this.code[this.code_len++] = 14;
					this.code[this.code_len++] = (byte)(position & 255U);
					this.code[this.code_len++] = (byte)((position >> 8) & 255U);
				}
			}
			else if (position < 4U)
			{
				this.code[this.code_len++] = (byte)(6U + position);
			}
			else if (position < 256U)
			{
				this.code[this.code_len++] = 17;
				this.code[this.code_len++] = (byte)position;
			}
			else
			{
				this.code[this.code_len++] = 254;
				this.code[this.code_len++] = 12;
				this.code[this.code_len++] = (byte)(position & 255U);
				this.code[this.code_len++] = (byte)((position >> 8) & 255U);
			}
		}

		public virtual void Emit(OpCode opcode, MethodInfo meth)
		{
			if (meth == null)
			{
				throw new ArgumentNullException("meth");
			}
			if (meth is DynamicMethod && (opcode == OpCodes.Ldftn || opcode == OpCodes.Ldvirtftn || opcode == OpCodes.Ldtoken))
			{
				throw new ArgumentException("Ldtoken, Ldftn and Ldvirtftn OpCodes cannot target DynamicMethods.");
			}
			int token = this.token_gen.GetToken(meth);
			this.make_room(6);
			this.ll_emit(opcode);
			Type declaringType = meth.DeclaringType;
			if (declaringType != null && declaringType.Module == this.module)
			{
				this.add_token_fixup(meth);
			}
			this.emit_int(token);
			if (meth.ReturnType != ILGenerator.void_type)
			{
				this.cur_stack++;
			}
			if (opcode.StackBehaviourPop == StackBehaviour.Varpop)
			{
				this.cur_stack -= meth.GetParameterCount();
			}
		}

		private void Emit(OpCode opcode, MethodInfo method, int token)
		{
			this.make_room(6);
			this.ll_emit(opcode);
			Type declaringType = method.DeclaringType;
			if (declaringType != null && declaringType.Module == this.module)
			{
				this.add_token_fixup(method);
			}
			this.emit_int(token);
			if (method.ReturnType != ILGenerator.void_type)
			{
				this.cur_stack++;
			}
			if (opcode.StackBehaviourPop == StackBehaviour.Varpop)
			{
				this.cur_stack -= method.GetParameterCount();
			}
		}

		[CLSCompliant(false)]
		public void Emit(OpCode opcode, sbyte arg)
		{
			this.make_room(3);
			this.ll_emit(opcode);
			this.code[this.code_len++] = (byte)arg;
		}

		public virtual void Emit(OpCode opcode, SignatureHelper signature)
		{
			int token = this.token_gen.GetToken(signature);
			this.make_room(6);
			this.ll_emit(opcode);
			this.emit_int(token);
		}

		public virtual void Emit(OpCode opcode, float arg)
		{
			byte[] bytes = BitConverter.GetBytes(arg);
			this.make_room(6);
			this.ll_emit(opcode);
			if (BitConverter.IsLittleEndian)
			{
				Array.Copy(bytes, 0, this.code, this.code_len, 4);
				this.code_len += 4;
			}
			else
			{
				this.code[this.code_len++] = bytes[3];
				this.code[this.code_len++] = bytes[2];
				this.code[this.code_len++] = bytes[1];
				this.code[this.code_len++] = bytes[0];
			}
		}

		public virtual void Emit(OpCode opcode, string str)
		{
			int token = this.token_gen.GetToken(str);
			this.make_room(6);
			this.ll_emit(opcode);
			this.emit_int(token);
		}

		public virtual void Emit(OpCode opcode, Type cls)
		{
			this.make_room(6);
			this.ll_emit(opcode);
			this.emit_int(this.token_gen.GetToken(cls));
		}

		[MonoLimitation("vararg methods are not supported")]
		public virtual void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			short value = opcode.Value;
			if (value != OpCodes.Call.Value && value != OpCodes.Callvirt.Value)
			{
				throw new NotSupportedException("Only Call and CallVirt are allowed");
			}
			if ((methodInfo.CallingConvention & CallingConventions.VarArgs) == (CallingConventions)0)
			{
				optionalParameterTypes = null;
			}
			if (optionalParameterTypes == null)
			{
				this.Emit(opcode, methodInfo);
				return;
			}
			if ((methodInfo.CallingConvention & CallingConventions.VarArgs) == (CallingConventions)0)
			{
				throw new InvalidOperationException("Method is not VarArgs method and optional types were passed");
			}
			int token = this.token_gen.GetToken(methodInfo, optionalParameterTypes);
			this.Emit(opcode, methodInfo, token);
		}

		public virtual void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
		{
			SignatureHelper methodSigHelper = SignatureHelper.GetMethodSigHelper(this.module, (CallingConventions)0, unmanagedCallConv, returnType, parameterTypes);
			this.Emit(opcode, methodSigHelper);
		}

		public virtual void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			if (optionalParameterTypes != null)
			{
				throw new NotImplementedException();
			}
			SignatureHelper methodSigHelper = SignatureHelper.GetMethodSigHelper(this.module, callingConvention, (CallingConvention)0, returnType, parameterTypes);
			this.Emit(opcode, methodSigHelper);
		}

		public virtual void EmitWriteLine(FieldInfo fld)
		{
			if (fld == null)
			{
				throw new ArgumentNullException("fld");
			}
			if (fld.IsStatic)
			{
				this.Emit(OpCodes.Ldsfld, fld);
			}
			else
			{
				this.Emit(OpCodes.Ldarg_0);
				this.Emit(OpCodes.Ldfld, fld);
			}
			this.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { fld.FieldType }));
		}

		public virtual void EmitWriteLine(LocalBuilder localBuilder)
		{
			if (localBuilder == null)
			{
				throw new ArgumentNullException("localBuilder");
			}
			if (localBuilder.LocalType is TypeBuilder)
			{
				throw new ArgumentException("Output streams do not support TypeBuilders.");
			}
			this.Emit(OpCodes.Ldloc, localBuilder);
			this.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { localBuilder.LocalType }));
		}

		public virtual void EmitWriteLine(string value)
		{
			this.Emit(OpCodes.Ldstr, value);
			this.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
		}

		public virtual void EndExceptionBlock()
		{
			if (this.open_blocks == null)
			{
				this.open_blocks = new Stack(2);
			}
			if (this.open_blocks.Count <= 0)
			{
				throw new NotSupportedException("Not in an exception block");
			}
			if (this.ex_handlers[this.cur_block].LastClauseType() == -1)
			{
				throw new InvalidOperationException("Incorrect code generation for exception block.");
			}
			this.InternalEndClause();
			this.MarkLabel(this.ex_handlers[this.cur_block].end);
			this.ex_handlers[this.cur_block].End(this.code_len);
			this.ex_handlers[this.cur_block].Debug(this.cur_block);
			this.open_blocks.Pop();
			if (this.open_blocks.Count > 0)
			{
				this.cur_block = (int)this.open_blocks.Peek();
			}
		}

		public virtual void EndScope()
		{
		}

		public virtual void MarkLabel(Label loc)
		{
			if (loc.label < 0 || loc.label >= this.num_labels)
			{
				throw new ArgumentException("The label is not valid");
			}
			if (this.labels[loc.label].addr >= 0)
			{
				throw new ArgumentException("The label was already defined");
			}
			this.labels[loc.label].addr = this.code_len;
			if (this.labels[loc.label].maxStack > this.cur_stack)
			{
				this.cur_stack = this.labels[loc.label].maxStack;
			}
		}

		public virtual void MarkSequencePoint(ISymbolDocumentWriter document, int startLine, int startColumn, int endLine, int endColumn)
		{
			if (this.currentSequence == null || this.currentSequence.Document != document)
			{
				if (this.sequencePointLists == null)
				{
					this.sequencePointLists = new ArrayList();
				}
				this.currentSequence = new SequencePointList(document);
				this.sequencePointLists.Add(this.currentSequence);
			}
			this.currentSequence.AddSequencePoint(this.code_len, startLine, startColumn, endLine, endColumn);
		}

		internal void GenerateDebugInfo(ISymbolWriter symbolWriter)
		{
			if (this.sequencePointLists != null)
			{
				SequencePointList sequencePointList = (SequencePointList)this.sequencePointLists[0];
				SequencePointList sequencePointList2 = (SequencePointList)this.sequencePointLists[this.sequencePointLists.Count - 1];
				symbolWriter.SetMethodSourceRange(sequencePointList.Document, sequencePointList.StartLine, sequencePointList.StartColumn, sequencePointList2.Document, sequencePointList2.EndLine, sequencePointList2.EndColumn);
				foreach (object obj in this.sequencePointLists)
				{
					SequencePointList sequencePointList3 = (SequencePointList)obj;
					symbolWriter.DefineSequencePoints(sequencePointList3.Document, sequencePointList3.GetOffsets(), sequencePointList3.GetLines(), sequencePointList3.GetColumns(), sequencePointList3.GetEndLines(), sequencePointList3.GetEndColumns());
				}
				if (this.locals != null)
				{
					foreach (LocalBuilder localBuilder in this.locals)
					{
						if (localBuilder.Name != null && localBuilder.Name.Length > 0)
						{
							SignatureHelper localVarSigHelper = SignatureHelper.GetLocalVarSigHelper(this.module);
							localVarSigHelper.AddArgument(localBuilder.LocalType);
							byte[] signature = localVarSigHelper.GetSignature();
							symbolWriter.DefineLocalVariable(localBuilder.Name, FieldAttributes.Public, signature, SymAddressKind.ILOffset, (int)localBuilder.position, 0, 0, localBuilder.StartOffset, localBuilder.EndOffset);
						}
					}
				}
				this.sequencePointLists = null;
			}
		}

		internal bool HasDebugInfo
		{
			get
			{
				return this.sequencePointLists != null;
			}
		}

		public virtual void ThrowException(Type excType)
		{
			if (excType == null)
			{
				throw new ArgumentNullException("excType");
			}
			if (excType != typeof(Exception) && !excType.IsSubclassOf(typeof(Exception)))
			{
				throw new ArgumentException("Type should be an exception type", "excType");
			}
			ConstructorInfo constructor = excType.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
			{
				throw new ArgumentException("Type should have a default constructor", "excType");
			}
			this.Emit(OpCodes.Newobj, constructor);
			this.Emit(OpCodes.Throw);
		}

		[MonoTODO("Not implemented")]
		public virtual void UsingNamespace(string usingNamespace)
		{
			throw new NotImplementedException();
		}

		internal void label_fixup()
		{
			for (int i = 0; i < this.num_fixups; i++)
			{
				if (this.labels[this.fixups[i].label_idx].addr < 0)
				{
					throw new ArgumentException("Label not marked");
				}
				int num = this.labels[this.fixups[i].label_idx].addr - (this.fixups[i].pos + this.fixups[i].offset);
				if (this.fixups[i].offset == 1)
				{
					this.code[this.fixups[i].pos] = (byte)((sbyte)num);
				}
				else
				{
					int num2 = this.code_len;
					this.code_len = this.fixups[i].pos;
					this.emit_int(num);
					this.code_len = num2;
				}
			}
		}

		[Obsolete("Use ILOffset")]
		internal static int Mono_GetCurrentOffset(ILGenerator ig)
		{
			return ig.code_len;
		}

		private const int defaultFixupSize = 4;

		private const int defaultLabelsSize = 4;

		private const int defaultExceptionStackSize = 2;

		private static readonly Type void_type = typeof(void);

		private byte[] code;

		private int code_len;

		private int max_stack;

		private int cur_stack;

		private LocalBuilder[] locals;

		private ILExceptionInfo[] ex_handlers;

		private int num_token_fixups;

		private ILTokenInfo[] token_fixups;

		private ILGenerator.LabelData[] labels;

		private int num_labels;

		private ILGenerator.LabelFixup[] fixups;

		private int num_fixups;

		internal Module module;

		private int cur_block;

		private Stack open_blocks;

		private TokenGenerator token_gen;

		private ArrayList sequencePointLists;

		private SequencePointList currentSequence;

		private struct LabelFixup
		{
			public int offset;

			public int pos;

			public int label_idx;
		}

		private struct LabelData
		{
			public LabelData(int addr, int maxStack)
			{
				this.addr = addr;
				this.maxStack = maxStack;
			}

			public int addr;

			public int maxStack;
		}
	}
}
