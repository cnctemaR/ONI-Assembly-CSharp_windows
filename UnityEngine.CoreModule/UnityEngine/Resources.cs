using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngineInternal;

namespace UnityEngine
{
	/// <summary>
	///   <para>The Resources class allows you to find and access Objects including assets.</para>
	/// </summary>
	[NativeHeader("Runtime/Export/Resources.bindings.h")]
	[NativeHeader("Runtime/Misc/ResourceManagerUtility.h")]
	public sealed class Resources
	{
		internal static T[] ConvertObjects<T>(Object[] rawObjects) where T : Object
		{
			T[] array;
			if (rawObjects == null)
			{
				array = null;
			}
			else
			{
				T[] array2 = new T[rawObjects.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = (T)((object)rawObjects[i]);
				}
				array = array2;
			}
			return array;
		}

		/// <summary>
		///   <para>Returns a list of all objects of Type type.</para>
		/// </summary>
		/// <param name="type">Type of the class to match while searching.</param>
		/// <returns>
		///   <para>An array of objects whose class is type or is derived from type.</para>
		/// </returns>
		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		[FreeFunction("Resources_Bindings::FindObjectsOfTypeAll")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindObjectsOfTypeAll(Type type);

		public static T[] FindObjectsOfTypeAll<T>() where T : Object
		{
			return Resources.ConvertObjects<T>(Resources.FindObjectsOfTypeAll(typeof(T)));
		}

		/// <summary>
		///   <para>Loads an asset stored at path in a folder called Resources.</para>
		/// </summary>
		/// <param name="path">Pathname of the target folder.</param>
		/// <returns>
		///   <para>The requested asset returned as a Type.</para>
		/// </returns>
		public static Object Load(string path)
		{
			return Resources.Load(path, typeof(Object));
		}

		public static T Load<T>(string path) where T : Object
		{
			return (T)((object)Resources.Load(path, typeof(T)));
		}

		/// <summary>
		///   <para>Loads an asset stored at path in a Resources folder.</para>
		/// </summary>
		/// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
		/// <param name="systemTypeInstance">Type filter for objects returned.</param>
		/// <returns>
		///   <para>The requested asset returned as an Object.</para>
		/// </returns>
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		[NativeThrows]
		[FreeFunction("Resources_Bindings::Load")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object Load(string path, [NotNull] Type systemTypeInstance);

		/// <summary>
		///   <para>Asynchronously loads an asset stored at path in a Resources folder.</para>
		/// </summary>
		/// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
		public static ResourceRequest LoadAsync(string path)
		{
			return Resources.LoadAsync(path, typeof(Object));
		}

		public static ResourceRequest LoadAsync<T>(string path) where T : Object
		{
			return Resources.LoadAsync(path, typeof(T));
		}

		/// <summary>
		///   <para>Asynchronously loads an asset stored at path in a Resources folder.</para>
		/// </summary>
		/// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
		/// <param name="systemTypeInstance">Type filter for objects returned.</param>
		/// <param name="type"></param>
		public static ResourceRequest LoadAsync(string path, Type type)
		{
			ResourceRequest resourceRequest = Resources.LoadAsyncInternal(path, type);
			resourceRequest.m_Path = path;
			resourceRequest.m_Type = type;
			return resourceRequest;
		}

		[FreeFunction("Resources_Bindings::LoadAsyncInternal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern ResourceRequest LoadAsyncInternal(string path, Type type);

		/// <summary>
		///   <para>Loads all assets in a folder or file at path in a Resources folder.</para>
		/// </summary>
		/// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
		/// <param name="systemTypeInstance">Type filter for objects returned.</param>
		[NativeThrows]
		[FreeFunction("Resources_Bindings::LoadAll")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] LoadAll([NotNull] string path, [NotNull] Type systemTypeInstance);

		/// <summary>
		///   <para>Loads all assets in a folder or file at path in a Resources folder.</para>
		/// </summary>
		/// <param name="path">Pathname of the target folder. When using the empty string (i.e., ""), the function will load the entire contents of the Resources folder.</param>
		public static Object[] LoadAll(string path)
		{
			return Resources.LoadAll(path, typeof(Object));
		}

		public static T[] LoadAll<T>(string path) where T : Object
		{
			return Resources.ConvertObjects<T>(Resources.LoadAll(path, typeof(T)));
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		[FreeFunction("GetScriptingBuiltinResource")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object GetBuiltinResource([NotNull] Type type, string path);

		public static T GetBuiltinResource<T>(string path) where T : Object
		{
			return (T)((object)Resources.GetBuiltinResource(typeof(T), path));
		}

		/// <summary>
		///   <para>Unloads assetToUnload from memory.</para>
		/// </summary>
		/// <param name="assetToUnload"></param>
		[FreeFunction("Scripting::UnloadAssetFromScripting")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnloadAsset(Object assetToUnload);

		/// <summary>
		///   <para>Unloads assets that are not used.</para>
		/// </summary>
		/// <returns>
		///   <para>Object on which you can yield to wait until the operation completes.</para>
		/// </returns>
		[FreeFunction("Resources_Bindings::UnloadUnusedAssets")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AsyncOperation UnloadUnusedAssets();
	}
}
