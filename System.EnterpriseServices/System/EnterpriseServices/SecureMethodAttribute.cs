using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	[ComVisible(false)]
	public sealed class SecureMethodAttribute : Attribute
	{
	}
}
