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
		private static string ToJsonInternal([NotNull] object obj, bool prettyPrint)
		{
			if (obj == null)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				JsonUtility.ToJsonInternal_Injected(obj, prettyPrint, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction("FromJsonInternal", true, ThrowsException = true)]
		[ThreadSafe]
		private unsafe static object FromJsonInternal(string json, object objectToOverwrite, Type type)
		{
			object obj;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(json, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = json.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				obj = JsonUtility.FromJsonInternal_Injected(ref managedSpanWrapper, objectToOverwrite, type);
			}
			finally
			{
				char* ptr = null;
			}
			return obj;
		}

		public static string ToJson(object obj)
		{
			return JsonUtility.ToJson(obj, false);
		}

		public static string ToJson(object obj, bool prettyPrint)
		{
			bool flag = obj == null;
			string text;
			if (flag)
			{
				text = "";
			}
			else
			{
				bool flag2 = obj is Object && !(obj is MonoBehaviour) && !(obj is ScriptableObject);
				if (flag2)
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
			bool flag = string.IsNullOrEmpty(json);
			object obj;
			if (flag)
			{
				obj = null;
			}
			else
			{
				bool flag2 = type == null;
				if (flag2)
				{
					throw new ArgumentNullException("type");
				}
				bool flag3 = type.IsAbstract || type.IsSubclassOf(typeof(Object));
				if (flag3)
				{
					throw new ArgumentException("Cannot deserialize JSON to new instances of type '" + type.Name + ".'");
				}
				obj = JsonUtility.FromJsonInternal(json, null, type);
			}
			return obj;
		}

		public static void FromJsonOverwrite(string json, object objectToOverwrite)
		{
			bool flag = string.IsNullOrEmpty(json);
			if (!flag)
			{
				bool flag2 = objectToOverwrite == null;
				if (flag2)
				{
					throw new ArgumentNullException("objectToOverwrite");
				}
				bool flag3 = objectToOverwrite is Object && !(objectToOverwrite is MonoBehaviour) && !(objectToOverwrite is ScriptableObject);
				if (flag3)
				{
					throw new ArgumentException("Engine types cannot be overwritten from JSON outside of the Editor.");
				}
				JsonUtility.FromJsonInternal(json, objectToOverwrite, objectToOverwrite.GetType());
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ToJsonInternal_Injected(object obj, bool prettyPrint, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object FromJsonInternal_Injected(ref ManagedSpanWrapper json, object objectToOverwrite, Type type);
	}
}
