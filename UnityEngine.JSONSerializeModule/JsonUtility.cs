using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Utility functions for working with JSON data.</para>
	/// </summary>
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

		/// <summary>
		///   <para>Generate a JSON representation of the public fields of an object.</para>
		/// </summary>
		/// <param name="obj">The object to convert to JSON form.</param>
		/// <param name="prettyPrint">If true, format the output for readability. If false, format the output for minimum size. Default is false.</param>
		/// <returns>
		///   <para>The object's data in JSON format.</para>
		/// </returns>
		public static string ToJson(object obj)
		{
			return JsonUtility.ToJson(obj, false);
		}

		/// <summary>
		///   <para>Generate a JSON representation of the public fields of an object.</para>
		/// </summary>
		/// <param name="obj">The object to convert to JSON form.</param>
		/// <param name="prettyPrint">If true, format the output for readability. If false, format the output for minimum size. Default is false.</param>
		/// <returns>
		///   <para>The object's data in JSON format.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Create an object from its JSON representation.</para>
		/// </summary>
		/// <param name="json">The JSON representation of the object.</param>
		/// <param name="type">The type of object represented by the Json.</param>
		/// <returns>
		///   <para>An instance of the object.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Overwrite data in an object by reading from its JSON representation.</para>
		/// </summary>
		/// <param name="json">The JSON representation of the object.</param>
		/// <param name="objectToOverwrite">The object that should be overwritten.</param>
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
