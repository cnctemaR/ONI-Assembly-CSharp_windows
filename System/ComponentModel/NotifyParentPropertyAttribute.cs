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
			return obj == this || (obj != null && obj is NotifyParentPropertyAttribute && ((NotifyParentPropertyAttribute)obj).NotifyParent == this.notifyParent);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(NotifyParentPropertyAttribute.Default);
		}

		public static readonly NotifyParentPropertyAttribute Yes = new NotifyParentPropertyAttribute(true);

		public static readonly NotifyParentPropertyAttribute No = new NotifyParentPropertyAttribute(false);

		public static readonly NotifyParentPropertyAttribute Default = NotifyParentPropertyAttribute.No;

		private bool notifyParent;
	}
}
