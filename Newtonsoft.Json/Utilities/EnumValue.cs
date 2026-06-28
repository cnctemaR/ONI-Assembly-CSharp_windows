using System;

namespace Newtonsoft.Json.Utilities
{
	internal class EnumValue<T> where T : struct
	{
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public T Value
		{
			get
			{
				return this._value;
			}
		}

		public EnumValue(string name, T value)
		{
			this._name = name;
			this._value = value;
		}

		private readonly string _name;

		private readonly T _value;
	}
}
