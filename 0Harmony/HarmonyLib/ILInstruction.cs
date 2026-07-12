using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HarmonyLib
{
	internal class ILInstruction
	{
		internal ILInstruction(OpCode opcode, object operand = null)
		{
			this.opcode = opcode;
			this.operand = operand;
			this.argument = operand;
		}

		internal CodeInstruction GetCodeInstruction()
		{
			CodeInstruction codeInstruction = new CodeInstruction(this.opcode, this.argument);
			if (this.opcode.OperandType == OperandType.InlineNone)
			{
				codeInstruction.operand = null;
			}
			codeInstruction.labels = this.labels;
			codeInstruction.blocks = this.blocks;
			return codeInstruction;
		}

		internal int GetSize()
		{
			int num = this.opcode.Size;
			switch (this.opcode.OperandType)
			{
			case OperandType.InlineBrTarget:
			case OperandType.InlineField:
			case OperandType.InlineI:
			case OperandType.InlineMethod:
			case OperandType.InlineSig:
			case OperandType.InlineString:
			case OperandType.InlineTok:
			case OperandType.InlineType:
			case OperandType.ShortInlineR:
				num += 4;
				break;
			case OperandType.InlineI8:
			case OperandType.InlineR:
				num += 8;
				break;
			case OperandType.InlineSwitch:
				num += (1 + ((Array)this.operand).Length) * 4;
				break;
			case OperandType.InlineVar:
				num += 2;
				break;
			case OperandType.ShortInlineBrTarget:
			case OperandType.ShortInlineI:
			case OperandType.ShortInlineVar:
				num++;
				break;
			}
			return num;
		}

		public override string ToString()
		{
			string text = "";
			ILInstruction.AppendLabel(ref text, this);
			text = text + ": " + this.opcode.Name;
			if (this.operand == null)
			{
				return text;
			}
			text += " ";
			OperandType operandType = this.opcode.OperandType;
			if (operandType <= OperandType.InlineString)
			{
				if (operandType != OperandType.InlineBrTarget)
				{
					if (operandType != OperandType.InlineString)
					{
						goto IL_00BE;
					}
					return text + string.Format("\"{0}\"", this.operand);
				}
			}
			else
			{
				if (operandType == OperandType.InlineSwitch)
				{
					ILInstruction[] array = (ILInstruction[])this.operand;
					for (int i = 0; i < array.Length; i++)
					{
						if (i > 0)
						{
							text += ",";
						}
						ILInstruction.AppendLabel(ref text, array[i]);
					}
					return text;
				}
				if (operandType != OperandType.ShortInlineBrTarget)
				{
					goto IL_00BE;
				}
			}
			ILInstruction.AppendLabel(ref text, this.operand);
			return text;
			IL_00BE:
			string text2 = text;
			object obj = this.operand;
			text = text2 + ((obj != null) ? obj.ToString() : null);
			return text;
		}

		private static void AppendLabel(ref string str, object argument)
		{
			ILInstruction ilinstruction = argument as ILInstruction;
			str += string.Format("IL_{0}", ((ilinstruction != null) ? ilinstruction.offset.ToString("X4") : null) ?? argument);
		}

		internal int offset;

		internal OpCode opcode;

		internal object operand;

		internal object argument;

		internal List<Label> labels = new List<Label>();

		internal List<ExceptionBlock> blocks = new List<ExceptionBlock>();
	}
}
