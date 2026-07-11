using System;
using System.Security;

namespace System.Threading.Tasks
{
	internal class StackGuard
	{
		[SecuritySafeCritical]
		internal bool TryBeginInliningScope()
		{
			if (this.m_inliningDepth < 20 || this.CheckForSufficientStack())
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

		[SecurityCritical]
		private bool CheckForSufficientStack()
		{
			return true;
		}

		private int m_inliningDepth;

		private const int MAX_UNCHECKED_INLINING_DEPTH = 20;
	}
}
