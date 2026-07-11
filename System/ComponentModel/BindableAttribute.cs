using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class BindableAttribute : Attribute
	{
		public BindableAttribute(bool bindable)
			: this(bindable, BindingDirection.OneWay)
		{
		}

		public BindableAttribute(bool bindable, BindingDirection direction)
		{
			this.bindable = bindable;
			this.direction = direction;
		}

		public BindableAttribute(BindableSupport flags)
			: this(flags, BindingDirection.OneWay)
		{
		}

		public BindableAttribute(BindableSupport flags, BindingDirection direction)
		{
			this.bindable = flags > BindableSupport.No;
			this.isDefault = flags == BindableSupport.Default;
			this.direction = direction;
		}

		public bool Bindable
		{
			get
			{
				return this.bindable;
			}
		}

		public BindingDirection Direction
		{
			get
			{
				return this.direction;
			}
		}

		public override bool Equals(object obj)
		{
			return obj == this || (obj != null && obj is BindableAttribute && ((BindableAttribute)obj).Bindable == this.bindable);
		}

		public override int GetHashCode()
		{
			return this.bindable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(BindableAttribute.Default) || this.isDefault;
		}

		public static readonly BindableAttribute Yes = new BindableAttribute(true);

		public static readonly BindableAttribute No = new BindableAttribute(false);

		public static readonly BindableAttribute Default = BindableAttribute.No;

		private bool bindable;

		private bool isDefault;

		private BindingDirection direction;
	}
}
