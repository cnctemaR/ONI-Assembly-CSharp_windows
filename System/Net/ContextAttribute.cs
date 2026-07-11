using System;

namespace System.Net
{
	internal enum ContextAttribute
	{
		Sizes,
		Names,
		Lifespan,
		DceInfo,
		StreamSizes,
		Authority = 6,
		PackageInfo = 10,
		NegotiationInfo = 12,
		UniqueBindings = 25,
		EndpointBindings,
		ClientSpecifiedSpn,
		RemoteCertificate = 83,
		LocalCertificate,
		RootStore,
		IssuerListInfoEx = 89,
		ConnectionInfo,
		UiInfo = 104
	}
}
