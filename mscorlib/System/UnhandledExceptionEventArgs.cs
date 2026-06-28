using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class UnhandledExceptionEventArgs : EventArgs
	{
		public UnhandledExceptionEventArgs(object exception, bool isTerminating)
		{
			this.exception = exception;
			this.m_isTerminating = isTerminating;
		}

		public object ExceptionObject
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return this.exception;
			}
		}

		public bool IsTerminating
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return this.m_isTerminating;
			}
		}

		private object exception;

		private bool m_isTerminating;
	}
}
