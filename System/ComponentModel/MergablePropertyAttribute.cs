using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class MergablePropertyAttribute : Attribute
	{
		public MergablePropertyAttribute(bool allowMerge)
		{
			this.allowMerge = allowMerge;
		}

		public bool AllowMerge
		{
			get
			{
				return this.allowMerge;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			MergablePropertyAttribute mergablePropertyAttribute = obj as MergablePropertyAttribute;
			return mergablePropertyAttribute != null && mergablePropertyAttribute.AllowMerge == this.allowMerge;
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

		private bool allowMerge;
	}
}
