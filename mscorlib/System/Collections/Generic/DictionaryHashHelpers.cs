using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
	internal class DictionaryHashHelpers
	{
		internal static ConditionalWeakTable<object, SerializationInfo> SerializationInfoTable { get; } = new ConditionalWeakTable<object, SerializationInfo>();
	}
}
