using System;
using Mono.Collections.Generic;

namespace Mono.Cecil.Cil
{
	internal sealed class VariableDefinitionCollection : Collection<VariableDefinition>
	{
		internal VariableDefinitionCollection(MethodDefinition method)
		{
			this.method = method;
		}

		internal VariableDefinitionCollection(MethodDefinition method, int capacity)
			: base(capacity)
		{
			this.method = method;
		}

		protected override void OnAdd(VariableDefinition item, int index)
		{
			item.index = index;
		}

		protected override void OnInsert(VariableDefinition item, int index)
		{
			item.index = index;
			this.UpdateVariableIndices(index, 1, null);
		}

		protected override void OnSet(VariableDefinition item, int index)
		{
			item.index = index;
		}

		protected override void OnRemove(VariableDefinition item, int index)
		{
			this.UpdateVariableIndices(index + 1, -1, item);
			item.index = -1;
		}

		private void UpdateVariableIndices(int startIndex, int offset, VariableDefinition variableToRemove = null)
		{
			for (int i = startIndex; i < this.size; i++)
			{
				this.items[i].index = i + offset;
			}
			MethodDebugInformation methodDebugInformation = ((this.method == null) ? null : this.method.debug_info);
			if (methodDebugInformation == null || methodDebugInformation.Scope == null)
			{
				return;
			}
			foreach (ScopeDebugInformation scopeDebugInformation in methodDebugInformation.GetScopes())
			{
				if (scopeDebugInformation.HasVariables)
				{
					Collection<VariableDebugInformation> variables = scopeDebugInformation.Variables;
					int num = -1;
					for (int j = 0; j < variables.Count; j++)
					{
						VariableDebugInformation variableDebugInformation = variables[j];
						if (variableToRemove != null && ((variableDebugInformation.index.IsResolved && variableDebugInformation.index.ResolvedVariable == variableToRemove) || (!variableDebugInformation.index.IsResolved && variableDebugInformation.Index == variableToRemove.Index)))
						{
							num = j;
						}
						else if (!variableDebugInformation.index.IsResolved && variableDebugInformation.Index >= startIndex)
						{
							variableDebugInformation.index = new VariableIndex(variableDebugInformation.Index + offset);
						}
					}
					if (num >= 0)
					{
						variables.RemoveAt(num);
					}
				}
			}
		}

		private readonly MethodDefinition method;
	}
}
