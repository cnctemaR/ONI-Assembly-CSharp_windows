using System;

namespace System.Transactions
{
	public enum DependentCloneOption
	{
		BlockCommitUntilComplete,
		RollbackIfNotComplete
	}
}
