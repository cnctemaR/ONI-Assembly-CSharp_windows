using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class BindableAttribute : Attribute
	{
		public BindableAttribute(BindableSupport flags)
		{
			if (flags == BindableSupport.No)
			{
				this.bindable = false;
			}
			if (flags == BindableSupport.Yes || flags == BindableSupport.Default)
			{
				this.bindable = true;
			}
		}

		public BindableAttribute(bool bindable)
		{
			this.bindable = bindable;
		}

		public BindableAttribute(bool bindable, BindingDirection direction)
		{
			this.bindable = bindable;
			this.direction = direction;
		}

		public BindableAttribute(BindableSupport flags, BindingDirection direction)
			: this(flags)
		{
			this.direction = direction;
		}

		public BindingDirection Direction
		{
			get
			{
				return this.direction;
			}
		}

		public bool Bindable
		{
			get
			{
				return this.bindable;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is BindableAttribute && (obj == this || ((BindableAttribute)obj).Bindable == this.bindable);
		}

		public override int GetHashCode()
		{
			return this.bindable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.bindable == BindableAttribute.Default.Bindable;
		}

		private bool bindable;

		private BindingDirection direction;

		public static readonly BindableAttribute No = new BindableAttribute(BindableSupport.No);

		public static readonly BindableAttribute Yes = new BindableAttribute(BindableSupport.Yes);

		public static readonly BindableAttribute Default = new BindableAttribute(BindableSupport.Default);
	}
}
