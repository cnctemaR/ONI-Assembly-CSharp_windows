using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class NotifyParentPropertyAttribute : Attribute
	{
		public NotifyParentPropertyAttribute(bool notifyParent)
		{
			this.notifyParent = notifyParent;
		}

		public bool NotifyParent
		{
			get
			{
				return this.notifyParent;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is NotifyParentPropertyAttribute && (obj == this || ((NotifyParentPropertyAttribute)obj).NotifyParent == this.notifyParent);
		}

		public override int GetHashCode()
		{
			return this.notifyParent.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.notifyParent == NotifyParentPropertyAttribute.Default.NotifyParent;
		}

		private bool notifyParent;

		public static readonly NotifyParentPropertyAttribute Default = new NotifyParentPropertyAttribute(false);

		public static readonly NotifyParentPropertyAttribute No = new NotifyParentPropertyAttribute(false);

		public static readonly NotifyParentPropertyAttribute Yes = new NotifyParentPropertyAttribute(true);
	}
}
