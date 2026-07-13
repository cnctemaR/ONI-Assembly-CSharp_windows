using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	public class CodeMatcher
	{
		public int Pos { get; private set; } = -1;

		private void FixStart()
		{
			this.Pos = Math.Max(0, this.Pos);
		}

		private T HandleException<T>(string error, T defaultValue)
		{
			if (this.errorHandler != null && this.errorHandler(this, error))
			{
				return defaultValue;
			}
			this.lastError = error;
			throw new InvalidOperationException(error);
		}

		private void HandleException(string error)
		{
			this.lastError = error;
			if (this.errorHandler != null)
			{
				this.errorHandler(this, error);
				return;
			}
			throw new InvalidOperationException(error);
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
				lastMatches = new Dictionary<string, CodeInstruction>(this.lastMatches),
				lastError = this.lastError,
				lastMatchCall = this.lastMatchCall,
				errorHandler = this.errorHandler
			};
		}

		public CodeMatcher Reset(bool atFirstInstruction = true)
		{
			this.Pos = (atFirstInstruction ? 0 : (-1));
			this.lastMatches.Clear();
			this.lastError = null;
			this.lastMatchCall = null;
			return this;
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
			if (this.Pos < 0 || this.Pos + count > this.Length)
			{
				return this.HandleException<List<CodeInstruction>>("Cannot retrieve instructions: range is out-of-bounds.", new List<CodeInstruction>());
			}
			return (from c in this.codes.GetRange(this.Pos, count)
				select new CodeInstruction(c)).ToList<CodeInstruction>();
		}

		public List<CodeInstruction> InstructionsInRange(int start, int end)
		{
			List<CodeInstruction> range = this.codes;
			if (start > end)
			{
				int num = start;
				start = end;
				end = num;
			}
			if (start < 0 || end >= this.Length)
			{
				return this.HandleException<List<CodeInstruction>>("Cannot retrieve instructions: range is out-of-bounds.", new List<CodeInstruction>());
			}
			range = range.GetRange(start, end - start + 1);
			return range.Select<CodeInstruction, CodeInstruction>((CodeInstruction c) => new CodeInstruction(c)).ToList<CodeInstruction>();
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
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral(" in ");
			defaultInterpolatedStringHandler.AppendFormatted<MethodBase>(method);
			logger(defaultInterpolatedStringHandler.ToStringAndClear());
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
				return this.HandleException<CodeMatcher>(explanation + " - Current state is invalid", this);
			}
			return this;
		}

		public CodeMatcher ThrowIfNotMatch(string explanation, params CodeMatch[] matches)
		{
			this.ThrowIfInvalid(explanation);
			if (!this.MatchSequence(this.Pos, matches))
			{
				return this.HandleException<CodeMatcher>(explanation + " - Match failed", this);
			}
			return this;
		}

		private void ThrowIfNotMatch(string explanation, int direction, CodeMatch[] matches)
		{
			this.ThrowIfInvalid(explanation);
			int pos = this.Pos;
			try
			{
				if (this.Match(matches, direction, CodeMatcher.MatchPosition.Start, false).IsInvalid)
				{
					this.HandleException(explanation + " - Match failed");
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
				return this.HandleException<CodeMatcher>(explanation + " - Check function returned false", this);
			}
			return this;
		}

		public CodeMatcher Do(Action<CodeMatcher> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			action(this);
			return this;
		}

		public CodeMatcher OnError(CodeMatcher.ErrorHandler errorHandler)
		{
			this.errorHandler = errorHandler;
			return this;
		}

		public CodeMatcher SetInstruction(CodeInstruction instruction)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot set instruction/opcode at invalid position.", this);
			}
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
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot set values at invalid position.", this);
			}
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
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot set opcode at invalid position.", this);
			}
			*this.Opcode = opcode;
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		public unsafe CodeMatcher SetOperandAndAdvance(object operand)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot set operand at invalid position.", this);
			}
			*this.Operand = operand;
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		public CodeMatcher DeclareLocal(Type variableType, out LocalBuilder localVariable)
		{
			if (this.generator == null)
			{
				localVariable = null;
				return this.HandleException<CodeMatcher>("Generator must be provided to use this method", this);
			}
			localVariable = this.generator.DeclareLocal(variableType);
			return this;
		}

		public CodeMatcher DefineLabel(out Label label)
		{
			if (this.generator == null)
			{
				label = default(Label);
				return this.HandleException<CodeMatcher>("Generator must be provided to use this method", this);
			}
			label = this.generator.DefineLabel();
			return this;
		}

		public unsafe CodeMatcher CreateLabel(out Label label)
		{
			if (this.generator == null)
			{
				label = default(Label);
				return this.HandleException<CodeMatcher>("Generator must be provided to use this method", this);
			}
			label = this.generator.DefineLabel();
			this.Labels->Add(label);
			return this;
		}

		public CodeMatcher CreateLabelAt(int position, out Label label)
		{
			if (this.generator == null)
			{
				label = default(Label);
				return this.HandleException<CodeMatcher>("Generator must be provided to use this method", this);
			}
			label = this.generator.DefineLabel();
			this.AddLabelsAt(position, new <>z__ReadOnlySingleElementList<Label>(label));
			return this;
		}

		public CodeMatcher CreateLabelWithOffsets(int offset, out Label label)
		{
			if (this.generator == null)
			{
				label = default(Label);
				return this.HandleException<CodeMatcher>("Generator must be provided to use this method", this);
			}
			label = this.generator.DefineLabel();
			return this.AddLabelsAt(this.Pos + offset, new <>z__ReadOnlySingleElementList<Label>(label));
		}

		public unsafe CodeMatcher AddLabels(IEnumerable<Label> labels)
		{
			this.Labels->AddRange(labels);
			return this;
		}

		public CodeMatcher AddLabelsAt(int position, IEnumerable<Label> labels)
		{
			if (position < 0 || position >= this.Length)
			{
				return this.HandleException<CodeMatcher>("Cannot add labels at invalid position.", this);
			}
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
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					if (this.IsInvalid)
					{
						return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
					}
					this.codes.InsertRange(this.Pos, instructions);
					return this;
				}
			}
			throw new ArgumentNullException("instructions");
		}

		public CodeMatcher Insert(IEnumerable<CodeInstruction> instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					if (this.IsInvalid)
					{
						return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
					}
					this.codes.InsertRange(this.Pos, instructions);
					return this;
				}
			}
			throw new ArgumentNullException("instructions");
		}

		public CodeMatcher InsertBranch(OpCode opcode, int destination)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
			}
			Label label;
			this.CreateLabelAt(destination, out label);
			this.codes.Insert(this.Pos, new CodeInstruction(opcode, label));
			return this;
		}

		public CodeMatcher InsertAndAdvance(params CodeInstruction[] instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					foreach (CodeInstruction codeInstruction in instructions)
					{
						this.Insert(new CodeInstruction[] { codeInstruction });
						int pos = this.Pos;
						this.Pos = pos + 1;
					}
					return this;
				}
			}
			throw new ArgumentNullException("instructions");
		}

		public CodeMatcher InsertAndAdvance(IEnumerable<CodeInstruction> instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					foreach (CodeInstruction codeInstruction in instructions)
					{
						this.InsertAndAdvance(new CodeInstruction[] { codeInstruction });
					}
					return this;
				}
			}
			throw new ArgumentNullException("instructions");
		}

		public CodeMatcher InsertBranchAndAdvance(OpCode opcode, int destination)
		{
			this.InsertBranch(opcode, destination);
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		public CodeMatcher InsertAfter(params CodeInstruction[] instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					if (this.IsInvalid)
					{
						return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
					}
					this.codes.InsertRange(this.Pos + 1, instructions);
					return this;
				}
			}
			throw new ArgumentNullException("instructions");
		}

		public CodeMatcher InsertAfter(IEnumerable<CodeInstruction> instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					if (this.IsInvalid)
					{
						return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
					}
					this.codes.InsertRange(this.Pos + 1, instructions);
					return this;
				}
			}
			return this.HandleException<CodeMatcher>("Cannot insert null instructions.", this);
		}

		public CodeMatcher InsertBranchAfter(OpCode opcode, int destination)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
			}
			Label label;
			this.CreateLabelAt(destination, out label);
			this.codes.Insert(this.Pos + 1, new CodeInstruction(opcode, label));
			return this;
		}

		public CodeMatcher InsertAfterAndAdvance(params CodeInstruction[] instructions)
		{
			this.InsertAfter(instructions);
			this.Pos += instructions.Length;
			return this;
		}

		public CodeMatcher InsertAfterAndAdvance(IEnumerable<CodeInstruction> instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
					this.InsertAfter(list);
					this.Pos += list.Count;
					return this;
				}
			}
			return this.HandleException<CodeMatcher>("Cannot insert null instructions.", this);
		}

		public CodeMatcher InsertBranchAfterAndAdvance(OpCode opcode, int destination)
		{
			this.InsertBranchAfter(opcode, destination);
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		public CodeMatcher RemoveInstruction()
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid position.", this);
			}
			this.codes.RemoveAt(this.Pos);
			return this;
		}

		public CodeMatcher RemoveInstructions(int count)
		{
			if (this.IsInvalid || this.Pos + count > this.Length)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid or out-of-range position.", this);
			}
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
			if (start < 0 || end >= this.Length)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions: range is out-of-bounds.", this);
			}
			this.codes.RemoveRange(start, end - start + 1);
			return this;
		}

		public CodeMatcher RemoveInstructionsWithOffsets(int startOffset, int endOffset)
		{
			return this.RemoveInstructionsInRange(this.Pos + startOffset, this.Pos + endOffset);
		}

		public CodeMatcher Advance(int offset = 1)
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
			string text;
			if (!this.IsInvalid)
			{
				text = null;
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Cannot find ");
				defaultInterpolatedStringHandler.AppendFormatted<Func<CodeInstruction, bool>>(predicate);
				text = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			this.lastError = text;
			return this;
		}

		public CodeMatcher MatchStartForward(params CodeMatch[] matches)
		{
			return this.Match(matches, 1, CodeMatcher.MatchPosition.Start, false);
		}

		public CodeMatcher PrepareMatchStartForward(params CodeMatch[] matches)
		{
			return this.Match(matches, 1, CodeMatcher.MatchPosition.Start, true);
		}

		public CodeMatcher MatchEndForward(params CodeMatch[] matches)
		{
			return this.Match(matches, 1, CodeMatcher.MatchPosition.End, false);
		}

		public CodeMatcher PrepareMatchEndForward(params CodeMatch[] matches)
		{
			return this.Match(matches, 1, CodeMatcher.MatchPosition.End, true);
		}

		public CodeMatcher MatchStartBackwards(params CodeMatch[] matches)
		{
			return this.Match(matches, -1, CodeMatcher.MatchPosition.Start, false);
		}

		public CodeMatcher PrepareMatchStartBackwards(params CodeMatch[] matches)
		{
			return this.Match(matches, -1, CodeMatcher.MatchPosition.Start, true);
		}

		public CodeMatcher MatchEndBackwards(params CodeMatch[] matches)
		{
			return this.Match(matches, -1, CodeMatcher.MatchPosition.End, false);
		}

		public CodeMatcher PrepareMatchEndBackwards(params CodeMatch[] matches)
		{
			return this.Match(matches, -1, CodeMatcher.MatchPosition.End, true);
		}

		public CodeMatcher RemoveSearchForward(Func<CodeInstruction, bool> predicate)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid position.", this);
			}
			int pos = this.Pos;
			CodeMatcher codeMatcher = this.Clone().SearchForward(predicate);
			if (codeMatcher.IsInvalid)
			{
				this.lastError = codeMatcher.lastError;
				this.SetOutOfBounds(1);
				return this;
			}
			int num = codeMatcher.Pos - 1;
			if (num >= pos)
			{
				this.RemoveInstructionsInRange(pos, num);
			}
			return this;
		}

		public CodeMatcher RemoveSearchBackward(Func<CodeInstruction, bool> predicate)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid position.", this);
			}
			int pos = this.Pos;
			CodeMatcher codeMatcher = this.Clone().SearchBackwards(predicate);
			if (codeMatcher.IsInvalid)
			{
				this.lastError = codeMatcher.lastError;
				this.SetOutOfBounds(-1);
				return this;
			}
			int pos2 = codeMatcher.Pos;
			int num = pos2 + 1;
			if (pos >= num)
			{
				this.RemoveInstructionsInRange(num, pos);
			}
			this.Pos = pos2;
			return this;
		}

		public CodeMatcher RemoveUntilForward(params CodeMatch[] matches)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid position.", this);
			}
			int pos = this.Pos;
			CodeMatcher codeMatcher = this.Clone().MatchStartForward(matches);
			if (codeMatcher.IsInvalid)
			{
				this.lastError = codeMatcher.lastError;
				this.SetOutOfBounds(1);
				return this;
			}
			int num = codeMatcher.Pos - 1;
			if (num >= pos)
			{
				this.RemoveInstructionsInRange(pos, num);
			}
			return this;
		}

		public CodeMatcher RemoveUntilBackward(params CodeMatch[] matches)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid position.", this);
			}
			int pos = this.Pos;
			CodeMatcher codeMatcher = this.Clone().MatchEndBackwards(matches);
			if (codeMatcher.IsInvalid)
			{
				this.lastError = codeMatcher.lastError;
				this.SetOutOfBounds(-1);
				return this;
			}
			int pos2 = codeMatcher.Pos;
			if (pos > pos2)
			{
				this.RemoveInstructionsInRange(pos2 + 1, pos);
			}
			this.Pos = pos2;
			return this;
		}

		private CodeMatcher Match(CodeMatch[] matches, int direction, CodeMatcher.MatchPosition mode, bool prepareOnly)
		{
			this.lastMatchCall = delegate
			{
				while (this.IsValid)
				{
					if (this.MatchSequence(this.Pos, matches))
					{
						if (mode == CodeMatcher.MatchPosition.End)
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
			if (prepareOnly)
			{
				return this;
			}
			this.FixStart();
			return this.lastMatchCall();
		}

		public CodeMatcher Repeat(Action<CodeMatcher> matchAction, Action<string> notFoundAction = null)
		{
			int num = 0;
			if (this.lastMatchCall == null)
			{
				return this.HandleException<CodeMatcher>("No previous Match operation - cannot repeat", this);
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

		private CodeMatcher.ErrorHandler errorHandler;

		public delegate bool ErrorHandler(CodeMatcher matcher, string error);

		private enum MatchPosition
		{
			Start,
			End
		}

		private delegate CodeMatcher MatchDelegate();
	}
}
