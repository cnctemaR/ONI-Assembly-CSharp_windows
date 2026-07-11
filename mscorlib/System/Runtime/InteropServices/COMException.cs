using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;
using Microsoft.Win32;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public class COMException : ExternalException
	{
		public COMException()
			: base(Environment.GetResourceString("Error HRESULT E_FAIL has been returned from a call to a COM component."))
		{
			base.SetErrorCode(-2147467259);
		}

		public COMException(string message)
			: base(message)
		{
			base.SetErrorCode(-2147467259);
		}

		public COMException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2147467259);
		}

		public COMException(string message, int errorCode)
			: base(message)
		{
			base.SetErrorCode(errorCode);
		}

		[SecuritySafeCritical]
		internal COMException(int hresult)
			: base(Win32Native.GetMessage(hresult))
		{
			base.SetErrorCode(hresult);
		}

		internal COMException(string message, int hresult, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(hresult);
		}

		protected COMException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public override string ToString()
		{
			string message = this.Message;
			string text = base.GetType().ToString() + " (0x" + base.HResult.ToString("X8", CultureInfo.InvariantCulture) + ")";
			if (message != null && message.Length > 0)
			{
				text = text + ": " + message;
			}
			Exception innerException = base.InnerException;
			if (innerException != null)
			{
				text = text + " ---> " + innerException.ToString();
			}
			if (this.StackTrace != null)
			{
				text = text + Environment.NewLine + this.StackTrace;
			}
			return text;
		}
	}
}
