using System;

namespace EventSystem2Syntax
{
	public static class EventSystem2Extensions
	{
		public static void Subscribe(this AttributeExample example, GameHashes event_id)
		{
		}

		public static void Subscribe(this InterfaceExample example, GameHashes event_id)
		{
		}

		public static void Subscribe(this StaticExample example, GameHashes event_id, Action<StaticExample, KPrefabID> callback)
		{
		}
	}
}
