using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class RankException : SystemException
	{
		public RankException()
			: base(Environment.GetResourceString("Attempted to operate on an array with the incorrect number of dimensions."))
		{
			base.SetErrorCode(-2146233065);
		}

		public RankException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233065);
		}

		public RankException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.SetErrorCode(-2146233065);
		}

		protected RankException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
