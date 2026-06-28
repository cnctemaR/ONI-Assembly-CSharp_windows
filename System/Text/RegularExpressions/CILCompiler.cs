using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace System.Text.RegularExpressions
{
	internal class CILCompiler : RxCompiler, ICompiler
	{
		public CILCompiler()
		{
			this.generic_ops = new Dictionary<int, int>();
			this.op_flags = new Dictionary<int, int>();
		}

		IMachineFactory ICompiler.GetMachineFactory()
		{
			byte[] array = new byte[this.curpos];
			Buffer.BlockCopy(this.program, 0, array, 0, this.curpos);
			this.eval_methods = new DynamicMethod[array.Length];
			this.eval_methods_defined = new bool[array.Length];
			DynamicMethod evalMethod = this.GetEvalMethod(array, 11);
			if (evalMethod != null)
			{
				return new RxInterpreterFactory(array, (EvalDelegate)evalMethod.CreateDelegate(typeof(EvalDelegate)));
			}
			return new RxInterpreterFactory(array, null);
		}

		private DynamicMethod GetEvalMethod(byte[] program, int pc)
		{
			if (this.eval_methods_defined[pc])
			{
				return this.eval_methods[pc];
			}
			this.eval_methods_defined[pc] = true;
			this.eval_methods[pc] = this.CreateEvalMethod(program, pc);
			return this.eval_methods[pc];
		}

		private MethodInfo GetMethod(Type t, string name, ref MethodInfo cached)
		{
			if (cached == null)
			{
				cached = t.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (cached == null)
				{
					throw new Exception("Method not found: " + name);
				}
			}
			return cached;
		}

		private MethodInfo GetMethod(string name, ref MethodInfo cached)
		{
			return this.GetMethod(typeof(RxInterpreter), name, ref cached);
		}

		private int ReadInt(byte[] code, int pc)
		{
			int num = (int)code[pc];
			num |= (int)code[pc + 1] << 8;
			num |= (int)code[pc + 2] << 16;
			return num | ((int)code[pc + 3] << 24);
		}

		private static OpFlags MakeFlags(bool negate, bool ignore, bool reverse, bool lazy)
		{
			OpFlags opFlags = OpFlags.None;
			if (negate)
			{
				opFlags |= OpFlags.Negate;
			}
			if (ignore)
			{
				opFlags |= OpFlags.IgnoreCase;
			}
			if (reverse)
			{
				opFlags |= OpFlags.RightToLeft;
			}
			if (lazy)
			{
				opFlags |= OpFlags.Lazy;
			}
			return opFlags;
		}

		private void EmitGenericOp(RxOp op, bool negate, bool ignore, bool reverse, bool lazy)
		{
			this.generic_ops[this.curpos] = (int)op;
			this.op_flags[this.curpos] = (int)CILCompiler.MakeFlags(negate, ignore, reverse, false);
		}

		public override void EmitOp(RxOp op, bool negate, bool ignore, bool reverse)
		{
			this.EmitGenericOp(op, negate, ignore, reverse, false);
			base.EmitOp(op, negate, ignore, reverse);
		}

		public override void EmitOpIgnoreReverse(RxOp op, bool ignore, bool reverse)
		{
			this.EmitGenericOp(op, false, ignore, reverse, false);
			base.EmitOpIgnoreReverse(op, ignore, reverse);
		}

		public override void EmitOpNegateReverse(RxOp op, bool negate, bool reverse)
		{
			this.EmitGenericOp(op, negate, false, reverse, false);
			base.EmitOpNegateReverse(op, negate, reverse);
		}

		private DynamicMethod CreateEvalMethod(byte[] program, int pc)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("Eval_" + pc, typeof(bool), new Type[]
			{
				typeof(RxInterpreter),
				typeof(int),
				typeof(int).MakeByRefType()
			}, typeof(RxInterpreter), true);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			CILCompiler.Frame frame = new CILCompiler.Frame(ilgenerator);
			this.local_textinfo = ilgenerator.DeclareLocal(typeof(TextInfo));
			ilgenerator.Emit(OpCodes.Call, typeof(Thread).GetMethod("get_CurrentThread"));
			ilgenerator.Emit(OpCodes.Call, typeof(Thread).GetMethod("get_CurrentCulture"));
			ilgenerator.Emit(OpCodes.Call, typeof(CultureInfo).GetMethod("get_TextInfo"));
			ilgenerator.Emit(OpCodes.Stloc, this.local_textinfo);
			dynamicMethod = this.EmitEvalMethodBody(dynamicMethod, ilgenerator, frame, program, pc, program.Length, false, false, out pc);
			if (dynamicMethod == null)
			{
				return null;
			}
			ilgenerator.MarkLabel(frame.label_pass);
			ilgenerator.Emit(OpCodes.Ldarg_2);
			ilgenerator.Emit(OpCodes.Ldarg_1);
			ilgenerator.Emit(OpCodes.Stind_I4);
			ilgenerator.Emit(OpCodes.Ldc_I4_1);
			ilgenerator.Emit(OpCodes.Ret);
			ilgenerator.MarkLabel(frame.label_fail);
			ilgenerator.Emit(OpCodes.Ldc_I4_0);
			ilgenerator.Emit(OpCodes.Ret);
			return dynamicMethod;
		}

		private int ReadShort(byte[] program, int pc)
		{
			return (int)program[pc] | ((int)program[pc + 1] << 8);
		}

		private Label CreateLabelForPC(ILGenerator ilgen, int pc)
		{
			if (this.labels == null)
			{
				this.labels = new Dictionary<int, Label>();
			}
			Label label;
			if (!this.labels.TryGetValue(pc, out label))
			{
				label = ilgen.DefineLabel();
				this.labels[pc] = label;
			}
			return label;
		}

		private int GetILOffset(ILGenerator ilgen)
		{
			return (int)typeof(ILGenerator).GetField("code_len", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ilgen);
		}

		private DynamicMethod EmitEvalMethodBody(DynamicMethod m, ILGenerator ilgen, CILCompiler.Frame frame, byte[] program, int pc, int end_pc, bool one_op, bool no_bump, out int out_pc)
		{
			out_pc = 0;
			int num = 1 + this.ReadShort(program, 1);
			while (pc < end_pc)
			{
				RxOp rxOp = (RxOp)program[pc];
				if (this.generic_ops.ContainsKey(pc))
				{
					rxOp = (RxOp)this.generic_ops[pc];
				}
				if (CILCompiler.trace_compile)
				{
					Console.WriteLine("compiling {0} pc={1} end_pc={2}, il_offset=0x{3:x}", new object[]
					{
						rxOp,
						pc,
						end_pc,
						this.GetILOffset(ilgen)
					});
				}
				Label label;
				if (this.labels != null && this.labels.TryGetValue(pc, out label))
				{
					ilgen.MarkLabel(label);
					this.labels.Remove(pc);
				}
				if (RxInterpreter.trace_rx)
				{
					ilgen.Emit(OpCodes.Ldstr, "evaluating: {0} at pc: {1}, strpos: {2}");
					ilgen.Emit(OpCodes.Ldc_I4, (int)rxOp);
					ilgen.Emit(OpCodes.Box, typeof(RxOp));
					ilgen.Emit(OpCodes.Ldc_I4, pc);
					ilgen.Emit(OpCodes.Box, typeof(int));
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Box, typeof(int));
					ilgen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[]
					{
						typeof(string),
						typeof(object),
						typeof(object),
						typeof(object)
					}));
				}
				RxOp rxOp2 = rxOp;
				switch (rxOp2)
				{
				case RxOp.False:
					ilgen.Emit(OpCodes.Br, frame.label_fail);
					pc++;
					break;
				case RxOp.True:
					ilgen.Emit(OpCodes.Br, frame.label_pass);
					pc++;
					break;
				case RxOp.AnyPosition:
					pc++;
					break;
				case RxOp.StartOfString:
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Bgt, frame.label_fail);
					pc++;
					break;
				case RxOp.StartOfLine:
				{
					Label label2 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Beq, label2);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Sub);
					ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
					ilgen.Emit(OpCodes.Ldc_I4, 10);
					ilgen.Emit(OpCodes.Beq, label2);
					ilgen.Emit(OpCodes.Br, frame.label_fail);
					ilgen.MarkLabel(label2);
					pc++;
					break;
				}
				case RxOp.StartOfScan:
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_start);
					ilgen.Emit(OpCodes.Bne_Un, frame.label_fail);
					pc++;
					break;
				case RxOp.EndOfString:
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
					ilgen.Emit(OpCodes.Bne_Un, frame.label_fail);
					pc++;
					break;
				case RxOp.EndOfLine:
				{
					Label label3 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
					ilgen.Emit(OpCodes.Beq, label3);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
					ilgen.Emit(OpCodes.Ldc_I4, 10);
					ilgen.Emit(OpCodes.Beq, label3);
					ilgen.Emit(OpCodes.Br, frame.label_fail);
					ilgen.MarkLabel(label3);
					pc++;
					break;
				}
				case RxOp.End:
				{
					Label label4 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
					ilgen.Emit(OpCodes.Beq, label4);
					Label label5 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Sub);
					ilgen.Emit(OpCodes.Bne_Un, label5);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
					ilgen.Emit(OpCodes.Ldc_I4, 10);
					ilgen.Emit(OpCodes.Bne_Un, label5);
					ilgen.Emit(OpCodes.Br, label4);
					ilgen.MarkLabel(label5);
					ilgen.Emit(OpCodes.Br, frame.label_fail);
					ilgen.MarkLabel(label4);
					pc++;
					break;
				}
				case RxOp.WordBoundary:
				case RxOp.NoWordBoundary:
				{
					bool flag = rxOp == RxOp.NoWordBoundary;
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Beq, frame.label_fail);
					Label label6 = ilgen.DefineLabel();
					Label label7 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Bne_Un, label7);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
					ilgen.Emit(OpCodes.Call, this.GetMethod("IsWordChar", ref CILCompiler.mi_is_word_char));
					ilgen.Emit((!flag) ? OpCodes.Brfalse : OpCodes.Brtrue, frame.label_fail);
					ilgen.Emit(OpCodes.Br, label6);
					ilgen.MarkLabel(label7);
					Label label8 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
					ilgen.Emit(OpCodes.Bne_Un, label8);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Sub);
					ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
					ilgen.Emit(OpCodes.Call, this.GetMethod("IsWordChar", ref CILCompiler.mi_is_word_char));
					ilgen.Emit((!flag) ? OpCodes.Brfalse : OpCodes.Brtrue, frame.label_fail);
					ilgen.Emit(OpCodes.Br, label6);
					ilgen.MarkLabel(label8);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
					ilgen.Emit(OpCodes.Call, this.GetMethod("IsWordChar", ref CILCompiler.mi_is_word_char));
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Sub);
					ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
					ilgen.Emit(OpCodes.Call, this.GetMethod("IsWordChar", ref CILCompiler.mi_is_word_char));
					ilgen.Emit((!flag) ? OpCodes.Beq : OpCodes.Bne_Un, frame.label_fail);
					ilgen.Emit(OpCodes.Br, label6);
					ilgen.MarkLabel(label6);
					pc++;
					break;
				}
				case RxOp.String:
				case RxOp.UnicodeString:
				{
					OpFlags opFlags = (OpFlags)this.op_flags[pc];
					bool flag2 = (ushort)(opFlags & OpFlags.IgnoreCase) > 0;
					bool flag3 = (ushort)(opFlags & OpFlags.RightToLeft) > 0;
					bool flag4 = rxOp == RxOp.UnicodeString;
					int i;
					int num2;
					if (flag4)
					{
						i = pc + 3;
						num2 = this.ReadShort(program, pc + 1);
					}
					else
					{
						i = pc + 2;
						num2 = (int)program[pc + 1];
					}
					if (flag3)
					{
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Ldc_I4, num2);
						ilgen.Emit(OpCodes.Blt, frame.label_fail);
					}
					else
					{
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Ldc_I4, num2);
						ilgen.Emit(OpCodes.Add);
						ilgen.Emit(OpCodes.Ldarg_0);
						ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
						ilgen.Emit(OpCodes.Bgt, frame.label_fail);
					}
					LocalBuilder localBuilder = ilgen.DeclareLocal(typeof(string));
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
					ilgen.Emit(OpCodes.Stloc, localBuilder);
					if (flag3)
					{
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Ldc_I4, num2);
						ilgen.Emit(OpCodes.Sub);
						ilgen.Emit(OpCodes.Starg, 1);
					}
					int num3 = i + ((!flag4) ? num2 : (num2 * 2));
					while (i < num3)
					{
						if (flag2)
						{
							ilgen.Emit(OpCodes.Ldloc, this.local_textinfo);
						}
						ilgen.Emit(OpCodes.Ldloc, localBuilder);
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
						if (flag2)
						{
							ilgen.Emit(OpCodes.Callvirt, typeof(TextInfo).GetMethod("ToLower", new Type[] { typeof(char) }));
						}
						ilgen.Emit(OpCodes.Ldc_I4, (!flag4) ? ((int)program[i]) : this.ReadShort(program, i));
						ilgen.Emit(OpCodes.Bne_Un, frame.label_fail);
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Ldc_I4_1);
						ilgen.Emit(OpCodes.Add);
						ilgen.Emit(OpCodes.Starg, 1);
						if (flag4)
						{
							i += 2;
						}
						else
						{
							i++;
						}
					}
					if (flag3)
					{
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Ldc_I4, num2);
						ilgen.Emit(OpCodes.Sub);
						ilgen.Emit(OpCodes.Starg, 1);
					}
					pc = num3;
					break;
				}
				default:
					switch (rxOp2)
					{
					case RxOp.Reference:
					{
						OpFlags opFlags2 = (OpFlags)this.op_flags[pc];
						bool flag5 = (ushort)(opFlags2 & OpFlags.IgnoreCase) > 0;
						bool flag6 = (ushort)(opFlags2 & OpFlags.RightToLeft) > 0;
						LocalBuilder localBuilder2 = ilgen.DeclareLocal(typeof(int));
						ilgen.Emit(OpCodes.Ldarg_0);
						ilgen.Emit(OpCodes.Ldc_I4, this.ReadShort(program, pc + 1));
						ilgen.Emit(OpCodes.Call, this.GetMethod(typeof(RxInterpreter), "GetLastDefined", ref CILCompiler.mi_get_last_defined));
						ilgen.Emit(OpCodes.Stloc, localBuilder2);
						ilgen.Emit(OpCodes.Ldloc, localBuilder2);
						ilgen.Emit(OpCodes.Ldc_I4_0);
						ilgen.Emit(OpCodes.Blt, frame.label_fail);
						LocalBuilder localBuilder3 = ilgen.DeclareLocal(typeof(int));
						ilgen.Emit(OpCodes.Ldarg_0);
						ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_marks);
						ilgen.Emit(OpCodes.Ldloc, localBuilder2);
						ilgen.Emit(OpCodes.Ldelema, typeof(Mark));
						ilgen.Emit(OpCodes.Call, this.GetMethod(typeof(Mark), "get_Index", ref CILCompiler.mi_mark_get_index));
						ilgen.Emit(OpCodes.Stloc, localBuilder3);
						ilgen.Emit(OpCodes.Ldarg_0);
						ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_marks);
						ilgen.Emit(OpCodes.Ldloc, localBuilder2);
						ilgen.Emit(OpCodes.Ldelema, typeof(Mark));
						ilgen.Emit(OpCodes.Call, this.GetMethod(typeof(Mark), "get_Length", ref CILCompiler.mi_mark_get_length));
						ilgen.Emit(OpCodes.Stloc, localBuilder2);
						if (flag6)
						{
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Ldloc, localBuilder2);
							ilgen.Emit(OpCodes.Sub);
							ilgen.Emit(OpCodes.Starg, 1);
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Ldc_I4_0);
							ilgen.Emit(OpCodes.Blt, frame.label_fail);
						}
						else
						{
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Ldloc, localBuilder2);
							ilgen.Emit(OpCodes.Add);
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
							ilgen.Emit(OpCodes.Bgt, frame.label_fail);
						}
						LocalBuilder localBuilder4 = ilgen.DeclareLocal(typeof(string));
						ilgen.Emit(OpCodes.Ldarg_0);
						ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
						ilgen.Emit(OpCodes.Stloc, localBuilder4);
						LocalBuilder localBuilder5 = ilgen.DeclareLocal(typeof(int));
						ilgen.Emit(OpCodes.Ldloc, localBuilder3);
						ilgen.Emit(OpCodes.Ldloc, localBuilder2);
						ilgen.Emit(OpCodes.Add);
						ilgen.Emit(OpCodes.Stloc, localBuilder5);
						Label label9 = ilgen.DefineLabel();
						ilgen.Emit(OpCodes.Br, label9);
						Label label10 = ilgen.DefineLabel();
						ilgen.MarkLabel(label10);
						if (flag5)
						{
							ilgen.Emit(OpCodes.Ldloc, this.local_textinfo);
						}
						ilgen.Emit(OpCodes.Ldloc, localBuilder4);
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
						if (flag5)
						{
							ilgen.Emit(OpCodes.Callvirt, typeof(TextInfo).GetMethod("ToLower", new Type[] { typeof(char) }));
						}
						if (flag5)
						{
							ilgen.Emit(OpCodes.Ldloc, this.local_textinfo);
						}
						ilgen.Emit(OpCodes.Ldloc, localBuilder4);
						ilgen.Emit(OpCodes.Ldloc, localBuilder3);
						ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
						if (flag5)
						{
							ilgen.Emit(OpCodes.Callvirt, typeof(TextInfo).GetMethod("ToLower", new Type[] { typeof(char) }));
						}
						ilgen.Emit(OpCodes.Bne_Un, frame.label_fail);
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Ldc_I4_1);
						ilgen.Emit(OpCodes.Add);
						ilgen.Emit(OpCodes.Starg, 1);
						ilgen.Emit(OpCodes.Ldloc, localBuilder3);
						ilgen.Emit(OpCodes.Ldc_I4_1);
						ilgen.Emit(OpCodes.Add);
						ilgen.Emit(OpCodes.Stloc, localBuilder3);
						ilgen.MarkLabel(label9);
						ilgen.Emit(OpCodes.Ldloc, localBuilder3);
						ilgen.Emit(OpCodes.Ldloc, localBuilder5);
						ilgen.Emit(OpCodes.Blt, label10);
						if (flag6)
						{
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Ldloc, localBuilder2);
							ilgen.Emit(OpCodes.Sub);
							ilgen.Emit(OpCodes.Starg, 1);
						}
						pc += 3;
						break;
					}
					default:
					{
						if (rxOp2 != RxOp.Bitmap)
						{
							if (rxOp2 == RxOp.UnicodeChar || rxOp2 == RxOp.UnicodeRange)
							{
								goto IL_0935;
							}
							if (rxOp2 != RxOp.UnicodeBitmap)
							{
								if (rxOp2 != RxOp.CategoryAny && rxOp2 != RxOp.CategoryAnySingleline && rxOp2 != RxOp.CategoryDigit && rxOp2 != RxOp.CategoryWord && rxOp2 != RxOp.CategoryWhiteSpace && rxOp2 != RxOp.CategoryEcmaWord && rxOp2 != RxOp.CategoryEcmaWhiteSpace && rxOp2 != RxOp.CategoryUnicode && rxOp2 != RxOp.CategoryUnicodeSpecials)
								{
									throw new NotImplementedException("Opcode '" + rxOp + "' not supported by the regex->IL compiler.");
								}
								OpFlags opFlags3 = (OpFlags)this.op_flags[pc];
								bool flag7 = (ushort)(opFlags3 & OpFlags.Negate) > 0;
								bool flag8 = (ushort)(opFlags3 & OpFlags.RightToLeft) > 0;
								Label label11 = ilgen.DefineLabel();
								if (flag8)
								{
									ilgen.Emit(OpCodes.Ldarg_1);
									ilgen.Emit(OpCodes.Ldc_I4_0);
									ilgen.Emit(OpCodes.Ble, label11);
								}
								else
								{
									ilgen.Emit(OpCodes.Ldarg_1);
									ilgen.Emit(OpCodes.Ldarg_0);
									ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
									ilgen.Emit(OpCodes.Bge, label11);
								}
								LocalBuilder localBuilder6 = ilgen.DeclareLocal(typeof(char));
								ilgen.Emit(OpCodes.Ldarg_0);
								ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
								ilgen.Emit(OpCodes.Ldarg_1);
								if (flag8)
								{
									ilgen.Emit(OpCodes.Ldc_I4_1);
									ilgen.Emit(OpCodes.Sub);
								}
								ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
								ilgen.Emit(OpCodes.Stloc, localBuilder6);
								Label label12 = ilgen.DefineLabel();
								Label label13 = ((!flag7) ? label12 : label11);
								Label label14 = ((!flag7) ? label11 : label12);
								RxOp rxOp3 = rxOp;
								if (rxOp3 != RxOp.CategoryAny)
								{
									if (rxOp3 != RxOp.CategoryAnySingleline)
									{
										if (rxOp3 != RxOp.CategoryDigit)
										{
											if (rxOp3 != RxOp.CategoryWord)
											{
												if (rxOp3 != RxOp.CategoryWhiteSpace)
												{
													if (rxOp3 != RxOp.CategoryEcmaWord)
													{
														if (rxOp3 != RxOp.CategoryEcmaWhiteSpace)
														{
															if (rxOp3 != RxOp.CategoryUnicode)
															{
																if (rxOp3 == RxOp.CategoryUnicodeSpecials)
																{
																	ilgen.Emit(OpCodes.Ldloc, localBuilder6);
																	ilgen.Emit(OpCodes.Ldc_I4, 65278);
																	ilgen.Emit(OpCodes.Cgt);
																	ilgen.Emit(OpCodes.Ldloc, localBuilder6);
																	ilgen.Emit(OpCodes.Ldc_I4, 65280);
																	ilgen.Emit(OpCodes.Clt);
																	ilgen.Emit(OpCodes.And);
																	ilgen.Emit(OpCodes.Brtrue, label13);
																	ilgen.Emit(OpCodes.Ldloc, localBuilder6);
																	ilgen.Emit(OpCodes.Ldc_I4, 65519);
																	ilgen.Emit(OpCodes.Cgt);
																	ilgen.Emit(OpCodes.Ldloc, localBuilder6);
																	ilgen.Emit(OpCodes.Ldc_I4, 65534);
																	ilgen.Emit(OpCodes.Clt);
																	ilgen.Emit(OpCodes.And);
																	ilgen.Emit(OpCodes.Brtrue, label13);
																}
															}
															else
															{
																ilgen.Emit(OpCodes.Ldloc, localBuilder6);
																ilgen.Emit(OpCodes.Call, typeof(char).GetMethod("GetUnicodeCategory", new Type[] { typeof(char) }));
																ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
																ilgen.Emit(OpCodes.Beq, label13);
															}
														}
														else
														{
															ilgen.Emit(OpCodes.Ldloc, localBuilder6);
															ilgen.Emit(OpCodes.Ldc_I4, 32);
															ilgen.Emit(OpCodes.Beq, label13);
															ilgen.Emit(OpCodes.Ldloc, localBuilder6);
															ilgen.Emit(OpCodes.Ldc_I4, 9);
															ilgen.Emit(OpCodes.Beq, label13);
															ilgen.Emit(OpCodes.Ldloc, localBuilder6);
															ilgen.Emit(OpCodes.Ldc_I4, 10);
															ilgen.Emit(OpCodes.Beq, label13);
															ilgen.Emit(OpCodes.Ldloc, localBuilder6);
															ilgen.Emit(OpCodes.Ldc_I4, 13);
															ilgen.Emit(OpCodes.Beq, label13);
															ilgen.Emit(OpCodes.Ldloc, localBuilder6);
															ilgen.Emit(OpCodes.Ldc_I4, 12);
															ilgen.Emit(OpCodes.Beq, label13);
															ilgen.Emit(OpCodes.Ldloc, localBuilder6);
															ilgen.Emit(OpCodes.Ldc_I4, 11);
															ilgen.Emit(OpCodes.Beq, label13);
														}
													}
													else
													{
														ilgen.Emit(OpCodes.Ldloc, localBuilder6);
														ilgen.Emit(OpCodes.Ldc_I4, 96);
														ilgen.Emit(OpCodes.Cgt);
														ilgen.Emit(OpCodes.Ldloc, localBuilder6);
														ilgen.Emit(OpCodes.Ldc_I4, 123);
														ilgen.Emit(OpCodes.Clt);
														ilgen.Emit(OpCodes.And);
														ilgen.Emit(OpCodes.Brtrue, label13);
														ilgen.Emit(OpCodes.Ldloc, localBuilder6);
														ilgen.Emit(OpCodes.Ldc_I4, 64);
														ilgen.Emit(OpCodes.Cgt);
														ilgen.Emit(OpCodes.Ldloc, localBuilder6);
														ilgen.Emit(OpCodes.Ldc_I4, 91);
														ilgen.Emit(OpCodes.Clt);
														ilgen.Emit(OpCodes.And);
														ilgen.Emit(OpCodes.Brtrue, label13);
														ilgen.Emit(OpCodes.Ldloc, localBuilder6);
														ilgen.Emit(OpCodes.Ldc_I4, 47);
														ilgen.Emit(OpCodes.Cgt);
														ilgen.Emit(OpCodes.Ldloc, localBuilder6);
														ilgen.Emit(OpCodes.Ldc_I4, 58);
														ilgen.Emit(OpCodes.Clt);
														ilgen.Emit(OpCodes.And);
														ilgen.Emit(OpCodes.Brtrue, label13);
														ilgen.Emit(OpCodes.Ldloc, localBuilder6);
														ilgen.Emit(OpCodes.Ldc_I4, 95);
														ilgen.Emit(OpCodes.Beq, label13);
													}
												}
												else
												{
													ilgen.Emit(OpCodes.Ldloc, localBuilder6);
													ilgen.Emit(OpCodes.Call, typeof(char).GetMethod("IsWhiteSpace", new Type[] { typeof(char) }));
													ilgen.Emit(OpCodes.Brtrue, label13);
												}
											}
											else
											{
												ilgen.Emit(OpCodes.Ldloc, localBuilder6);
												ilgen.Emit(OpCodes.Call, typeof(char).GetMethod("IsLetterOrDigit", new Type[] { typeof(char) }));
												ilgen.Emit(OpCodes.Brtrue, label13);
												ilgen.Emit(OpCodes.Ldloc, localBuilder6);
												ilgen.Emit(OpCodes.Call, typeof(char).GetMethod("GetUnicodeCategory", new Type[] { typeof(char) }));
												ilgen.Emit(OpCodes.Ldc_I4, 18);
												ilgen.Emit(OpCodes.Beq, label13);
											}
										}
										else
										{
											ilgen.Emit(OpCodes.Ldloc, localBuilder6);
											ilgen.Emit(OpCodes.Call, typeof(char).GetMethod("IsDigit", new Type[] { typeof(char) }));
											ilgen.Emit(OpCodes.Brtrue, label13);
										}
									}
									else
									{
										ilgen.Emit(OpCodes.Br, label13);
									}
								}
								else
								{
									ilgen.Emit(OpCodes.Ldloc, localBuilder6);
									ilgen.Emit(OpCodes.Ldc_I4, 10);
									ilgen.Emit(OpCodes.Bne_Un, label13);
								}
								ilgen.Emit(OpCodes.Br, label14);
								ilgen.MarkLabel(label12);
								if (!no_bump)
								{
									ilgen.Emit(OpCodes.Ldarg_1);
									ilgen.Emit(OpCodes.Ldc_I4_1);
									if (flag8)
									{
										ilgen.Emit(OpCodes.Sub);
									}
									else
									{
										ilgen.Emit(OpCodes.Add);
									}
									ilgen.Emit(OpCodes.Starg, 1);
								}
								Label label15 = ilgen.DefineLabel();
								ilgen.Emit(OpCodes.Br, label15);
								ilgen.MarkLabel(label11);
								ilgen.Emit(OpCodes.Br, frame.label_fail);
								ilgen.MarkLabel(label15);
								if (rxOp == RxOp.CategoryUnicode)
								{
									pc += 2;
								}
								else
								{
									pc++;
								}
								break;
							}
						}
						OpFlags opFlags4 = (OpFlags)this.op_flags[pc];
						bool flag9 = (ushort)(opFlags4 & OpFlags.Negate) > 0;
						bool flag10 = (ushort)(opFlags4 & OpFlags.IgnoreCase) > 0;
						bool flag11 = (ushort)(opFlags4 & OpFlags.RightToLeft) > 0;
						bool flag12 = rxOp == RxOp.UnicodeBitmap;
						Label label16 = ilgen.DefineLabel();
						Label label17 = ilgen.DefineLabel();
						Label label18 = ilgen.DefineLabel();
						if (flag11)
						{
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Ldc_I4_0);
							ilgen.Emit(OpCodes.Ble, label16);
						}
						else
						{
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
							ilgen.Emit(OpCodes.Bge, label16);
						}
						LocalBuilder localBuilder7 = ilgen.DeclareLocal(typeof(int));
						if (flag10)
						{
							ilgen.Emit(OpCodes.Ldloc, this.local_textinfo);
						}
						ilgen.Emit(OpCodes.Ldarg_0);
						ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
						ilgen.Emit(OpCodes.Ldarg_1);
						if (flag11)
						{
							ilgen.Emit(OpCodes.Ldc_I4_1);
							ilgen.Emit(OpCodes.Sub);
						}
						ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
						ilgen.Emit(OpCodes.Conv_I4);
						if (flag10)
						{
							ilgen.Emit(OpCodes.Callvirt, typeof(TextInfo).GetMethod("ToLower", new Type[] { typeof(char) }));
						}
						int num2;
						if (flag12)
						{
							ilgen.Emit(OpCodes.Ldc_I4, this.ReadShort(program, pc + 1));
							ilgen.Emit(OpCodes.Sub);
							ilgen.Emit(OpCodes.Stloc, localBuilder7);
							num2 = this.ReadShort(program, pc + 3);
							pc += 5;
						}
						else
						{
							ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
							ilgen.Emit(OpCodes.Sub);
							ilgen.Emit(OpCodes.Stloc, localBuilder7);
							num2 = (int)program[pc + 2];
							pc += 3;
						}
						ilgen.Emit(OpCodes.Ldloc, localBuilder7);
						ilgen.Emit(OpCodes.Ldc_I4_0);
						ilgen.Emit(OpCodes.Blt, (!flag9) ? frame.label_fail : label18);
						ilgen.Emit(OpCodes.Ldloc, localBuilder7);
						ilgen.Emit(OpCodes.Ldc_I4, num2 << 3);
						ilgen.Emit(OpCodes.Bge, (!flag9) ? frame.label_fail : label18);
						if (num2 <= 4)
						{
							uint num4 = (uint)program[pc];
							if (num2 > 1)
							{
								num4 |= (uint)((uint)program[pc + 1] << 8);
							}
							if (num2 > 2)
							{
								num4 |= (uint)((uint)program[pc + 2] << 16);
							}
							if (num2 > 3)
							{
								num4 |= (uint)((uint)program[pc + 3] << 24);
							}
							ilgen.Emit(OpCodes.Ldc_I4, (long)((ulong)num4));
							ilgen.Emit(OpCodes.Ldloc, localBuilder7);
							ilgen.Emit(OpCodes.Shr_Un);
							ilgen.Emit(OpCodes.Ldc_I4_1);
							ilgen.Emit(OpCodes.And);
							ilgen.Emit((!flag9) ? OpCodes.Brfalse : OpCodes.Brtrue, label16);
						}
						else
						{
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_program);
							ilgen.Emit(OpCodes.Ldloc, localBuilder7);
							ilgen.Emit(OpCodes.Ldc_I4_3);
							ilgen.Emit(OpCodes.Shr);
							ilgen.Emit(OpCodes.Ldc_I4, pc);
							ilgen.Emit(OpCodes.Add);
							ilgen.Emit(OpCodes.Ldelem_I1);
							ilgen.Emit(OpCodes.Ldc_I4_1);
							ilgen.Emit(OpCodes.Ldloc, localBuilder7);
							ilgen.Emit(OpCodes.Ldc_I4, 7);
							ilgen.Emit(OpCodes.And);
							ilgen.Emit(OpCodes.Shl);
							ilgen.Emit(OpCodes.And);
							ilgen.Emit(OpCodes.Ldc_I4_0);
							ilgen.Emit((!flag9) ? OpCodes.Beq : OpCodes.Bne_Un, label16);
						}
						ilgen.MarkLabel(label18);
						if (!no_bump)
						{
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Ldc_I4_1);
							if (flag11)
							{
								ilgen.Emit(OpCodes.Sub);
							}
							else
							{
								ilgen.Emit(OpCodes.Add);
							}
							ilgen.Emit(OpCodes.Starg, 1);
						}
						ilgen.Emit(OpCodes.Br, label17);
						ilgen.MarkLabel(label16);
						ilgen.Emit(OpCodes.Br, frame.label_fail);
						ilgen.MarkLabel(label17);
						pc += num2;
						break;
					}
					case RxOp.OpenGroup:
					{
						int num5 = this.ReadShort(program, pc + 1);
						ilgen.Emit(OpCodes.Ldarg_0);
						ilgen.Emit(OpCodes.Ldc_I4, num5);
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Call, this.GetMethod("Open", ref CILCompiler.mi_open));
						pc += 3;
						break;
					}
					case RxOp.CloseGroup:
					{
						int num6 = this.ReadShort(program, pc + 1);
						ilgen.Emit(OpCodes.Ldarg_0);
						ilgen.Emit(OpCodes.Ldc_I4, num6);
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Call, this.GetMethod("Close", ref CILCompiler.mi_close));
						pc += 3;
						break;
					}
					case RxOp.IfDefined:
					case RxOp.Repeat:
					case RxOp.RepeatLazy:
						if (RxInterpreter.trace_rx || CILCompiler.trace_compile)
						{
							Console.WriteLine("Opcode " + rxOp + " not supported.");
						}
						return null;
					case RxOp.Jump:
					{
						int num7 = pc + this.ReadShort(program, pc + 1);
						if (num7 > end_pc)
						{
							return null;
						}
						if (CILCompiler.trace_compile)
						{
							Console.WriteLine("\tjump target: {0}", num7);
						}
						if (this.labels == null)
						{
							this.labels = new Dictionary<int, Label>();
						}
						Label label19 = this.CreateLabelForPC(ilgen, num7);
						ilgen.Emit(OpCodes.Br, label19);
						pc += 3;
						break;
					}
					case RxOp.SubExpression:
					{
						int num8 = pc + this.ReadShort(program, pc + 1);
						if (CILCompiler.trace_compile)
						{
							Console.WriteLine("\temitting <sub_expr>");
						}
						LocalBuilder localBuilder8 = ilgen.DeclareLocal(typeof(int));
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Stloc, localBuilder8);
						CILCompiler.Frame frame2 = new CILCompiler.Frame(ilgen);
						m = this.EmitEvalMethodBody(m, ilgen, frame2, program, pc + 3, num8, false, false, out pc);
						if (m == null)
						{
							return null;
						}
						if (CILCompiler.trace_compile)
						{
							Console.WriteLine("\temitted <sub_expr>");
							Console.WriteLine("\ttarget = {0}", num8);
						}
						Label label20 = this.CreateLabelForPC(ilgen, num8);
						ilgen.MarkLabel(frame2.label_pass);
						ilgen.Emit(OpCodes.Br, label20);
						ilgen.MarkLabel(frame2.label_fail);
						ilgen.Emit(OpCodes.Ldloc, localBuilder8);
						ilgen.Emit(OpCodes.Starg, 1);
						ilgen.Emit(OpCodes.Br, frame.label_fail);
						break;
					}
					case RxOp.Test:
					{
						int num9 = pc + this.ReadShort(program, pc + 1);
						int num10 = pc + this.ReadShort(program, pc + 3);
						if (CILCompiler.trace_compile)
						{
							Console.WriteLine("\temitting <test_expr>");
						}
						LocalBuilder localBuilder9 = ilgen.DeclareLocal(typeof(int));
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Stloc, localBuilder9);
						CILCompiler.Frame frame3 = new CILCompiler.Frame(ilgen);
						m = this.EmitEvalMethodBody(m, ilgen, frame3, program, pc + 5, (num9 >= num10) ? num10 : num9, false, false, out pc);
						if (m == null)
						{
							return null;
						}
						if (CILCompiler.trace_compile)
						{
							Console.WriteLine("\temitted <test_expr>");
							Console.WriteLine("\ttarget1 = {0}", num9);
							Console.WriteLine("\ttarget2 = {0}", num10);
						}
						Label label21 = this.CreateLabelForPC(ilgen, num9);
						Label label22 = this.CreateLabelForPC(ilgen, num10);
						ilgen.MarkLabel(frame3.label_pass);
						ilgen.Emit(OpCodes.Ldloc, localBuilder9);
						ilgen.Emit(OpCodes.Starg, 1);
						ilgen.Emit(OpCodes.Br, label21);
						ilgen.MarkLabel(frame3.label_fail);
						ilgen.Emit(OpCodes.Ldloc, localBuilder9);
						ilgen.Emit(OpCodes.Starg, 1);
						ilgen.Emit(OpCodes.Br, label22);
						break;
					}
					case RxOp.Branch:
					{
						int num11 = pc + this.ReadShort(program, pc + 1);
						CILCompiler.Frame frame4 = new CILCompiler.Frame(ilgen);
						LocalBuilder localBuilder10 = ilgen.DeclareLocal(typeof(int));
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Stloc, localBuilder10);
						m = this.EmitEvalMethodBody(m, ilgen, frame4, program, pc + 3, num11, false, false, out out_pc);
						if (m == null)
						{
							return null;
						}
						ilgen.MarkLabel(frame4.label_pass);
						ilgen.Emit(OpCodes.Br, frame.label_pass);
						ilgen.MarkLabel(frame4.label_fail);
						ilgen.Emit(OpCodes.Ldloc, localBuilder10);
						ilgen.Emit(OpCodes.Starg, 1);
						pc = num11;
						break;
					}
					case RxOp.TestCharGroup:
					{
						int num12 = pc + this.ReadShort(program, pc + 1);
						pc += 3;
						Label label23 = ilgen.DefineLabel();
						OpFlags opFlags5 = (OpFlags)this.op_flags[pc];
						bool flag13 = (ushort)(opFlags5 & OpFlags.Negate) > 0;
						bool flag14 = (ushort)(opFlags5 & OpFlags.RightToLeft) > 0;
						while (pc < num12)
						{
							CILCompiler.Frame frame5 = new CILCompiler.Frame(ilgen);
							m = this.EmitEvalMethodBody(m, ilgen, frame5, program, pc, int.MaxValue, true, true, out pc);
							if (m == null)
							{
								return null;
							}
							if (!flag13)
							{
								ilgen.MarkLabel(frame5.label_pass);
								ilgen.Emit(OpCodes.Br, label23);
								ilgen.MarkLabel(frame5.label_fail);
							}
							else
							{
								ilgen.MarkLabel(frame5.label_pass);
								Label label24 = ilgen.DefineLabel();
								ilgen.Emit(OpCodes.Br, label24);
								ilgen.MarkLabel(frame5.label_fail);
								ilgen.Emit(OpCodes.Br, frame.label_fail);
								ilgen.MarkLabel(label24);
							}
						}
						if (flag13)
						{
							ilgen.Emit(OpCodes.Br, label23);
						}
						else
						{
							ilgen.Emit(OpCodes.Br, frame.label_fail);
						}
						ilgen.MarkLabel(label23);
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Ldc_I4_1);
						if (flag14)
						{
							ilgen.Emit(OpCodes.Sub);
						}
						else
						{
							ilgen.Emit(OpCodes.Add);
						}
						ilgen.Emit(OpCodes.Starg, 1);
						break;
					}
					case RxOp.Anchor:
					case RxOp.AnchorReverse:
					{
						bool flag15 = program[pc] == 151;
						int num2 = this.ReadShort(program, pc + 3);
						pc += this.ReadShort(program, pc + 1);
						RxOp rxOp4 = (RxOp)program[pc];
						if (!flag15 && num == 1 && rxOp4 == RxOp.Char && program[pc + 2] == 2)
						{
							LocalBuilder localBuilder11 = ilgen.DeclareLocal(typeof(int));
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
							ilgen.Emit(OpCodes.Stloc, localBuilder11);
							LocalBuilder localBuilder12 = ilgen.DeclareLocal(typeof(string));
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
							ilgen.Emit(OpCodes.Stloc, localBuilder12);
							Label label25 = ilgen.DefineLabel();
							Label label26 = ilgen.DefineLabel();
							ilgen.Emit(OpCodes.Br, label26);
							ilgen.MarkLabel(label25);
							Label label27 = ilgen.DefineLabel();
							ilgen.Emit(OpCodes.Ldloc, localBuilder12);
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
							ilgen.Emit(OpCodes.Conv_I4);
							ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
							ilgen.Emit(OpCodes.Beq, label27);
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Ldc_I4_1);
							ilgen.Emit(OpCodes.Add);
							ilgen.Emit(OpCodes.Starg, 1);
							ilgen.MarkLabel(label26);
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Ldloc, localBuilder11);
							ilgen.Emit(OpCodes.Blt, label25);
							ilgen.Emit(OpCodes.Br, frame.label_fail);
							ilgen.MarkLabel(label27);
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Call, this.GetMethod(typeof(RxInterpreter), "SetStartOfMatch", ref CILCompiler.mi_set_start_of_match));
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Ldc_I4_1);
							ilgen.Emit(OpCodes.Add);
							ilgen.Emit(OpCodes.Starg, 1);
							ilgen.Emit(OpCodes.Br, frame.label_pass);
						}
						else
						{
							LocalBuilder localBuilder13 = ilgen.DeclareLocal(typeof(int));
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
							ilgen.Emit(OpCodes.Ldc_I4_1);
							ilgen.Emit(OpCodes.Add);
							ilgen.Emit(OpCodes.Stloc, localBuilder13);
							Label label28 = ilgen.DefineLabel();
							Label label29 = ilgen.DefineLabel();
							ilgen.Emit(OpCodes.Br, label29);
							ilgen.MarkLabel(label28);
							if (num > 1)
							{
								ilgen.Emit(OpCodes.Ldarg_0);
								ilgen.Emit(OpCodes.Call, this.GetMethod("ResetGroups", ref CILCompiler.mi_reset_groups));
								ilgen.Emit(OpCodes.Ldarg_0);
								ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_marks);
								ilgen.Emit(OpCodes.Ldarg_0);
								ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_groups);
								ilgen.Emit(OpCodes.Ldc_I4_0);
								ilgen.Emit(OpCodes.Ldelem_I4);
								ilgen.Emit(OpCodes.Ldelema, typeof(Mark));
								ilgen.Emit(OpCodes.Ldarg_1);
								ilgen.Emit(OpCodes.Stfld, CILCompiler.fi_mark_start);
							}
							CILCompiler.Frame frame6 = new CILCompiler.Frame(ilgen);
							LocalBuilder localBuilder14 = ilgen.DeclareLocal(typeof(int));
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Stloc, localBuilder14);
							m = this.EmitEvalMethodBody(m, ilgen, frame6, program, pc, end_pc, false, false, out out_pc);
							if (m == null)
							{
								return null;
							}
							ilgen.MarkLabel(frame6.label_pass);
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_marks);
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_groups);
							ilgen.Emit(OpCodes.Ldc_I4_0);
							ilgen.Emit(OpCodes.Ldelem_I4);
							ilgen.Emit(OpCodes.Ldelema, typeof(Mark));
							ilgen.Emit(OpCodes.Ldloc, localBuilder14);
							ilgen.Emit(OpCodes.Stfld, CILCompiler.fi_mark_start);
							if (num > 1)
							{
								ilgen.Emit(OpCodes.Ldarg_0);
								ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_marks);
								ilgen.Emit(OpCodes.Ldarg_0);
								ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_groups);
								ilgen.Emit(OpCodes.Ldc_I4_0);
								ilgen.Emit(OpCodes.Ldelem_I4);
								ilgen.Emit(OpCodes.Ldelema, typeof(Mark));
								ilgen.Emit(OpCodes.Ldarg_1);
								ilgen.Emit(OpCodes.Stfld, CILCompiler.fi_mark_end);
							}
							ilgen.Emit(OpCodes.Br, frame.label_pass);
							ilgen.MarkLabel(frame6.label_fail);
							ilgen.Emit(OpCodes.Ldloc, localBuilder14);
							ilgen.Emit(OpCodes.Ldc_I4_1);
							if (flag15)
							{
								ilgen.Emit(OpCodes.Sub);
							}
							else
							{
								ilgen.Emit(OpCodes.Add);
							}
							ilgen.Emit(OpCodes.Starg, 1);
							ilgen.MarkLabel(label29);
							if (flag15)
							{
								ilgen.Emit(OpCodes.Ldarg_1);
								ilgen.Emit(OpCodes.Ldc_I4_0);
								ilgen.Emit(OpCodes.Bge, label28);
							}
							else
							{
								ilgen.Emit(OpCodes.Ldarg_1);
								ilgen.Emit(OpCodes.Ldloc, localBuilder13);
								ilgen.Emit(OpCodes.Blt, label28);
							}
							ilgen.Emit(OpCodes.Br, frame.label_fail);
						}
						goto IL_3722;
					}
					case RxOp.FastRepeat:
					case RxOp.FastRepeatLazy:
					{
						bool flag16 = program[pc] == 156;
						int num13 = pc + this.ReadShort(program, pc + 1);
						int i = this.ReadInt(program, pc + 3);
						int num3 = this.ReadInt(program, pc + 7);
						ilgen.Emit(OpCodes.Ldarg_0);
						ilgen.Emit(OpCodes.Ldnull);
						ilgen.Emit(OpCodes.Stfld, CILCompiler.fi_deep);
						LocalBuilder localBuilder15 = ilgen.DeclareLocal(typeof(int));
						ilgen.Emit(OpCodes.Ldc_I4_0);
						ilgen.Emit(OpCodes.Stloc, localBuilder15);
						LocalBuilder localBuilder16 = ilgen.DeclareLocal(typeof(int));
						if (i > 0)
						{
							Label label30 = ilgen.DefineLabel();
							ilgen.Emit(OpCodes.Br, label30);
							Label label31 = ilgen.DefineLabel();
							ilgen.MarkLabel(label31);
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Stloc, localBuilder16);
							CILCompiler.Frame frame7 = new CILCompiler.Frame(ilgen);
							m = this.EmitEvalMethodBody(m, ilgen, frame7, program, pc + 11, num13, false, false, out out_pc);
							if (m == null)
							{
								return null;
							}
							ilgen.MarkLabel(frame7.label_fail);
							ilgen.Emit(OpCodes.Br, frame.label_fail);
							ilgen.MarkLabel(frame7.label_pass);
							ilgen.Emit(OpCodes.Ldloc, localBuilder15);
							ilgen.Emit(OpCodes.Ldc_I4_1);
							ilgen.Emit(OpCodes.Add);
							ilgen.Emit(OpCodes.Stloc, localBuilder15);
							ilgen.MarkLabel(label30);
							ilgen.Emit(OpCodes.Ldloc, localBuilder15);
							ilgen.Emit(OpCodes.Ldc_I4, i);
							ilgen.Emit(OpCodes.Blt, label31);
						}
						if (flag16)
						{
							Label label32 = ilgen.DefineLabel();
							ilgen.Emit(OpCodes.Br, label32);
							Label label33 = ilgen.DefineLabel();
							ilgen.MarkLabel(label33);
							LocalBuilder localBuilder17 = ilgen.DeclareLocal(typeof(int));
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Call, this.GetMethod("Checkpoint", ref CILCompiler.mi_checkpoint));
							ilgen.Emit(OpCodes.Stloc, localBuilder17);
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Stloc, localBuilder16);
							CILCompiler.Frame frame8 = new CILCompiler.Frame(ilgen);
							m = this.EmitEvalMethodBody(m, ilgen, frame8, program, num13, end_pc, false, false, out out_pc);
							if (m == null)
							{
								return null;
							}
							ilgen.MarkLabel(frame8.label_pass);
							ilgen.Emit(OpCodes.Br, frame.label_pass);
							ilgen.MarkLabel(frame8.label_fail);
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldloc, localBuilder17);
							ilgen.Emit(OpCodes.Call, this.GetMethod("Backtrack", ref CILCompiler.mi_backtrack));
							ilgen.Emit(OpCodes.Ldloc, localBuilder16);
							ilgen.Emit(OpCodes.Starg, 1);
							ilgen.Emit(OpCodes.Ldloc, localBuilder15);
							ilgen.Emit(OpCodes.Ldc_I4, num3);
							ilgen.Emit(OpCodes.Bge, frame.label_fail);
							frame8 = new CILCompiler.Frame(ilgen);
							m = this.EmitEvalMethodBody(m, ilgen, frame8, program, pc + 11, num13, false, false, out out_pc);
							if (m == null)
							{
								return null;
							}
							ilgen.MarkLabel(frame8.label_pass);
							ilgen.Emit(OpCodes.Ldloc, localBuilder15);
							ilgen.Emit(OpCodes.Ldc_I4_1);
							ilgen.Emit(OpCodes.Add);
							ilgen.Emit(OpCodes.Stloc, localBuilder15);
							ilgen.Emit(OpCodes.Br, label33);
							ilgen.MarkLabel(frame8.label_fail);
							ilgen.Emit(OpCodes.Br, frame.label_fail);
							ilgen.MarkLabel(label32);
							ilgen.Emit(OpCodes.Br, label33);
						}
						else
						{
							LocalBuilder localBuilder18 = ilgen.DeclareLocal(typeof(int));
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldflda, CILCompiler.fi_stack);
							ilgen.Emit(OpCodes.Call, this.GetMethod(typeof(RxInterpreter.IntStack), "get_Count", ref CILCompiler.mi_stack_get_count));
							ilgen.Emit(OpCodes.Stloc, localBuilder18);
							Label label34 = ilgen.DefineLabel();
							ilgen.Emit(OpCodes.Br, label34);
							Label label35 = ilgen.DefineLabel();
							ilgen.MarkLabel(label35);
							LocalBuilder localBuilder19 = ilgen.DeclareLocal(typeof(int));
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Call, this.GetMethod("Checkpoint", ref CILCompiler.mi_checkpoint));
							ilgen.Emit(OpCodes.Stloc, localBuilder19);
							ilgen.Emit(OpCodes.Ldarg_1);
							ilgen.Emit(OpCodes.Stloc, localBuilder16);
							CILCompiler.Frame frame9 = new CILCompiler.Frame(ilgen);
							m = this.EmitEvalMethodBody(m, ilgen, frame9, program, pc + 11, num13, false, false, out out_pc);
							if (m == null)
							{
								return null;
							}
							ilgen.MarkLabel(frame9.label_fail);
							ilgen.Emit(OpCodes.Ldloc, localBuilder16);
							ilgen.Emit(OpCodes.Starg, 1);
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldloc, localBuilder19);
							ilgen.Emit(OpCodes.Call, this.GetMethod("Backtrack", ref CILCompiler.mi_backtrack));
							Label label36 = ilgen.DefineLabel();
							ilgen.Emit(OpCodes.Br, label36);
							ilgen.MarkLabel(frame9.label_pass);
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldflda, CILCompiler.fi_stack);
							ilgen.Emit(OpCodes.Ldloc, localBuilder19);
							ilgen.Emit(OpCodes.Call, this.GetMethod(typeof(RxInterpreter.IntStack), "Push", ref CILCompiler.mi_stack_push));
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldflda, CILCompiler.fi_stack);
							ilgen.Emit(OpCodes.Ldloc, localBuilder16);
							ilgen.Emit(OpCodes.Call, this.GetMethod(typeof(RxInterpreter.IntStack), "Push", ref CILCompiler.mi_stack_push));
							ilgen.Emit(OpCodes.Ldloc, localBuilder15);
							ilgen.Emit(OpCodes.Ldc_I4_1);
							ilgen.Emit(OpCodes.Add);
							ilgen.Emit(OpCodes.Stloc, localBuilder15);
							ilgen.MarkLabel(label34);
							ilgen.Emit(OpCodes.Ldloc, localBuilder15);
							ilgen.Emit(OpCodes.Ldc_I4, num3);
							ilgen.Emit(OpCodes.Blt, label35);
							ilgen.MarkLabel(label36);
							label34 = ilgen.DefineLabel();
							ilgen.Emit(OpCodes.Br, label34);
							label35 = ilgen.DefineLabel();
							ilgen.MarkLabel(label35);
							if (RxInterpreter.trace_rx)
							{
								ilgen.Emit(OpCodes.Ldstr, "matching tail at: {0}");
								ilgen.Emit(OpCodes.Ldarg_1);
								ilgen.Emit(OpCodes.Box, typeof(int));
								ilgen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[]
								{
									typeof(string),
									typeof(object)
								}));
							}
							frame9 = new CILCompiler.Frame(ilgen);
							m = this.EmitEvalMethodBody(m, ilgen, frame9, program, num13, end_pc, false, false, out out_pc);
							if (m == null)
							{
								return null;
							}
							ilgen.MarkLabel(frame9.label_pass);
							if (RxInterpreter.trace_rx)
							{
								ilgen.Emit(OpCodes.Ldstr, "tail matched at: {0}");
								ilgen.Emit(OpCodes.Ldarg_1);
								ilgen.Emit(OpCodes.Box, typeof(int));
								ilgen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[]
								{
									typeof(string),
									typeof(object)
								}));
							}
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldflda, CILCompiler.fi_stack);
							ilgen.Emit(OpCodes.Ldloc, localBuilder18);
							ilgen.Emit(OpCodes.Call, this.GetMethod(typeof(RxInterpreter.IntStack), "set_Count", ref CILCompiler.mi_stack_set_count));
							ilgen.Emit(OpCodes.Br, frame.label_pass);
							ilgen.MarkLabel(frame9.label_fail);
							if (RxInterpreter.trace_rx)
							{
								ilgen.Emit(OpCodes.Ldstr, "tail failed to match at: {0}");
								ilgen.Emit(OpCodes.Ldarg_1);
								ilgen.Emit(OpCodes.Box, typeof(int));
								ilgen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[]
								{
									typeof(string),
									typeof(object)
								}));
							}
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldflda, CILCompiler.fi_stack);
							ilgen.Emit(OpCodes.Call, this.GetMethod(typeof(RxInterpreter.IntStack), "get_Count", ref CILCompiler.mi_stack_get_count));
							ilgen.Emit(OpCodes.Ldloc, localBuilder18);
							ilgen.Emit(OpCodes.Beq, frame.label_fail);
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldflda, CILCompiler.fi_stack);
							ilgen.Emit(OpCodes.Call, this.GetMethod(typeof(RxInterpreter.IntStack), "Pop", ref CILCompiler.mi_stack_pop));
							ilgen.Emit(OpCodes.Starg, 1);
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldarg_0);
							ilgen.Emit(OpCodes.Ldflda, CILCompiler.fi_stack);
							ilgen.Emit(OpCodes.Call, this.GetMethod(typeof(RxInterpreter.IntStack), "Pop", ref CILCompiler.mi_stack_pop));
							ilgen.Emit(OpCodes.Call, this.GetMethod("Backtrack", ref CILCompiler.mi_backtrack));
							if (RxInterpreter.trace_rx)
							{
								ilgen.Emit(OpCodes.Ldstr, "backtracking to: {0}");
								ilgen.Emit(OpCodes.Ldarg_1);
								ilgen.Emit(OpCodes.Box, typeof(int));
								ilgen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[]
								{
									typeof(string),
									typeof(object)
								}));
							}
							ilgen.MarkLabel(label34);
							ilgen.Emit(OpCodes.Br, label35);
						}
						pc = out_pc;
						goto IL_3722;
					}
					}
					break;
				case RxOp.Char:
				case RxOp.Range:
					goto IL_0935;
				}
				IL_370D:
				if (one_op)
				{
					break;
				}
				continue;
				IL_0935:
				OpFlags opFlags6 = (OpFlags)this.op_flags[pc];
				bool flag17 = (ushort)(opFlags6 & OpFlags.Negate) > 0;
				bool flag18 = (ushort)(opFlags6 & OpFlags.IgnoreCase) > 0;
				bool flag19 = (ushort)(opFlags6 & OpFlags.RightToLeft) > 0;
				Label label37 = ilgen.DefineLabel();
				if (flag19)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Ble, label37);
				}
				else
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_string_end);
					ilgen.Emit(OpCodes.Bge, label37);
				}
				if (flag18)
				{
					ilgen.Emit(OpCodes.Ldloc, this.local_textinfo);
				}
				LocalBuilder localBuilder20 = ilgen.DeclareLocal(typeof(char));
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, CILCompiler.fi_str);
				ilgen.Emit(OpCodes.Ldarg_1);
				if (flag19)
				{
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Sub);
				}
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				if (flag18)
				{
					ilgen.Emit(OpCodes.Callvirt, typeof(TextInfo).GetMethod("ToLower", new Type[] { typeof(char) }));
				}
				if (rxOp == RxOp.Char)
				{
					ilgen.Emit(OpCodes.Conv_I4);
					ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
					ilgen.Emit((!flag17) ? OpCodes.Bne_Un : OpCodes.Beq, label37);
					pc += 2;
				}
				else if (rxOp == RxOp.UnicodeChar)
				{
					ilgen.Emit(OpCodes.Conv_I4);
					ilgen.Emit(OpCodes.Ldc_I4, this.ReadShort(program, pc + 1));
					ilgen.Emit((!flag17) ? OpCodes.Bne_Un : OpCodes.Beq, label37);
					pc += 3;
				}
				else if (rxOp == RxOp.Range)
				{
					ilgen.Emit(OpCodes.Stloc, localBuilder20);
					if (flag17)
					{
						Label label38 = ilgen.DefineLabel();
						ilgen.Emit(OpCodes.Ldloc, localBuilder20);
						ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
						ilgen.Emit(OpCodes.Blt, label38);
						ilgen.Emit(OpCodes.Ldloc, localBuilder20);
						ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 2]);
						ilgen.Emit(OpCodes.Bgt, label38);
						ilgen.Emit(OpCodes.Br, label37);
						ilgen.MarkLabel(label38);
					}
					else
					{
						ilgen.Emit(OpCodes.Ldloc, localBuilder20);
						ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
						ilgen.Emit(OpCodes.Blt, label37);
						ilgen.Emit(OpCodes.Ldloc, localBuilder20);
						ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 2]);
						ilgen.Emit(OpCodes.Bgt, label37);
					}
					pc += 3;
				}
				else
				{
					if (rxOp != RxOp.UnicodeRange)
					{
						throw new NotSupportedException();
					}
					ilgen.Emit(OpCodes.Stloc, localBuilder20);
					if (flag17)
					{
						Label label39 = ilgen.DefineLabel();
						ilgen.Emit(OpCodes.Ldloc, localBuilder20);
						ilgen.Emit(OpCodes.Ldc_I4, this.ReadShort(program, pc + 1));
						ilgen.Emit(OpCodes.Blt, label39);
						ilgen.Emit(OpCodes.Ldloc, localBuilder20);
						ilgen.Emit(OpCodes.Ldc_I4, this.ReadShort(program, pc + 3));
						ilgen.Emit(OpCodes.Bgt, label39);
						ilgen.Emit(OpCodes.Br, label37);
						ilgen.MarkLabel(label39);
					}
					else
					{
						ilgen.Emit(OpCodes.Ldloc, localBuilder20);
						ilgen.Emit(OpCodes.Ldc_I4, this.ReadShort(program, pc + 1));
						ilgen.Emit(OpCodes.Blt, label37);
						ilgen.Emit(OpCodes.Ldloc, localBuilder20);
						ilgen.Emit(OpCodes.Ldc_I4, this.ReadShort(program, pc + 3));
						ilgen.Emit(OpCodes.Bgt, label37);
					}
					pc += 5;
				}
				if (!no_bump)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					if (flag19)
					{
						ilgen.Emit(OpCodes.Sub);
					}
					else
					{
						ilgen.Emit(OpCodes.Add);
					}
					ilgen.Emit(OpCodes.Starg, 1);
				}
				Label label40 = ilgen.DefineLabel();
				ilgen.Emit(OpCodes.Br, label40);
				ilgen.MarkLabel(label37);
				ilgen.Emit(OpCodes.Br, frame.label_fail);
				ilgen.MarkLabel(label40);
				goto IL_370D;
			}
			IL_3722:
			out_pc = pc;
			return m;
		}

		private DynamicMethod[] eval_methods;

		private bool[] eval_methods_defined;

		private Dictionary<int, int> generic_ops;

		private Dictionary<int, int> op_flags;

		private Dictionary<int, Label> labels;

		private static FieldInfo fi_str = typeof(RxInterpreter).GetField("str", BindingFlags.Instance | BindingFlags.NonPublic);

		private static FieldInfo fi_string_start = typeof(RxInterpreter).GetField("string_start", BindingFlags.Instance | BindingFlags.NonPublic);

		private static FieldInfo fi_string_end = typeof(RxInterpreter).GetField("string_end", BindingFlags.Instance | BindingFlags.NonPublic);

		private static FieldInfo fi_program = typeof(RxInterpreter).GetField("program", BindingFlags.Instance | BindingFlags.NonPublic);

		private static FieldInfo fi_marks = typeof(RxInterpreter).GetField("marks", BindingFlags.Instance | BindingFlags.NonPublic);

		private static FieldInfo fi_groups = typeof(RxInterpreter).GetField("groups", BindingFlags.Instance | BindingFlags.NonPublic);

		private static FieldInfo fi_deep = typeof(RxInterpreter).GetField("deep", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		private static FieldInfo fi_stack = typeof(RxInterpreter).GetField("stack", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		private static FieldInfo fi_mark_start = typeof(Mark).GetField("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		private static FieldInfo fi_mark_end = typeof(Mark).GetField("End", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		private static FieldInfo fi_mark_index = typeof(Mark).GetField("Index", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		private static MethodInfo mi_stack_get_count;

		private static MethodInfo mi_stack_set_count;

		private static MethodInfo mi_stack_push;

		private static MethodInfo mi_stack_pop;

		private static MethodInfo mi_set_start_of_match;

		private static MethodInfo mi_is_word_char;

		private static MethodInfo mi_reset_groups;

		private static MethodInfo mi_checkpoint;

		private static MethodInfo mi_backtrack;

		private static MethodInfo mi_open;

		private static MethodInfo mi_close;

		private static MethodInfo mi_get_last_defined;

		private static MethodInfo mi_mark_get_index;

		private static MethodInfo mi_mark_get_length;

		public static readonly bool trace_compile = Environment.GetEnvironmentVariable("MONO_TRACE_RX_COMPILE") != null;

		private LocalBuilder local_textinfo;

		private class Frame
		{
			public Frame(ILGenerator ilgen)
			{
				this.label_fail = ilgen.DefineLabel();
				this.label_pass = ilgen.DefineLabel();
			}

			public Label label_pass;

			public Label label_fail;
		}
	}
}
