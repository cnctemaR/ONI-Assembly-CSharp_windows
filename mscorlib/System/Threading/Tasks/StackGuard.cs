using System;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
	internal class StackGuard
	{
		internal bool TryBeginInliningScope()
		{
			if (this.m_inliningDepth < 20 || RuntimeHelpers.TryEnsureSufficientExecutionStack())
			{
				this.m_inliningDepth++;
				return true;
			}
			return false;
		}

		internal void EndInliningScope()
		{
			this.m_inliningDepth--;
			if (this.m_inliningDepth < 0)
			{
				this.m_inliningDepth = 0;
			}
		}

		private int m_inliningDepth;

		private const int MAX_UNCHECKED_INLINING_DEPTH = 20;
	}
}
