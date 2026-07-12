using System;

namespace System.Runtime.InteropServices
{
	public sealed class ErrorWrapper
	{
		public ErrorWrapper(int errorCode)
		{
			this.m_ErrorCode = errorCode;
		}

		public ErrorWrapper(object errorCode)
		{
			if (!(errorCode is int))
			{
				throw new ArgumentException("Object must be of type Int32.", "errorCode");
			}
			this.m_ErrorCode = (int)errorCode;
		}

		public ErrorWrapper(Exception e)
		{
			this.m_ErrorCode = Marshal.GetHRForException(e);
		}

		public int ErrorCode
		{
			get
			{
				return this.m_ErrorCode;
			}
		}

		private int m_ErrorCode;
	}
}
