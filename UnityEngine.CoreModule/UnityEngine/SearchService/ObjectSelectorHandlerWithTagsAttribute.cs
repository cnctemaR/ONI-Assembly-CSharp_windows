using System;

namespace UnityEngine.SearchService
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ObjectSelectorHandlerWithTagsAttribute : Attribute
	{
		public string[] tags { get; }

		public ObjectSelectorHandlerWithTagsAttribute(params string[] tags)
		{
			this.tags = tags;
		}
	}
}
