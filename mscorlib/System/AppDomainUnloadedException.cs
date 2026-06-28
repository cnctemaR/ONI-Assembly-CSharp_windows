using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class AppDomainUnloadedException : SystemException
	{
		public AppDomainUnloadedException()
			: base(Locale.GetText("Can't access an unloaded application domain."))
		{
			base.HResult = -2146234348;
		}

		public AppDomainUnloadedException(string message)
			: base(message)
		{
			base.HResult = -2146234348;
		}

		public AppDomainUnloadedException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146234348;
		}

		protected AppDomainUnloadedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private const int Result = -2146234348;
	}
}
