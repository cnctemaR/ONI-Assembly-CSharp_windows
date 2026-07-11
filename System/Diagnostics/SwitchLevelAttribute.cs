using System;

namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SwitchLevelAttribute : Attribute
	{
		public SwitchLevelAttribute(Type switchLevelType)
		{
			this.SwitchLevelType = switchLevelType;
		}

		public Type SwitchLevelType
		{
			get
			{
				return this.type;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.type = value;
			}
		}

		private Type type;
	}
}
