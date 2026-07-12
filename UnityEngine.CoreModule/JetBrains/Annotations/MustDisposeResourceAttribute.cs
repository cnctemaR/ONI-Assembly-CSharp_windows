using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Parameter)]
	public sealed class MustDisposeResourceAttribute : Attribute
	{
		public MustDisposeResourceAttribute()
		{
			this.Value = true;
		}

		public MustDisposeResourceAttribute(bool value)
		{
			this.Value = value;
		}

		public bool Value { get; }
	}
}
