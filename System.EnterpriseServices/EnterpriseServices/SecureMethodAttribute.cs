using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public sealed class SecureMethodAttribute : Attribute
	{
	}
}
