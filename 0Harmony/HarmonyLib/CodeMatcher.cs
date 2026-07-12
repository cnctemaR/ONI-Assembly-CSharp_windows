using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyLib
{
	public class CodeMatcher
	{
		public int Pos { get; private set; } = -1;

		private void FixStart()
		{
			this.Pos = Math.Max(0, this.Pos);
		}

		private void SetOutOfBounds(int direction)
		{
			this.Pos = ((direction > 0) ? this.Length : (-1));
		}

		public int Length
		{
			get
			{
				return this.codes.Count;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.Pos >= 0 && this.Pos < this.Length;
			}
		}

		public bool IsInvalid
		{
			get
			{
				return this.Pos < 0 || this.Pos >= this.Length;
			}
		}

		public int Remaining
		{
			get
			{
				return this.Length - Math.Max(0, this.Pos);
			}
		}

		public ref OpCode Opcode
		{
			get
			{
				return ref this.codes[this.Pos].opcode;
			}
		}

		public ref object Operand
		{
			get
			{
				return ref this.codes[this.Pos].operand;
			}
		}

		public ref List<Label> Labels
		{
			get
			{
				return ref this.codes[this.Pos].labels;
			}
		}

		public ref List<ExceptionBlock> Blocks
		{
			get
			{
				return ref this.codes[this.Pos].blocks;
			}
		}

		public CodeMatcher()
		{
		}

		public CodeMatcher(IEnumerable<CodeInstruction> instructions, ILGenerator generator = null)
		{
			this.generator = generator;
			this.codes = instructions.Select<CodeInstruction, CodeInstruction>((CodeInstruction c) => new CodeInstruction(c)).ToList<CodeInstruction>();
		}

		public CodeMatcher Clone()
		{
			return new CodeMatcher(this.codes, this.generator)
			{
				Pos = this.Pos,
				lastMatches = this.lastMatches,
				lastError = this.lastError,
				lastMatchCall = this.lastMatchCall
			};
		}

		public CodeInstruction Instruction
		{
			get
			{
				return this.codes[this.Pos];
			}
		}

		public CodeInstruction InstructionAt(int offset)
		{
			return this.codes[this.Pos + offset];
		}

		public List<CodeInstruction> Instructions()
		{
			return this.codes;
		}

		public IEnumerable<CodeInstruction> InstructionEnumeration()
		{
			return this.codes.AsEnumerable<CodeInstruction>();
		}

		public List<CodeInstruction> Instructions(int count)
		{
			return (from c in this.codes.GetRange(this.Pos, count)
				select new CodeInstruction(c)).ToList<CodeInstruction>();
		}

		public List<CodeInstruction> InstructionsInRange(int start, int end)
		{
			List<CodeInstruction> list = this.codes;
			if (start > end)
			{
				int num = start;
				start = end;
				end = num;
			}
			return (from c in list.GetRange(start, end - start + 1)
				select new CodeInstruction(c)).ToList<CodeInstruction>();
		}

		public List<CodeInstruction> InstructionsWithOffsets(int startOffset, int endOffset)
		{
			return this.InstructionsInRange(this.Pos + startOffset, this.Pos + endOffset);
		}

		public List<Label> DistinctLabels(IEnumerable<CodeInstruction> instructions)
		{
			return instructions.SelectMany<CodeInstruction, Label>((CodeInstruction instruction) => instruction.labels).Distinct<Label>().ToList<Label>();
		}

		public bool ReportFailure(MethodBase method, Action<string> logger)
		{
			if (this.IsValid)
			{
				return false;
			}
			string text = this.lastError ?? "Unexpected code";
			logger(string.Format("{0} in {1}", text, method));
			return true;
		}

		public CodeMatcher ThrowIfInvalid(string explanation)
		{
			if (explanation == null)
			{
				throw new ArgumentNullException("explanation");
			}
			if (this.IsInvalid)
			{
				throw new InvalidOperationException(explanation + " - Current state is invalid");
			}
			return this;
		}

		public CodeMatcher ThrowIfNotMatch(string explanation, params CodeMatch[] matches)
		{
			this.ThrowIfInvalid(explanation);
			if (!this.MatchSequence(this.Pos, matches))
			{
				throw new InvalidOperationException(explanation + " - Match failed");
			}
			return this;
		}

		private void ThrowIfNotMatch(string explanation, int direction, CodeMatch[] matches)
		{
			this.ThrowIfInvalid(explanation);
			int pos = this.Pos;
			try
			{
				if (this.Match(matches, direction, false).IsInvalid)
				{
					throw new InvalidOperationException(explanation + " - Match failed");
				}
			}
			finally
			{
				this.Pos = pos;
			}
		}

		public CodeMatcher ThrowIfNotMatchForward(string explanation, params CodeMatch[] matches)
		{
			this.ThrowIfNotMatch(explanation, 1, matches);
			return this;
		}

		public CodeMatcher ThrowIfNotMatchBack(string explanation, params CodeMatch[] matches)
		{
			this.ThrowIfNotMatch(explanation, -1, matches);
			return this;
		}

		public CodeMatcher ThrowIfFalse(string explanation, Func<CodeMatcher, bool> stateCheckFunc)
		{
			if (stateCheckFunc == null)
			{
				throw new ArgumentNullException("stateCheckFunc");
			}
			this.ThrowIfInvalid(explanation);
			if (!stateCheckFunc(this))
			{
				throw new InvalidOperationException(explanation + " - Check function returned false");
			}
			return this;
		}

		public CodeMatcher SetInstruction(CodeInstruction instruction)
		{
			this.codes[this.Pos] = instruction;
			return this;
		}

		public CodeMatcher SetInstructionAndAdvance(CodeInstruction instruction)
		{
			this.SetInstruction(instruction);
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		public unsafe CodeMatcher Set(OpCode opcode, object operand)
		{
			*this.Opcode = opcode;
			*this.Operand = operand;
			return this;
		}

		public CodeMatcher SetAndAdvance(OpCode opcode, object operand)
		{
			this.Set(opcode, operand);
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		public unsafe CodeMatcher SetOpcodeAndAdvance(OpCode opcode)
		{
			*this.Opcode = opcode;
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		public unsafe CodeMatcher SetOperandAndAdvance(object operand)
		{
			*this.Operand = operand;
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		public unsafe CodeMatcher CreateLabel(out Label label)
		{
			label = this.generator.DefineLabel();
			this.Labels->Add(label);
			return this;
		}

		public CodeMatcher CreateLabelAt(int position, out Label label)
		{
			label = this.generator.DefineLabel();
			this.AddLabelsAt(position, new Label[] { label });
			return this;
		}

		public CodeMatcher CreateLabelWithOffsets(int offset, out Label label)
		{
			label = this.generator.DefineLabel();
			return this.AddLabelsAt(this.Pos + offset, new Label[] { label });
		}

		public unsafe CodeMatcher AddLabels(IEnumerable<Label> labels)
		{
			this.Labels->AddRange(labels);
			return this;
		}

		public CodeMatcher AddLabelsAt(int position, IEnumerable<Label> labels)
		{
			this.codes[position].labels.AddRange(labels);
			return this;
		}

		public CodeMatcher SetJumpTo(OpCode opcode, int destination, out Label label)
		{
			this.CreateLabelAt(destination, out label);
			return this.Set(opcode, label);
		}

		public CodeMatcher Insert(params CodeInstruction[] instructions)
		{
			this.codes.InsertRange(this.Pos, instructions);
			return this;
		}

		public CodeMatcher Insert(IEnumerable<CodeInstruction> instructions)
		{
			this.codes.InsertRange(this.Pos, instructions);
			return this;
		}

		public CodeMatcher InsertBranch(OpCode opcode, int destination)
		{
			Label label;
			this.CreateLabelAt(destination, out label);
			this.codes.Insert(this.Pos, new CodeInstruction(opcode, label));
			return this;
		}

		public CodeMatcher InsertAndAdvance(params CodeInstruction[] instructions)
		{
			foreach (CodeInstruction codeInstruction in instructions)
			{
				this.Insert(new CodeInstruction[] { codeInstruction });
				int pos = this.Pos;
				this.Pos = pos + 1;
			}
			return this;
		}

		public CodeMatcher InsertAndAdvance(IEnumerable<CodeInstruction> instructions)
		{
			foreach (CodeInstruction codeInstruction in instructions)
			{
				this.InsertAndAdvance(new CodeInstruction[] { codeInstruction });
			}
			return this;
		}

		public CodeMatcher InsertBranchAndAdvance(OpCode opcode, int destination)
		{
			this.InsertBranch(opcode, destination);
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		public CodeMatcher RemoveInstruction()
		{
			this.codes.RemoveAt(this.Pos);
			return this;
		}

		public CodeMatcher RemoveInstructions(int count)
		{
			this.codes.RemoveRange(this.Pos, count);
			return this;
		}

		public CodeMatcher RemoveInstructionsInRange(int start, int end)
		{
			if (start > end)
			{
				int num = start;
				start = end;
				end = num;
			}
			this.codes.RemoveRange(start, end - start + 1);
			return this;
		}

		public CodeMatcher RemoveInstructionsWithOffsets(int startOffset, int endOffset)
		{
			return this.RemoveInstructionsInRange(this.Pos + startOffset, this.Pos + endOffset);
		}

		public CodeMatcher Advance(int offset)
		{
			this.Pos += offset;
			if (!this.IsValid)
			{
				this.SetOutOfBounds(offset);
			}
			return this;
		}

		public CodeMatcher Start()
		{
			this.Pos = 0;
			return this;
		}

		public CodeMatcher End()
		{
			this.Pos = this.Length - 1;
			return this;
		}

		public CodeMatcher SearchForward(Func<CodeInstruction, bool> predicate)
		{
			return this.Search(predicate, 1);
		}

		public CodeMatcher SearchBackwards(Func<CodeInstruction, bool> predicate)
		{
			return this.Search(predicate, -1);
		}

		private CodeMatcher Search(Func<CodeInstruction, bool> predicate, int direction)
		{
			this.FixStart();
			while (this.IsValid && !predicate(this.Instruction))
			{
				this.Pos += direction;
			}
			this.lastError = (this.IsInvalid ? string.Format("Cannot find {0}", predicate) : null);
			return this;
		}

		public CodeMatcher MatchStartForward(params CodeMatch[] matches)
		{
			return this.Match(matches, 1, false);
		}

		public CodeMatcher MatchEndForward(params CodeMatch[] matches)
		{
			return this.Match(matches, 1, true);
		}

		public CodeMatcher MatchStartBackwards(params CodeMatch[] matches)
		{
			return this.Match(matches, -1, false);
		}

		public CodeMatcher MatchEndBackwards(params CodeMatch[] matches)
		{
			return this.Match(matches, -1, true);
		}

		private CodeMatcher Match(CodeMatch[] matches, int direction, bool useEnd)
		{
			this.lastMatchCall = delegate
			{
				this.FixStart();
				while (this.IsValid)
				{
					if (this.MatchSequence(this.Pos, matches))
					{
						if (useEnd)
						{
							this.Pos += matches.Length - 1;
							break;
						}
						break;
					}
					else
					{
						this.Pos += direction;
					}
				}
				this.lastError = (this.IsInvalid ? ("Cannot find " + matches.Join<CodeMatch>(null, ", ")) : null);
				return this;
			};
			return this.lastMatchCall();
		}

		public CodeMatcher Repeat(Action<CodeMatcher> matchAction, Action<string> notFoundAction = null)
		{
			int num = 0;
			if (this.lastMatchCall == null)
			{
				throw new InvalidOperationException("No previous Match operation - cannot repeat");
			}
			while (this.IsValid)
			{
				matchAction(this);
				this.lastMatchCall();
				num++;
			}
			this.lastMatchCall = null;
			if (num == 0 && notFoundAction != null)
			{
				notFoundAction(this.lastError);
			}
			return this;
		}

		public CodeInstruction NamedMatch(string name)
		{
			return this.lastMatches[name];
		}

		private bool MatchSequence(int start, CodeMatch[] matches)
		{
			if (start < 0)
			{
				return false;
			}
			this.lastMatches = new Dictionary<string, CodeInstruction>();
			foreach (CodeMatch codeMatch in matches)
			{
				if (start >= this.Length || !codeMatch.Matches(this.codes, this.codes[start]))
				{
					return false;
				}
				if (codeMatch.name != null)
				{
					this.lastMatches.Add(codeMatch.name, this.codes[start]);
				}
				start++;
			}
			return true;
		}

		private readonly ILGenerator generator;

		private readonly List<CodeInstruction> codes = new List<CodeInstruction>();

		private Dictionary<string, CodeInstruction> lastMatches = new Dictionary<string, CodeInstruction>();

		private string lastError;

		private CodeMatcher.MatchDelegate lastMatchCall;

		private delegate CodeMatcher MatchDelegate();
	}
}
