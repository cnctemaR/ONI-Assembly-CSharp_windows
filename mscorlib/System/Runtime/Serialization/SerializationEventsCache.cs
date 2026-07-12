using System;
using System.Collections.Concurrent;

namespace System.Runtime.Serialization
{
	internal static class SerializationEventsCache
	{
		internal static SerializationEvents GetSerializationEventsForType(Type t)
		{
			return SerializationEventsCache.s_cache.GetOrAdd(t, (Type type) => new SerializationEvents(type));
		}

		private static readonly ConcurrentDictionary<Type, SerializationEvents> s_cache = new ConcurrentDictionary<Type, SerializationEvents>();
	}
}
