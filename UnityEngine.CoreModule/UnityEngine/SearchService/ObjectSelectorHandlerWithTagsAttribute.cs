using System;

namespace UnityEngine.SearchService
{
	[AttributeUsage(AttributeTargets.Field)]
	[Obsolete("ObjectSelectorHandlerWithTagsAttribute has been deprecated. Use SearchContextAttribute instead.", true)]
	public class ObjectSelectorHandlerWithTagsAttribute : Attribute
	{
		public string[] tags { get; }

		public ObjectSelectorHandlerWithTagsAttribute(params string[] tags)
		{
			this.tags = tags;
		}
	}
}
