using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/JSONSerialize/Public/JsonUtility.bindings.h")]
	public static class JsonUtility
	{
		[FreeFunction("ToJsonInternal", true)]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string ToJsonInternal([NotNull] object obj, bool prettyPrint);

		[FreeFunction("FromJsonInternal", true, ThrowsException = true)]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object FromJsonInternal(string json, object objectToOverwrite, Type type);

		public static string ToJson(object obj)
		{
			return JsonUtility.ToJson(obj, false);
		}

		public static string ToJson(object obj, bool prettyPrint)
		{
			string text;
			if (obj == null)
			{
				text = "";
			}
			else
			{
				if (obj is Object && !(obj is MonoBehaviour) && !(obj is ScriptableObject))
				{
					throw new ArgumentException("JsonUtility.ToJson does not support engine types.");
				}
				text = JsonUtility.ToJsonInternal(obj, prettyPrint);
			}
			return text;
		}

		public static T FromJson<T>(string json)
		{
			return (T)((object)JsonUtility.FromJson(json, typeof(T)));
		}

		public static object FromJson(string json, Type type)
		{
			object obj;
			if (string.IsNullOrEmpty(json))
			{
				obj = null;
			}
			else
			{
				if (type == null)
				{
					throw new ArgumentNullException("type");
				}
				if (type.IsAbstract || type.IsSubclassOf(typeof(Object)))
				{
					throw new ArgumentException("Cannot deserialize JSON to new instances of type '" + type.Name + ".'");
				}
				obj = JsonUtility.FromJsonInternal(json, null, type);
			}
			return obj;
		}

		public static void FromJsonOverwrite(string json, object objectToOverwrite)
		{
			if (!string.IsNullOrEmpty(json))
			{
				if (objectToOverwrite == null)
				{
					throw new ArgumentNullException("objectToOverwrite");
				}
				if (objectToOverwrite is Object && !(objectToOverwrite is MonoBehaviour) && !(objectToOverwrite is ScriptableObject))
				{
					throw new ArgumentException("Engine types cannot be overwritten from JSON outside of the Editor.");
				}
				JsonUtility.FromJsonInternal(json, objectToOverwrite, objectToOverwrite.GetType());
			}
		}
	}
}
