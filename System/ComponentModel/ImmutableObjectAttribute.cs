using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class ImmutableObjectAttribute : Attribute
	{
		public ImmutableObjectAttribute(bool immutable)
		{
			this.immutable = immutable;
		}

		public bool Immutable
		{
			get
			{
				return this.immutable;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ImmutableObjectAttribute && (obj == this || ((ImmutableObjectAttribute)obj).Immutable == this.immutable);
		}

		public override int GetHashCode()
		{
			return this.immutable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.immutable == ImmutableObjectAttribute.Default.Immutable;
		}

		private bool immutable;

		public static readonly ImmutableObjectAttribute Default = new ImmutableObjectAttribute(false);

		public static readonly ImmutableObjectAttribute No = new ImmutableObjectAttribute(false);

		public static readonly ImmutableObjectAttribute Yes = new ImmutableObjectAttribute(true);
	}
}
