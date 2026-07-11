using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class SettingsBindableAttribute : Attribute
	{
		public SettingsBindableAttribute(bool bindable)
		{
			this._bindable = bindable;
		}

		public bool Bindable
		{
			get
			{
				return this._bindable;
			}
		}

		public override bool Equals(object obj)
		{
			return obj == this || (obj != null && obj is SettingsBindableAttribute && ((SettingsBindableAttribute)obj).Bindable == this._bindable);
		}

		public override int GetHashCode()
		{
			return this._bindable.GetHashCode();
		}

		public static readonly SettingsBindableAttribute Yes = new SettingsBindableAttribute(true);

		public static readonly SettingsBindableAttribute No = new SettingsBindableAttribute(false);

		private bool _bindable;
	}
}
