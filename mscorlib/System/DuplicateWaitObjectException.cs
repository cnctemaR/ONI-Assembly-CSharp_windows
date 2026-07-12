using System;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class DuplicateWaitObjectException : ArgumentException
	{
		private static string DuplicateWaitObjectMessage
		{
			get
			{
				if (DuplicateWaitObjectException.s_duplicateWaitObjectMessage == null)
				{
					DuplicateWaitObjectException.s_duplicateWaitObjectMessage = "Duplicate objects in argument.";
				}
				return DuplicateWaitObjectException.s_duplicateWaitObjectMessage;
			}
		}

		public DuplicateWaitObjectException()
			: base(DuplicateWaitObjectException.DuplicateWaitObjectMessage)
		{
			base.HResult = -2146233047;
		}

		public DuplicateWaitObjectException(string parameterName)
			: base(DuplicateWaitObjectException.DuplicateWaitObjectMessage, parameterName)
		{
			base.HResult = -2146233047;
		}

		public DuplicateWaitObjectException(string parameterName, string message)
			: base(message, parameterName)
		{
			base.HResult = -2146233047;
		}

		public DuplicateWaitObjectException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233047;
		}

		protected DuplicateWaitObjectException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private static volatile string s_duplicateWaitObjectMessage;
	}
}
