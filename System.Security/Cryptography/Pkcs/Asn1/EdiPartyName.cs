using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct EdiPartyName
	{
		[OptionalValue]
		internal DirectoryString? NameAssigner;

		internal DirectoryString PartyName;
	}
}
