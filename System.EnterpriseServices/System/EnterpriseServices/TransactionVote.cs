using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[Serializable]
	public enum TransactionVote
	{
		Abort = 1,
		Commit = 0
	}
}
