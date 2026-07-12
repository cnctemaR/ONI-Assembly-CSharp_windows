using System;

namespace System.Net
{
	[Flags]
	internal enum ContextFlagsPal
	{
		None = 0,
		Delegate = 1,
		MutualAuth = 2,
		ReplayDetect = 4,
		SequenceDetect = 8,
		Confidentiality = 16,
		UseSessionKey = 32,
		AllocateMemory = 256,
		Connection = 2048,
		InitExtendedError = 16384,
		AcceptExtendedError = 32768,
		InitStream = 32768,
		AcceptStream = 65536,
		InitIntegrity = 65536,
		AcceptIntegrity = 131072,
		InitManualCredValidation = 524288,
		InitUseSuppliedCreds = 128,
		InitIdentify = 131072,
		AcceptIdentify = 524288,
		ProxyBindings = 67108864,
		AllowMissingBindings = 268435456,
		UnverifiedTargetName = 536870912
	}
}
