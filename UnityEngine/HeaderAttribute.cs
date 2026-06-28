using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class HeaderAttribute : PropertyAttribute
	{
		public HeaderAttribute(string header)
		{
			this.header = header;
		}

		public readonly string header;
	}
}
