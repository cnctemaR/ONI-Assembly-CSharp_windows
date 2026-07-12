using System;

namespace UnityEngine.SearchService
{
	[Obsolete("ObjectSelectorHandlerWithLabelsAttribute has been deprecated. Use SearchContextAttribute instead.", true)]
	[AttributeUsage(AttributeTargets.Field)]
	public class ObjectSelectorHandlerWithLabelsAttribute : Attribute
	{
		public string[] labels { get; }

		public bool matchAll { get; }

		public ObjectSelectorHandlerWithLabelsAttribute(params string[] labels)
		{
			this.labels = labels;
			this.matchAll = true;
		}

		public ObjectSelectorHandlerWithLabelsAttribute(bool matchAll, params string[] labels)
		{
			this.labels = labels;
			this.matchAll = matchAll;
		}
	}
}
