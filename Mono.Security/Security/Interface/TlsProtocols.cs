using System;

namespace Mono.Security.Interface
{
	[Flags]
	public enum TlsProtocols
	{
		Zero = 0,
		Tls10Client = 128,
		Tls10Server = 64,
		Tls10 = 192,
		Tls11Client = 512,
		Tls11Server = 256,
		Tls11 = 768,
		Tls12Client = 2048,
		Tls12Server = 1024,
		Tls12 = 3072,
		ClientMask = 2688,
		ServerMask = 1344
	}
}
