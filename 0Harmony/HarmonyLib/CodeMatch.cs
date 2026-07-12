using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace HarmonyLib
{
	public class CodeMatch : CodeInstruction
	{
		internal CodeMatch Set(object operand, string name)
		{
			if (this.operand == null)
			{
				this.operand = operand;
			}
			if (this.name == null)
			{
				this.name = name;
			}
			return this;
		}

		public CodeMatch(OpCode? opcode = null, object operand = null, string name = null)
		{
			if (opcode != null)
			{
				OpCode valueOrDefault = opcode.GetValueOrDefault();
				this.opcode = valueOrDefault;
				this.opcodes.Add(valueOrDefault);
			}
			if (operand != null)
			{
				this.operands.Add(operand);
			}
			this.operand = operand;
			this.name = name;
		}

		public CodeMatch(Expression<Action> expression, string name = null)
		{
			this.opcodes.AddRange(new OpCode[]
			{
				OpCodes.Call,
				OpCodes.Callvirt
			});
			this.operand = SymbolExtensions.GetMethodInfo(expression);
			this.name = name;
		}

		public CodeMatch(LambdaExpression expression, string name = null)
		{
			this.opcodes.AddRange(new OpCode[]
			{
				OpCodes.Call,
				OpCodes.Callvirt
			});
			this.operand = SymbolExtensions.GetMethodInfo(expression);
			this.name = name;
		}

		public CodeMatch(CodeInstruction instruction, string name = null)
			: this(new OpCode?(instruction.opcode), instruction.operand, name)
		{
		}

		public CodeMatch(Func<CodeInstruction, bool> predicate, string name = null)
		{
			this.predicate = predicate;
			this.name = name;
		}

		internal bool Matches(List<CodeInstruction> codes, CodeInstruction instruction)
		{
			if (this.predicate != null)
			{
				return this.predicate(instruction);
			}
			if (this.opcodes.Count > 0 && !this.opcodes.Contains(instruction.opcode))
			{
				return false;
			}
			if (this.operands.Count > 0 && !this.operands.Contains(instruction.operand))
			{
				return false;
			}
			if (this.labels.Count > 0 && !this.labels.Intersect<Label>(instruction.labels).Any<Label>())
			{
				return false;
			}
			if (this.blocks.Count > 0 && !this.blocks.Intersect<ExceptionBlock>(instruction.blocks).Any<ExceptionBlock>())
			{
				return false;
			}
			if (this.jumpsFrom.Count > 0 && !this.jumpsFrom.Select<int, object>((int index) => codes[index].operand).OfType<Label>().Intersect<Label>(instruction.labels)
				.Any<Label>())
			{
				return false;
			}
			if (this.jumpsTo.Count > 0)
			{
				object operand = instruction.operand;
				if (operand == null || operand.GetType() != typeof(Label))
				{
					return false;
				}
				Label label = (Label)operand;
				IEnumerable<int> enumerable = from idx in Enumerable.Range(0, codes.Count)
					where codes[idx].labels.Contains(label)
					select idx;
				if (!this.jumpsTo.Intersect<int>(enumerable).Any<int>())
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			string text = "[";
			if (this.name != null)
			{
				text = text + this.name + ": ";
			}
			if (this.opcodes.Count > 0)
			{
				text = text + "opcodes=" + this.opcodes.Join<OpCode>(null, ", ") + " ";
			}
			if (this.operands.Count > 0)
			{
				text = text + "operands=" + this.operands.Join<object>(null, ", ") + " ";
			}
			if (this.labels.Count > 0)
			{
				text = text + "labels=" + this.labels.Join<Label>(null, ", ") + " ";
			}
			if (this.blocks.Count > 0)
			{
				text = text + "blocks=" + this.blocks.Join<ExceptionBlock>(null, ", ") + " ";
			}
			if (this.jumpsFrom.Count > 0)
			{
				text = text + "jumpsFrom=" + this.jumpsFrom.Join<int>(null, ", ") + " ";
			}
			if (this.jumpsTo.Count > 0)
			{
				text = text + "jumpsTo=" + this.jumpsTo.Join<int>(null, ", ") + " ";
			}
			if (this.predicate != null)
			{
				text += "predicate=yes ";
			}
			return text.TrimEnd(Array.Empty<char>()) + "]";
		}

		public string name;

		public List<OpCode> opcodes = new List<OpCode>();

		public List<object> operands = new List<object>();

		public List<int> jumpsFrom = new List<int>();

		public List<int> jumpsTo = new List<int>();

		public Func<CodeInstruction, bool> predicate;
	}
}
