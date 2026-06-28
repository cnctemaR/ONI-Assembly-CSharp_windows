using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public struct SerializationEntry
	{
		internal SerializationEntry(string name, Type type, object value)
		{
			this.name = name;
			this.objectType = type;
			this.value = value;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public Type ObjectType
		{
			get
			{
				return this.objectType;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		private string name;

		private Type objectType;

		private object value;
	}
}
