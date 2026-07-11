using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Resources
{
	[ComVisible(true)]
	[Serializable]
	public class MissingManifestResourceException : SystemException
	{
		public MissingManifestResourceException()
			: base(Environment.GetResourceString("Unable to find manifest resource."))
		{
			base.SetErrorCode(-2146233038);
		}

		public MissingManifestResourceException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233038);
		}

		public MissingManifestResourceException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2146233038);
		}

		protected MissingManifestResourceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
