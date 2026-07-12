using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[Choice]
	internal struct RecipientInfoAsn
	{
		internal KeyTransRecipientInfoAsn Ktri;

		[ExpectedTag(1)]
		internal KeyAgreeRecipientInfoAsn Kari;
	}
}
