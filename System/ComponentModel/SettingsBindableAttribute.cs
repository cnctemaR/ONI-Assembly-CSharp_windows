using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class SettingsBindableAttribute : Attribute
	{
		public SettingsBindableAttribute(bool bindable)
		{
			this.Bindable = bindable;
		}

		public bool Bindable { get; }

		public override bool Equals(object obj)
		{
			return obj == this || (obj != null && obj is SettingsBindableAttribute && ((SettingsBindableAttribute)obj).Bindable == this.Bindable);
		}

		public override int GetHashCode()
		{
			return this.Bindable.GetHashCode();
		}

		public static readonly SettingsBindableAttribute Yes = new SettingsBindableAttribute(true);

		public static readonly SettingsBindableAttribute No = new SettingsBindableAttribute(false);
	}
}
