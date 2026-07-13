using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngineInternal;

namespace UnityEngine
{
	[NativeHeader("AssetBundleScriptingClasses.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleUtility.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleSaveAndLoadHelper.h")]
	[NativeHeader("Runtime/Scripting/ScriptingUtility.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadAssetUtility.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadAssetOperation.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadFromManagedStreamAsyncOperation.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadFromMemoryAsyncOperation.h")]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadFromFileAsyncOperation.h")]
	[NativeHeader("Runtime/Scripting/ScriptingExportUtility.h")]
	[ExcludeFromPreset]
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
		internal static Object returnMainAsset([NotNull] AssetBundle bundle)
		{
			if (bundle == null)
			{
				ThrowHelper.ThrowArgumentNullException(bundle, "bundle");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundle>(bundle);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(bundle, "bundle");
			}
			return Unmarshal.UnmarshalUnityObject<Object>(AssetBundle.returnMainAsset_Injected(intPtr));
		}

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
		internal unsafe static AssetBundleCreateRequest LoadFromFileAsync_Internal(string path, uint crc, ulong offset)
		{
			AssetBundleCreateRequest assetBundleCreateRequest;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr = AssetBundle.LoadFromFileAsync_Internal_Injected(ref managedSpanWrapper, crc, offset);
			}
			finally
			{
				IntPtr intPtr;
				IntPtr intPtr2 = intPtr;
				assetBundleCreateRequest = ((intPtr2 == 0) ? null : AssetBundleCreateRequest.BindingsMarshaller.ConvertToManaged(intPtr2));
				char* ptr = null;
			}
			return assetBundleCreateRequest;
		}

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
		internal unsafe static AssetBundle LoadFromFile_Internal(string path, uint crc, ulong offset)
		{
			AssetBundle assetBundle;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr = AssetBundle.LoadFromFile_Internal_Injected(ref managedSpanWrapper, crc, offset);
			}
			finally
			{
				IntPtr intPtr;
				assetBundle = Unmarshal.UnmarshalUnityObject<AssetBundle>(intPtr);
				char* ptr = null;
			}
			return assetBundle;
		}

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
		internal unsafe static AssetBundleCreateRequest LoadFromMemoryAsync_Internal(byte[] binary, uint crc)
		{
			Span<byte> span = new Span<byte>(binary);
			AssetBundleCreateRequest assetBundleCreateRequest;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				IntPtr intPtr = AssetBundle.LoadFromMemoryAsync_Internal_Injected(ref managedSpanWrapper, crc);
				assetBundleCreateRequest = ((intPtr == 0) ? null : AssetBundleCreateRequest.BindingsMarshaller.ConvertToManaged(intPtr));
			}
			return assetBundleCreateRequest;
		}

		public static AssetBundleCreateRequest LoadFromMemoryAsync(byte[] binary)
		{
			return AssetBundle.LoadFromMemoryAsync_Internal(binary, 0U);
		}

		public static AssetBundleCreateRequest LoadFromMemoryAsync(byte[] binary, uint crc)
		{
			return AssetBundle.LoadFromMemoryAsync_Internal(binary, crc);
		}

		[FreeFunction("LoadFromMemory")]
		internal unsafe static AssetBundle LoadFromMemory_Internal(byte[] binary, uint crc)
		{
			Span<byte> span = new Span<byte>(binary);
			AssetBundle assetBundle;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				assetBundle = Unmarshal.UnmarshalUnityObject<AssetBundle>(AssetBundle.LoadFromMemory_Internal_Injected(ref managedSpanWrapper, crc));
			}
			return assetBundle;
		}

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
		internal static AssetBundleCreateRequest LoadFromStreamAsyncInternal(Stream stream, uint crc, uint managedReadBufferSize)
		{
			IntPtr intPtr = AssetBundle.LoadFromStreamAsyncInternal_Injected(stream, crc, managedReadBufferSize);
			return (intPtr == 0) ? null : AssetBundleCreateRequest.BindingsMarshaller.ConvertToManaged(intPtr);
		}

		[FreeFunction("LoadFromStreamInternal")]
		internal static AssetBundle LoadFromStreamInternal(Stream stream, uint crc, uint managedReadBufferSize)
		{
			return Unmarshal.UnmarshalUnityObject<AssetBundle>(AssetBundle.LoadFromStreamInternal_Injected(stream, crc, managedReadBufferSize));
		}

		public bool isStreamedSceneAssetBundle
		{
			[NativeMethod("GetIsStreamedSceneAssetBundle")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AssetBundle.get_isStreamedSceneAssetBundle_Injected(intPtr);
			}
		}

		[NativeMethod("Contains")]
		public unsafe bool Contains(string name)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = AssetBundle.Contains_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
		public Object Load(string name)
		{
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
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
		private unsafe Object LoadAsset_Internal(string name, Type type)
		{
			Object @object;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr2 = AssetBundle.LoadAsset_Internal_Injected(intPtr, ref managedSpanWrapper, type);
			}
			finally
			{
				IntPtr intPtr2;
				@object = Unmarshal.UnmarshalUnityObject<Object>(intPtr2);
				char* ptr = null;
			}
			return @object;
		}

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
		private unsafe AssetBundleRequest LoadAssetAsync_Internal(string name, Type type)
		{
			AssetBundleRequest assetBundleRequest;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr2 = AssetBundle.LoadAssetAsync_Internal_Injected(intPtr, ref managedSpanWrapper, type);
			}
			finally
			{
				IntPtr intPtr2;
				IntPtr intPtr3 = intPtr2;
				assetBundleRequest = ((intPtr3 == 0) ? null : AssetBundleRequest.BindingsMarshaller.ConvertToManaged(intPtr3));
				char* ptr = null;
			}
			return assetBundleRequest;
		}

		[NativeMethod("Unload")]
		[NativeThrows]
		public void Unload(bool unloadAllLoadedObjects)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundle>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AssetBundle.Unload_Injected(intPtr, unloadAllLoadedObjects);
		}

		[NativeThrows]
		[NativeMethod("UnloadAsync")]
		public AssetBundleUnloadOperation UnloadAsync(bool unloadAllLoadedObjects)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundle>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = AssetBundle.UnloadAsync_Injected(intPtr, unloadAllLoadedObjects);
			return (intPtr2 == 0) ? null : AssetBundleUnloadOperation.BindingsMarshaller.ConvertToManaged(intPtr2);
		}

		[NativeMethod("GetAllAssetNames")]
		public string[] GetAllAssetNames()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundle>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AssetBundle.GetAllAssetNames_Injected(intPtr);
		}

		[NativeMethod("GetAllScenePaths")]
		public string[] GetAllScenePaths()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundle>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AssetBundle.GetAllScenePaths_Injected(intPtr);
		}

		[NativeThrows]
		[NativeMethod("LoadAssetWithSubAssets_Internal")]
		internal unsafe Object[] LoadAssetWithSubAssets_Internal(string name, Type type)
		{
			Object[] array;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				array = AssetBundle.LoadAssetWithSubAssets_Internal_Injected(intPtr, ref managedSpanWrapper, type);
			}
			finally
			{
				char* ptr = null;
			}
			return array;
		}

		[NativeThrows]
		[NativeMethod("LoadAssetWithSubAssetsAsync_Internal")]
		private unsafe AssetBundleRequest LoadAssetWithSubAssetsAsync_Internal(string name, Type type)
		{
			AssetBundleRequest assetBundleRequest;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AssetBundle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr2 = AssetBundle.LoadAssetWithSubAssetsAsync_Internal_Injected(intPtr, ref managedSpanWrapper, type);
			}
			finally
			{
				IntPtr intPtr2;
				IntPtr intPtr3 = intPtr2;
				assetBundleRequest = ((intPtr3 == 0) ? null : AssetBundleRequest.BindingsMarshaller.ConvertToManaged(intPtr3));
				char* ptr = null;
			}
			return assetBundleRequest;
		}

		public static AssetBundleRecompressOperation RecompressAssetBundleAsync(string inputPath, string outputPath, BuildCompression method, uint expectedCRC = 0U, ThreadPriority priority = ThreadPriority.Low)
		{
			return AssetBundle.RecompressAssetBundleAsync_Internal(inputPath, outputPath, method, expectedCRC, priority);
		}

		[NativeThrows]
		[FreeFunction("RecompressAssetBundleAsync_Internal")]
		internal unsafe static AssetBundleRecompressOperation RecompressAssetBundleAsync_Internal(string inputPath, string outputPath, BuildCompression method, uint expectedCRC, ThreadPriority priority)
		{
			AssetBundleRecompressOperation assetBundleRecompressOperation;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(inputPath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = inputPath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(outputPath, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = outputPath.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				IntPtr intPtr = AssetBundle.RecompressAssetBundleAsync_Internal_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, ref method, expectedCRC, priority);
			}
			finally
			{
				IntPtr intPtr;
				IntPtr intPtr2 = intPtr;
				assetBundleRecompressOperation = ((intPtr2 == 0) ? null : AssetBundleRecompressOperation.BindingsMarshaller.ConvertToManaged(intPtr2));
				char* ptr = null;
				char* ptr2 = null;
			}
			return assetBundleRecompressOperation;
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
		private static extern IntPtr returnMainAsset_Injected(IntPtr bundle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr LoadFromFileAsync_Internal_Injected(ref ManagedSpanWrapper path, uint crc, ulong offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr LoadFromFile_Internal_Injected(ref ManagedSpanWrapper path, uint crc, ulong offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr LoadFromMemoryAsync_Internal_Injected(ref ManagedSpanWrapper binary, uint crc);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr LoadFromMemory_Internal_Injected(ref ManagedSpanWrapper binary, uint crc);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr LoadFromStreamAsyncInternal_Injected(Stream stream, uint crc, uint managedReadBufferSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr LoadFromStreamInternal_Injected(Stream stream, uint crc, uint managedReadBufferSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isStreamedSceneAssetBundle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Contains_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr LoadAsset_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr LoadAssetAsync_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Unload_Injected(IntPtr _unity_self, bool unloadAllLoadedObjects);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr UnloadAsync_Injected(IntPtr _unity_self, bool unloadAllLoadedObjects);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetAllAssetNames_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetAllScenePaths_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object[] LoadAssetWithSubAssets_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr LoadAssetWithSubAssetsAsync_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr RecompressAssetBundleAsync_Internal_Injected(ref ManagedSpanWrapper inputPath, ref ManagedSpanWrapper outputPath, [In] ref BuildCompression method, uint expectedCRC, ThreadPriority priority);
	}
}
