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
			this.Bindable = bindable;
			this.Direction = direction;
		}

		public BindableAttribute(BindableSupport flags)
			: this(flags, BindingDirection.OneWay)
		{
		}

		public BindableAttribute(BindableSupport flags, BindingDirection direction)
		{
			this.Bindable = flags > BindableSupport.No;
			this._isDefault = flags == BindableSupport.Default;
			this.Direction = direction;
		}

		public bool Bindable { get; }

		public BindingDirection Direction { get; }

		public override bool Equals(object obj)
		{
			return obj == this || (obj != null && obj is BindableAttribute && ((BindableAttribute)obj).Bindable == this.Bindable);
		}

		public override int GetHashCode()
		{
			return this.Bindable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(BindableAttribute.Default) || this._isDefault;
		}

		public static readonly BindableAttribute Yes = new BindableAttribute(true);

		public static readonly BindableAttribute No = new BindableAttribute(false);

		public static readonly BindableAttribute Default = BindableAttribute.No;

		private bool _isDefault;
	}
}
