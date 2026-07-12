using System;

namespace System.Runtime.Serialization
{
	public readonly struct SerializationEntry
	{
		internal SerializationEntry(string entryName, object entryValue, Type entryType)
		{
			this._name = entryName;
			this._value = entryValue;
			this._type = entryType;
		}

		public object Value
		{
			get
			{
				return this._value;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public Type ObjectType
		{
			get
			{
				return this._type;
			}
		}

		private readonly string _name;

		private readonly object _value;

		private readonly Type _type;
	}
}
