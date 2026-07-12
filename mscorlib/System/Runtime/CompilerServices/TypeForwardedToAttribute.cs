using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
	public sealed class TypeForwardedToAttribute : Attribute
	{
		public TypeForwardedToAttribute(Type destination)
		{
			this.Destination = destination;
		}

		public Type Destination { get; }
	}
}
