using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class DecoratedNameAttribute : Attribute
	{
		public DecoratedNameAttribute(string decoratedName)
		{
		}
	}
}
