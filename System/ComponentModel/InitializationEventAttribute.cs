using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class InitializationEventAttribute : Attribute
	{
		public InitializationEventAttribute(string eventName)
		{
			this.eventName = eventName;
		}

		public string EventName
		{
			get
			{
				return this.eventName;
			}
		}

		private string eventName;
	}
}
