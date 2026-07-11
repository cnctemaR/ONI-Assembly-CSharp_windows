using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class SettingsBindableAttribute : Attribute
	{
		public SettingsBindableAttribute(bool bindable)
		{
			this.bindable = bindable;
		}

		public bool Bindable
		{
			get
			{
				return this.bindable;
			}
		}

		public override int GetHashCode()
		{
			return (!this.bindable) ? (-1) : 1;
		}

		public override bool Equals(object obj)
		{
			SettingsBindableAttribute settingsBindableAttribute = obj as SettingsBindableAttribute;
			return settingsBindableAttribute != null && this.bindable == settingsBindableAttribute.bindable;
		}

		public static readonly SettingsBindableAttribute Yes = new SettingsBindableAttribute(true);

		public static readonly SettingsBindableAttribute No = new SettingsBindableAttribute(false);

		private bool bindable;
	}
}
