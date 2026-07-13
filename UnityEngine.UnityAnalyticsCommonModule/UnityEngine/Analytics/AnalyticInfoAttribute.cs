using System;
using UnityEngine.Internal;

namespace UnityEngine.Analytics
{
	[ExcludeFromDocs]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class AnalyticInfoAttribute : Attribute
	{
		public int version { get; }

		public string vendorKey { get; }

		public string eventName { get; }

		internal int maxEventsPerHour { get; }

		internal int maxNumberOfElements { get; }

		public AnalyticInfoAttribute(string eventName, string vendorKey = "", int version = 1, int maxEventsPerHour = 1000, int maxNumberOfElements = 1000)
		{
			this.version = version;
			this.vendorKey = vendorKey;
			this.eventName = eventName;
			this.maxEventsPerHour = maxEventsPerHour;
			this.maxNumberOfElements = maxNumberOfElements;
		}
	}
}
