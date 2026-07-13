using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyLib
{
	public class CodeMatch : CodeInstruction
	{
		[Obsolete("Use opcodeSet instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public List<OpCode> opcodes
		{
			get
			{
				return this.opcodeSet.ToList<OpCode>();
			}
			set
			{
				HashSet<OpCode> hashSet = new HashSet<OpCode>();
				foreach (OpCode opCode in value)
				{
					hashSet.Add(opCode);
				}
				this.opcodeSet = hashSet;
			}
		}

		internal CodeMatch Set(object operand, string name)
		{
			if (this.operand == null)
			{
				this.operand = operand;
			}
			if (operand != null)
			{
				this.operands.Add(operand);
			}
			if (this.name == null)
			{
				this.name = name;
			}
			return this;
		}

		internal CodeMatch Set(OpCode opcode, object operand, string name)
		{
			this.opcode = opcode;
			this.opcodeSet.Add(opcode);
			if (this.operand == null)
			{
				this.operand = operand;
			}
			if (operand != null)
			{
				this.operands.Add(operand);
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
				this.opcodeSet.Add(valueOrDefault);
			}
			if (operand != null)
			{
				this.operands.Add(operand);
			}
			this.operand = operand;
			this.name = name;
		}

		public static CodeMatch WithOpcodes(HashSet<OpCode> opcodes, object operand = null, string name = null)
		{
			return new CodeMatch(null, operand, name)
			{
				opcodeSet = opcodes
			};
		}

		public CodeMatch(Expression<Action> expression, string name = null)
		{
			this.opcodeSet.UnionWith(CodeInstructionExtensions.opcodesCalling);
			this.operand = SymbolExtensions.GetMethodInfo(expression);
			if (this.operand != null)
			{
				this.operands.Add(this.operand);
			}
			this.name = name;
		}

		public CodeMatch(LambdaExpression expression, string name = null)
		{
			this.opcodeSet.UnionWith(CodeInstructionExtensions.opcodesCalling);
			this.operand = SymbolExtensions.GetMethodInfo(expression);
			if (this.operand != null)
			{
				this.operands.Add(this.operand);
			}
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
			if (this.opcodeSet.Count > 0 && !this.opcodeSet.Contains(instruction.opcode))
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

		public static CodeMatch IsLdarg(int? n = null)
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.IsLdarg(n), null);
		}

		public static CodeMatch IsLdarga(int? n = null)
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.IsLdarga(n), null);
		}

		public static CodeMatch IsStarg(int? n = null)
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.IsStarg(n), null);
		}

		public static CodeMatch IsLdloc(LocalBuilder variable = null)
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.IsLdloc(variable), null);
		}

		public static CodeMatch IsStloc(LocalBuilder variable = null)
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.IsStloc(variable), null);
		}

		public static CodeMatch Calls(MethodInfo method)
		{
			return CodeMatch.WithOpcodes(CodeInstructionExtensions.opcodesCalling, method, null);
		}

		public static CodeMatch LoadsConstant()
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.LoadsConstant(), null);
		}

		public static CodeMatch LoadsConstant(long number)
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.LoadsConstant(number), null);
		}

		public static CodeMatch LoadsConstant(double number)
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.LoadsConstant(number), null);
		}

		public static CodeMatch LoadsConstant(Enum e)
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.LoadsConstant(e), null);
		}

		public static CodeMatch LoadsConstant(string str)
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.LoadsConstant(str), null);
		}

		public static CodeMatch LoadsField(FieldInfo field, bool byAddress = false)
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.LoadsField(field, byAddress), null);
		}

		public static CodeMatch StoresField(FieldInfo field)
		{
			return new CodeMatch((CodeInstruction instruction) => instruction.StoresField(field), null);
		}

		public static CodeMatch Calls(Expression<Action> expression)
		{
			return new CodeMatch(expression, null);
		}

		public static CodeMatch Calls(LambdaExpression expression)
		{
			return new CodeMatch(expression, null);
		}

		public static CodeMatch LoadsLocal(bool useAddress = false, string name = null)
		{
			return CodeMatch.WithOpcodes(useAddress ? CodeInstructionExtensions.opcodesLoadingLocalByAddress : CodeInstructionExtensions.opcodesLoadingLocalNormal, null, name);
		}

		public static CodeMatch StoresLocal(string name = null)
		{
			return CodeMatch.WithOpcodes(CodeInstructionExtensions.opcodesStoringLocal, null, name);
		}

		public static CodeMatch LoadsArgument(bool useAddress = false, string name = null)
		{
			return CodeMatch.WithOpcodes(useAddress ? CodeInstructionExtensions.opcodesLoadingArgumentByAddress : CodeInstructionExtensions.opcodesLoadingArgumentNormal, null, name);
		}

		public static CodeMatch StoresArgument(string name = null)
		{
			return CodeMatch.WithOpcodes(CodeInstructionExtensions.opcodesStoringArgument, null, name);
		}

		public static CodeMatch Branches(string name = null)
		{
			return CodeMatch.WithOpcodes(CodeInstructionExtensions.opcodesBranching, null, name);
		}

		public override string ToString()
		{
			string text = "[";
			if (this.name != null)
			{
				text = text + this.name + ": ";
			}
			if (this.opcodeSet.Count > 0)
			{
				text = text + "opcodes=" + this.opcodeSet.Join<OpCode>(null, ", ") + " ";
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

		public HashSet<OpCode> opcodeSet = new HashSet<OpCode>();

		public List<object> operands = new List<object>();

		public List<int> jumpsFrom = new List<int>();

		public List<int> jumpsTo = new List<int>();

		public Func<CodeInstruction, bool> predicate;
	}
}
