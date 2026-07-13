using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class HeaderAttribute : PropertyAttribute
	{
		public HeaderAttribute(string header)
			: base(true)
		{
			this.header = header;
		}

		public readonly string header;
	}
}
