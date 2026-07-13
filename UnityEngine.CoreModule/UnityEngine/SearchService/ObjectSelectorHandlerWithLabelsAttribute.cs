using System;

namespace UnityEngine.SearchService
{
	[AttributeUsage(AttributeTargets.Field)]
	[Obsolete("ObjectSelectorHandlerWithLabelsAttribute has been deprecated. Use SearchContextAttribute instead.", true)]
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
