using System;

namespace Internal.Cryptography.Pal
{
	internal enum GeneralNameType
	{
		OtherName,
		Rfc822Name,
		Email = 1,
		DnsName,
		X400Address,
		DirectoryName,
		EdiPartyName,
		UniformResourceIdentifier,
		IPAddress,
		RegisteredId
	}
}
