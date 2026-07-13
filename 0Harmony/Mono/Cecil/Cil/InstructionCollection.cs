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
			this.UpdateDebugInformation(null, null);
		}

		protected override void OnSet(Instruction item, int index)
		{
			Instruction instruction = this.items[index];
			item.previous = instruction.previous;
			item.next = instruction.next;
			instruction.previous = null;
			instruction.next = null;
			this.UpdateDebugInformation(item, instruction);
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
			this.UpdateDebugInformation(item, next ?? previous);
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

		private void UpdateDebugInformation(Instruction removedInstruction, Instruction existingInstruction)
		{
			InstructionCollection.InstructionOffsetResolver instructionOffsetResolver = new InstructionCollection.InstructionOffsetResolver(this.items, removedInstruction, existingInstruction);
			if (this.method.debug_info != null)
			{
				this.UpdateLocalScope(this.method.debug_info.Scope, ref instructionOffsetResolver);
			}
			Collection<CustomDebugInformation> collection;
			if ((collection = this.method.custom_infos) == null)
			{
				MethodDebugInformation debug_info = this.method.debug_info;
				collection = ((debug_info != null) ? debug_info.custom_infos : null);
			}
			Collection<CustomDebugInformation> collection2 = collection;
			if (collection2 != null)
			{
				foreach (CustomDebugInformation customDebugInformation in collection2)
				{
					StateMachineScopeDebugInformation stateMachineScopeDebugInformation = customDebugInformation as StateMachineScopeDebugInformation;
					if (stateMachineScopeDebugInformation == null)
					{
						AsyncMethodBodyDebugInformation asyncMethodBodyDebugInformation = customDebugInformation as AsyncMethodBodyDebugInformation;
						if (asyncMethodBodyDebugInformation != null)
						{
							this.UpdateAsyncMethodBody(asyncMethodBodyDebugInformation, ref instructionOffsetResolver);
						}
					}
					else
					{
						this.UpdateStateMachineScope(stateMachineScopeDebugInformation, ref instructionOffsetResolver);
					}
				}
			}
		}

		private void UpdateLocalScope(ScopeDebugInformation scope, ref InstructionCollection.InstructionOffsetResolver resolver)
		{
			if (scope == null)
			{
				return;
			}
			scope.Start = resolver.Resolve(scope.Start);
			if (scope.HasScopes)
			{
				foreach (ScopeDebugInformation scopeDebugInformation in scope.Scopes)
				{
					this.UpdateLocalScope(scopeDebugInformation, ref resolver);
				}
			}
			scope.End = resolver.Resolve(scope.End);
		}

		private void UpdateStateMachineScope(StateMachineScopeDebugInformation debugInfo, ref InstructionCollection.InstructionOffsetResolver resolver)
		{
			resolver.Restart();
			foreach (StateMachineScope stateMachineScope in debugInfo.Scopes)
			{
				stateMachineScope.Start = resolver.Resolve(stateMachineScope.Start);
				stateMachineScope.End = resolver.Resolve(stateMachineScope.End);
			}
		}

		private void UpdateAsyncMethodBody(AsyncMethodBodyDebugInformation debugInfo, ref InstructionCollection.InstructionOffsetResolver resolver)
		{
			if (!debugInfo.CatchHandler.IsResolved)
			{
				resolver.Restart();
				debugInfo.CatchHandler = resolver.Resolve(debugInfo.CatchHandler);
			}
			resolver.Restart();
			for (int i = 0; i < debugInfo.Yields.Count; i++)
			{
				debugInfo.Yields[i] = resolver.Resolve(debugInfo.Yields[i]);
			}
			resolver.Restart();
			for (int j = 0; j < debugInfo.Resumes.Count; j++)
			{
				debugInfo.Resumes[j] = resolver.Resolve(debugInfo.Resumes[j]);
			}
		}

		private readonly MethodDefinition method;

		private struct InstructionOffsetResolver
		{
			public int LastOffset
			{
				get
				{
					return this.cache_offset;
				}
			}

			public InstructionOffsetResolver(Instruction[] instructions, Instruction removedInstruction, Instruction existingInstruction)
			{
				this.items = instructions;
				this.removed_instruction = removedInstruction;
				this.existing_instruction = existingInstruction;
				this.cache_offset = 0;
				this.cache_index = 0;
				this.cache_instruction = this.items[0];
			}

			public void Restart()
			{
				this.cache_offset = 0;
				this.cache_index = 0;
				this.cache_instruction = this.items[0];
			}

			public InstructionOffset Resolve(InstructionOffset inputOffset)
			{
				InstructionOffset instructionOffset = this.ResolveInstructionOffset(inputOffset);
				if (!instructionOffset.IsEndOfMethod && instructionOffset.ResolvedInstruction == this.removed_instruction)
				{
					instructionOffset = new InstructionOffset(this.existing_instruction);
				}
				return instructionOffset;
			}

			private InstructionOffset ResolveInstructionOffset(InstructionOffset inputOffset)
			{
				if (inputOffset.IsResolved)
				{
					return inputOffset;
				}
				int offset = inputOffset.Offset;
				if (this.cache_offset == offset)
				{
					return new InstructionOffset(this.cache_instruction);
				}
				if (this.cache_offset > offset)
				{
					int num = 0;
					for (int i = 0; i < this.items.Length; i++)
					{
						if (this.items[i] == null)
						{
							return new InstructionOffset((i == 0) ? this.items[0] : this.items[i - 1]);
						}
						if (num == offset)
						{
							return new InstructionOffset(this.items[i]);
						}
						if (num > offset)
						{
							return new InstructionOffset((i == 0) ? this.items[0] : this.items[i - 1]);
						}
						num += this.items[i].GetSize();
					}
					return default(InstructionOffset);
				}
				int num2 = this.cache_offset;
				for (int j = this.cache_index; j < this.items.Length; j++)
				{
					this.cache_index = j;
					this.cache_offset = num2;
					Instruction instruction = this.items[j];
					if (instruction == null)
					{
						return new InstructionOffset((j == 0) ? this.items[0] : this.items[j - 1]);
					}
					this.cache_instruction = instruction;
					if (this.cache_offset == offset)
					{
						return new InstructionOffset(this.cache_instruction);
					}
					if (this.cache_offset > offset)
					{
						return new InstructionOffset((j == 0) ? this.items[0] : this.items[j - 1]);
					}
					num2 += instruction.GetSize();
				}
				return default(InstructionOffset);
			}

			private readonly Instruction[] items;

			private readonly Instruction removed_instruction;

			private readonly Instruction existing_instruction;

			private int cache_offset;

			private int cache_index;

			private Instruction cache_instruction;
		}
	}
}
