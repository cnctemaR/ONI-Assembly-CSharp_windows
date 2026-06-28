using System;

namespace KSerialization
{
	public sealed class SerializationConfig : Attribute
	{
		public SerializationConfig(MemberSerialization memberSerialization)
		{
			this.MemberSerialization = memberSerialization;
		}

		public MemberSerialization MemberSerialization { get; set; }
	}
}
