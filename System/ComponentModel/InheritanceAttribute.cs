using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event)]
	public sealed class InheritanceAttribute : Attribute
	{
		public InheritanceAttribute()
		{
			this.level = InheritanceLevel.NotInherited;
		}

		public InheritanceAttribute(InheritanceLevel inheritanceLevel)
		{
			this.level = inheritanceLevel;
		}

		public InheritanceLevel InheritanceLevel
		{
			get
			{
				return this.level;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is InheritanceAttribute && (obj == this || ((InheritanceAttribute)obj).InheritanceLevel == this.level);
		}

		public override int GetHashCode()
		{
			return this.level.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.level == InheritanceAttribute.Default.InheritanceLevel;
		}

		public override string ToString()
		{
			return this.level.ToString();
		}

		private InheritanceLevel level;

		public static readonly InheritanceAttribute Default = new InheritanceAttribute();

		public static readonly InheritanceAttribute Inherited = new InheritanceAttribute(InheritanceLevel.Inherited);

		public static readonly InheritanceAttribute InheritedReadOnly = new InheritanceAttribute(InheritanceLevel.InheritedReadOnly);

		public static readonly InheritanceAttribute NotInherited = new InheritanceAttribute(InheritanceLevel.NotInherited);
	}
}
