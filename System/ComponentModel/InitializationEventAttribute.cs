using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class InitializationEventAttribute : Attribute
	{
		public InitializationEventAttribute(string eventName)
		{
			this.EventName = eventName;
		}

		public string EventName { get; }
	}
}
