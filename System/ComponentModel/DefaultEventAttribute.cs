using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultEventAttribute : Attribute
	{
		public DefaultEventAttribute(string name)
		{
			this.eventName = name;
		}

		public string Name
		{
			get
			{
				return this.eventName;
			}
		}

		public override bool Equals(object o)
		{
			return o is DefaultEventAttribute && ((DefaultEventAttribute)o).eventName == this.eventName;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private string eventName;

		public static readonly DefaultEventAttribute Default = new DefaultEventAttribute(null);
	}
}
