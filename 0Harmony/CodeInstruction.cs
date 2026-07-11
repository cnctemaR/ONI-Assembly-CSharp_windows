using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony.ILCopying;

namespace Harmony
{
	public class CodeInstruction
	{
		public CodeInstruction(OpCode opcode, object operand = null)
		{
			this.opcode = opcode;
			this.operand = operand;
		}

		public CodeInstruction(CodeInstruction instruction)
		{
			this.opcode = instruction.opcode;
			this.operand = instruction.operand;
			this.labels = instruction.labels.ToArray().ToList<Label>();
		}

		public CodeInstruction Clone()
		{
			return new CodeInstruction(this)
			{
				labels = new List<Label>()
			};
		}

		public CodeInstruction Clone(OpCode opcode)
		{
			CodeInstruction codeInstruction = new CodeInstruction(this)
			{
				labels = new List<Label>()
			};
			codeInstruction.opcode = opcode;
			return codeInstruction;
		}

		public CodeInstruction Clone(OpCode opcode, object operand)
		{
			CodeInstruction codeInstruction = new CodeInstruction(this)
			{
				labels = new List<Label>()
			};
			codeInstruction.opcode = opcode;
			codeInstruction.operand = operand;
			return codeInstruction;
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (Label label in this.labels)
			{
				list.Add("Label" + label.GetHashCode());
			}
			foreach (ExceptionBlock exceptionBlock in this.blocks)
			{
				list.Add("EX_" + exceptionBlock.blockType.ToString().Replace("Block", ""));
			}
			string text = ((list.Count > 0) ? (" [" + string.Join(", ", list.ToArray()) + "]") : "");
			string text2 = Emitter.FormatArgument(this.operand);
			bool flag = text2 != "";
			if (flag)
			{
				text2 = " " + text2;
			}
			return this.opcode + text2 + text;
		}

		public OpCode opcode;

		public object operand;

		public List<Label> labels = new List<Label>();

		public List<ExceptionBlock> blocks = new List<ExceptionBlock>();
	}
}
