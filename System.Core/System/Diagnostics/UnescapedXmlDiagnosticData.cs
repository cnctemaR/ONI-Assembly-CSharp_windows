using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class UnescapedXmlDiagnosticData
	{
		public UnescapedXmlDiagnosticData(string xmlPayload)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public string UnescapedXml
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}
	}
}
