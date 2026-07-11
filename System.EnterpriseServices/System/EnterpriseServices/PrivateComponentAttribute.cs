using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class PrivateComponentAttribute : Attribute
	{
	}
}
