using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngineInternal;

namespace UnityEngine
{
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadFromMemoryAsyncOperation.h")]
	[ExcludeFromPreset]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadAssetOperation.h")]
	[NativeHeader("Runtime/Scripting/ScriptingExportUtility.h")]
	[NativeHeader("Runtime/Scripting/ScriptingObjectWithIntPtrField.h")]
	[NativeHeader("Runtime/Scripting/ScriptingUtility.h")]
	[NativeHeader("AssetBundleScriptingClasses.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleSaveAndLoadHelper.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleUtility.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadAssetUtility.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadFromFileAsyncOperation.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadFromManagedStreamAsyncOperation.h")]
	public class AssetBundle : Object
	{
		private AssetBundle()
		{
		}

		[Obsolete("mainAsset has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public Object mainAsset
		{
			get
			{
				return AssetBundle.returnMainAsset(this);
			}
		}

		[FreeFunction("LoadMainObjectFromAssetBundle", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Object returnMainAsset([NotNull("NullExceptionObject")] AssetBundle bundle);

		[FreeFunction("UnloadAllAssetBundles")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnloadAllAssetBundles(bool unloadAllObjects);

		[FreeFunction("GetAllAssetBundles")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AssetBundle[] GetAllLoadedAssetBundles_Native();

		public static IEnumerable<AssetBundle> GetAllLoadedAssetBundles()
		{
			return AssetBundle.GetAllLoadedAssetBundles_Native();
		}

		[FreeFunction("LoadFromFileAsync")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AssetBundleCreateRequest LoadFromFileAsync_Internal(string path, uint crc, ulong offset);

		public static AssetBundleCreateRequest LoadFromFileAsync(string path)
		{
			return AssetBundle.LoadFromFileAsync_Internal(path, 0U, 0UL);
		}

		public static AssetBundleCreateRequest LoadFromFileAsync(string path, uint crc)
		{
			return AssetBundle.LoadFromFileAsync_Internal(path, crc, 0UL);
		}

		public static AssetBundleCreateRequest LoadFromFileAsync(string path, uint crc, ulong offset)
		{
			return AssetBundle.LoadFromFileAsync_Internal(path, crc, offset);
		}

		[FreeFunction("LoadFromFile")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AssetBundle LoadFromFile_Internal(string path, uint crc, ulong offset);

		public static AssetBundle LoadFromFile(string path)
		{
			return AssetBundle.LoadFromFile_Internal(path, 0U, 0UL);
		}

		public static AssetBundle LoadFromFile(string path, uint crc)
		{
			return AssetBundle.LoadFromFile_Internal(path, crc, 0UL);
		}

		public static AssetBundle LoadFromFile(string path, uint crc, ulong offset)
		{
			return AssetBundle.LoadFromFile_Internal(path, crc, offset);
		}

		[FreeFunction("LoadFromMemoryAsync")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AssetBundleCreateRequest LoadFromMemoryAsync_Internal(byte[] binary, uint crc);

		public static AssetBundleCreateRequest LoadFromMemoryAsync(byte[] binary)
		{
			return AssetBundle.LoadFromMemoryAsync_Internal(binary, 0U);
		}

		public static AssetBundleCreateRequest LoadFromMemoryAsync(byte[] binary, uint crc)
		{
			return AssetBundle.LoadFromMemoryAsync_Internal(binary, crc);
		}

		[FreeFunction("LoadFromMemory")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AssetBundle LoadFromMemory_Internal(byte[] binary, uint crc);

		public static AssetBundle LoadFromMemory(byte[] binary)
		{
			return AssetBundle.LoadFromMemory_Internal(binary, 0U);
		}

		public static AssetBundle LoadFromMemory(byte[] binary, uint crc)
		{
			return AssetBundle.LoadFromMemory_Internal(binary, crc);
		}

		internal static void ValidateLoadFromStream(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("ManagedStream object must be non-null", "stream");
			}
			bool flag2 = !stream.CanRead;
			if (flag2)
			{
				throw new ArgumentException("ManagedStream object must be readable (stream.CanRead must return true)", "stream");
			}
			bool flag3 = !stream.CanSeek;
			if (flag3)
			{
				throw new ArgumentException("ManagedStream object must be seekable (stream.CanSeek must return true)", "stream");
			}
		}

		public static AssetBundleCreateRequest LoadFromStreamAsync(Stream stream, uint crc, uint managedReadBufferSize)
		{
			AssetBundle.ValidateLoadFromStream(stream);
			return AssetBundle.LoadFromStreamAsyncInternal(stream, crc, managedReadBufferSize);
		}

		public static AssetBundleCreateRequest LoadFromStreamAsync(Stream stream, uint crc)
		{
			AssetBundle.ValidateLoadFromStream(stream);
			return AssetBundle.LoadFromStreamAsyncInternal(stream, crc, 0U);
		}

		public static AssetBundleCreateRequest LoadFromStreamAsync(Stream stream)
		{
			AssetBundle.ValidateLoadFromStream(stream);
			return AssetBundle.LoadFromStreamAsyncInternal(stream, 0U, 0U);
		}

		public static AssetBundle LoadFromStream(Stream stream, uint crc, uint managedReadBufferSize)
		{
			AssetBundle.ValidateLoadFromStream(stream);
			return AssetBundle.LoadFromStreamInternal(stream, crc, managedReadBufferSize);
		}

		public static AssetBundle LoadFromStream(Stream stream, uint crc)
		{
			AssetBundle.ValidateLoadFromStream(stream);
			return AssetBundle.LoadFromStreamInternal(stream, crc, 0U);
		}

		public static AssetBundle LoadFromStream(Stream stream)
		{
			AssetBundle.ValidateLoadFromStream(stream);
			return AssetBundle.LoadFromStreamInternal(stream, 0U, 0U);
		}

		[FreeFunction("LoadFromStreamAsyncInternal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AssetBundleCreateRequest LoadFromStreamAsyncInternal(Stream stream, uint crc, uint managedReadBufferSize);

		[FreeFunction("LoadFromStreamInternal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AssetBundle LoadFromStreamInternal(Stream stream, uint crc, uint managedReadBufferSize);

		public extern bool isStreamedSceneAssetBundle
		{
			[NativeMethod("GetIsStreamedSceneAssetBundle")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeMethod("Contains")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Contains(string name);

		[Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Object Load(string name)
		{
			return null;
		}

		[Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Object Load<T>(string name)
		{
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
		private Object Load(string name, Type type)
		{
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Method LoadAsync has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAssetAsync instead and check the documentation for details.", true)]
		private AssetBundleRequest LoadAsync(string name, Type type)
		{
			return null;
		}

		[Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		private Object[] LoadAll(Type type)
		{
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
		public Object[] LoadAll()
		{
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
		public T[] LoadAll<T>() where T : Object
		{
			return null;
		}

		public Object LoadAsset(string name)
		{
			return this.LoadAsset(name, typeof(Object));
		}

		public T LoadAsset<T>(string name) where T : Object
		{
			return (T)((object)this.LoadAsset(name, typeof(T)));
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		public Object LoadAsset(string name, Type type)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new NullReferenceException("The input asset name cannot be null.");
			}
			bool flag2 = name.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("The input asset name cannot be empty.");
			}
			bool flag3 = type == null;
			if (flag3)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAsset_Internal(name, type);
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		[NativeMethod("LoadAsset_Internal")]
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Object LoadAsset_Internal(string name, Type type);

		public AssetBundleRequest LoadAssetAsync(string name)
		{
			return this.LoadAssetAsync(name, typeof(Object));
		}

		public AssetBundleRequest LoadAssetAsync<T>(string name)
		{
			return this.LoadAssetAsync(name, typeof(T));
		}

		public AssetBundleRequest LoadAssetAsync(string name, Type type)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new NullReferenceException("The input asset name cannot be null.");
			}
			bool flag2 = name.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("The input asset name cannot be empty.");
			}
			bool flag3 = type == null;
			if (flag3)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAssetAsync_Internal(name, type);
		}

		public Object[] LoadAssetWithSubAssets(string name)
		{
			return this.LoadAssetWithSubAssets(name, typeof(Object));
		}

		internal static T[] ConvertObjects<T>(Object[] rawObjects) where T : Object
		{
			bool flag = rawObjects == null;
			T[] array;
			if (flag)
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

		public T[] LoadAssetWithSubAssets<T>(string name) where T : Object
		{
			return AssetBundle.ConvertObjects<T>(this.LoadAssetWithSubAssets(name, typeof(T)));
		}

		public Object[] LoadAssetWithSubAssets(string name, Type type)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new NullReferenceException("The input asset name cannot be null.");
			}
			bool flag2 = name.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("The input asset name cannot be empty.");
			}
			bool flag3 = type == null;
			if (flag3)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAssetWithSubAssets_Internal(name, type);
		}

		public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name)
		{
			return this.LoadAssetWithSubAssetsAsync(name, typeof(Object));
		}

		public AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name)
		{
			return this.LoadAssetWithSubAssetsAsync(name, typeof(T));
		}

		public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name, Type type)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new NullReferenceException("The input asset name cannot be null.");
			}
			bool flag2 = name.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("The input asset name cannot be empty.");
			}
			bool flag3 = type == null;
			if (flag3)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAssetWithSubAssetsAsync_Internal(name, type);
		}

		public Object[] LoadAllAssets()
		{
			return this.LoadAllAssets(typeof(Object));
		}

		public T[] LoadAllAssets<T>() where T : Object
		{
			return AssetBundle.ConvertObjects<T>(this.LoadAllAssets(typeof(T)));
		}

		public Object[] LoadAllAssets(Type type)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAssetWithSubAssets_Internal("", type);
		}

		public AssetBundleRequest LoadAllAssetsAsync()
		{
			return this.LoadAllAssetsAsync(typeof(Object));
		}

		public AssetBundleRequest LoadAllAssetsAsync<T>()
		{
			return this.LoadAllAssetsAsync(typeof(T));
		}

		public AssetBundleRequest LoadAllAssetsAsync(Type type)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new NullReferenceException("The input type cannot be null.");
			}
			return this.LoadAssetWithSubAssetsAsync_Internal("", type);
		}

		[Obsolete("This method is deprecated.Use GetAllAssetNames() instead.", false)]
		public string[] AllAssetNames()
		{
			return this.GetAllAssetNames();
		}

		[NativeMethod("LoadAssetAsync_Internal")]
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AssetBundleRequest LoadAssetAsync_Internal(string name, Type type);

		[NativeMethod("Unload")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Unload(bool unloadAllLoadedObjects);

		[NativeMethod("GetAllAssetNames")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllAssetNames();

		[NativeMethod("GetAllScenePaths")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetAllScenePaths();

		[NativeThrows]
		[NativeMethod("LoadAssetWithSubAssets_Internal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Object[] LoadAssetWithSubAssets_Internal(string name, Type type);

		[NativeThrows]
		[NativeMethod("LoadAssetWithSubAssetsAsync_Internal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AssetBundleRequest LoadAssetWithSubAssetsAsync_Internal(string name, Type type);

		public static AssetBundleRecompressOperation RecompressAssetBundleAsync(string inputPath, string outputPath, BuildCompression method, uint expectedCRC = 0U, ThreadPriority priority = ThreadPriority.Low)
		{
			return AssetBundle.RecompressAssetBundleAsync_Internal(inputPath, outputPath, method, expectedCRC, priority);
		}

		[NativeThrows]
		[FreeFunction("RecompressAssetBundleAsync_Internal")]
		internal static AssetBundleRecompressOperation RecompressAssetBundleAsync_Internal(string inputPath, string outputPath, BuildCompression method, uint expectedCRC, ThreadPriority priority)
		{
			return AssetBundle.RecompressAssetBundleAsync_Internal_Injected(inputPath, outputPath, ref method, expectedCRC, priority);
		}

		public static uint memoryBudgetKB
		{
			get
			{
				return AssetBundleLoadingCache.memoryBudgetKB;
			}
			set
			{
				AssetBundleLoadingCache.memoryBudgetKB = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AssetBundleRecompressOperation RecompressAssetBundleAsync_Internal_Injected(string inputPath, string outputPath, ref BuildCompression method, uint expectedCRC, ThreadPriority priority);
	}
}
