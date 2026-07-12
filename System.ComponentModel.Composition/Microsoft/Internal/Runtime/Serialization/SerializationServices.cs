using System;
using System.Runtime.Serialization;

namespace Microsoft.Internal.Runtime.Serialization
{
	internal static class SerializationServices
	{
		public static T GetValue<T>(this SerializationInfo info, string name)
		{
			Assumes.NotNull<SerializationInfo, string>(info, name);
			return (T)((object)info.GetValue(name, typeof(T)));
		}
	}
}
