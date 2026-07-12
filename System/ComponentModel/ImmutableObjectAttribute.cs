using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ImmutableObjectAttribute : Attribute
	{
		public ImmutableObjectAttribute(bool immutable)
		{
			this.Immutable = immutable;
		}

		public bool Immutable { get; }

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ImmutableObjectAttribute immutableObjectAttribute = obj as ImmutableObjectAttribute;
			bool? flag = ((immutableObjectAttribute != null) ? new bool?(immutableObjectAttribute.Immutable) : null);
			bool immutable = this.Immutable;
			return (flag.GetValueOrDefault() == immutable) & (flag != null);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(ImmutableObjectAttribute.Default);
		}

		public static readonly ImmutableObjectAttribute Yes = new ImmutableObjectAttribute(true);

		public static readonly ImmutableObjectAttribute No = new ImmutableObjectAttribute(false);

		public static readonly ImmutableObjectAttribute Default = ImmutableObjectAttribute.No;
	}
}
