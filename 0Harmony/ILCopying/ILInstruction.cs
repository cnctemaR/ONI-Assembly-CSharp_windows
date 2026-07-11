using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Harmony.ILCopying
{
	public class ILInstruction
	{
		public ILInstruction(OpCode opcode, object operand = null)
		{
			this.opcode = opcode;
			this.operand = operand;
			this.argument = operand;
		}

		public CodeInstruction GetCodeInstruction()
		{
			CodeInstruction codeInstruction = new CodeInstruction(this.opcode, this.argument);
			bool flag = this.opcode.OperandType == OperandType.InlineNone;
			if (flag)
			{
				codeInstruction.operand = null;
			}
			codeInstruction.labels = this.labels;
			codeInstruction.blocks = this.blocks;
			return codeInstruction;
		}

		public int GetSize()
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
			bool flag = this.operand == null;
			string text2;
			if (flag)
			{
				text2 = text;
			}
			else
			{
				text += " ";
				OperandType operandType = this.opcode.OperandType;
				if (operandType <= OperandType.InlineString)
				{
					if (operandType != OperandType.InlineBrTarget)
					{
						if (operandType != OperandType.InlineString)
						{
							goto IL_0104;
						}
						text = string.Concat(new object[] { text, "\"", this.operand, "\"" });
						goto IL_0113;
					}
				}
				else
				{
					if (operandType == OperandType.InlineSwitch)
					{
						ILInstruction[] array = (ILInstruction[])this.operand;
						for (int i = 0; i < array.Length; i++)
						{
							bool flag2 = i > 0;
							if (flag2)
							{
								text += ",";
							}
							ILInstruction.AppendLabel(ref text, array[i]);
						}
						goto IL_0113;
					}
					if (operandType != OperandType.ShortInlineBrTarget)
					{
						goto IL_0104;
					}
				}
				ILInstruction.AppendLabel(ref text, this.operand);
				goto IL_0113;
				IL_0104:
				text += this.operand;
				IL_0113:
				text2 = text;
			}
			return text2;
		}

		private static void AppendLabel(ref string str, object argument)
		{
			ILInstruction ilinstruction = argument as ILInstruction;
			bool flag = ilinstruction != null;
			if (flag)
			{
				str = str + "IL_" + ilinstruction.offset.ToString("X4");
			}
			else
			{
				str = str + "IL_" + argument;
			}
		}

		public int offset;

		public OpCode opcode;

		public object operand;

		public object argument;

		public List<Label> labels = new List<Label>();

		public List<ExceptionBlock> blocks = new List<ExceptionBlock>();
	}
}
