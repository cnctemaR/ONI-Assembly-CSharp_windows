using System;
using System.Collections.Generic;
using System.Security;

namespace UnityEngine
{
	internal class GUIStateObjects
	{
		[SecuritySafeCritical]
		internal static object GetStateObject(Type t, int controlID)
		{
			object obj;
			if (!GUIStateObjects.s_StateCache.TryGetValue(controlID, out obj) || obj.GetType() != t)
			{
				obj = Activator.CreateInstance(t);
				GUIStateObjects.s_StateCache[controlID] = obj;
			}
			return obj;
		}

		internal static object QueryStateObject(Type t, int controlID)
		{
			object obj = GUIStateObjects.s_StateCache[controlID];
			object obj2;
			if (t.IsInstanceOfType(obj))
			{
				obj2 = obj;
			}
			else
			{
				obj2 = null;
			}
			return obj2;
		}

		internal static void Tests_ClearObjects()
		{
			GUIStateObjects.s_StateCache.Clear();
		}

		private static Dictionary<int, object> s_StateCache = new Dictionary<int, object>();
	}
}
