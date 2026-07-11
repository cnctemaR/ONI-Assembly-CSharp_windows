using System;

namespace System.ComponentModel.Design
{
	public interface IDesignerHostTransactionState
	{
		bool IsClosingTransaction { get; }
	}
}
