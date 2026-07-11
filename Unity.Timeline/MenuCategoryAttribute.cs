using System;

namespace UnityEngine.Timeline
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class MenuCategoryAttribute : Attribute
	{
		public MenuCategoryAttribute(string category)
		{
			this.category = category ?? string.Empty;
		}

		public readonly string category;
	}
}
