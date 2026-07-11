using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class MergablePropertyAttribute : Attribute
	{
		public MergablePropertyAttribute(bool allowMerge)
		{
			this.mergable = allowMerge;
		}

		public bool AllowMerge
		{
			get
			{
				return this.mergable;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is MergablePropertyAttribute && (obj == this || ((MergablePropertyAttribute)obj).AllowMerge == this.mergable);
		}

		public override int GetHashCode()
		{
			return this.mergable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.mergable == MergablePropertyAttribute.Default.AllowMerge;
		}

		private bool mergable;

		public static readonly MergablePropertyAttribute Default = new MergablePropertyAttribute(true);

		public static readonly MergablePropertyAttribute No = new MergablePropertyAttribute(false);

		public static readonly MergablePropertyAttribute Yes = new MergablePropertyAttribute(true);
	}
}
