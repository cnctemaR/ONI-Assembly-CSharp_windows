using System;

namespace UnityEngine.Timeline
{
	internal static class SelectionExtensions
	{
		public static ObjectId GetObjectId(this Object obj)
		{
			return obj.GetEntityId();
		}
	}
}
