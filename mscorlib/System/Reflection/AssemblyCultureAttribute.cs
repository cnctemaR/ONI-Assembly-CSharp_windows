using System;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyCultureAttribute : Attribute
	{
		public AssemblyCultureAttribute(string culture)
		{
			this.Culture = culture;
		}

		public string Culture { get; }
	}
}
