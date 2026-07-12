using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class MergablePropertyAttribute : Attribute
	{
		public MergablePropertyAttribute(bool allowMerge)
		{
			this.AllowMerge = allowMerge;
		}

		public bool AllowMerge { get; }

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			MergablePropertyAttribute mergablePropertyAttribute = obj as MergablePropertyAttribute;
			bool? flag = ((mergablePropertyAttribute != null) ? new bool?(mergablePropertyAttribute.AllowMerge) : null);
			bool allowMerge = this.AllowMerge;
			return (flag.GetValueOrDefault() == allowMerge) & (flag != null);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(MergablePropertyAttribute.Default);
		}

		public static readonly MergablePropertyAttribute Yes = new MergablePropertyAttribute(true);

		public static readonly MergablePropertyAttribute No = new MergablePropertyAttribute(false);

		public static readonly MergablePropertyAttribute Default = MergablePropertyAttribute.Yes;
	}
}
