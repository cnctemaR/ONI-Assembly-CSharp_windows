using System;
using System.Runtime.InteropServices;

namespace System.Security
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum HostSecurityManagerOptions
	{
		None = 0,
		HostAppDomainEvidence = 1,
		HostPolicyLevel = 2,
		HostAssemblyEvidence = 4,
		HostDetermineApplicationTrust = 8,
		HostResolvePolicy = 16,
		AllFlags = 31
	}
}
