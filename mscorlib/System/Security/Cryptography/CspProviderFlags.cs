using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum CspProviderFlags
	{
		UseMachineKeyStore = 1,
		UseDefaultKeyContainer = 2,
		UseExistingKey = 8,
		NoFlags = 0,
		NoPrompt = 64,
		UseArchivableKey = 16,
		UseNonExportableKey = 4,
		UseUserProtectedKey = 32
	}
}
