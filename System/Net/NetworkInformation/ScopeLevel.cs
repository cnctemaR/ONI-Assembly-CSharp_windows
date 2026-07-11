using System;

namespace System.Net.NetworkInformation
{
	public enum ScopeLevel
	{
		None,
		Interface,
		Link,
		Subnet,
		Admin,
		Site,
		Organization = 8,
		Global = 14
	}
}
