using System;

namespace System.Runtime.Serialization
{
	internal struct EnumMemberInfo
	{
		public EnumMemberInfo(string name, object value)
		{
			this.XmlName = name;
			this.Value = value;
		}

		public readonly string XmlName;

		public readonly object Value;
	}
}
