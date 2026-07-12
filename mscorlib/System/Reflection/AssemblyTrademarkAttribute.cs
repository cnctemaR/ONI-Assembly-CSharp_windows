using System;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyTrademarkAttribute : Attribute
	{
		public AssemblyTrademarkAttribute(string trademark)
		{
			this.Trademark = trademark;
		}

		public string Trademark { get; }
	}
}
