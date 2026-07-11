using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	[Serializable]
	public class CheckoutException : ExternalException
	{
		public CheckoutException()
		{
		}

		public CheckoutException(string message)
			: base(message)
		{
		}

		public CheckoutException(string message, int errorCode)
			: base(message, errorCode)
		{
		}

		protected CheckoutException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public CheckoutException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public static readonly CheckoutException Canceled = new CheckoutException(global::SR.GetString("The checkout was canceled by the user."), -2147467260);
	}
}
