using System;

namespace System.Diagnostics
{
	[global::System.MonoLimitation("This attribute is not considered in trace support.")]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SwitchLevelAttribute : Attribute
	{
		public SwitchLevelAttribute(Type switchLevelType)
		{
			if (switchLevelType == null)
			{
				throw new ArgumentNullException("switchLevelType");
			}
			this.type = switchLevelType;
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
