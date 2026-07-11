using System;
using System.Collections.ObjectModel;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class ManifestSignatureInformationCollection : ReadOnlyCollection<ManifestSignatureInformation>
	{
		internal ManifestSignatureInformationCollection()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
