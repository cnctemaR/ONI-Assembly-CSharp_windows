using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public sealed class ErrorWrapper
	{
		public ErrorWrapper(Exception e)
		{
			this.errorCode = Marshal.GetHRForException(e);
		}

		public ErrorWrapper(int errorCode)
		{
			this.errorCode = errorCode;
		}

		public ErrorWrapper(object errorCode)
		{
			if (errorCode.GetType() != typeof(int))
			{
				throw new ArgumentException("errorCode has to be an int type");
			}
			this.errorCode = (int)errorCode;
		}

		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		private int errorCode;
	}
}
