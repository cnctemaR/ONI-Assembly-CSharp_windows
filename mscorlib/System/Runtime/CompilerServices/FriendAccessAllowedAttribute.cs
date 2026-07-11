using System;

namespace System.Runtime.CompilerServices
{
	[FriendAccessAllowed]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	internal sealed class FriendAccessAllowedAttribute : Attribute
	{
	}
}
