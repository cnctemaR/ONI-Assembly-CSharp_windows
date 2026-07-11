using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[Serializable]
	public sealed class ServicedComponentException : SystemException
	{
		public ServicedComponentException()
		{
		}

		public ServicedComponentException(string message)
			: base(message)
		{
		}

		public ServicedComponentException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
