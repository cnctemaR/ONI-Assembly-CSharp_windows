using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[Serializable]
	public class ExternalException : SystemException
	{
		public ExternalException()
			: base("External component has thrown an exception.")
		{
			base.HResult = -2147467259;
		}

		public ExternalException(string message)
			: base(message)
		{
			base.HResult = -2147467259;
		}

		public ExternalException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2147467259;
		}

		public ExternalException(string message, int errorCode)
			: base(message)
		{
			base.HResult = errorCode;
		}

		protected ExternalException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public virtual int ErrorCode
		{
			get
			{
				return base.HResult;
			}
		}

		public override string ToString()
		{
			string message = this.Message;
			string text = base.GetType().ToString() + " (0x" + base.HResult.ToString("X8", CultureInfo.InvariantCulture) + ")";
			if (!string.IsNullOrEmpty(message))
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
