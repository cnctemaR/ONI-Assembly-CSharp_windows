using System;
using Mono.Collections.Generic;

namespace Mono.Cecil.Cil
{
	internal class InstructionCollection : Collection<Instruction>
	{
		internal InstructionCollection(MethodDefinition method)
		{
			this.method = method;
		}

		internal InstructionCollection(MethodDefinition method, int capacity)
			: base(capacity)
		{
			this.method = method;
		}

		protected override void OnAdd(Instruction item, int index)
		{
			if (index == 0)
			{
				return;
			}
			Instruction instruction = this.items[index - 1];
			instruction.next = item;
			item.previous = instruction;
		}

		protected override void OnInsert(Instruction item, int index)
		{
			if (this.size != 0)
			{
				Instruction instruction = this.items[index];
				if (instruction == null)
				{
					Instruction instruction2 = this.items[index - 1];
					instruction2.next = item;
					item.previous = instruction2;
					return;
				}
				int offset = instruction.Offset;
				Instruction previous = instruction.previous;
				if (previous != null)
				{
					previous.next = item;
					item.previous = previous;
				}
				instruction.previous = item;
				item.next = instruction;
			}
			this.UpdateLocalScopes(null, null);
		}

		protected override void OnSet(Instruction item, int index)
		{
			Instruction instruction = this.items[index];
			item.previous = instruction.previous;
			item.next = instruction.next;
			instruction.previous = null;
			instruction.next = null;
			this.UpdateLocalScopes(item, instruction);
		}

		protected override void OnRemove(Instruction item, int index)
		{
			Instruction previous = item.previous;
			if (previous != null)
			{
				previous.next = item.next;
			}
			Instruction next = item.next;
			if (next != null)
			{
				next.previous = item.previous;
			}
			this.RemoveSequencePoint(item);
			this.UpdateLocalScopes(item, next ?? previous);
			item.previous = null;
			item.next = null;
		}

		private void RemoveSequencePoint(Instruction instruction)
		{
			MethodDebugInformation debug_info = this.method.debug_info;
			if (debug_info == null || !debug_info.HasSequencePoints)
			{
				return;
			}
			Collection<SequencePoint> sequence_points = debug_info.sequence_points;
			for (int i = 0; i < sequence_points.Count; i++)
			{
				if (sequence_points[i].Offset == instruction.offset)
				{
					sequence_points.RemoveAt(i);
					return;
				}
			}
		}

		private void UpdateLocalScopes(Instruction removedInstruction, Instruction existingInstruction)
		{
			MethodDebugInformation debug_info = this.method.debug_info;
			if (debug_info == null)
			{
				return;
			}
			InstructionCollection.InstructionOffsetCache instructionOffsetCache = new InstructionCollection.InstructionOffsetCache
			{
				Offset = 0,
				Index = 0,
				Instruction = this.items[0]
			};
			this.UpdateLocalScope(debug_info.Scope, removedInstruction, existingInstruction, ref instructionOffsetCache);
		}

		private void UpdateLocalScope(ScopeDebugInformation scope, Instruction removedInstruction, Instruction existingInstruction, ref InstructionCollection.InstructionOffsetCache cache)
		{
			if (scope == null)
			{
				return;
			}
			if (!scope.Start.IsResolved)
			{
				scope.Start = this.ResolveInstructionOffset(scope.Start, ref cache);
			}
			if (!scope.Start.IsEndOfMethod && scope.Start.ResolvedInstruction == removedInstruction)
			{
				scope.Start = new InstructionOffset(existingInstruction);
			}
			if (scope.HasScopes)
			{
				foreach (ScopeDebugInformation scopeDebugInformation in scope.Scopes)
				{
					this.UpdateLocalScope(scopeDebugInformation, removedInstruction, existingInstruction, ref cache);
				}
			}
			if (!scope.End.IsResolved)
			{
				scope.End = this.ResolveInstructionOffset(scope.End, ref cache);
			}
			if (!scope.End.IsEndOfMethod && scope.End.ResolvedInstruction == removedInstruction)
			{
				scope.End = new InstructionOffset(existingInstruction);
			}
		}

		private InstructionOffset ResolveInstructionOffset(InstructionOffset inputOffset, ref InstructionCollection.InstructionOffsetCache cache)
		{
			if (inputOffset.IsResolved)
			{
				return inputOffset;
			}
			int offset = inputOffset.Offset;
			if (cache.Offset == offset)
			{
				return new InstructionOffset(cache.Instruction);
			}
			if (cache.Offset > offset)
			{
				int num = 0;
				for (int i = 0; i < this.items.Length; i++)
				{
					if (num == offset)
					{
						return new InstructionOffset(this.items[i]);
					}
					if (num > offset)
					{
						return new InstructionOffset(this.items[i - 1]);
					}
					num += this.items[i].GetSize();
				}
				return default(InstructionOffset);
			}
			int num2 = cache.Offset;
			for (int j = cache.Index; j < this.items.Length; j++)
			{
				cache.Index = j;
				cache.Offset = num2;
				cache.Instruction = this.items[j];
				if (cache.Offset == offset)
				{
					return new InstructionOffset(cache.Instruction);
				}
				if (cache.Offset > offset)
				{
					return new InstructionOffset(this.items[j - 1]);
				}
				num2 += this.items[j].GetSize();
			}
			return default(InstructionOffset);
		}

		private readonly MethodDefinition method;

		private struct InstructionOffsetCache
		{
			public int Offset;

			public int Index;

			public Instruction Instruction;
		}
	}
}
