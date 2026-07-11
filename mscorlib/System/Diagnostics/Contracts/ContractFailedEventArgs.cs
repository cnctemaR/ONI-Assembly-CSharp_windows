using System;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace System.Diagnostics.Contracts
{
	public sealed class ContractFailedEventArgs : EventArgs
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public ContractFailedEventArgs(ContractFailureKind failureKind, string message, string condition, Exception originalException)
		{
			this._failureKind = failureKind;
			this._message = message;
			this._condition = condition;
			this._originalException = originalException;
		}

		public string Message
		{
			get
			{
				return this._message;
			}
		}

		public string Condition
		{
			get
			{
				return this._condition;
			}
		}

		public ContractFailureKind FailureKind
		{
			get
			{
				return this._failureKind;
			}
		}

		public Exception OriginalException
		{
			get
			{
				return this._originalException;
			}
		}

		public bool Handled
		{
			get
			{
				return this._handled;
			}
		}

		[SecurityCritical]
		public void SetHandled()
		{
			this._handled = true;
		}

		public bool Unwind
		{
			get
			{
				return this._unwind;
			}
		}

		[SecurityCritical]
		public void SetUnwind()
		{
			this._unwind = true;
		}

		private ContractFailureKind _failureKind;

		private string _message;

		private string _condition;

		private Exception _originalException;

		private bool _handled;

		private bool _unwind;

		internal Exception thrownDuringHandler;
	}
}
