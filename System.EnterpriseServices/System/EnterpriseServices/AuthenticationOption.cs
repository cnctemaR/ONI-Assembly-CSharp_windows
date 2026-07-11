using System;

namespace System.EnterpriseServices
{
	[Serializable]
	public enum AuthenticationOption
	{
		Call = 3,
		Connect = 2,
		Default = 0,
		Integrity = 5,
		None = 1,
		Packet = 4,
		Privacy = 6
	}
}
