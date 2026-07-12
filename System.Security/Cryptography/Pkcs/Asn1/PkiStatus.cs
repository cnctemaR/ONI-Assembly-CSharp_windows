using System;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal enum PkiStatus
	{
		Granted,
		GrantedWithMods,
		Rejection,
		Waiting,
		RevocationWarning,
		RevocationNotification,
		KeyUpdateWarning
	}
}
