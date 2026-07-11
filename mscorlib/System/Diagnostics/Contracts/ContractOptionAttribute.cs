using System;

namespace System.Diagnostics.Contracts
{
	[Conditional("CONTRACTS_FULL")]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	public sealed class ContractOptionAttribute : Attribute
	{
		public ContractOptionAttribute(string category, string setting, bool enabled)
		{
			this._category = category;
			this._setting = setting;
			this._enabled = enabled;
		}

		public ContractOptionAttribute(string category, string setting, string value)
		{
			this._category = category;
			this._setting = setting;
			this._value = value;
		}

		public string Category
		{
			get
			{
				return this._category;
			}
		}

		public string Setting
		{
			get
			{
				return this._setting;
			}
		}

		public bool Enabled
		{
			get
			{
				return this._enabled;
			}
		}

		public string Value
		{
			get
			{
				return this._value;
			}
		}

		private string _category;

		private string _setting;

		private bool _enabled;

		private string _value;
	}
}
